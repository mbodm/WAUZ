using System.Windows.Forms;
using WAUZ.BL;

namespace WAUZ
{
    public partial class MainForm : Form
    {
        private readonly IBusinessLogic businessLogic;
        private readonly SemaphoreSlim semaphoreSlim = new(0, 1);

        private CancellationTokenSource cancellationTokenSource = new();

        public MainForm(IBusinessLogic businessLogic)
        {
            this.businessLogic = businessLogic ?? throw new ArgumentNullException(nameof(businessLogic));

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
            if (Directory.Exists(textBoxSource.Text))
            {
                SelectFolder(textBoxSource, textBoxSource.Text);
            }
            else
            {
                SelectFolder(textBoxSource, GetDesktopFolder());
            }
        }

        private void ButtonDest_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(textBoxDest.Text))
            {
                SelectFolder(textBoxDest, textBoxDest.Text);
            }
            else
            {
                var wowAddonsDefaultFolder = @"C:\Program Files (x86)\World of Warcraft\_retail_\Interface\AddOns";

                if (Directory.Exists(wowAddonsDefaultFolder))
                {
                    SelectFolder(textBoxDest, wowAddonsDefaultFolder);
                }
                else
                {
                    SelectFolder(textBoxDest, GetDesktopFolder());
                }
            }
        }

        private async void ButtonUnzip_Click(object sender, EventArgs e)
        {
            SetControls(false);
            labelProgressBar.Enabled = true;
            progressBar.Enabled = true;

            cancellationTokenSource = new(new TimeSpan(0, 0, 30));
            
            try
            {
                businessLogic.SourceFolder = textBoxSource.Text;
                businessLogic.DestFolder = textBoxDest.Text;

                progressBar.Maximum = businessLogic.GetZipFiles().Count();
                progressBar.Value = progressBar.Minimum;

                await businessLogic.UnzipAsync(new Progress<ProgressData>(_ =>
                {
                    progressBar.Value++;
                    labelProgressBar.Text = $"Progress: Unzip {progressBar.Value} / {progressBar.Maximum} addons.";

                    if (progressBar.Value == progressBar.Maximum)
                    {
                        semaphoreSlim.Release();
                    }
                }),
                cancellationTokenSource.Token);

                await semaphoreSlim.WaitAsync();

                ShowError("früh");

                labelProgressBar.Text = "Progress: All addons successfully unzipped.";
            }
            catch (InvalidOperationException ioex)
            {
                ShowError(ioex.Message);
            }
            catch (OperationCanceledException)
            {
                ShowError("WOOFFI Canceled");
            }

            SetControls(true);
        }

        private static void SelectFolder(TextBox textBox, string startFolder)
        {
            using var dialog = new FolderBrowserDialog
            {
                InitialDirectory = startFolder
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBox.Text = Path.TrimEndingDirectorySeparator(dialog.SelectedPath);
            }
        }

        private static string GetDesktopFolder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

        private static void ShowError(string errorText)
        {
            MessageBox.Show(errorText, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void SetControls(bool state)
        {
            Controls.Cast<Control>().ToList().ForEach(control => control.Enabled = state);
        }
    }
}
