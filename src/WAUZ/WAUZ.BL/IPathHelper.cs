namespace WAUZ.BL
{
    public interface IPathHelper
    {
        bool PathExists(string path);
        bool PathExistsAndIsFile(string path);
        bool PathExistsAndIsDirectory(string path);
        string GetParentDirectoryFromPath(string path);
        string GetFileOrDirectoryNameFromPath(string path);
        string GetFullPathWithoutEndingDirectorySeparator(string path);
    }
}
