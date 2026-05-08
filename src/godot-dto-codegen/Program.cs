using System.Text;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

var exitCode = await new GeneratorApp().RunAsync(args);
return exitCode;

internal sealed class GeneratorApp
{
    public Task<int> RunAsync(string[] args)
    {
        if (!GeneratorOptions.TryParse(args, out var options, out var errorMessage))
        {
            Console.Error.WriteLine(errorMessage);
            Console.Error.WriteLine(GeneratorOptions.Usage);
            return Task.FromResult(1);
        }

        var definitions = DtoParser.ParseDirectory(options.InputDirectory, options.DtoNamespace);
        if (definitions.Count == 0)
        {
            Console.Error.WriteLine($"No DTO declarations were found in '{options.InputDirectory}'.");
            return Task.FromResult(1);
        }

        var script = GdScriptGenerator.Generate(options.ScriptClassName, definitions);

        Directory.CreateDirectory(Path.GetDirectoryName(options.OutputFilePath)!);
        File.WriteAllText(options.OutputFilePath, script, new UTF8Encoding(false));

        Console.WriteLine($"Generated '{options.OutputFilePath}' from {definitions.Count} DTO declarations.");
        return Task.FromResult(0);
    }
}

internal sealed record GeneratorOptions(string InputDirectory, string OutputFilePath, string DtoNamespace, string ScriptClassName)
{
    public const string Usage = "Usage: godot-dto-codegen --input <dto-directory> --output <gdscript-file> [--namespace <dto-namespace>] [--class-name <gdscript-class-name>]";

    public static bool TryParse(string[] args, out GeneratorOptions options, out string errorMessage)
    {
        options = default!;
        errorMessage = string.Empty;

        string? inputDirectory = null;
        string? outputFilePath = null;
        var dtoNamespace = "GalaxyFootball.Server.DataTransferObjects";
        var scriptClassName = "GalaxyFootballDtos";

        for (var index = 0; index < args.Length; index++)
        {
            var argument = args[index];
            if (index == args.Length - 1)
            {
                errorMessage = $"Missing value for argument '{argument}'.";
                return false;
            }

            var value = args[index + 1];
            switch (argument)
            {
                case "--input":
                    inputDirectory = value;
                    index++;
                    break;
                case "--output":
                    outputFilePath = value;
                    index++;
                    break;
                case "--namespace":
                    dtoNamespace = value;
                    index++;
                    break;
                case "--class-name":
                    scriptClassName = value;
                    index++;
                    break;
                default:
                    errorMessage = $"Unknown argument '{argument}'.";
                    return false;
            }
        }

        if (string.IsNullOrWhiteSpace(inputDirectory) || !Directory.Exists(inputDirectory))
        {
            errorMessage = $"Input directory '{inputDirectory}' does not exist.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(outputFilePath))
        {
            errorMessage = "An output file path is required.";
            return false;
        }

        options = new GeneratorOptions(Path.GetFullPath(inputDirectory), Path.GetFullPath(outputFilePath), dtoNamespace, scriptClassName);
        return true;
    }
}

internal static class DtoParser
{
    public static IReadOnlyList<DtoDefinition> ParseDirectory(string directoryPath, string dtoNamespace)
    {
        var definitions = new List<DtoDefinition>();

        foreach (var filePath in Directory.GetFiles(directoryPath, "*.cs", SearchOption.AllDirectories).OrderBy(path => path, StringComparer.OrdinalIgnoreCase))
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(File.ReadAllText(filePath));
            var root = syntaxTree.GetCompilationUnitRoot();

            foreach (var namespaceDeclaration in root.DescendantNodes().OfType<BaseNamespaceDeclarationSyntax>())
            {
                if (!string.Equals(namespaceDeclaration.Name.ToString(), dtoNamespace, StringComparison.Ordinal))
                {
                    continue;
                }

                foreach (var recordDeclaration in namespaceDeclaration.Members.OfType<RecordDeclarationSyntax>())
                {
                    if (!recordDeclaration.Modifiers.Any(SyntaxKind.PublicKeyword))
                    {
                        continue;
                    }

                    var properties = recordDeclaration.ParameterList is { Parameters.Count: > 0 }
                        ? recordDeclaration.ParameterList.Parameters
                            .Select(parameter => new DtoProperty(parameter.Identifier.ValueText, TypeDescriptorFactory.Create(parameter.Type), parameter.Type?.ToString() ?? "Variant"))
                            .ToList()
                        : recordDeclaration.Members.OfType<PropertyDeclarationSyntax>()
                            .Where(property => property.Modifiers.Any(SyntaxKind.PublicKeyword))
                            .Select(property => new DtoProperty(property.Identifier.ValueText, TypeDescriptorFactory.Create(property.Type), property.Type.ToString()))
                            .ToList();

                    if (properties.Count > 0)
                    {
                        definitions.Add(new DtoDefinition(recordDeclaration.Identifier.ValueText, properties));
                    }
                }

                foreach (var classDeclaration in namespaceDeclaration.Members.OfType<ClassDeclarationSyntax>())
                {
                    if (!classDeclaration.Modifiers.Any(SyntaxKind.PublicKeyword))
                    {
                        continue;
                    }

                    var properties = classDeclaration.Members.OfType<PropertyDeclarationSyntax>()
                        .Where(property => property.Modifiers.Any(SyntaxKind.PublicKeyword))
                        .Select(property => new DtoProperty(property.Identifier.ValueText, TypeDescriptorFactory.Create(property.Type), property.Type.ToString()))
                        .ToList();

                    if (properties.Count > 0)
                    {
                        definitions.Add(new DtoDefinition(classDeclaration.Identifier.ValueText, properties));
                    }
                }
            }
        }

        return definitions;
    }
}

internal static class GdScriptGenerator
{
    public static string Generate(string scriptClassName, IReadOnlyList<DtoDefinition> definitions)
    {
        var builder = new StringBuilder();
        builder.AppendLine("# This file is auto-generated. Do not edit manually.");
        builder.AppendLine($"class_name {scriptClassName}");
        builder.AppendLine("extends RefCounted");
        builder.AppendLine();

        foreach (var definition in definitions)
        {
            AppendDtoClass(builder, definition);
            builder.AppendLine();
            AppendParserFunction(builder, definition, useVariantReturn:true);
            builder.AppendLine();
            AppendJsonParserFunction(builder, definition, useVariantReturn:true);
            builder.AppendLine();
        }

        AppendHelperFunctions(builder);
        return builder.ToString().TrimEnd() + Environment.NewLine;
    }

    private static void AppendDtoClass(StringBuilder builder, DtoDefinition definition)
    {
        builder.AppendLine($"class {definition.Name}:");
        if (definition.Properties.Count == 0)
        {
            builder.AppendLine("    pass");
            return;
        }

        foreach (var property in definition.Properties)
        {
            builder.AppendLine($"    var {property.ScriptPropertyName}: {property.Type.GdTypeName} = {property.Type.DefaultValueLiteral}");
        }
    }

    private static void AppendParserFunction(StringBuilder builder, DtoDefinition definition, bool useVariantReturn = false)
    {
        var returnType = useVariantReturn ? "Variant" : definition.Name;
        builder.AppendLine($"static func {definition.ParserFunctionName}(data: Variant) -> {returnType}:");
        builder.AppendLine("    if data == null:");
        builder.AppendLine("        return null");
        builder.AppendLine("    if not (data is Dictionary):");
        builder.AppendLine($"        push_warning(\"Expected Dictionary for {definition.Name}, got %s.\" % type_string(typeof(data)))");
        builder.AppendLine("        return null");
        builder.AppendLine();
        builder.AppendLine("    var source: Dictionary = data");
        builder.AppendLine($"    var dto: {definition.Name} = {definition.Name}.new()");

        foreach (var property in definition.Properties)
        {
            AppendPropertyAssignment(builder, definition, property, 1, $"source.get(\"{property.JsonPropertyName}\")", $"dto.{property.ScriptPropertyName}");
        }

        builder.AppendLine("    return dto");
    }

    private static void AppendJsonParserFunction(StringBuilder builder, DtoDefinition definition, bool useVariantReturn = false)
    {
        var returnType = useVariantReturn ? "Variant" : definition.Name;
        builder.AppendLine($"static func {definition.JsonParserFunctionName}(json_text: String) -> {returnType}:");
        builder.AppendLine("    var json := JSON.new()");
        builder.AppendLine("    var parse_result = json.parse(json_text)");
        builder.AppendLine("    if parse_result != OK:");
        builder.AppendLine($"        push_error(\"Failed to parse JSON for {definition.Name}: %s at line %d.\" % [json.get_error_message(), json.get_error_line()])");
        builder.AppendLine("        return null");
        builder.AppendLine($"    return {definition.ParserFunctionName}(json.data)");
    }

    private static void AppendPropertyAssignment(StringBuilder builder, DtoDefinition definition, DtoProperty property, int indentLevel, string sourceExpression, string targetExpression)
    {
        var indent = new string(' ', indentLevel * 4);

        if (!property.Type.IsNullable)
        {
            builder.AppendLine($"{indent}if not source.has(\"{property.JsonPropertyName}\") or source.get(\"{property.JsonPropertyName}\") == null:");
            builder.AppendLine($"{indent}    push_warning(\"Missing required field '{property.JsonPropertyName}' while parsing {definition.Name}.\")");
        }

        switch (property.Type)
        {
            case PrimitiveTypeDescriptor primitive:
                builder.AppendLine($"{indent}{targetExpression} = {primitive.ConversionExpression(sourceExpression)}");
                break;
            case ObjectTypeDescriptor objectDescriptor:
                builder.AppendLine($"{indent}{targetExpression} = {objectDescriptor.ParserFunctionName}({sourceExpression})");
                break;
            case ListTypeDescriptor listDescriptor:
                builder.AppendLine($"{indent}{targetExpression} = {listDescriptor.DefaultValueLiteral}");
                builder.AppendLine($"{indent}var {property.ScriptPropertyName}_values: Variant = {sourceExpression}");
                builder.AppendLine($"{indent}if {property.ScriptPropertyName}_values is Array:");
                builder.AppendLine($"{indent}    for item in {property.ScriptPropertyName}_values:");
                if (listDescriptor.ElementType is ObjectTypeDescriptor elementObject)
                {
                    builder.AppendLine($"{indent}        var parsed_item: Variant = {elementObject.ParserFunctionName}(item)");
                    builder.AppendLine($"{indent}        if parsed_item != null:");
                    builder.AppendLine($"{indent}            {targetExpression}.append(parsed_item)");
                    builder.AppendLine($"{indent}        else:");
                    builder.AppendLine($"{indent}            push_warning(\"Skipping invalid item in '{property.JsonPropertyName}' while parsing {definition.Name}.\")");
                }
                else
                {
                    var itemExpression = listDescriptor.ElementType switch
                    {
                        PrimitiveTypeDescriptor elementPrimitive => elementPrimitive.ConversionExpression("item"),
                        ListTypeDescriptor => "item",
                        _ => "item"
                    };
                    builder.AppendLine($"{indent}        {targetExpression}.append({itemExpression})");
                }
                builder.AppendLine($"{indent}elif {property.ScriptPropertyName}_values != null:");
                builder.AppendLine($"{indent}    push_warning(\"Expected Array for '{property.JsonPropertyName}', got %s.\" % type_string(typeof({property.ScriptPropertyName}_values)))");
                break;
            default:
                builder.AppendLine($"{indent}{targetExpression} = {sourceExpression}");
                break;
        }
    }

    private static void AppendHelperFunctions(StringBuilder builder)
    {
        builder.AppendLine("static func dto_to_string(value: Variant) -> String:");
        builder.AppendLine("    if value == null:");
        builder.AppendLine("        return \"\"");
        builder.AppendLine("    return str(value)");
        builder.AppendLine();
        builder.AppendLine("static func dto_to_nullable_string(value: Variant) -> Variant:");
        builder.AppendLine("    if value == null:");
        builder.AppendLine("        return null");
        builder.AppendLine("    return str(value)");
        builder.AppendLine();
        builder.AppendLine("static func dto_to_int(value: Variant) -> int:");
        builder.AppendLine("    if value == null:");
        builder.AppendLine("        return 0");
        builder.AppendLine("    return int(value)");
        builder.AppendLine();
        builder.AppendLine("static func dto_to_nullable_int(value: Variant) -> Variant:");
        builder.AppendLine("    if value == null:");
        builder.AppendLine("        return null");
        builder.AppendLine("    return int(value)");
        builder.AppendLine();
        builder.AppendLine("static func dto_to_float(value: Variant) -> float:");
        builder.AppendLine("    if value == null:");
        builder.AppendLine("        return 0.0");
        builder.AppendLine("    return float(value)");
        builder.AppendLine();
        builder.AppendLine("static func dto_to_nullable_float(value: Variant) -> Variant:");
        builder.AppendLine("    if value == null:");
        builder.AppendLine("        return null");
        builder.AppendLine("    return float(value)");
        builder.AppendLine();
        builder.AppendLine("static func dto_to_bool(value: Variant) -> bool:");
        builder.AppendLine("    if value == null:");
        builder.AppendLine("        return false");
        builder.AppendLine("    return bool(value)");
        builder.AppendLine();
        builder.AppendLine("static func dto_to_nullable_bool(value: Variant) -> Variant:");
        builder.AppendLine("    if value == null:");
        builder.AppendLine("        return null");
        builder.AppendLine("    return bool(value)");
    }
}

internal sealed record DtoDefinition(string Name, IReadOnlyList<DtoProperty> Properties)
{
    public string ParserFunctionName => $"parse_{Name.ToSnakeCase()}";
    public string JsonParserFunctionName => $"parse_json_{Name.ToSnakeCase()}";
}

internal sealed record DtoProperty(string Name, TypeDescriptor Type, string SourceTypeName)
{
    public string JsonPropertyName => JsonNamingPolicy.CamelCase.ConvertName(Name);
    public string ScriptPropertyName => Name.ToSnakeCase();
}

internal abstract record TypeDescriptor(bool IsNullable)
{
    public abstract string DefaultValueLiteral { get; }
    public abstract string GdTypeName { get; }
}

internal sealed record PrimitiveTypeDescriptor(string PrimitiveName, bool IsNullable) : TypeDescriptor(IsNullable)
{
    public override string DefaultValueLiteral => (PrimitiveName, IsNullable) switch
    {
        (_, true) => "null",
        ("string", false) => "\"\"",
        ("int", false) => "0",
        ("float", false) => "0.0",
        ("bool", false) => "false",
        _ => "null"
    };

    public override string GdTypeName => (PrimitiveName, IsNullable) switch
    {
        (_, true) => "Variant",
        ("string", false) => "String",
        ("int", false) => "int",
        ("float", false) => "float",
        ("bool", false) => "bool",
        _ => "Variant"
    };

    public string ConversionExpression(string sourceExpression)
    {
        return (PrimitiveName, IsNullable) switch
        {
            ("string", false) => $"dto_to_string({sourceExpression})",
            ("string", true) => $"dto_to_nullable_string({sourceExpression})",
            ("int", false) => $"dto_to_int({sourceExpression})",
            ("int", true) => $"dto_to_nullable_int({sourceExpression})",
            ("float", false) => $"dto_to_float({sourceExpression})",
            ("float", true) => $"dto_to_nullable_float({sourceExpression})",
            ("bool", false) => $"dto_to_bool({sourceExpression})",
            ("bool", true) => $"dto_to_nullable_bool({sourceExpression})",
            _ => $"dto_to_string({sourceExpression})"
        };
    }
}

internal sealed record ObjectTypeDescriptor(string TypeName, bool IsNullable) : TypeDescriptor(IsNullable)
{
    public override string DefaultValueLiteral => "null";
    public override string GdTypeName => TypeName;

    public string ParserFunctionName => $"parse_{TypeName.ToSnakeCase()}";
}

internal sealed record ListTypeDescriptor(TypeDescriptor ElementType, bool IsNullable) : TypeDescriptor(IsNullable)
{
    public override string DefaultValueLiteral => IsNullable ? "null" : "[]";
    public override string GdTypeName => IsNullable ? "Variant" : $"Array[{ElementType.GdTypeName}]";
}

internal static class TypeDescriptorFactory
{
    private static readonly HashSet<string> StringLikeTypes = new(StringComparer.Ordinal)
    {
        "string",
        "guid",
        "datetime",
        "datetimeoffset",
        "dateonly",
        "timeonly",
        "timespan"
    };

    private static readonly HashSet<string> IntegerTypes = new(StringComparer.Ordinal)
    {
        "byte",
        "sbyte",
        "short",
        "ushort",
        "int",
        "uint",
        "long",
        "ulong"
    };

    private static readonly HashSet<string> FloatTypes = new(StringComparer.Ordinal)
    {
        "float",
        "double",
        "decimal"
    };

    public static TypeDescriptor Create(TypeSyntax? typeSyntax)
    {
        if (typeSyntax is null)
        {
            return new PrimitiveTypeDescriptor("string", true);
        }

        if (typeSyntax is NullableTypeSyntax nullableTypeSyntax)
        {
            return CreateNullable(nullableTypeSyntax.ElementType);
        }

        if (typeSyntax is GenericNameSyntax genericNameSyntax && IsListType(genericNameSyntax.Identifier.ValueText) && genericNameSyntax.TypeArgumentList.Arguments.Count == 1)
        {
            return new ListTypeDescriptor(Create(genericNameSyntax.TypeArgumentList.Arguments[0]), false);
        }

        if (typeSyntax is QualifiedNameSyntax qualifiedNameSyntax)
        {
            return Create(qualifiedNameSyntax.Right);
        }

        if (typeSyntax is AliasQualifiedNameSyntax aliasQualifiedNameSyntax)
        {
            return Create(aliasQualifiedNameSyntax.Name);
        }

        var typeName = typeSyntax switch
        {
            PredefinedTypeSyntax predefinedTypeSyntax => predefinedTypeSyntax.Keyword.ValueText,
            IdentifierNameSyntax identifierNameSyntax => identifierNameSyntax.Identifier.ValueText,
            GenericNameSyntax otherGenericName => otherGenericName.Identifier.ValueText,
            _ => typeSyntax.ToString()
        };

        return CreateFromTypeName(typeName, false);
    }

    private static TypeDescriptor CreateNullable(TypeSyntax typeSyntax)
    {
        if (typeSyntax is GenericNameSyntax genericNameSyntax && string.Equals(genericNameSyntax.Identifier.ValueText, "Nullable", StringComparison.Ordinal) && genericNameSyntax.TypeArgumentList.Arguments.Count == 1)
        {
            return CreateNullable(genericNameSyntax.TypeArgumentList.Arguments[0]);
        }

        var descriptor = Create(typeSyntax);
        return descriptor switch
        {
            PrimitiveTypeDescriptor primitive => primitive with { IsNullable = true },
            ObjectTypeDescriptor objectDescriptor => objectDescriptor with { IsNullable = true },
            ListTypeDescriptor listDescriptor => listDescriptor with { IsNullable = true },
            _ => descriptor
        };
    }

    private static TypeDescriptor CreateFromTypeName(string typeName, bool isNullable)
    {
        var normalizedTypeName = typeName.Trim();
        var lookupName = normalizedTypeName.ToLowerInvariant();

        if (StringLikeTypes.Contains(lookupName))
        {
            return new PrimitiveTypeDescriptor("string", isNullable);
        }

        if (IntegerTypes.Contains(lookupName))
        {
            return new PrimitiveTypeDescriptor("int", isNullable);
        }

        if (FloatTypes.Contains(lookupName))
        {
            return new PrimitiveTypeDescriptor("float", isNullable);
        }

        if (string.Equals(lookupName, "bool", StringComparison.Ordinal))
        {
            return new PrimitiveTypeDescriptor("bool", isNullable);
        }

        if (string.Equals(lookupName, "string", StringComparison.Ordinal))
        {
            return new PrimitiveTypeDescriptor("string", isNullable);
        }

        return new ObjectTypeDescriptor(normalizedTypeName, isNullable);
    }

    private static bool IsListType(string identifier) =>
        string.Equals(identifier, "IEnumerable", StringComparison.Ordinal) ||
        string.Equals(identifier, "IReadOnlyCollection", StringComparison.Ordinal) ||
        string.Equals(identifier, "IReadOnlyList", StringComparison.Ordinal) ||
        string.Equals(identifier, "IList", StringComparison.Ordinal) ||
        string.Equals(identifier, "ICollection", StringComparison.Ordinal) ||
        string.Equals(identifier, "List", StringComparison.Ordinal) ||
        string.Equals(identifier, "Array", StringComparison.Ordinal);
}

internal static class StringExtensions
{
    public static string ToSnakeCase(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        var builder = new StringBuilder(value.Length + 8);
        for (var index = 0; index < value.Length; index++)
        {
            var character = value[index];
            if (char.IsUpper(character))
            {
                if (index > 0 && (!char.IsUpper(value[index - 1]) || (index + 1 < value.Length && char.IsLower(value[index + 1]))))
                {
                    builder.Append('_');
                }

                builder.Append(char.ToLowerInvariant(character));
            }
            else
            {
                builder.Append(character);
            }
        }

        return builder.ToString();
    }
}