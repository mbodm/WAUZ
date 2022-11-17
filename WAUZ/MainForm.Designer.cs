namespace WAUZ
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonSource = new System.Windows.Forms.Button();
            this.buttonDest = new System.Windows.Forms.Button();
            this.textBoxSource = new System.Windows.Forms.TextBox();
            this.textBoxDest = new System.Windows.Forms.TextBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.buttonUnzip = new System.Windows.Forms.Button();
            this.labelSource = new System.Windows.Forms.Label();
            this.labelDest = new System.Windows.Forms.Label();
            this.labelProgressBar = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonSource
            // 
            this.buttonSource.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSource.Location = new System.Drawing.Point(497, 12);
            this.buttonSource.Name = "buttonSource";
            this.buttonSource.Size = new System.Drawing.Size(75, 25);
            this.buttonSource.TabIndex = 1;
            this.buttonSource.Text = "Search...";
            this.buttonSource.UseVisualStyleBackColor = true;
            this.buttonSource.Click += new System.EventHandler(this.ButtonSource_Click);
            // 
            // buttonDest
            // 
            this.buttonDest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDest.Location = new System.Drawing.Point(497, 103);
            this.buttonDest.Name = "buttonDest";
            this.buttonDest.Size = new System.Drawing.Size(75, 25);
            this.buttonDest.TabIndex = 4;
            this.buttonDest.Text = "Search...";
            this.buttonDest.UseVisualStyleBackColor = true;
            this.buttonDest.Click += new System.EventHandler(this.ButtonDest_Click);
            // 
            // textBoxSource
            // 
            this.textBoxSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSource.Location = new System.Drawing.Point(12, 43);
            this.textBoxSource.Name = "textBoxSource";
            this.textBoxSource.Size = new System.Drawing.Size(560, 23);
            this.textBoxSource.TabIndex = 2;
            // 
            // textBoxDest
            // 
            this.textBoxDest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDest.Location = new System.Drawing.Point(12, 134);
            this.textBoxDest.Name = "textBoxDest";
            this.textBoxDest.Size = new System.Drawing.Size(560, 23);
            this.textBoxDest.TabIndex = 5;
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(12, 276);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(560, 23);
            this.progressBar.TabIndex = 8;
            // 
            // buttonUnzip
            // 
            this.buttonUnzip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonUnzip.Location = new System.Drawing.Point(497, 245);
            this.buttonUnzip.Name = "buttonUnzip";
            this.buttonUnzip.Size = new System.Drawing.Size(75, 25);
            this.buttonUnzip.TabIndex = 7;
            this.buttonUnzip.Text = "&Unzip";
            this.buttonUnzip.UseVisualStyleBackColor = true;
            this.buttonUnzip.Click += new System.EventHandler(this.ButtonUnzip_Click);
            // 
            // labelSource
            // 
            this.labelSource.AutoSize = true;
            this.labelSource.Location = new System.Drawing.Point(9, 25);
            this.labelSource.Margin = new System.Windows.Forms.Padding(0);
            this.labelSource.Name = "labelSource";
            this.labelSource.Size = new System.Drawing.Size(46, 15);
            this.labelSource.TabIndex = 0;
            this.labelSource.Text = "Source:";
            // 
            // labelDest
            // 
            this.labelDest.AutoSize = true;
            this.labelDest.Location = new System.Drawing.Point(9, 116);
            this.labelDest.Margin = new System.Windows.Forms.Padding(0);
            this.labelDest.Name = "labelDest";
            this.labelDest.Size = new System.Drawing.Size(70, 15);
            this.labelDest.TabIndex = 3;
            this.labelDest.Text = "Destination:";
            // 
            // labelProgressBar
            // 
            this.labelProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelProgressBar.AutoSize = true;
            this.labelProgressBar.Location = new System.Drawing.Point(9, 258);
            this.labelProgressBar.Margin = new System.Windows.Forms.Padding(0);
            this.labelProgressBar.Name = "labelProgressBar";
            this.labelProgressBar.Size = new System.Drawing.Size(55, 15);
            this.labelProgressBar.TabIndex = 6;
            this.labelProgressBar.Text = "Progress:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 311);
            this.Controls.Add(this.labelProgressBar);
            this.Controls.Add(this.labelDest);
            this.Controls.Add(this.labelSource);
            this.Controls.Add(this.buttonUnzip);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.textBoxDest);
            this.Controls.Add(this.textBoxSource);
            this.Controls.Add(this.buttonDest);
            this.Controls.Add(this.buttonSource);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WAUZ";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button buttonSource;
        private Button buttonDest;
        private TextBox textBoxSource;
        private TextBox textBoxDest;
        private ProgressBar progressBar;
        private Button buttonUnzip;
        private Label labelSource;
        private Label labelDest;
        private Label labelProgressBar;
    }
}