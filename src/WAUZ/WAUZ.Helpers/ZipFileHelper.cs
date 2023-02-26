using System.IO.Compression;

namespace WAUZ.Helpers
{
    public sealed class ZipFileHelper : IZipFileHelper
    {
        public Task<bool> ValidateZipFileAsync(string zipFile, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(zipFile))
            {
                throw new ArgumentException($"'{nameof(zipFile)}' cannot be null or whitespace.", nameof(zipFile));
            }

            // No need for a ThrowIfCancellationRequested() here, since Task.Run() cancels on its own (if the
            // task has not already started) and since the sync .ToList() method can not be cancelled anyway.

            return Task.Run(() =>
            {
                try
                {
                    // Validate if zip file is readable (not corrupted) and contains content.
                    // Call .ToList() on the archive entries to perform a full content read.

                    using var zipArchive = ZipFile.OpenRead(zipFile);

                    var hasContent = zipArchive.Entries.ToList().Any();

                    if (!hasContent)
                    {
                        return false;
                    }

                    return true;
                }
                catch
                {
                    return false;
                }
            },
            cancellationToken);
        }

        public Task ExtractZipFileAsync(string zipFile, string destFolder, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(zipFile))
            {
                throw new ArgumentException($"'{nameof(zipFile)}' cannot be null or whitespace.", nameof(zipFile));
            }

            if (string.IsNullOrWhiteSpace(destFolder))
            {
                throw new ArgumentException($"'{nameof(destFolder)}' cannot be null or whitespace.", nameof(destFolder));
            }

            // No need for a ThrowIfCancellationRequested() here, since Task.Run() cancels on its own (if the
            // task has not already started) and since the sync method one-liner can not be cancelled anyway.

            return Task.Run(() => ZipFile.ExtractToDirectory(zipFile, destFolder, true), cancellationToken);
        }
    }
}
