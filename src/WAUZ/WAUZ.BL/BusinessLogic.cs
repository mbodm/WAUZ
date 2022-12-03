namespace WAUZ.BL
{
    public sealed class BusinessLogic : IBusinessLogic
    {
        private readonly IAppSettings appSettings;
        private readonly IAppLogging appLogging;
        private readonly IZipHelper zipHelper;

        public BusinessLogic(IAppSettings appSettings, IAppLogging appLogging, IZipHelper zipHelper)
        {
            this.appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            this.appLogging = appLogging ?? throw new ArgumentNullException(nameof(appLogging));
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

            if (appSettings.Settings.TryGetValue("destination", out var destFolder))
            {
                DestFolder = destFolder;
            }
        }

        public void SaveSettings()
        {
            appSettings.Settings["source"] = SourceFolder;
            appSettings.Settings["destination"] = DestFolder;

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

        public void ValidateSourceFolder()
        {
            ValidateFolder(SourceFolder, "Source-Folder");
        }

        public void ValidateDestFolder()
        {
            ValidateFolder(DestFolder, "Destination-Folder");
        }

        public IEnumerable<string> GetZipFiles()
        {
            ValidateSourceFolder();

            var sourceFolder = Path.TrimEndingDirectorySeparator(Path.GetFullPath(SourceFolder)); // Just to make sure

            var zipFiles = Directory.GetFiles(sourceFolder, "*.zip", SearchOption.TopDirectoryOnly);

            if (!zipFiles.Any())
            {
                throw new InvalidOperationException("Source-Folder not contains any zip files.");
            }

            return zipFiles;
        }

        public async Task UnzipAsync(IProgress<ProgressData>? progress = null, CancellationToken cancellationToken = default)
        {
            var zipFiles = GetZipFiles();

            ValidateDestFolder();

            var destFolder = Path.TrimEndingDirectorySeparator(Path.GetFullPath(DestFolder));  // Just to make sure

            CleanUpTempFolder();

            var tasks = zipFiles.Select(zipFile => Task.Run(() =>
            {
                // No need for ThrowIfCancellationRequested() here, since Task.Run() will cancel on its own,
                // if task not already started. Which results in exactly the same behavior, since unzipping
                // and reporting should be viewed as atomic. Cause if a file was unzipped, it is a progress.

                try
                {
                    zipHelper.UnzipFile(zipFile, destFolder);
                }
                catch (Exception e)
                {
                    appLogging.Log(e);

                    throw;
                }

                progress?.Report(new()
                {
                    ZipFile = zipFile,
                    DestFolder = destFolder
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

        private static void ValidateFolder(string folderValue, string folderName)
        {
            if (string.IsNullOrWhiteSpace(folderValue))
            {
                throw new InvalidOperationException($"{folderName} missing.");
            }

            if (!IsValidAbsolutePath(folderValue) || !Directory.Exists(folderValue))
            {
                throw new InvalidOperationException($"{folderName} is not a valid path. " +
                    "Given path must be a valid, absolute path, to an existing folder.");
            }

            // Easy to foresee max length of source. Not that easy to foresee max length of dest,
            // when also considering zip file content (files and subfolders). Instead just using
            // half of MAX_PATH here, as some "rule of thumb". If in some rare cases a full dest
            // path exceeds MAX_PATH, it is ok to let unzip operation fail gracefully on its own.

            var maxPath = 260;
            var maxLength = maxPath / 2;

            if (folderValue.Length > maxLength)
            {
                throw new InvalidOperationException($"{folderName} path is too long. " +
                    $"Make sure given path is smaller than {maxLength} characters.");
            }
        }

        private static bool IsValidAbsolutePath(string path)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(path))
                {
                    var parentFolder = Path.GetDirectoryName(path);

                    if (!string.IsNullOrEmpty(parentFolder))
                    {
                        if (!Path.GetInvalidPathChars().Any(invalidPathChar => parentFolder.Contains(invalidPathChar)))
                        {
                            var fileOrFolderName = Path.GetFileName(path);

                            if (!string.IsNullOrEmpty(fileOrFolderName))
                            {
                                if (!Path.GetInvalidFileNameChars().Any(invalidFileNameChar => fileOrFolderName.Contains(invalidFileNameChar)))
                                {
                                    if (Path.IsPathFullyQualified(path))
                                    {
                                        if (Path.IsPathRooted(path))
                                        {
                                            var root = Path.GetPathRoot(path);

                                            if (!string.IsNullOrEmpty(root))
                                            {
                                                var drives = DriveInfo.GetDrives().Select(drive => drive.Name);

                                                if (drives.Contains(root))
                                                {
                                                    return true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                // Hiding exceptions is intended design for this method.
            }

            return false;
        }

        private static void CleanUpTempFolder()
        {
            var userTempFolder = Path.TrimEndingDirectorySeparator(Path.GetFullPath(Path.GetTempPath()));

            var tempFolders = Directory.GetDirectories(userTempFolder, "MBODM.WAUZ.*.tmp", SearchOption.TopDirectoryOnly);

            foreach (var tempFolder in tempFolders)
            {
                if (Directory.Exists(tempFolder))
                {
                    Directory.Delete(tempFolder, true);
                }
            }
        }
    }
}
