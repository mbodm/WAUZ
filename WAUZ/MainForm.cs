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
            var sourceFolder = Path.TrimEndingDirectorySeparator(textBoxSource.Text.Trim());
            var destFolder = Path.TrimEndingDirectorySeparator(textBoxDest.Text.Trim());

            if (string.IsNullOrEmpty(sourceFolder))
            {
                ShowError("Todo Source");

                return;
            }

            if (string.IsNullOrEmpty(destFolder))
            {
                ShowError("Todo Dest");

                return;
            }

            var zipFiles = Directory.GetFiles(sourceFolder, "*.zip", SearchOption.TopDirectoryOnly);

            progressBar.Maximum = zipFiles.Length;
            progressBar.Value = progressBar.Minimum;

            await businessLogic.UnzipFiles(new Progress<ProgressData>(_ =>
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

            using var dialog = new FolderBrowserDialog { InitialDirectory = initialDirectory };

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
    }
}
