using WAUZ.Core;
using WAUZ.Helpers;

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
            var fileSystemHelper = new FileSystemHelper();
            var folderValidator = new FolderValidator(fileSystemHelper);
            Application.Run(new MainForm(new BusinessLogic(new AppSettings(), new ErrorLogger(), folderValidator, fileSystemHelper, new ZipFileHelper())));
        }
    }
}
