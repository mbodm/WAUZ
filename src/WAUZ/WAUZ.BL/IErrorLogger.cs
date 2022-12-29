namespace WAUZ.BL
{
    public interface IErrorLogger
    {
        void Log(string message);
        void Log(Exception exception);
    }
}
