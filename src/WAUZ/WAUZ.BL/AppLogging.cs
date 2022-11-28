using System.Text;

namespace WAUZ.BL
{
    public sealed class AppLogging : IAppLogging
    {
        private readonly object syncRoot = new();
        private readonly string logFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "AppDataMBODM", "WAUZ.log");

        public void Log(string message)
        {
            lock (syncRoot)
            {
                WriteLogEntry(message);
            }
        }

        public void Log(Exception exception)
        {
            lock (syncRoot)
            {
                WriteLogEntry(
                    $"Exception occurred.{Environment.NewLine}" +
                    $"Exception-Type: {exception.GetType().Name}{Environment.NewLine}" +
                    $"Exception-Message: {exception.Message}{Environment.NewLine}" +
                    $"Exception-StackTrace: {exception.StackTrace}");
            }
        }

        private void WriteLogEntry(string s)
        {
            var now = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

            var text = $"[{now}]{Environment.NewLine}{s}{Environment.NewLine}";

            File.AppendAllText(logFile, text, Encoding.UTF8);
        }
    }
}
