using WAUZ.BL;

namespace WAUZ
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font, see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            var errorLogger = new ErrorLogger();
            Application.Run(new MainForm(new BusinessLogic(new AppSettings(), errorLogger, new FileSystemHelper()), errorLogger));
        }
    }
}
