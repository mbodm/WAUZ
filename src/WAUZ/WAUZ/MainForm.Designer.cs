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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.buttonSource = new System.Windows.Forms.Button();
            this.buttonDest = new System.Windows.Forms.Button();
            this.textBoxSource = new System.Windows.Forms.TextBox();
            this.textBoxDest = new System.Windows.Forms.TextBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.buttonUnzip = new System.Windows.Forms.Button();
            this.labelSource = new System.Windows.Forms.Label();
            this.labelDest = new System.Windows.Forms.Label();
            this.labelProgressBar = new System.Windows.Forms.Label();
            this.labelSourceLink = new System.Windows.Forms.Label();
            this.labelDestLink = new System.Windows.Forms.Label();
            this.buttonClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonSource
            // 
            this.buttonSource.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSource.Location = new System.Drawing.Point(497, 12);
            this.buttonSource.Name = "buttonSource";
            this.buttonSource.Size = new System.Drawing.Size(75, 25);
            this.buttonSource.TabIndex = 0;
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
            this.textBoxSource.BackColor = System.Drawing.Color.WhiteSmoke;
            this.textBoxSource.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.textBoxSource.Location = new System.Drawing.Point(12, 43);
            this.textBoxSource.Name = "textBoxSource";
            this.textBoxSource.Size = new System.Drawing.Size(560, 23);
            this.textBoxSource.TabIndex = 2;
            // 
            // textBoxDest
            // 
            this.textBoxDest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDest.BackColor = System.Drawing.Color.WhiteSmoke;
            this.textBoxDest.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.textBoxDest.Location = new System.Drawing.Point(12, 134);
            this.textBoxDest.Name = "textBoxDest";
            this.textBoxDest.Size = new System.Drawing.Size(560, 23);
            this.textBoxDest.TabIndex = 6;
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(12, 276);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(560, 23);
            this.progressBar.TabIndex = 10;
            // 
            // buttonUnzip
            // 
            this.buttonUnzip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonUnzip.Location = new System.Drawing.Point(416, 245);
            this.buttonUnzip.Name = "buttonUnzip";
            this.buttonUnzip.Size = new System.Drawing.Size(75, 25);
            this.buttonUnzip.TabIndex = 8;
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
            this.labelSource.Size = new System.Drawing.Size(84, 15);
            this.labelSource.TabIndex = 1;
            this.labelSource.Text = "Source-Folder:";
            // 
            // labelDest
            // 
            this.labelDest.AutoSize = true;
            this.labelDest.Location = new System.Drawing.Point(9, 116);
            this.labelDest.Margin = new System.Windows.Forms.Padding(0);
            this.labelDest.Name = "labelDest";
            this.labelDest.Size = new System.Drawing.Size(108, 15);
            this.labelDest.TabIndex = 5;
            this.labelDest.Text = "Destination-Folder:";
            // 
            // labelProgressBar
            // 
            this.labelProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelProgressBar.AutoEllipsis = true;
            this.labelProgressBar.Location = new System.Drawing.Point(9, 258);
            this.labelProgressBar.Margin = new System.Windows.Forms.Padding(0);
            this.labelProgressBar.Name = "labelProgressBar";
            this.labelProgressBar.Size = new System.Drawing.Size(375, 15);
            this.labelProgressBar.TabIndex = 9;
            this.labelProgressBar.Text = "Progress:";
            // 
            // labelSourceLink
            // 
            this.labelSourceLink.AutoSize = true;
            this.labelSourceLink.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelSourceLink.Location = new System.Drawing.Point(9, 69);
            this.labelSourceLink.Name = "labelSourceLink";
            this.labelSourceLink.Size = new System.Drawing.Size(129, 15);
            this.labelSourceLink.TabIndex = 3;
            this.labelSourceLink.Text = "Open folder in Explorer";
            this.labelSourceLink.Click += new System.EventHandler(this.LabelSourceLink_Click);
            // 
            // labelDestLink
            // 
            this.labelDestLink.AutoSize = true;
            this.labelDestLink.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelDestLink.Location = new System.Drawing.Point(9, 160);
            this.labelDestLink.Name = "labelDestLink";
            this.labelDestLink.Size = new System.Drawing.Size(129, 15);
            this.labelDestLink.TabIndex = 7;
            this.labelDestLink.Text = "Open folder in Explorer";
            this.labelDestLink.Click += new System.EventHandler(this.LabelDestLink_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(497, 245);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 25);
            this.buttonClose.TabIndex = 11;
            this.buttonClose.Text = "&Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.ButtonClose_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(584, 311);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.labelDestLink);
            this.Controls.Add(this.labelSourceLink);
            this.Controls.Add(this.labelProgressBar);
            this.Controls.Add(this.labelDest);
            this.Controls.Add(this.labelSource);
            this.Controls.Add(this.buttonUnzip);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.textBoxDest);
            this.Controls.Add(this.textBoxSource);
            this.Controls.Add(this.buttonDest);
            this.Controls.Add(this.buttonSource);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WAUZ";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
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
        private Label labelSourceLink;
        private Label labelDestLink;
        private Button buttonClose;
    }
}