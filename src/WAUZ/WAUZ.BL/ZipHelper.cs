using System.IO.Compression;

namespace WAUZ.BL
{
    public sealed class ZipHelper : IZipHelper
    {
        private readonly IPathHelper pathHelper;
        private readonly IFileSystemHelper fileSystemHelper;

        public ZipHelper(IPathHelper pathHelper, IFileSystemHelper fileSystemHelper)
        {
            this.pathHelper = pathHelper ?? throw new ArgumentNullException(nameof(pathHelper));
            this.fileSystemHelper = fileSystemHelper ?? throw new ArgumentNullException(nameof(fileSystemHelper));
        }

        public void UnzipFile(string zipFile, string destFolder)
        {
            // Guard method arguments.

            if (string.IsNullOrWhiteSpace(zipFile))
            {
                throw new ArgumentException($"'{nameof(zipFile)}' cannot be null or whitespace.", nameof(zipFile));
            }

            if (string.IsNullOrWhiteSpace(destFolder))
            {
                throw new ArgumentException($"'{nameof(destFolder)}' cannot be null or whitespace.", nameof(destFolder));
            }

            // Rely on valid, absolute and existing paths only.

            if (!pathHelper.IsValidAbsolutePathToExistingFile(zipFile))
            {
                throw new InvalidOperationException($"The '{zipFile}' argument must be a valid absolute path to an existing file.");
            }

            if (!pathHelper.IsValidAbsolutePathToExistingDirectory(destFolder))
            {
                throw new InvalidOperationException($"The '{destFolder}' argument must be a valid absolute path to an existing directory.");
            }

            // Rely only on file name with proper file extension.

            if (zipFile[^3..^0] != ".zip")
            {
                throw new InvalidOperationException($"The '{zipFile}' argument must be a file name ending with the '.zip' file extension.");
            }

            // Rely only on folder with trailing slash/backslash trimmed (if existing).

            destFolder = Path.TrimEndingDirectorySeparator(destFolder);

            // Create temp folder, with random name.

            var tempFolder = CreateTempFolder();

            // Extract zip file into temp folder (there are some good reasons to
            // use a temp folder, instead of unzip directly into the dest folder).

            ZipFile.ExtractToDirectory(zipFile, tempFolder);

            // Move all files and directories inside of temp folder, into destination folder.
            // If destination folder already contains some of these files/directories, they
            // are deleted from the destination folder, before the move operation starts.

            fileSystemHelper.MoveFolderContent(tempFolder, destFolder);

            // Delete temp folder, after the content of temp folder was moved.

            if (Directory.Exists(tempFolder))
            {
                Directory.Delete(tempFolder, true);
            }
        }

        private static string CreateTempFolder()
        {
            while (true)
            {
                var randomFolderName = Path.GetRandomFileName().Replace(".", string.Empty).ToLower();

                var tempFolder = Path.Combine(Path.GetTempPath(), $"wauz-{randomFolderName}");

                if (!Directory.Exists(tempFolder))
                {
                    Directory.CreateDirectory(tempFolder);

                    return tempFolder;
                }
            }
        }
    }
}
