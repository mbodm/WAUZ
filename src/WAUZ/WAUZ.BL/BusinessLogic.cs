using System.Diagnostics;
using System.IO.Compression;

namespace WAUZ.BL
{
    public sealed class BusinessLogic : IBusinessLogic
    {
        private readonly IAppLogging appLogging;
        private readonly IAppSettings appSettings;
        private readonly IFileSystemHelper fileSystemHelper;

        public BusinessLogic(IAppLogging appLogging, IAppSettings appSettings, IFileSystemHelper fileSystemHelper)
        {
            this.appLogging = appLogging ?? throw new ArgumentNullException(nameof(appLogging));
            this.appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            this.fileSystemHelper = fileSystemHelper ?? throw new ArgumentNullException(nameof(fileSystemHelper));
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

        public string ValidateSourceFolder() => ValidateFolder(SourceFolder, "Source-Folder");

        public string ValidateDestFolder() => ValidateFolder(DestFolder, "Destination-Folder");

        public IEnumerable<string> GetZipFiles()
        {
            var sourceFolder = ValidateSourceFolder();
            var zipFiles = Directory.EnumerateFiles(sourceFolder, "*.zip", SearchOption.TopDirectoryOnly);

            if (!zipFiles.Any())
            {
                throw new InvalidOperationException("Source-Folder not contains any zip files.");
            }

            return zipFiles;
        }

        public async Task<long> UnzipAsync(IProgress<UnzipProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            var zipFiles = GetZipFiles();
            var destFolder = ValidateDestFolder();

            var stopwatch = Stopwatch.StartNew();

            var tempFolder = await fileSystemHelper.CreateTempFolderAsync(cancellationToken).ConfigureAwait(false);

            var tasks = zipFiles.Select(zipFile => Task.Run(() =>
            {
                // No need for ThrowIfCancellationRequested() here, since Task.Run() cancels on its own if the task
                // has not already started. Also this workload is "atomic" (if a file was unzipped, it is a progress).

                try
                {
                    ZipFile.ExtractToDirectory(zipFile, tempFolder);
                }
                catch (Exception e)
                {
                    appLogging.Log(e);

                    throw;
                }

                progress?.Report(new() { ZipFile = zipFile });
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

            await fileSystemHelper.DeleteFolderContentAsync(destFolder, cancellationToken).ConfigureAwait(false);
            await fileSystemHelper.MoveFolderContentAsync(tempFolder, destFolder, cancellationToken).ConfigureAwait(false);

            stopwatch.Stop();

            return stopwatch.ElapsedMilliseconds;
        }

        private string ValidateFolder(string folderValue, string folderName)
        {
            if (string.IsNullOrWhiteSpace(folderValue))
            {
                throw new InvalidOperationException($"{folderName} missing.");
            }

            if (!fileSystemHelper.IsValidAbsolutePath(folderValue) || !Directory.Exists(folderValue))
            {
                throw new InvalidOperationException($"{folderName} is not a valid path. " +
                    "Given path must be a valid, absolute path, to an existing folder.");
            }

            // Easy to foresee max length of source. Not that easy to foresee max length of dest, when considering content of
            // zip file (files and subfolders). Therefore just using half of MAX_PATH here, as some "rule of thumb". If in a
            // rare case a full dest path exceeds MAX_PATH, it seems ok to let the unzip operation fail gracefully on its own.

            var maxPath = 260;
            var maxLength = maxPath / 2;

            if (folderValue.Length > maxLength)
            {
                throw new InvalidOperationException($"{folderName} path is too long. " +
                    $"Make sure given path is smaller than {maxLength} characters.");
            }

            return Path.TrimEndingDirectorySeparator(Path.GetFullPath(folderValue));
        }
    }
}
