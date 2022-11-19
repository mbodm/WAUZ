namespace WAUZ.BL
{
    public interface IAppSettings
    {
        IDictionary<string, string> Settings { get; }

        void Load();
        void Save();
    }
}
