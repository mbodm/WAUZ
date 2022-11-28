namespace WAUZ.BL
{
    public interface IPathHelper
    {
        bool IsValidAbsolutePath(string path);
        bool IsValidAbsolutePathToExistingFile(string path);
        bool IsValidAbsolutePathToExistingDirectory(string path);
        string GetParentDirectoryFromValidAbsolutePath(string path);
        string GetFileOrDirectoryNameFromValidAbsolutePath(string path);
    }
}
