﻿namespace WAUZ.BL
{
    public sealed class FileSystemHelper : IFileSystemHelper
    {
        public void MoveFolderContent(string sourceFolder, string destFolder)
        {
            // Guard method arguments.

            if (string.IsNullOrWhiteSpace(sourceFolder))
            {
                throw new ArgumentException($"'{nameof(sourceFolder)}' cannot be null or whitespace.", nameof(sourceFolder));
            }

            if (string.IsNullOrWhiteSpace(destFolder))
            {
                throw new ArgumentException($"'{nameof(destFolder)}' cannot be null or whitespace.", nameof(destFolder));
            }

            // Rely on existing folders only.

            if (!Directory.Exists(sourceFolder))
            {
                throw new InvalidOperationException($"'{nameof(sourceFolder)}' has to be an existing folder.");
            }

            if (!Directory.Exists(destFolder))
            {
                throw new InvalidOperationException($"'{nameof(destFolder)}' has to be an existing folder.");
            }

            // Rely on well-formed folders only.

            sourceFolder = GetWellFormedFolder(sourceFolder);
            destFolder = GetWellFormedFolder(destFolder);

            // Create tuples, where every tuple represents a source path
            // and a corresponding destination path, to a file or folder.

            var tuples = CreateTuples(sourceFolder, destFolder);

            // Move all files and directories inside of source folder, into destination folder.
            // If destination folder already contains some of these files or directories, they
            // are deleted from the destination folder first, before the move operation starts.

            foreach (var (source, dest) in tuples)
            {
                // Determine if file.

                if (File.Exists(source) && !HasDirectoryAttribute(source))
                {
                    if (File.Exists(dest))
                    {
                        File.Delete(dest);
                    }

                    File.Move(source, dest);

                    continue;
                }

                // Determine if folder.

                if (Directory.Exists(source) && HasDirectoryAttribute(source))
                {
                    if (Directory.Exists(dest))
                    {
                        Directory.Delete(dest, true);
                    }

                    Directory.Move(source, dest);

                    continue;
                }

                // Throw an exception if source can not be determined, wether as file, nor as folder.

                throw new InvalidOperationException("The tuples contain a source path, which seems to be wether a file, nor a folder.");
            }
        }

        private static string GetWellFormedFolder(string folder)
        {
            // Logic in this class expects folders with absolute path
            // and with trailing slash/backslash trimmed (if existing).

            return Path.TrimEndingDirectorySeparator(Path.GetFullPath(folder));
        }

        private static IEnumerable<(string Source, string Dest)> CreateTuples(string sourceFolder, string destFolder)
        {
            // The MSDN page for the EnumerateFileSystemEntries() method has informations about the support of absolute and
            // relative paths, relating to the input argument of the method. But the MSDN page says nothing about the paths
            // returned by the method. Therefore i tested this by myself. Result: If the input argument is an absolute path,
            // all output paths are also absolute paths. And if the input argument is a relative path, all output paths are
            // also relative paths. Means: When it is safe to rely on the input here, it is also safe to rely on the output.

            return Directory.EnumerateFileSystemEntries(sourceFolder).Select(sourceEntry =>
            {
                // Not sure if a folder path can contain an ending separator here.
                // But such a pre-emptive ending separator trimming has to happen
                // anyway, for the Path.GetFileName() method below. Otherwise the
                // result of that method could lead to some undesirable behaviour.
                // So, maybe the trim call is unnecessary here. But: Safety first!

                var source = Path.TrimEndingDirectorySeparator(sourceEntry);

                // In .NET the Path.GetFileName() method is used for both jobs: To get the name of
                // the file, as well as to get the name of the folder, a given path is pointing to.
                // It is important to trim an existing ending separator char if used with a folder.
                // Also keep in mind: The method does not care if the file or folder (a given path
                // is pointing to) really exists and relies solely on the path string itself. Also
                // note: The method can throw, in older .NET versions, but this is totally ok here.

                var fileOrFolderName = Path.GetFileName(source);

                // It is important to throw an exception here imo,
                // instead of silently ignore some file or folder.

                if (string.IsNullOrEmpty(fileOrFolderName))
                {
                    throw new InvalidOperationException("Could not determine the name of some file or folder, while creating the tuples.");
                }

                // Build destination. Not sure if the Path.Combine() method will ever
                // return a folder with an ending separator at all. But: Safety first!

                var dest = Path.TrimEndingDirectorySeparator(Path.Combine(destFolder, fileOrFolderName));

                // Return tuple, containing source and destination.

                return (source, dest);
            });
        }

        private static bool HasDirectoryAttribute(string path)
        {
            return (File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory;
        }
    }
}
