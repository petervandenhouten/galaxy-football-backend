using System.Reflection;

public class VersionInfo
{
    public static string GetVersion()
    {
        string versionString = string.Empty;
        try
        {
            var infoVersion = Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "unknown";
            // Only display the short version (e.g., 1.2.3)
            if (!string.IsNullOrEmpty(infoVersion))
            {
                // Split on '+' (for SemVer) or '-' (for pre-release) and take the first part
                var shortVersion = infoVersion.Split('+', '-')[0];
                versionString = shortVersion;
            }
            else
            {
                versionString = "unknown";
            }
        }
        catch
        {
            versionString = "unknown";
        }
        return versionString;
    }

    public static string GetBuildTime()
    {
        return Assembly.GetExecutingAssembly()
            .GetCustomAttributes(typeof(AssemblyMetadataAttribute), false)
            .OfType<AssemblyMetadataAttribute>()
            .FirstOrDefault(a => a.Key == "BuildDateTime")?.Value ?? "unknown";
    }
}
