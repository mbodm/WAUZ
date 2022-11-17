namespace WAUZ.BL
{
    public sealed class BusinessLogic : IBusinessLogic
    {
        private readonly IZipHelper zipHelper;

        public BusinessLogic(IZipHelper zipHelper)
        {
            this.zipHelper = zipHelper ?? throw new ArgumentNullException(nameof(zipHelper));
        }

        public async Task UnzipFiles(IEnumerable<string> zipFiles, string destFolder,
            IProgress<ProgressData>? progress = null, CancellationToken cancellationToken = default)
        {
            var tasks = zipFiles.Select(zipFile => Task.Run(() =>
            {
                zipHelper.UnzipFile(zipFile, destFolder);

                var progessData = new ProgressData
                {
                    ZipFile = zipFile,
                    DestFolder = destFolder
                };

                progress?.Report(progessData);
            }));

            await Task.WhenAll(tasks);
        }
    }
}
