using System.Reflection;

namespace GalaxyFootball.Infrastructure.Git
{
    public class BuildInfo
    {
        public static string GetGitBranchName()
        {
            // Retrieve branch name from environment variable GIT_BRANCH
            return Environment.GetEnvironmentVariable("GIT_BRANCH") ?? "unknown";
        }

        public static bool IsProductionBuild()
        {
            return GetGitBranchName().Equals("main", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsDevelopmentBuild()
        {
            return !IsProductionBuild();
        }

    }
}