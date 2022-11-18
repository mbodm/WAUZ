namespace WAUZ.BL
{
    public interface IBusinessLogic
    {
        string SourceFolder { get; set; }
        string DestFolder { get; set; }
        
        void LoadSettings();
        void SaveSettings();

        Task UnzipFiles(IProgress<ProgressData>? progress = default, CancellationToken cancellationToken = default);
    }
}
