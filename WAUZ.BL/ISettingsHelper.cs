namespace WAUZ.BL
{
    public interface ISettingsHelper
    {
        string Location { get; }
        IDictionary<string, string> Settings { get; }
        
        void Load();
        void Save();
    }
}
