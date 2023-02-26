namespace WAUZ.Helpers
{
    public interface IFileSystemHelper
    {
        bool IsValidAbsolutePath(string path);
        Task DeleteFolderContentAsync(string folder, CancellationToken cancellationToken = default);
        IEnumerable<string> GetAllZipFilesInFolder(string folder);
    }
}
