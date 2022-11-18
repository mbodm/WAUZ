namespace WAUZ.BL
{
    public sealed class BusinessLogic : IBusinessLogic
    {
        private readonly ISettingsHelper settingsHelper;
        private readonly IZipHelper zipHelper;

        public BusinessLogic(ISettingsHelper settingsHelper, IZipHelper zipHelper)
        {
            this.settingsHelper = settingsHelper ?? throw new ArgumentNullException(nameof(settingsHelper));
            this.zipHelper = zipHelper ?? throw new ArgumentNullException(nameof(zipHelper));
        }

        public string SourceFolder { get; set; } = string.Empty;
        public string DestFolder { get; set; } = string.Empty;
        
        public void LoadSettings()
        {
            var desktopFolder = Path.TrimEndingDirectorySeparator(
                Path.GetFullPath(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.Desktop)));

            settingsHelper.Load();
            
            if (settingsHelper.Settings.TryGetValue(nameof(SourceFolder), out var sourceFolder))
            {
                SourceFolder = sourceFolder;
            }
            else
            {
                // Default source folder

                SourceFolder = desktopFolder;
            }

            if (settingsHelper.Settings.TryGetValue(nameof(DestFolder), out var destFolder))
            {
                DestFolder = destFolder;
            }
            else
            {
                // Default dest folder

                var wowAddonsDefaultFolder = @"C:\Program Files (x86)\World of Warcraft\_retail_\Interface\AddOns";

                if (Directory.Exists(wowAddonsDefaultFolder))
                {
                    DestFolder = Path.TrimEndingDirectorySeparator(
                        Path.GetFullPath(
                            wowAddonsDefaultFolder));
                }
                else
                {
                    DestFolder = desktopFolder;
                }
            }
        }

        public void SaveSettings()
        {
            settingsHelper.Settings["SourceFolder"] = SourceFolder;
            settingsHelper.Settings["DestFolder"] = DestFolder;

            settingsHelper.Save();
        }

        public async Task UnzipFiles(IProgress<ProgressData>? progress = null, CancellationToken cancellationToken = default)
        {
            var zipFiles = GetZipFiles();

            var destFolder = GetDestFolder();

            var tasks = zipFiles.Select(zipFile => Task.Run(() =>
            {
                zipHelper.UnzipFile(zipFile, destFolder);

                var progessData = new ProgressData
                {
                    ZipFile = zipFile,
                    DestFolder = destFolder
                };

                progress?.Report(progessData);
            }));

            await Task.WhenAll(tasks);
        }

        private IEnumerable<string> GetZipFiles()
        {
            if (string.IsNullOrWhiteSpace(SourceFolder))
            {
                throw new InvalidOperationException("Todo - Empty source folder.");
            }

            if (!Directory.Exists(SourceFolder))
            {
                throw new InvalidOperationException("Todo - Source folder not exists.");
            }

            var zipFiles = Directory.GetFiles(SourceFolder, "*.zip", SearchOption.TopDirectoryOnly);

            if (!zipFiles.Any())
            {
                throw new InvalidOperationException("Todo - Source folder not contains any zip files.");
            }

            return zipFiles;
        }

        private string GetDestFolder()
        {
            if (string.IsNullOrWhiteSpace(DestFolder))
            {
                throw new InvalidOperationException("Todo - Empty dest folder.");
            }

            if (!Directory.Exists(DestFolder))
            {
                throw new InvalidOperationException("Todo - Dest folder not exists.");
            }

            return DestFolder;
        }
    }
}
