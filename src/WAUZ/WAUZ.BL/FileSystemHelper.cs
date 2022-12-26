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

        public async Task DeleteFolderContentAsync(string folder)
        {
            if (string.IsNullOrWhiteSpace(folder))
            {
                throw new ArgumentException($"'{nameof(folder)}' cannot be null or whitespace.", nameof(folder));
            }

            // Doing no further input validation here (i.e. if path is an existing folder). It
            // seems fine to me if below code will do that job and maybe fails gracefully then,
            // since such checks will be done by the logic anyway before this method is called.

            var directoryInfo = new DirectoryInfo(folder);

            // This async approach is around 3 times faster than the sync approach, after some
            // measurements. Looks like a modern SSD/OS seems to be rather concurrent-friendly.

            var tasks = directoryInfo.EnumerateFileSystemInfos().Select(fsi => Task.Run(() =>
            {
                // Need to differ here, since FileSystemInfo.Delete() only works with empty
                // folders. And using this early-exit approach to ignore the rare case when
                // a FileSystemInfo object is wether a file nor a folder. Just to make sure.

                if (fsi is DirectoryInfo di)
                {
                    di.Delete(true);

                    return;
                }

                if (fsi is FileInfo fi)
                {
                    fi.Delete();

                    return;
                }
            }));

            await Task.WhenAll(tasks).ConfigureAwait(false);

            // Wait for deletion, as described at:
            // https://stackoverflow.com/questions/34981143/is-directory-delete-create-synchronous

            var counter = 0;

            while (directoryInfo.EnumerateFileSystemInfos().Any())
            {
                await Task.Delay(50).ConfigureAwait(false);

                // Throw exception after ~500ms to prevent blocking forever.

                counter++;

                if (counter > 10)
                {
                    throw new InvalidOperationException("Could not delete folder content.");
                }
            }
        }
    }
}
