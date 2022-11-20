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

            if (appSettings.Settings.TryGetValue(nameof(SourceFolder), out var sourceFolder))
            {
                SourceFolder = sourceFolder;
            }

            if (appSettings.Settings.TryGetValue(nameof(DestFolder), out var destFolder))
            {
                DestFolder = destFolder;
            }
        }

        public void SaveSettings()
        {
            appSettings.Settings[nameof(SourceFolder)] = SourceFolder;
            appSettings.Settings[nameof(DestFolder)] = DestFolder;

            appSettings.Save();
        }

        public async Task Unzip(IProgress<ProgressData>? progress = null, CancellationToken cancellationToken = default)
        {
            SourceFolder = pathHelper.GetFullPathWithoutEndingDirectorySeparator(SourceFolder);
            DestFolder = pathHelper.GetFullPathWithoutEndingDirectorySeparator(DestFolder);

            Validate();

            var tasks = GetZipFiles().Select(zipFile => Task.Run(() =>
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

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(SourceFolder))
            {
                throw new InvalidOperationException("Source folder missing.");
            }

            if (!Directory.Exists(SourceFolder))
            {
                throw new InvalidOperationException("Source folder not exists.");
            }

            if (string.IsNullOrWhiteSpace(DestFolder))
            {
                throw new InvalidOperationException("Destination folder missing.");
            }

            if (!Directory.Exists(DestFolder))
            {
                throw new InvalidOperationException("Destination folder not exists.");
            }

            var maxPathLength = 200;

            if (SourceFolder.Length > maxPathLength)
            {
                throw new InvalidOperationException("Absolute source folder path is too long." +
                    "Make sure the absolute path for given path is smaller than 200 characters.");
            }

            if (DestFolder.Length > maxPathLength)
            {
                throw new InvalidOperationException("Absolute destination folder path is too long." +
                    "Make sure the absoulte path for given path is smaller than 200 characters.");
            }

            if (!GetZipFiles().Any())
            {
                throw new InvalidOperationException("Source folder not contains any zip files.");
            }
        }

        private IEnumerable<string> GetZipFiles()
        {
            return Directory.GetFiles(SourceFolder, "*.zip", SearchOption.TopDirectoryOnly);
        }
    }
}
