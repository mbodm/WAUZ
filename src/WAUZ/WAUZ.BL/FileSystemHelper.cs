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

            // Normally some further input validations (i.e. if path is an existing folder) should happen here,
            // since this helper is an independent module and not some private method. But it seems fine to me
            // if the code below will do that job instead and maybe fails gracefully then. And not doing those
            // validations twice (since the app logic does them anyway) seems to be more beneficial here to me.

            var files = Directory.EnumerateFiles(folder);
            var directories = Directory.EnumerateDirectories(folder);

            if (!files.Any() && !directories.Any())
            {
                return;
            }

            // This async approach is around 3 times faster than the sync approach, after some
            // measurements. Looks like a modern SSD/OS seems to be rather concurrent-friendly.

            var tasks = new List<Task>();

            // No need for ThrowIfCancellationRequested() here, since Task.Run() will cancel on its own,
            // if task not already started. Which results in exactly the same behavior, since this task
            // is somewhat "atomic", by just calling a single method, which can not be cancelled anyway.

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

        public async Task MoveFolderContentAsync(string sourceFolder, string destinationFolder, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
