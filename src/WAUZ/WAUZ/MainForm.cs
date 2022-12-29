using System.Diagnostics;
using WAUZ.BL;

namespace WAUZ
{
    public partial class MainForm : Form
    {
        private CancellationTokenSource cancellationTokenSource = new();

        private readonly IBusinessLogic businessLogic;
        private readonly IErrorLogger errorLogger;

        public MainForm(IBusinessLogic businessLogic, IErrorLogger errorLogger)
        {
            this.businessLogic = businessLogic ?? throw new ArgumentNullException(nameof(businessLogic));
            this.errorLogger = errorLogger ?? throw new ArgumentNullException(nameof(errorLogger));

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
            businessLogic.LoadSettings();

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

        private void LabelSourceLink_Click(object sender, EventArgs e)
        {
            if (e is not MouseEventArgs mouseEventArgs) return;
            if (mouseEventArgs.Button != MouseButtons.Left) return;

            try
            {
                businessLogic.SourceFolder = textBoxSource.Text;
                businessLogic.ValidateSourceFolder();
                Process.Start("explorer", businessLogic.SourceFolder);
            }
            catch (Exception ex)
            {
                HandleError(ex);
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

        private void LabelDestLink_Click(object sender, EventArgs e)
        {
            if (e is not MouseEventArgs mouseEventArgs) return;
            if (mouseEventArgs.Button != MouseButtons.Left) return;

            try
            {
                businessLogic.DestFolder = textBoxDest.Text;
                businessLogic.ValidateDestFolder();
                Process.Start("explorer", businessLogic.DestFolder);
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        private async void ButtonUnzip_Click(object sender, EventArgs e)
        {
            businessLogic.SourceFolder = textBoxSource.Text;
            businessLogic.DestFolder = textBoxDest.Text;

            try
            {
                businessLogic.ValidateSourceFolder();
                businessLogic.ValidateDestFolder();
            }
            catch (Exception ex)
            {
                HandleError(ex);
                return;
            }

            SetControls(false);

            labelProgressBar.Enabled = true;
            progressBar.Enabled = true;
            progressBar.Value = progressBar.Minimum;

            try
            {
                var zipFilesCount = businessLogic.GetZipFiles().Count();

                progressBar.Maximum = zipFilesCount;

                cancellationTokenSource = new CancellationTokenSource(new TimeSpan(0, 0, 30));

                var milliSeconds = await businessLogic.UnzipAsync(new Progress<ProgressData>(progressData =>
                {
                    switch (progressData.State)
                    {
                        case ProgressState.Started:
                            // State not used at the moment.
                            break;
                        case ProgressState.UnzipAddon:
                            // State not used at the moment.
                            break;
                        case ProgressState.UnzippedAddon:
                            progressBar.Value++;
                            var zipFileName = Path.GetFileName(progressData.Zip);
                            labelProgressBar.Text = $"Progress: Unzipped {progressBar.Value} / {zipFilesCount} addons ... ({zipFileName})";
                            break;
                        case ProgressState.ClearDestFolder:
                            // State not used at the moment.
                            break;
                        case ProgressState.ClearedDestFolder:
                            // State not used at the moment.
                            break;
                        case ProgressState.MoveFromTempToDest:
                            // State not used at the moment.
                            break;
                        case ProgressState.MovedFromTempToDest:
                            // State not used at the moment.
                            break;
                        case ProgressState.Finished:
                            // State not used at the moment.
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

                await Task.Delay(1000);

                var seconds = (double)(milliSeconds + 1000) / 1000;

                labelProgressBar.Text = $"Successfully unzipped {zipFilesCount} addons after {seconds:0.00} seconds.";
            }
            catch (Exception ex)
            {
                HandleError(ex);

                labelProgressBar.Text = "Error occurred.";
                progressBar.Value = progressBar.Minimum;
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

        private void HandleError(Exception exception)
        {
            if (exception is InvalidOperationException)
            {
                ShowError(exception.Message);
            }
            else
            {
                errorLogger.Log(exception);

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
