namespace WAUZ.BL
{
    public sealed class FileSystemHelper : IFileSystemHelper
    {
        public bool IsValidAbsolutePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or whitespace.", nameof(path));
            }

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

        public async Task DeleteFolderContentAsync(string folder, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(folder))
            {
                throw new ArgumentException($"'{nameof(folder)}' cannot be null or whitespace.", nameof(folder));
            }

            if (!Directory.Exists(folder))
            {
                throw new InvalidOperationException("Given folder not exists.");
            }

            folder = Path.TrimEndingDirectorySeparator(Path.GetFullPath(folder));

            // It does not matter which .NET methods are used to enumerate files and folders.
            // At least this was the result of measurements i did for the following methods:
            // - Directory.GetXXX()
            // - Directory.EnumerateXXX()
            // - DirectoryInfo.GetXXX()
            // - DirectoryInfo.EnumerateXXX()
            // All had the exact same performance and it does not matter which to use. If at all,
            // it is more beneficial to use directly the xxxFiles() and xxxDirectories() versions,
            // instead of the xxxFileSystemEntries() and xxxFileSystemInfos() methods. Cause the
            // latter ones need some additional if-clauses then, to differ files from directories.

            var files = Directory.EnumerateFiles(folder);
            var directories = Directory.EnumerateDirectories(folder);

            if (!files.Any() && !directories.Any())
            {
                return;
            }

            // After some measurements this async approach seems to be around 3 times faster than
            // the sync approach. Looks like a modern SSD/OS seems to be rather concurrent-friendly.

            var tasks = new List<Task>();

            // No need for ThrowIfCancellationRequested() here, since Task.Run() cancels on its own if the
            // task has not already started and since the sync method one-liner can not be cancelled anyway.

            tasks.AddRange(files.Select(s => Task.Run(() => File.Delete(s), cancellationToken)));
            tasks.AddRange(directories.Select(s => Task.Run(() => Directory.Delete(s, true), cancellationToken)));

            await Task.WhenAll(tasks).ConfigureAwait(false);

            // Wait for deletion, as described at:
            // https://stackoverflow.com/questions/34981143/is-directory-delete-create-synchronous

            var counter = 0;

            while (Directory.EnumerateFileSystemEntries(folder).Any())
            {
                await Task.Delay(50, cancellationToken).ConfigureAwait(false);

                // Throw exception after ~500ms to prevent blocking forever.

                counter++;

                if (counter > 10)
                {
                    throw new InvalidOperationException("Could not delete folder content.");
                }
            }
        }

        public Task MoveFolderContentAsync(string sourceFolder, string destFolder, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(sourceFolder))
            {
                throw new ArgumentException($"'{nameof(sourceFolder)}' cannot be null or whitespace.", nameof(sourceFolder));
            }

            if (string.IsNullOrWhiteSpace(destFolder))
            {
                throw new ArgumentException($"'{nameof(destFolder)}' cannot be null or whitespace.", nameof(destFolder));
            }

            if (!Directory.Exists(sourceFolder))
            {
                throw new InvalidOperationException("Given source folder not exists.");
            }

            if (!Directory.Exists(destFolder))
            {
                throw new InvalidOperationException("Given destination folder not exists.");
            }

            sourceFolder = Path.TrimEndingDirectorySeparator(Path.GetFullPath(sourceFolder));
            destFolder = Path.TrimEndingDirectorySeparator(Path.GetFullPath(destFolder));

            var sourceFiles = Directory.EnumerateFiles(sourceFolder);
            var sourceDirectories = Directory.EnumerateDirectories(sourceFolder);

            if (!sourceFiles.Any() && !sourceDirectories.Any())
            {
                return Task.CompletedTask;
            }

            var buildDestPathFunc = (string sourcePath, string destFolder) =>
            {
                // In .NET the Path.GetFileName() method is used for both:
                // To get a file´s name, as well as to get a folder´s name.

                var fileOrFolderName = Path.GetFileName(sourcePath);
                var destPath = Path.Combine(destFolder, fileOrFolderName);

                return Path.TrimEndingDirectorySeparator(destPath);
            };

            var tasks = new List<Task>();

            // No need for ThrowIfCancellationRequested() here, since Task.Run() cancels on its own if the
            // task has not already started and since the sync method one-liner can not be cancelled anyway.

            tasks.AddRange(sourceFiles.Select(s => Task.Run(() => File.Move(s, buildDestPathFunc(s, destFolder)), cancellationToken)));
            tasks.AddRange(sourceDirectories.Select(s => Task.Run(() => Directory.Move(s, buildDestPathFunc(s, destFolder)), cancellationToken)));

            return Task.WhenAll(tasks);
        }

        public async Task<string> CreateTempFolderAsync(CancellationToken cancellationToken = default)
        {
            var userTempFolder = Path.GetFullPath(Path.GetTempPath());

            var tempFolder = Path.TrimEndingDirectorySeparator(Path.Combine(userTempFolder, "MBODM.WAUZ.tmp"));

            // Creating a task is more time-costy than calling Exists() directly.

            if (Directory.Exists(tempFolder))
            {
                // Creating a task for calling Delete() since its duration is not predictable.

                await Task.Run(() => Directory.Delete(tempFolder, true), cancellationToken).ConfigureAwait(false);
            }

            // Creating a task is more time-costy than calling CreateDirectory() directly.

            Directory.CreateDirectory(tempFolder);

            return tempFolder;
        }
    }
}
