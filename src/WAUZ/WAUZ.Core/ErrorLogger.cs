using System.Runtime.CompilerServices;
using System.Text;

namespace WAUZ.Core
{
    public sealed class ErrorLogger : IErrorLogger
    {
        private readonly object syncRoot = new();
        private readonly string logFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MBODM", "WAUZ.log");
        private readonly string newLine = Environment.NewLine;

        public void Log(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException($"'{nameof(message)}' cannot be null or whitespace.", nameof(message));
            }

            lock (syncRoot)
            {
                WriteLogEntry("Message", file, line, message);
            }
        }

        public void Log(Exception exception, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            if (exception is null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            lock (syncRoot)
            {
                WriteLogEntry(
                    "Exception",
                    file,
                    line,
                    $"Exception-Type: {exception.GetType().Name}{newLine}" +
                    $"Exception-Message: {exception.Message}{newLine}" +
                    $"Exception-StackTrace:{newLine}" +
                    $"{exception.StackTrace}");
            }
        }

        private void WriteLogEntry(string header, string file, int line, string message)
        {
            var now = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

            file = Path.GetFileName(file);

            var text = $"[{now}] {header}{newLine}File: {file}{newLine}Line: {line}{newLine}{message}{newLine}{newLine}";

            File.AppendAllText(logFile, text, Encoding.UTF8);
        }
    }
}
