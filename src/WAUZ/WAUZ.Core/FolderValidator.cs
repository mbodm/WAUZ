using WAUZ.Helpers;

namespace WAUZ.Core
{
    public sealed class FolderValidator : IFolderValidator
    {
        // See details and reasons for MaxPathLength value at
        // https://stackoverflow.com/questions/265769/maximum-filename-length-in-ntfs-windows-xp-and-windows-vista
        // https://stackoverflow.com/questions/23588944/better-to-check-if-length-exceeds-max-path-or-catch-pathtoolongexception

        private const int MaxPathLength = 240;

        private readonly IFileSystemHelper fileSystemHelper;

        public FolderValidator(IFileSystemHelper fileSystemHelper)
        {
            this.fileSystemHelper = fileSystemHelper ?? throw new ArgumentNullException(nameof(fileSystemHelper));
        }

        public void ValidateSourceFolder(string sourceFolder)
        {
            ValidateFolder(sourceFolder, "Source-Folder", MaxPathLength);
        }

        public void ValidateDestFolder(string destFolder)
        {
            // Easy to foresee max length of source. Not that easy to foresee max length of dest, when considering content of
            // zip file (files and subfolders). Therefore just using half of MAX_PATH here, as some "rule of thumb". If in a
            // rare case a full dest path exceeds MAX_PATH, it seems ok to let the unzip operation fail gracefully on its own.

            ValidateFolder(destFolder, "Destination-Folder", MaxPathLength / 2);
        }

        private void ValidateFolder(string folderValue, string folderName, int maxChars)
        {
            if (string.IsNullOrWhiteSpace(folderValue))
            {
                throw new ValidationException($"{folderName} missing.");
            }

            if (!fileSystemHelper.IsValidAbsolutePath(folderValue) || !Directory.Exists(folderValue))
            {
                throw new ValidationException($"{folderName} is not a valid path. Given path must be a valid, absolute path, to an existing folder.");
            }

            if (folderValue.Length > maxChars)
            {
                throw new ValidationException($"{folderName} path is too long. Make sure given path is smaller than {maxChars} characters.");
            }
        }
    }
}
