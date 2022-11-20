namespace WAUZ.BL
{
    public sealed record ProgressData
    {
        public string ZipFile { get; init; } = string.Empty;
        public string DestFolder { get; init; } = string.Empty;
    }
}
