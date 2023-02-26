using WAUZ.Core;

namespace WAUZ
{
    public partial class MainForm : Form
    {
        private CancellationTokenSource cancellationTokenSource = new();

        private readonly IBusinessLogic businessLogic;

        public MainForm(IBusinessLogic businessLogic)
        {
            this.businessLogic = businessLogic ?? throw new ArgumentNullException(nameof(businessLogic));

            InitializeComponent();

            MinimumSize = Size;
            Text = $"WAUZ {GetVersion()}";

            textBoxSource.PlaceholderText = "The folder which contains the addon zip files. Typically some temporary download folder.";
            textBoxDest.PlaceholderText = "The folder to unzip the addons into. Typically the World of Warcraft AddOns folder.";

            // I use the Label control for hyperlinks, instead of the LinkLabel control, for 2 reasons:
            // 1) Actually there is an issue with LinkLabel: Text is truncated when control is disabled.
            // Just have a look at https://github.com/dotnet/winforms/issues/7341 for more information.
            // 2) First i tried some workaround: Change LinkLabel color and click handler to "fake" its
            // disabled state. But this was even more complicated than the actual solution: Just use a
            // colored normal Label, since i do not benefit much from the advantages a LinkLabel offers.

            var defaultLinkLabelColor = new LinkLabel().LinkColor;

            labelSourceLink.ForeColor = defaultLinkLabelColor;
            labelDestLink.ForeColor = defaultLinkLabelColor;

            CancelButton = buttonClose;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                businessLogic.LoadSettings();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            textBoxSource.Text = businessLogic.SourceFolder;
            textBoxDest.Text = businessLogic.DestFolder;
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxSource.Text) && buttonSource.CanFocus)
            {
                buttonSource.Focus();
            }
            else if (string.IsNullOrEmpty(textBoxDest.Text) && buttonDest.CanFocus)
            {
                buttonDest.Focus();
            }
            else
            {
                if (buttonUnzip.CanFocus)
                {
                    buttonUnzip.Focus();
                }
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            businessLogic.SourceFolder = textBoxSource.Text;
            businessLogic.DestFolder = textBoxDest.Text;

            try
            {
                businessLogic.SaveSettings();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void ButtonSource_Click(object sender, EventArgs e)
        {
            var startFolder = Directory.Exists(textBoxSource.Text) ? textBoxSource.Text : GetDesktopFolder();

            SelectFolder(textBoxSource, startFolder);
        }

        private void LabelSourceLink_Click(object sender, EventArgs e)
        {
            OpenFolderInExplorer(e, true);
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
                var startFolder = Directory.Exists(wowAddonsDefaultFolder) ? wowAddonsDefaultFolder : GetDesktopFolder();

                SelectFolder(textBoxDest, startFolder);
            }
        }

        private void LabelDestLink_Click(object sender, EventArgs e)
        {
            OpenFolderInExplorer(e, false);
        }

        private async void ButtonUnzip_Click(object sender, EventArgs e)
        {
            SetControls(false);

            labelProgressBar.Enabled = true;
            progressBar.Enabled = true;
            progressBar.Value = progressBar.Minimum;

            businessLogic.SourceFolder = textBoxSource.Text;
            businessLogic.DestFolder = textBoxDest.Text;

            try
            {
                var zipFilesCount = businessLogic.CountZipFiles();

                progressBar.Maximum = zipFilesCount;

                cancellationTokenSource = new CancellationTokenSource(new TimeSpan(0, 0, 30));

                var milliSeconds = await businessLogic.UnzipAsync(new Progress<ProgressData>(progressData =>
                {
                    switch (progressData.State)
                    {
                        case ProgressState.UnzippedAddon:
                            progressBar.Value++;
                            var zipFileName = Path.GetFileName(progressData.Zip);
                            labelProgressBar.Text = $"Progress: Unzipped {progressBar.Value} / {zipFilesCount} addons ... ({zipFileName})";
                            break;
                        default:
                            // All other states are not used at the moment, since the human eye is not fast enough. :)
                            break;
                    }
                }),
                cancellationTokenSource.Token);

                // Even with a typical semaphore-blocking-mechanism* it is impossible to prevent a Windows.Forms
                // ProgressBar control from reaching its maximum shortly after the last async progress happened.
                // The control is painted natively by the WinApi/OS itself. Therefore also no event-based tricks
                // will solve the problem. I just added a short async wait delay instead, to keep things simple.
                // *(TAP concepts, when using IProgress<>, often need some semaphore-blocking-mechanism, because
                // a scheduler can still produce async progress, even when Task.WhenAll() already has finished).

                await Task.Delay(1250);

                var seconds = (double)(milliSeconds + 1250) / 1000;

                labelProgressBar.Text = $"Successfully unzipped {zipFilesCount} addons in {seconds:0.00} seconds.";
            }
            catch (Exception ex)
            {
                if (ex is not ValidationException)
                {
                    labelProgressBar.Text = "Error occurred.";
                    progressBar.Value = progressBar.Minimum;
                }

                HandleException(ex);
            }
            finally
            {
                SetControls(true);

                if (buttonClose.CanFocus)
                {
                    buttonClose.Focus();
                }
            }
        }

        private void ButtonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private static string GetVersion()
        {
            // Seems to be the most simple way to get the product version (semantic versioning) for .NET5/6 onwards.
            // Application.ProductVersion.ToString() is the counterpart of the "Version" entry in the .csproj file.

            return Application.ProductVersion.ToString();
        }

        private static void SelectFolder(TextBox textBox, string startFolder)
        {
            using var dialog = new FolderBrowserDialog { InitialDirectory = startFolder };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBox.Text = Path.TrimEndingDirectorySeparator(dialog.SelectedPath);
            }
        }

        private static string GetDesktopFolder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

        private void OpenFolderInExplorer(EventArgs e, bool isSourceFolder)
        {
            if (e is MouseEventArgs mouseEventArgs && mouseEventArgs.Button == MouseButtons.Left)
            {
                // Save and recover the original states is not really
                // necessary here and is just done to be super clean.

                var orgSourceFolder = businessLogic.SourceFolder;
                var orgDestFolder = businessLogic.DestFolder;

                businessLogic.SourceFolder = textBoxSource.Text;
                businessLogic.DestFolder = textBoxDest.Text;

                try
                {
                    if (isSourceFolder)
                    {
                        businessLogic.OpenSourceFolderInExplorer();
                    }
                    else
                    {
                        businessLogic.OpenDestFolderInExplorer();
                    }
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }

                businessLogic.SourceFolder = orgSourceFolder;
                businessLogic.DestFolder = orgDestFolder;
            }
        }

        private void HandleException(Exception ex)
        {
            if (ex is ValidationException || ex is InvalidOperationException)
            {
                ShowError(ex.Message);
            }
            else
            {
                businessLogic.LogUnexpectedException(ex);

                ShowError("An unexpected error occurred (see log file for details).");
            }
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
