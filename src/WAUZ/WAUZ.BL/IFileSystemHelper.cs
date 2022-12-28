namespace WAUZ.BL
{
    public interface IFileSystemHelper
    {
        bool IsValidAbsolutePath(string path);
        Task<string> CreateTempFolderAsync(CancellationToken cancellationToken = default);
        Task DeleteFolderContentAsync(string folder, CancellationToken cancellationToken = default);
        Task MoveFolderContentAsync(string sourceFolder, string destFolder, CancellationToken cancellationToken = default);
    }
}
