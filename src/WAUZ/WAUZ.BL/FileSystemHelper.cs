namespace WAUZ.BL
{
    public sealed class FileSystemHelper : IFileSystemHelper
    {
        private readonly IPathHelper pathHelper;

        public FileSystemHelper(IPathHelper pathHelper)
        {
            this.pathHelper = pathHelper ?? throw new ArgumentNullException(nameof(pathHelper));
        }

        public void MoveFolderContent(string sourceFolder, string destFolder)
        {
            // Guard method arguments.

            if (string.IsNullOrWhiteSpace(sourceFolder))
            {
                throw new ArgumentException($"'{nameof(sourceFolder)}' cannot be null or whitespace.", nameof(sourceFolder));
            }

            if (string.IsNullOrWhiteSpace(destFolder))
            {
                throw new ArgumentException($"'{nameof(destFolder)}' cannot be null or whitespace.", nameof(destFolder));
            }

            // Rely on valid, absolute and existing folder paths only, with trailing slash/backslash trimmed.

            if (!pathHelper.IsValidAbsolutePathToExistingDirectory(sourceFolder))
            {
                throw new InvalidOperationException($"The '{sourceFolder}' argument must be a valid absolute path to an existing directory.");
            }

            sourceFolder = pathHelper.TrimEndingDirectorySeparatorIfExistingFromValidAbsolutePath(sourceFolder);

            if (!pathHelper.IsValidAbsolutePathToExistingDirectory(destFolder))
            {
                throw new InvalidOperationException($"The '{destFolder}' argument must be a valid absolute path to an existing directory.");
            }

            destFolder = pathHelper.TrimEndingDirectorySeparatorIfExistingFromValidAbsolutePath(destFolder);

            // Create tuples, where every tuple represents a source path and
            // a corresponding destination path, to a file or to a directory.

            var tuples = CreateTuples(sourceFolder, destFolder);

            // Move all files and directories inside of source folder, into destination folder.
            // If destination folder already contains some of these files or directories, they
            // are deleted from the destination folder first, before the move operation starts.

            foreach (var (sourcePath, destPath) in tuples)
            {
                // Delete destination, if existing.

                if (pathHelper.IsValidAbsolutePathToExistingFileOrDirectory(destPath))
                {
                    if (pathHelper.IsValidAbsolutePathToExistingFile(destPath))
                    {
                        File.Delete(destPath);
                    }

                    if (pathHelper.IsValidAbsolutePathToExistingDirectory(destPath))
                    {
                        Directory.Delete(destPath, true);
                    }
                }

                // Move source to destination.

                if (pathHelper.IsValidAbsolutePathToExistingFile(sourcePath))
                {
                    File.Move(sourcePath, destPath);
                }

                if (pathHelper.IsValidAbsolutePathToExistingDirectory(sourcePath))
                {
                    Directory.Move(sourcePath, destPath);
                }
            }
        }

        private IEnumerable<(string SourcePath, string DestPath)> CreateTuples(string sourceFolder, string destFolder)
        {
            // The MSDN page for the EnumerateFileSystemEntries() method has informations about the support of absolute and
            // relative paths, relating to the input argument of the method. But the MSDN page says nothing about the paths
            // returned by the method. Therefore i tested this by myself. Result: If the input argument is an absolute path,
            // all output paths are also absolute paths. And if the input argument is a relative path, all output paths are
            // also relative paths. So, when source is guaranteed to be a valid absolute path here, destination will be too.
            
            return Directory.EnumerateFileSystemEntries(sourceFolder).
                Select(sourceEntry => new
                {
                    SourcePath = sourceEntry,
                    FileOrDirectoryName = pathHelper.GetFileOrDirectoryNameFromValidAbsolutePath(sourceEntry),
                }).
                Where(a => a.FileOrDirectoryName != string.Empty).
                Select(a => (a.SourcePath, DestPath: Path.Combine(destFolder, a.FileOrDirectoryName)));
        }
    }
}
