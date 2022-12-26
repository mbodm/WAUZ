namespace WAUZ.BL
{
    public interface IBusinessLogic
    {
        string SourceFolder { get; set; }
        string DestFolder { get; set; }

        void LoadSettings();
        void SaveSettings();

        string ValidateSourceFolder();
        string ValidateDestFolder();

        IEnumerable<string> GetZipFiles();
        Task<long> UnzipAsync(IProgress<ProgressData>? progress = default, CancellationToken cancellationToken = default);
    }
}
