namespace WAUZ.Helpers
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

            // It does not matter which .NET methods are used to enumerate files and folders here.
            // At least this was the result of some measurements i did for the following methods:
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

            // After some measurements this async approach seems to be around 3 times faster than the
            // sync approach. Looks like modern SSD/OS configurations are rather concurrent-friendly.

            var tasks = new List<Task>();

            // No need for a ThrowIfCancellationRequested() here, since Task.Run() cancels on its own (if the
            // task has not already started) and since the sync method one-liner can not be cancelled anyway.

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

        public IEnumerable<string> GetAllZipFilesInFolder(string folder)
        {
            return Directory.EnumerateFiles(folder, "*.zip", SearchOption.TopDirectoryOnly);
        }
    }
}
