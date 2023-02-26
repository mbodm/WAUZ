namespace WAUZ.Core
{
    public interface IAppSettings
    {
        IDictionary<string, string> Settings { get; }

        void Load();
        void Save();
    }
}
