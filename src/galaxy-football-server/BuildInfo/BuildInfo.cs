// todo: can be removed

public class BuildInfo
{
    public static string GetAspNetCoreEnvironment()
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "unknown";
    }

    public static bool IsProductionBuild()
    {
        return !IsDevelopmentBuild();
    }

    public static bool IsDevelopmentBuild()
    {
        return GetAspNetCoreEnvironment().Contains("dev", StringComparison.OrdinalIgnoreCase);
    }

}
