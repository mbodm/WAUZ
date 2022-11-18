namespace WAUZ.BL
{
    public sealed class PathHelper : IPathHelper
    {
        public bool PathExists(string path)
        {
            path = Guard(path);

            // If no path to a file exists and no path to a
            // directory exists, the path not exists at all.

            return File.Exists(path) || Directory.Exists(path);
        }

        public bool PathExistsAndIsFile(string path)
        {
            path = Guard(path);

            return File.Exists(path) && !HasDirectoryAttribute(path);
        }

        public bool PathExistsAndIsDirectory(string path)
        {
            path = Guard(path);

            return Directory.Exists(path) && HasDirectoryAttribute(path);
        }

        public string GetParentDirectoryFromPath(string path)
        {
            path = Guard(path);

            // In .NET the Path.GetDirectoryName() method is used to get that directory, which
            // contains the file or folder, a given path is pointing to. The method is wrapped
            // here, a) to make above more obvious to me (in future) and b) cause of next part.
            // Important: The method not cares, if that file or folder (given path is pointing
            // to) really exists. The method solely relys on the path string itself. Therefore
            // the design of this helper method also takes care about this fact. Otherwise bad
            // things may happen, if above fact is not in mind, while using this helper method.
            // Hiding the 2 possible exceptions, is also intended design of this helper method.

            if (!File.Exists(path) && !Directory.Exists(path))
            {
                return string.Empty;
            }

            try
            {
                var parentDirectory = Path.GetDirectoryName(path);

                return string.IsNullOrEmpty(parentDirectory) ? string.Empty : Path.TrimEndingDirectorySeparator(parentDirectory);
            }
            catch
            {
                return string.Empty;
            }
        }

        public string GetFileOrDirectoryNameFromPath(string path)
        {
            path = Guard(path);

            // In .NET the Path.GetFileName() method is used for both jobs: To get the name of
            // the file, as well as to get the name of the directory, some path is pointing to.
            // Important: The method not cares, if the file or directory (the path is pointing
            // to) really exists. The method solely relys on the path string itself. Therefore
            // the design of this helper method also takes care about this fact. Otherwise bad
            // things may happen, if above fact is not in mind, while using this helper method.
            // Hiding the 1 exception, that can occur (in old .NET versions), is also intended.

            if (!File.Exists(path) && !Directory.Exists(path))
            {
                return string.Empty;
            }

            try
            {
                return Path.GetFileName(path) ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string Guard(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or whitespace.", nameof(path));
            }

            // Rely on full paths only, with trailing slash/backslash trimmed.

            return Path.TrimEndingDirectorySeparator(Path.GetFullPath(path.Trim()));
        }

        private static bool HasDirectoryAttribute(string path)
        {
            return (File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory;
        }
    }
}
