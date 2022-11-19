using WAUZ.BL;

namespace WAUZ
{
    public partial class MainForm : Form
    {
        private readonly IBusinessLogic businessLogic;
        private readonly IPathHelper pathHelper;

        public MainForm(IBusinessLogic businessLogic, IPathHelper pathHelper)
        {
            this.businessLogic = businessLogic ?? throw new ArgumentNullException(nameof(businessLogic));
            this.pathHelper = pathHelper ?? throw new ArgumentNullException(nameof(pathHelper));

            InitializeComponent();

            MinimumSize = Size;

            textBoxSource.PlaceholderText = "The folder which contains the addon zip files. Normally some temporary download folder.";
            textBoxDest.PlaceholderText = "The folder to unzip the addons into. Normally the World of Warcraft AddOns folder.";
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            businessLogic.LoadSettings();

            textBoxSource.Text = businessLogic.SourceFolder;
            textBoxDest.Text = businessLogic.DestFolder;
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            businessLogic.SourceFolder = textBoxSource.Text;
            businessLogic.DestFolder = textBoxDest.Text;

            businessLogic.SaveSettings();
        }

        private void ButtonSource_Click(object sender, EventArgs e)
        {
            // todo: das hier richtig machen
            // überlegung: überall in der app directoryexists und fileexists gegen pathisfileandexists() pathhelper zeugs austauschen?

            var initialDirectory = textBoxSource.Text;

            if (Directory.Exists(initialDirectory))
            {
                SelectFolder(textBoxDest, initialDirectory);
            }
            else
            {
                SelectFolder(textBoxDest);
            }

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
            businessLogic.SourceFolder = textBoxSource.Text;
            businessLogic.DestFolder = textBoxDest.Text;

            progressBar.Maximum = Directory.GetFiles(businessLogic.SourceFolder, "*.zip", SearchOption.TopDirectoryOnly).Length;
            progressBar.Value = progressBar.Minimum;

            await businessLogic.Unzip(new Progress<ProgressData>(_ =>
            {
                progressBar.Value++;
                labelProgressBar.Text = $"Progress: Unzip {progressBar.Value} / {progressBar.Maximum} addons.";
            }));

            labelProgressBar.Text = "Progress: All addons successfully unzipped.";
        }

        private void SelectFolder(TextBox textBox, string initialDirectory = "")
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

            textBox.Text = pathHelper.GetFullPathWithoutEndingDirectorySeparator(dialog.SelectedPath);
        }

        private static void ShowError(string errorText)
        {
            MessageBox.Show(errorText, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
