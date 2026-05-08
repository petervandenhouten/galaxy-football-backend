# Godot DTO Code Generator

This tool generates a single GDScript file capable of parsing all backend DTOs found in the specified directory and namespace.

## Usage

Run the generator after adding, removing, or updating any DTO in your backend. The output GDScript will always include all public DTOs found in the directory, ensuring future DTOs are automatically supported after regeneration.

**Example:**

```
dotnet run --project src/godot-dto-codegen/godot-dto-codegen.csproj --input src/galaxy-football-server/DataTransferObjects --output artifacts/godot/GalaxyFootballDtos.gd --namespace GalaxyFootball.Server.DataTransferObjects --class-name GalaxyFootballDtos
```

- The generated GDScript contains:
  - One main class (class_name) for the DTO collection
  - All DTO classes as inner classes
  - Static parser functions for each DTO
  - Shared helper functions

## Notes
- Always regenerate the GDScript file after adding new DTOs to the backend.
- No need to manually update the generator for new DTOs—just rerun it.
- Only one GDScript file is needed for all DTOs.
