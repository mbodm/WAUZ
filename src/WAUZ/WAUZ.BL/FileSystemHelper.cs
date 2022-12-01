namespace WAUZ.BL
{
    public sealed class FileSystemHelper : IFileSystemHelper
    {
        public void MoveFolderContent(string sourceFolder, string destFolder)
        {
            if (string.IsNullOrWhiteSpace(sourceFolder))
            {
                throw new ArgumentException($"'{nameof(sourceFolder)}' cannot be null or whitespace.", nameof(sourceFolder));
            }

            if (string.IsNullOrWhiteSpace(destFolder))
            {
                throw new ArgumentException($"'{nameof(destFolder)}' cannot be null or whitespace.", nameof(destFolder));
            }

            if (!Directory.Exists(sourceFolder))
            {
                throw new InvalidOperationException($"'{nameof(sourceFolder)}' has to be an existing folder.");
            }

            if (!Directory.Exists(destFolder))
            {
                throw new InvalidOperationException($"'{nameof(destFolder)}' has to be an existing folder.");
            }

            // Create tuples, where every tuple represents a source path
            // and a corresponding destination path, to a file or folder.

            var tuples = CreateTuples(sourceFolder, destFolder);

            // Move all files and directories inside of source folder, into dest folder.
            // If dest folder already contains some of those files or directories, they
            // are removed from dest folder first, before running proper move operation.

            foreach (var (source, dest) in tuples)
            {
                if (File.Exists(source) && !HasDirectoryAttribute(source))
                {
                    if (File.Exists(dest))
                    {
                        File.Delete(dest);
                    }

                    File.Move(source, dest);

                    continue;
                }

                if (Directory.Exists(source) && HasDirectoryAttribute(source))
                {
                    if (Directory.Exists(dest))
                    {
                        Directory.Delete(dest, true);
                    }

                    Directory.Move(source, dest);

                    continue;
                }

                throw new InvalidOperationException("The tuples contain a source path, which seems to be wether a file, nor a folder.");
            }
        }

        private static IEnumerable<(string Source, string Dest)> CreateTuples(string sourceFolder, string destFolder)
        {
            sourceFolder = Path.TrimEndingDirectorySeparator(Path.GetFullPath(sourceFolder));

            destFolder = Path.TrimEndingDirectorySeparator(Path.GetFullPath(destFolder));

            return Directory.EnumerateFileSystemEntries(sourceFolder).Select(sourceEntry =>
            {
                var source = sourceEntry; // <-- If method input is absolute, this path is also absolute.

                var fileOrFolderName = Path.GetFileName(source); // <-- Is typically used for both, file names and folder names.

                if (string.IsNullOrEmpty(fileOrFolderName))
                {
                    throw new InvalidOperationException("Could not determine the name of some file or folder, while creating the tuples.");
                }

                var dest = Path.TrimEndingDirectorySeparator(Path.Combine(destFolder, fileOrFolderName));

                return (source, dest);
            });
        }

        private static bool HasDirectoryAttribute(string path)
        {
            return (File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory;
        }
    }
}
