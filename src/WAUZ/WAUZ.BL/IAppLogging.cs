namespace WAUZ.BL
{
    public interface IAppLogging
    {
        void Log(string message);
        void Log(Exception exception);
    }
}
