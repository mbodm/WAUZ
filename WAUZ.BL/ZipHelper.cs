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

            // Rely on full paths only, with trailing slash/backslash trimmed.

            zipFile = Path.TrimEndingDirectorySeparator(Path.GetFullPath(zipFile.Trim()));
            destFolder = Path.TrimEndingDirectorySeparator(Path.GetFullPath(destFolder.Trim()));

            // Early-stage validation, if zip file exists (despite the fact
            // that decompression method below would fail gracefully anyway).

            if (!File.Exists(zipFile))
            {
                throw new InvalidOperationException("Given zip file not exists.");
            }

            // Create destination folder, if it not already exists.

            if (!Directory.Exists(destFolder))
            {
                Directory.CreateDirectory(destFolder);
            }

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

            Directory.Delete(tempFolder);
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
