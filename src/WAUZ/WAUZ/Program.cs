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
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            var pathHelper = new PathHelper();
            var appSettings = new AppSettings(pathHelper);
            var appLogging = new AppLogging();
            var fileSystemHelper = new FileSystemHelper(pathHelper);
            var zipHelper = new ZipHelper(pathHelper, fileSystemHelper);
            var businessLogic = new BusinessLogic(appSettings, appLogging, pathHelper, zipHelper);
            Application.Run(new MainForm(businessLogic, pathHelper));
        }
    }
}
