namespace WAUZ.BL
{
    public interface IBusinessLogic
    {
        Task UnzipFiles(IEnumerable<string> zipFiles, string destFolder,
            IProgress<ProgressData>? progress = default, CancellationToken cancellationToken = default);
    }
}
