using System.IO.Compression;

namespace WAUZ.BL
{
    public sealed class ZipHelper : IZipHelper
    {
        private readonly IFileSystemHelper fileSystemHelper;

        public ZipHelper(IFileSystemHelper fileSystemHelper)
        {
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

            // Rely on existing file and folder only.

            if (!File.Exists(zipFile))
            {
                throw new InvalidOperationException($"'{nameof(zipFile)}' has to be an existing file.");
            }

            if (!Directory.Exists(destFolder))
            {
                throw new InvalidOperationException($"'{nameof(destFolder)}' has to be an existing folder.");
            }

            // Rely on file with proper file extension only.

            if (zipFile[^4..^0].ToLower() != ".zip")
            {
                throw new InvalidOperationException($"'{nameof(zipFile)}' has to be a file that ends with the '.zip' file extension.");
            }

            // Rely on well-formed folder only.

            destFolder = GetWellFormedFolder(destFolder);

            // Create temp folder, with random name.

            var tempFolder = CreateTempFolder();

            // Extract zip file into temp folder (there are some good reasons to
            // use a temp folder, instead of unzip directly into the dest folder).

            ZipFile.ExtractToDirectory(zipFile, tempFolder);

            // Move all files and directories inside of source folder, into destination folder.
            // If destination folder already contains some of these files or directories, they
            // are deleted from the destination folder first, before the move operation starts.

            fileSystemHelper.MoveFolderContent(tempFolder, destFolder);

            // Delete temp folder, after content of temp folder was moved.

            if (Directory.Exists(tempFolder))
            {
                Directory.Delete(tempFolder, true);
            }
        }

        private static string GetWellFormedFolder(string folder)
        {
            // Logic in this class expects folders with absolute path
            // and with trailing slash/backslash trimmed (if existing).

            return Path.TrimEndingDirectorySeparator(Path.GetFullPath(folder));
        }

        private static string CreateTempFolder()
        {
            // Repeat until an exclusive folder name was found and no other folder with
            // that name accidentally already exists there (i know... a VERY low chance).

            while (true)
            {
                var randomFolderName = Path.GetRandomFileName().Replace(".", string.Empty);

                var tempFolder = Path.Combine(Path.GetTempPath(), $"WAUZ-{randomFolderName.ToUpper()}");

                if (!Directory.Exists(tempFolder))
                {
                    Directory.CreateDirectory(tempFolder);

                    return tempFolder;
                }
            }
        }
    }
}
