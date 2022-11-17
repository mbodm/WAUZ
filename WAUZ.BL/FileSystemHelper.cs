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

            // Rely on full paths only, with trailing slash/backslash trimmed.

            sourceFolder = Path.TrimEndingDirectorySeparator(Path.GetFullPath(sourceFolder.Trim()));
            destFolder = Path.TrimEndingDirectorySeparator(Path.GetFullPath(destFolder.Trim()));

            // Create tuples, where every tuple represents a source path and
            // a corresponding destination path, to a file or to a directory.

            var tuples = CreateTuples(sourceFolder, destFolder);

            // Move all files and directories inside of source folder, into destination folder.
            // If destination folder already contains some of these files or directories, they
            // are deleted from the destination folder first, before the move operation starts.

            foreach (var (sourcePath, destPath, isFile) in tuples)
            {
                // Delete destination, if existing.

                if (pathHelper.PathExists(destPath))
                {
                    if (isFile)
                    {
                        File.Delete(destPath);
                    }
                    else
                    {
                        Directory.Delete(destPath, true);
                    }
                }

                // Move source to destination.

                if (isFile)
                {
                    File.Move(sourcePath, destPath);
                }
                else
                {
                    Directory.Move(sourcePath, destPath);
                }
            }
        }

        private IEnumerable<(string SourcePath, string DestPath, bool IsFile)> CreateTuples(string sourceFolder, string destFolder)
        {
            // The MSDN shows the support of absolute and relative input paths, but says nothing about
            // the output paths. Therefore the first LINQ Select() call may end up as unnecessary here.

            return Directory.EnumerateFileSystemEntries(sourceFolder).
                Select(sourceEntry => Path.TrimEndingDirectorySeparator(Path.GetFullPath(sourceEntry))).
                Select(sourcePath => new
                {
                    SourcePath = sourcePath,
                    FileOrDirectoryName = pathHelper.GetFileOrDirectoryNameFromPath(sourcePath),
                    IsFile = pathHelper.PathExistsAndIsFile(sourcePath),
                }).
                Where(a => a.FileOrDirectoryName != string.Empty).
                Select(a => (a.SourcePath, DestPath: Path.Combine(destFolder, a.FileOrDirectoryName), a.IsFile));
        }
    }
}
