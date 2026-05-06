using System.Reflection;

namespace GalaxyFootball.Infrastructure.Git
{
    public class GitInfo
    {
        public sealed record GitRepositoryInfo(string? Branch, string? Commit, bool IsDetachedHead, bool RepositoryFound)
        {
            public static GitRepositoryInfo ReadFrom(string startPath)
            {
                try
                {
                    var gitDirectory = FindGitDirectory(startPath);
                    if (gitDirectory is null)
                    {
                        return new GitRepositoryInfo(null, null, false, false);
                    }

                    var headPath = Path.Combine(gitDirectory, "HEAD");
                    if (!System.IO.File.Exists(headPath))
                    {
                        return new GitRepositoryInfo(null, null, false, true);
                    }

                    var headContents = System.IO.File.ReadAllText(headPath).Trim();
                    const string refPrefix = "ref: ";
                    if (headContents.StartsWith(refPrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        var reference = headContents[refPrefix.Length..].Trim();
                        var commit = ReadReferenceCommit(gitDirectory, reference);
                        var branch = reference.StartsWith("refs/heads/", StringComparison.Ordinal)
                            ? reference["refs/heads/".Length..]
                            : reference;

                        return new GitRepositoryInfo(branch, commit, false, true);
                    }

                    return new GitRepositoryInfo(null, headContents, true, true);
                }
                catch (Exception)
                {
                    return new GitRepositoryInfo(null, null, false, false);
                }
            }

            private static string? FindGitDirectory(string startPath)
            {
                var current = new DirectoryInfo(startPath);
                while (current is not null)
                {
                    var gitPath = Path.Combine(current.FullName, ".git");
                    if (Directory.Exists(gitPath))
                    {
                        return gitPath;
                    }

                    if (System.IO.File.Exists(gitPath))
                    {
                        var contents = System.IO.File.ReadAllText(gitPath).Trim();
                        const string gitDirPrefix = "gitdir: ";
                        if (contents.StartsWith(gitDirPrefix, StringComparison.OrdinalIgnoreCase))
                        {
                            var relativePath = contents[gitDirPrefix.Length..].Trim();
                            return Path.GetFullPath(Path.Combine(current.FullName, relativePath));
                        }
                    }

                    current = current.Parent;
                }

                return null;
            }

            private static string? ReadReferenceCommit(string gitDirectory, string reference)
            {
                var referencePath = Path.Combine(gitDirectory, reference.Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(referencePath))
                {
                    return System.IO.File.ReadAllText(referencePath).Trim();
                }

                var packedRefsPath = Path.Combine(gitDirectory, "packed-refs");
                if (!System.IO.File.Exists(packedRefsPath))
                {
                    return null;
                }

                foreach (var line in System.IO.File.ReadLines(packedRefsPath))
                {
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#", StringComparison.Ordinal) || line.StartsWith("^", StringComparison.Ordinal))
                    {
                        continue;
                    }

                    var separatorIndex = line.IndexOf(' ');
                    if (separatorIndex <= 0)
                    {
                        continue;
                    }

                    var refName = line[(separatorIndex + 1)..].Trim();
                    if (string.Equals(refName, reference, StringComparison.Ordinal))
                    {
                        return line[..separatorIndex].Trim();
                    }
                }

                return null;
            }
        }
    }
}