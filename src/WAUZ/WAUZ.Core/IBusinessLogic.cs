namespace WAUZ.Core
{
    public interface IBusinessLogic
    {
        string SourceFolder { get; set; }
        string DestFolder { get; set; }

        void LoadSettings();
        void SaveSettings();

        void OpenSourceFolderInExplorer();
        void OpenDestFolderInExplorer();

        int CountZipFiles();
        Task<long> UnzipAsync(IProgress<ProgressData>? progress = default, CancellationToken cancellationToken = default);

        void LogUnexpectedException(Exception e);
    }
}
