namespace WAUZ.BL
{
    public sealed class PathHelper : IPathHelper
    {
        public bool PathExists(string path)
        {
            Guard(path);

            path = GetWellFormedPath(path);

            // If no path to a file exists and no path to a
            // directory exists, the path not exists at all.

            return File.Exists(path) || Directory.Exists(path);
        }

        public bool PathExistsAndIsFile(string path)
        {
            Guard(path);

            path = GetWellFormedPath(path);

            return File.Exists(path) && !HasDirectoryAttribute(path);
        }

        public bool PathExistsAndIsDirectory(string path)
        {
            Guard(path);

            path = GetWellFormedPath(path);

            return Directory.Exists(path) && HasDirectoryAttribute(path);
        }

        public string GetParentDirectoryFromPath(string path)
        {
            // In .NET the Path.GetDirectoryName() method is used to get that directory, which
            // contains the file or folder, a given path is pointing to. The method is wrapped
            // here, a) to make above more obvious to me (in future) and b) cause of next part.
            // Important: The method not cares, if that file or folder (given path is pointing
            // to) really exists. The method solely relys on the path string itself. Therefore
            // encapsulating the method and writing this comment at a central location in code
            // is a good way to keep that fact in mind. Otherwise bad things may happen easily.
            // Hiding the 2 possible exceptions, is also intended design of this helper method.

            Guard(path);

            path = GetWellFormedPath(path);

            try
            {
                var parentDirectory = Path.GetDirectoryName(path);

                // Not sure if the result in example contains a directory separator char at the end.
                // So, the result is also formed here. Just to make sure. Maybe this is unnecessary.

                return string.IsNullOrEmpty(parentDirectory) ? string.Empty : GetWellFormedPath(parentDirectory);
            }
            catch
            {
                return string.Empty;
            }
        }

        public string GetFileOrDirectoryNameFromPath(string path)
        {
            // In .NET the Path.GetFileName() method is used for both jobs: To get the name of
            // the file, as well as to get the name of the directory, some path is pointing to.
            // Important: The method not cares, if the file or directory (the path is pointing
            // to) really exists. The method solely relys on the path string itself. Therefore
            // encapsulating the method and writing this comment at a central location in code
            // is a good way to keep that fact in mind. Otherwise bad things may happen easily.
            // Hiding the 1 exception, that can occur (in old .NET versions), is also intended.

            Guard(path);

            path = GetWellFormedPath(path);

            try
            {
                return Path.GetFileName(path) ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public string GetFullPathWithoutEndingDirectorySeparator(string path)
        {
            // Added this wrapped abstraction, to handle all paths the same way
            // and to prevent typing exact same code for a hundred times in app.
            // Because everywhere in app some path always shall be treated as a
            // trimmed full path, without a directory separator char at the end.

            Guard(path);

            return GetWellFormedPath(path);
        }

        private static void Guard(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or whitespace.", nameof(path));
            }
        }

        private static string GetWellFormedPath(string path)
        {
            // Rely on full paths only, with trailing slash/backslash trimmed.

            return Path.TrimEndingDirectorySeparator(Path.GetFullPath(path.Trim()));
        }

        private static bool HasDirectoryAttribute(string path)
        {
            return (File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory;
        }
    }
}
