namespace WAUZ.BL
{
    public sealed class PathHelper : IPathHelper
    {
        public bool IsValidAbsolutePath(string path)
        {
            // This method only relies on the given path string itself
            // and not depends on real/existing files or folders.
            
            try
            {
                if (!string.IsNullOrWhiteSpace(path))
                {
                    var parentDirectory = GetParentDirectory(path);
                    var invalidPathChars = Path.GetInvalidPathChars();

                    if (!parentDirectory.Any(c => invalidPathChars.Contains(c)))
                    {
                        var fileOrDirectoryName = GetFileOrDirectoryName(path);
                        var invalidFileNameChars = Path.GetInvalidFileNameChars();

                        if (!fileOrDirectoryName.Any(c => invalidFileNameChars.Contains(c)))
                        {
                            if (Path.IsPathFullyQualified(path))
                            {
                                if (Path.IsPathRooted(path))
                                {
                                    var root = Path.GetPathRoot(path);

                                    if (!string.IsNullOrEmpty(root))
                                    {
                                        var drives = DriveInfo.GetDrives().Select(drive => drive.Name);

                                        if (drives.Contains(root))
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                // Hiding exceptions is intended design for this class.
            }

            return false;
        }

        public bool IsValidAbsolutePathToExistingFile(string path)
        {
            return IsValidAbsolutePath(path) && File.Exists(path) && !HasDirectoryAttribute(path);
        }

        public bool IsValidAbsolutePathToExistingDirectory(string path)
        {
            return IsValidAbsolutePath(path) && Directory.Exists(path) && HasDirectoryAttribute(path);
        }

        public string GetParentDirectoryFromValidAbsolutePath(string path)
        {
            if (IsValidAbsolutePath(path))
            {
                return GetParentDirectory(path);
            }

            return string.Empty;
        }

        public string GetFileOrDirectoryNameFromValidAbsolutePath(string path)
        {
            if (IsValidAbsolutePath(path))
            {
                return GetFileOrDirectoryName(path);
            }

            return string.Empty;
        }
        
        private static string GetParentDirectory(string path)
        {
            // In .NET the Path.GetDirectoryName() method is used to get that directory, which
            // contains the file or folder, a given path is pointing to. The method is wrapped
            // here, a) to make above more obvious to me (in future) and b) cause of next part.
            // Important: The method not cares, if that file or folder (given path is pointing
            // to) really exists, and solely relies on the given path string itself. Therefore
            // encapsulating the method and writing this comment at a central location in code
            // is a good way to keep that fact in mind. Otherwise bad things may happen easily.
            
            try
            {
                return Path.GetDirectoryName(path) ?? string.Empty;
            }
            catch
            {
                // Hiding exceptions is intended design for this class.

                return string.Empty;
            }
        }

        private static string GetFileOrDirectoryName(string path)
        {
            // In .NET the Path.GetFileName() method is used for both jobs: To get the name of
            // the file, as well as to get the name of the directory, a given path is pointing
            // to. When used with files, result includes file extension. The method is wrapped
            // here, a) to make above more obvious to me (in future) and b) cause of next part.
            // Important: The method not cares, if the file or directory (the path is pointing
            // to) really exists, and solely relies on the given path string itself. Therefore
            // encapsulating the method and writing this comment at a central location in code
            // is a good way to keep that fact in mind. Otherwise bad things may happen easily.
            
            try
            {
                return Path.GetFileName(path) ?? string.Empty;
            }
            catch
            {
                // Hiding exceptions is intended design for this class.

                return string.Empty;
            }
        }

        private static bool HasDirectoryAttribute(string path)
        {
            try
            {
                return (File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory;
            }
            catch
            {
                // Hiding exceptions is intended design for this class.

                return false;
            }
        }
    }
}
