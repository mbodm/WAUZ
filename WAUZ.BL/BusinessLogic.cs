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
            try
            {
                settingsHelper.Load();
            }
            catch (Exception e)
            {
                LogException(e);

                throw new InvalidOperationException("An error occurred while loading the settings (see log file for details).");
            }

            // It is possible to use the null-forgiving operator ("!") here, or declare the out variable as nullable, like mentioned in this post:
            // https://stackoverflow.com/questions/58681729/net-non-nullable-reference-type-and-out-parameters

            SourceFolder = settingsHelper.Settings.TryGetValue(nameof(SourceFolder), out string? sourceFolder) ? sourceFolder : string.Empty;
            DestFolder = settingsHelper.Settings.TryGetValue(nameof(DestFolder), out string? destFolder) ? destFolder : string.Empty;
        }

        public void SaveSettings()
        {
            settingsHelper.Settings[nameof(SourceFolder)] = SourceFolder;
            settingsHelper.Settings[nameof(DestFolder)] = DestFolder;

            settingsHelper.Save();
        }

        // Todo: Wann mach "fullpath und trimending von SourceFolder und DestFolder ?
        // - Beim zuweisen ?
        // - Im Unzip ?
        // - Darauf verlassen dass es korrekt von aussen kommt ?
        // - Soll es zurückgeschrieben werden und im SettingsFile landen oder nur genutzt werden ?

        public async Task Unzip(IProgress<ProgressData>? progress = null, CancellationToken cancellationToken = default)
        {
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
                throw new InvalidOperationException("Source folder path is too long. Make sure given path is smaller than 200 characters.");
            }

            if (DestFolder.Length > maxPathLength)
            {
                throw new InvalidOperationException("Destination folder path is too long. Make sure given path is smaller than 200 characters.");
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
