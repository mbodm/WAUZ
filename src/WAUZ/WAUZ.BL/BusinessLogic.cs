namespace WAUZ.BL
{
    public sealed class BusinessLogic : IBusinessLogic
    {
        private readonly IAppSettings appSettings;
        private readonly IPathHelper pathHelper;
        private readonly IZipHelper zipHelper;

        public BusinessLogic(IAppSettings appSettings, IPathHelper pathHelper, IZipHelper zipHelper)
        {
            this.appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            this.pathHelper = pathHelper ?? throw new ArgumentNullException(nameof(pathHelper));
            this.zipHelper = zipHelper ?? throw new ArgumentNullException(nameof(zipHelper));
        }

        public string SourceFolder { get; set; } = string.Empty;
        public string DestFolder { get; set; } = string.Empty;

        public void LoadSettings()
        {
            try
            {
                appSettings.Load();
            }
            catch (Exception e)
            {
                LogException(e);

                throw new InvalidOperationException("An error occurred while loading the settings (see log file for details).");
            }

            if (appSettings.Settings.TryGetValue("source", out var sourceFolder))
            {
                SourceFolder = sourceFolder;
            }

            if (appSettings.Settings.TryGetValue("dest", out var destFolder))
            {
                DestFolder = destFolder;
            }
        }

        public void SaveSettings()
        {
            appSettings.Settings["source"] = SourceFolder;
            appSettings.Settings["dest"] = DestFolder;

            appSettings.Save();
        }

        public IEnumerable<string> GetZipFiles()
        {
            ValidateFolder(SourceFolder, "Source-Folder");

            var zipFiles = Directory.GetFiles(SourceFolder, "*.zip", SearchOption.TopDirectoryOnly);

            if (!zipFiles.Any())
            {
                throw new InvalidOperationException("Source-Folder not contains any zip files.");
            }

            return zipFiles;
        }

        public async Task Unzip(IProgress<ProgressData>? progress = null, CancellationToken cancellationToken = default)
        {
            var zipFiles = GetZipFiles();

            ValidateFolder(DestFolder, "Destination-Folder");

            SourceFolder = Path.TrimEndingDirectorySeparator(SourceFolder);
            DestFolder = Path.TrimEndingDirectorySeparator(DestFolder);

            var tasks = zipFiles.Select(zipFile => Task.Run(() =>
            {
                zipHelper.UnzipFile(zipFile, DestFolder);

                progress?.Report(new()
                {
                    ZipFile = zipFile,
                    DestFolder = DestFolder
                });
            },
            cancellationToken));
            
            await Task.WhenAll(tasks);
        }

        private static void LogException(Exception e)
        {
            // Todo: Create and use logger.
        }

        private void ValidateFolder(string folderValue, string folderName)
        {
            if (string.IsNullOrWhiteSpace(folderValue))
            {
                throw new InvalidOperationException($"{folderName} missing.");
            }

            if (!pathHelper.IsValidAbsolutePathToExistingDirectory(SourceFolder))
            {
                throw new InvalidOperationException($"{folderName} is not a valid path. " +
                    "Given path must be a valid, absolute path, to an existing directory.");
            }

            // It is possible to foresee the maximum length of a source path, for every zip file.
            // But it is not really possible to foresee the maximum length of a destination path,
            // after all of the zip file contents (files and subfolders) are also included there.
            // Therefore, as a "rule of thumb", the half of the maximum path length is used here.
            // It seems totally fine to me, to let the unzip operation fail gracefully (throwing
            // an exception), if in some special case a complete path exceeds the maximum length.

            var maxLength = 260 / 2;

            if (folderValue.Length > maxLength)
            {
                throw new InvalidOperationException($"{folderName} path is too long. " +
                    $"Make sure given path is smaller than {maxLength} characters.");
            }
        }
    }
}
