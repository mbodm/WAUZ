namespace WAUZ.BL
{
    public interface ISettingsHelper
    {
        IDictionary<string, string> Settings { get; }

        void Load();
        void Save();
    }
}
