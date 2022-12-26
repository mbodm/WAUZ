namespace WAUZ.BL
{
    public interface IFileSystemHelper
    {
        bool IsValidAbsolutePath(string path);
        Task DeleteFolderContentAsync(string folder);
    }
}
