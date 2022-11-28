namespace WAUZ.BL
{
    public sealed class BusinessLogic : IBusinessLogic
    {
        private readonly IAppSettings appSettings;
        private readonly IAppLogging appLogging;
        private readonly IPathHelper pathHelper;
        private readonly IZipHelper zipHelper;

        public BusinessLogic(IAppSettings appSettings, IAppLogging appLogging, IPathHelper pathHelper, IZipHelper zipHelper)
        {
            this.appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            this.appLogging = appLogging ?? throw new ArgumentNullException(nameof(appLogging));
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
                appLogging.Log(e);

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

            try
            {
                appSettings.Save();
            }
            catch (Exception e)
            {
                appLogging.Log(e);

                throw new InvalidOperationException("An error occurred while saving the settings (see log file for details).");
            }
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

        public async Task UnzipAsync(IProgress<ProgressData>? progress = null, CancellationToken cancellationToken = default)
        {
            var zipFiles = GetZipFiles();

            ValidateFolder(DestFolder, "Destination-Folder");

            var tasks = zipFiles.Select(zipFile => Task.Run(() =>
            {
                // There is no need to call the token´s ThrowIfCancellationRequested() method here,
                // since the Task.Run() method will cancel on its own, if the task was not already
                // started. This results in the exact same behaviour, as checking for cancellation
                // here, before unzipping. Additionally the unzipping and reporting here should be
                // viewed as an atomar process, because when a file has unzipped, it is a progress.

                try
                {
                    zipHelper.UnzipFile(zipFile, DestFolder);
                }
                catch (Exception e)
                {
                    appLogging.Log(e);

                    throw;
                }

                progress?.Report(new()
                {
                    ZipFile = zipFile,
                    DestFolder = Path.TrimEndingDirectorySeparator(DestFolder)
                });
            },
            cancellationToken));

            try
            {
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                appLogging.Log(e);

                throw new InvalidOperationException("An error occurred while extracting the zip files (see log file for details).");
            }
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
