using System.Text;

namespace WAUZ.BL
{
    public sealed class AppLogging : IAppLogging
    {
        private readonly object syncRoot = new();
        private readonly string logFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MBODM", "WAUZ.log");

        public void Log(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException($"'{nameof(message)}' cannot be null or whitespace.", nameof(message));
            }

            lock (syncRoot)
            {
                WriteLogEntry($"{message}{Environment.NewLine}");
            }
        }

        public void Log(Exception exception)
        {
            if (exception is null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            lock (syncRoot)
            {
                WriteLogEntry(
                    $"Exception occurred{Environment.NewLine}" +
                    $"Exception-Type: {exception.GetType().Name}{Environment.NewLine}" +
                    $"Exception-Message: {exception.Message}{Environment.NewLine}" +
                    $"Exception-StackTrace:{Environment.NewLine}" +
                    $"{exception.StackTrace}{Environment.NewLine}");
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
