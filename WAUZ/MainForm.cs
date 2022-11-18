using WAUZ.BL;

namespace WAUZ
{
    public partial class MainForm : Form
    {
        private readonly IBusinessLogic businessLogic;

        public MainForm(IBusinessLogic businessLogic)
        {
            this.businessLogic = businessLogic ?? throw new ArgumentNullException(nameof(businessLogic));

            InitializeComponent();

            MinimumSize = Size;

            textBoxSource.PlaceholderText = "The folder which contains the addon zip files. Normally some temporary download folder.";
            textBoxDest.PlaceholderText = "The folder to unzip the addons into. Normally the World of Warcraft AddOns folder.";
        }
        
        private void ButtonSource_Click(object sender, EventArgs e)
        {
            SelectFolder(textBoxSource);
        }

        private void ButtonDest_Click(object sender, EventArgs e)
        {
            var initialDirectory = @"C:\Program Files (x86)\World of Warcraft\_retail_\Interface\AddOns";

            if (Directory.Exists(initialDirectory))
            {
                SelectFolder(textBoxDest, initialDirectory);
            }
            else
            {
                SelectFolder(textBoxDest);
            }
        }

        private async void ButtonUnzip_Click(object sender, EventArgs e)
        {
            businessLogic.SourceFolder = Path.TrimEndingDirectorySeparator(textBoxSource.Text.Trim());
            businessLogic.DestFolder = Path.TrimEndingDirectorySeparator(textBoxDest.Text.Trim());

            progressBar.Maximum = Directory.GetFiles(businessLogic.SourceFolder, "*.zip", SearchOption.TopDirectoryOnly).Length;
            progressBar.Value = progressBar.Minimum;

            await businessLogic.Unzip(new Progress<ProgressData>(_ =>
            {
                progressBar.Value++;
                labelProgressBar.Text = $"Progress: Unzip {progressBar.Value} / {progressBar.Maximum} addons.";
            }));

            labelProgressBar.Text = "Progress: All addons successfully unzipped.";
        }

        private static void SelectFolder(TextBox textBox, string initialDirectory = "")
        {
            if (string.IsNullOrWhiteSpace(initialDirectory) || !Directory.Exists(initialDirectory))
            {
                initialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }

            using var dialog = new FolderBrowserDialog
            { 
                InitialDirectory = initialDirectory
            };

            if (dialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            textBox.Text = Path.TrimEndingDirectorySeparator(dialog.SelectedPath);
        }

        private static void ShowError(string errorText)
        {
            MessageBox.Show(errorText, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // Todo: Im Auswahl-Dialog, wenn vorhanden, gleich das AddOns Folder anzeigen.
        // Nein, das gehört nicht in die BusinessLogic (weil sonst leere Settings immer am Ende das drin haben).

        //private static string GetDefaultDestFolder()
        //{
        //    var wowAddonsDefaultFolder = @"C:\Program Files (x86)\World of Warcraft\_retail_\Interface\AddOns";

        //    if (Directory.Exists(wowAddonsDefaultFolder))
        //    {
        //        // Just returning the string itself, is also possible, of course. But it felt better to me,
        //        // when the string flows through the Path methods, cause of the "Program Files (x86)" part.

        //        return Path.TrimEndingDirectorySeparator(Path.GetFullPath(wowAddonsDefaultFolder));
        //    }

        //    return string.Empty;
        //}
    }
}
