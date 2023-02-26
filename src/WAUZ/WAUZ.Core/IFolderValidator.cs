namespace WAUZ.Core
{
    public interface IFolderValidator
    {
        void ValidateSourceFolder(string sourceFolder);
        void ValidateDestFolder(string destFolder);
    }
}
