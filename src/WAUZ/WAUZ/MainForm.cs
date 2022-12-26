using System.Diagnostics;
using WAUZ.BL;

namespace WAUZ
{
    public partial class MainForm : Form
    {
        private CancellationTokenSource cancellationTokenSource = new();

        private readonly IAppLogging appLogging;
        private readonly IBusinessLogic businessLogic;

        public MainForm(IAppLogging appLogging, IBusinessLogic businessLogic)
        {
            this.appLogging = appLogging ?? throw new ArgumentNullException(nameof(appLogging));
            this.businessLogic = businessLogic ?? throw new ArgumentNullException(nameof(businessLogic));

            InitializeComponent();

            MinimumSize = Size;
            Text = $"WAUZ {GetVersion()}";

            textBoxSource.PlaceholderText = "The folder which contains the addon zip files. Typically some temporary download folder.";
            textBoxDest.PlaceholderText = "The folder to unzip the addons into. Typically the World of Warcraft AddOns folder.";

            // Using Label control for "links", instead of LinkLabel, for 2 reasons:
            // 1) Actually there is an issue with LinkLabel. It´s text is truncated
            // if it´s disabled. See https://github.com/dotnet/winforms/issues/7341
            // 2) First workaround was: Changing the color and click handler of the
            // LinkLabel control to "fake" the disabled state of the LinkLabel. But
            // this was even more complicated than just alienate a normal Label for
            // my needs. And since i do not benefit from the advantages a LinkLabel
            // has, in contrast to a normal Label, i use colored normal Labels here.

            var defaultLinkLabelColor = new LinkLabel().LinkColor;

            labelSourceLink.ForeColor = defaultLinkLabelColor;
            labelDestLink.ForeColor = defaultLinkLabelColor;
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
            catch (InvalidOperationException ex)
            {
                ShowError(ex.Message);
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
            catch (InvalidOperationException ex)
            {
                ShowError(ex.Message);
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
            catch (InvalidOperationException ex)
            {
                ShowError(ex.Message);
                return;
            }

            SetControls(false);

            labelProgressBar.Enabled = true;
            progressBar.Enabled = true;
            progressBar.Value = progressBar.Minimum;

            try
            {
                progressBar.Maximum = businessLogic.GetZipFiles().Count();

                cancellationTokenSource = new CancellationTokenSource(new TimeSpan(0, 0, 30));

                var ms = await businessLogic.UnzipAsync(new Progress<ProgressData>(progressData =>
                {
                    progressBar.Value++;
                    var zipFileName = Path.GetFileName(progressData.ZipFile);
                    labelProgressBar.Text = $"Progress: Unzip {progressBar.Value} / {progressBar.Maximum} addons ... ({zipFileName})";
                }),
                cancellationTokenSource.Token);

                // Even with typical semaphore-blocking-mechanism* it is impossible to prevent Windows.Forms
                // ProgressBar control from reaching its maximum, shortly after last async progress happened.
                // Control is painted natively by the WinApi/OS itself, therefore also no event-based tricks
                // will solve problem. Just added some short async wait delay instead, to keep things simple.
                // *(TAP concepts, when using IProgress, often need some semaphore-blocking-mechanism, cause
                // scheduler can still produce async progress, even when Task.WhenAll() already has finished).

                await Task.Delay(1500);

                var duration = (double)(ms + 1500) / 1000;

                labelProgressBar.Text = $"Successfully unzipped {progressBar.Value} addons after {duration:0.00} seconds.";
            }
            catch (Exception ex)
            {
                appLogging.Log(ex);

                if (ex is InvalidOperationException)
                {
                    ShowError(ex.Message);
                }
                else
                {
                    ShowError("An unexpected error occurred (see log file for details).");
                }

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
            // Most simple way to get product version (semantic versioning)
            // in .NET5/6 onwards. Used as "Version" entry in .csproj file.

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

        private void SetControls(bool state)
        {
            Controls.Cast<Control>().ToList().ForEach(control => control.Enabled = state);
        }

        private static void ShowError(string errorText)
        {
            MessageBox.Show(errorText, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
