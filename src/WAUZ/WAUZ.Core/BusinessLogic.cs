using System.Diagnostics;
using WAUZ.Helpers;

namespace WAUZ.Core
{
    public sealed class BusinessLogic : IBusinessLogic
    {
        private readonly IAppSettings appSettings;
        private readonly IErrorLogger errorLogger;
        private readonly IFolderValidator folderValidator;
        private readonly IFileSystemHelper fileSystemHelper;
        private readonly IZipFileHelper zipFileHelper;

        public BusinessLogic(
            IAppSettings appSettings,
            IErrorLogger errorLogger,
            IFolderValidator folderValidator,
            IFileSystemHelper fileSystemHelper,
            IZipFileHelper zipFileHelper)
        {
            this.appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            this.errorLogger = errorLogger ?? throw new ArgumentNullException(nameof(errorLogger));
            this.folderValidator = folderValidator ?? throw new ArgumentNullException(nameof(folderValidator));
            this.fileSystemHelper = fileSystemHelper ?? throw new ArgumentNullException(nameof(fileSystemHelper));
            this.zipFileHelper = zipFileHelper ?? throw new ArgumentNullException(nameof(zipFileHelper));
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
                errorLogger.Log(e);
                throw new InvalidOperationException("Found settings file, but an error occurred while loading the settings (see log file for details).");
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
                errorLogger.Log(e);
                throw new InvalidOperationException("An error occurred while saving the settings (see log file for details).");
            }
        }

        public void OpenSourceFolderInExplorer()
        {
            folderValidator.ValidateSourceFolder(SourceFolder);

            OpenFolderInExplorer(SourceFolder);
        }

        public void OpenDestFolderInExplorer()
        {
            folderValidator.ValidateDestFolder(DestFolder);

            OpenFolderInExplorer(DestFolder);
        }

        public int CountZipFiles()
        {
            folderValidator.ValidateSourceFolder(SourceFolder);

            return GetZipFiles().Count();
        }

        public async Task<long> UnzipAsync(IProgress<ProgressData>? progress = null, CancellationToken cancellationToken = default)
        {
            folderValidator.ValidateSourceFolder(SourceFolder);
            folderValidator.ValidateDestFolder(DestFolder);

            // Next line is done before first progress on purpose, cause of ValidationException concept.

            var zipFiles = GetZipFiles();

            progress?.Report(new(ProgressState.Started));

            var stopwatch = Stopwatch.StartNew();

            progress?.Report(new(ProgressState.ValidateZipFiles));

            var validateTasks = zipFiles.Select(zipFile => zipFileHelper.ValidateZipFileAsync(zipFile, cancellationToken));
            bool[] validateResults;

            try
            {
                validateResults = await Task.WhenAll(validateTasks).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                errorLogger.Log(ex);
                throw new InvalidOperationException("An error occurred while validating the zip files in Source-Folder (see log file for details).");
            }

            if (validateResults.Contains(false))
            {
                errorLogger.Log("Open or read failed for one or more zip files.");
                var additionalHint = "No data in Destination-Folder has changed!";
                throw new InvalidOperationException($"Source-Folder contains corrupted zip files (see log file for details). {additionalHint}");
            }

            progress?.Report(new(ProgressState.ValidatedZipFiles));

            progress?.Report(new(ProgressState.ClearDestFolder));

            try
            {
                await fileSystemHelper.DeleteFolderContentAsync(DestFolder, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                errorLogger.Log(ex);
                throw new InvalidOperationException("An error occurred while deleting the content of Destination-Folder (see log file for details).");
            }

            progress?.Report(new(ProgressState.ClearedDestFolder));

            var unzipTasks = zipFiles.Select(zipFile => Task.Run(() =>
            {
                // No need for ThrowIfCancellationRequested() here, since Task.Run() cancels on its own if the task
                // has not already started. Also this workload is "atomic" (if a file was unzipped, it is a progress).

                progress?.Report(new(ProgressState.UnzipAddon, zipFile));

                try
                {
                    // No need for an async TAP call here, since this runs already inside a Task.Run() method.
                    // Therefore the TAP call is done sync here, to not create some unnecessary awaiter effort.

                    zipFileHelper.ExtractZipFileAsync(zipFile, DestFolder, cancellationToken).Wait();
                }
                catch (Exception e)
                {
                    errorLogger.Log(e);
                    throw;
                }

                progress?.Report(new(ProgressState.UnzippedAddon, zipFile));
            },
            cancellationToken));

            try
            {
                await Task.WhenAll(unzipTasks).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                errorLogger.Log(e);
                throw new InvalidOperationException("An error occurred while extracting the zip files (see log file for details).");
            }

            progress?.Report(new(ProgressState.Finished));

            stopwatch.Stop();

            return stopwatch.ElapsedMilliseconds;
        }

        public void LogUnexpectedException(Exception e)
        {
            errorLogger.Log(e);
            errorLogger.Log("An unexpected exception was catched and logged manually, from outside of business logic.");
        }

        private void OpenFolderInExplorer(string folder)
        {
            try
            {
                Process.Start("explorer", folder);
            }
            catch (Exception e)
            {
                errorLogger.Log(e);
                throw new InvalidOperationException("Could not start Explorer.exe process (see log file for details).");
            }
        }

        private IEnumerable<string> GetZipFiles()
        {
            IEnumerable<string> zipFiles;

            try
            {
                zipFiles = fileSystemHelper.GetAllZipFilesInFolder(SourceFolder);
            }
            catch (Exception e)
            {
                errorLogger.Log(e);
                throw new InvalidOperationException("Could not determine zip files in Source-Folder (see log file for details).");
            }

            if (!zipFiles.Any())
            {
                throw new ValidationException("Source-Folder not contains any zip files.");
            }

            return zipFiles;
        }
    }
}
