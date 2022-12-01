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
            if (string.IsNullOrWhiteSpace(zipFile))
            {
                throw new ArgumentException($"'{nameof(zipFile)}' cannot be null or whitespace.", nameof(zipFile));
            }

            if (string.IsNullOrWhiteSpace(destFolder))
            {
                throw new ArgumentException($"'{nameof(destFolder)}' cannot be null or whitespace.", nameof(destFolder));
            }

            if (!File.Exists(zipFile))
            {
                throw new InvalidOperationException($"'{nameof(zipFile)}' has to be an existing file.");
            }

            if (!Directory.Exists(destFolder))
            {
                throw new InvalidOperationException($"'{nameof(destFolder)}' has to be an existing folder.");
            }

            if (zipFile[^4..^0].ToLower() != ".zip")
            {
                throw new InvalidOperationException($"'{nameof(zipFile)}' has to be a file that ends with the '.zip' file extension.");
            }

            var tempFolder = CreateTempFolder();

            // Extract zip file into temp folder. Then move temp folder content into dest folder.
            // Using temp folder for good reasons, instead of extracting directly to dest folder.

            ZipFile.ExtractToDirectory(Path.GetFullPath(zipFile), tempFolder);

            fileSystemHelper.MoveFolderContent(tempFolder, Path.TrimEndingDirectorySeparator(Path.GetFullPath(destFolder)));

            DeleteTempFolder(tempFolder);
        }

        private static string CreateTempFolder()
        {
            var userTempFolder = Path.TrimEndingDirectorySeparator(Path.GetFullPath(Path.GetTempPath()));

            for (int i = 1; i < 10; i++)
            {
                var randomFolderName = Path.GetRandomFileName().Replace(".", string.Empty);

                var tempFolder = Path.Combine(userTempFolder, $"MBODM.WAUZ.{randomFolderName.ToLower()}.tmp");

                if (!Directory.Exists(tempFolder))
                {
                    Directory.CreateDirectory(tempFolder);

                    return tempFolder;
                }
            }

            throw new InvalidOperationException("Could not create temp folder.");
        }

        private static void DeleteTempFolder(string tempFolder)
        {
            if (Directory.Exists(tempFolder))
            {
                Directory.Delete(tempFolder, true);
            }

            if (Directory.Exists(tempFolder))
            {
                throw new InvalidOperationException("Could not delete temp folder.");
            }
        }
    }
}
