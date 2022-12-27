namespace WAUZ.BL
{
    public interface IFileSystemHelper
    {
        bool IsValidAbsolutePath(string path);
        // Todo: CreateTempFolder() Aber diesmal alle in 1 Folder und das vorher löschen
        Task DeleteFolderContentAsync(string folder, CancellationToken cancellationToken = default);
        Task MoveFolderContentAsync(string sourceFolder, string destinationFolder, CancellationToken cancellationToken = default);
    }
}
