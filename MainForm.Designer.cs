namespace AstroImageTracker
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtFolderPath;
        private System.Windows.Forms.Button btnAnalyze;
        private System.Windows.Forms.RichTextBox rtxtResults;
        private System.Windows.Forms.Button btnExportCsv;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblProgress;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtFolderPath = new System.Windows.Forms.TextBox();
            this.btnAnalyze = new System.Windows.Forms.Button();
            this.rtxtResults = new System.Windows.Forms.RichTextBox();
            this.btnExportCsv = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblProgress = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(513, 12);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 0;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtFolderPath
            // 
            this.txtFolderPath.Location = new System.Drawing.Point(12, 12);
            this.txtFolderPath.Name = "txtFolderPath";
            this.txtFolderPath.Size = new System.Drawing.Size(495, 20);
            this.txtFolderPath.TabIndex = 1;
            // 
            // btnAnalyze
            // 
            this.btnAnalyze.Location = new System.Drawing.Point(12, 38);
            this.btnAnalyze.Name = "btnAnalyze";
            this.btnAnalyze.Size = new System.Drawing.Size(75, 23);
            this.btnAnalyze.TabIndex = 2;
            this.btnAnalyze.Text = "Analyze";
            this.btnAnalyze.UseVisualStyleBackColor = true;
            this.btnAnalyze.Click += new System.EventHandler(this.btnAnalyze_Click);
            // 
            // rtxtResults
            // 
            this.rtxtResults.Location = new System.Drawing.Point(12, 97);
            this.rtxtResults.Name = "rtxtResults";
            this.rtxtResults.Size = new System.Drawing.Size(576, 341);
            this.rtxtResults.TabIndex = 3;
            this.rtxtResults.Text = "";
            // 
            // btnExportCsv
            // 
            this.btnExportCsv.Location = new System.Drawing.Point(93, 38);
            this.btnExportCsv.Name = "btnExportCsv";
            this.btnExportCsv.Size = new System.Drawing.Size(75, 23);
            this.btnExportCsv.TabIndex = 4;
            this.btnExportCsv.Text = "Export CSV";
            this.btnExportCsv.UseVisualStyleBackColor = true;
            this.btnExportCsv.Click += new System.EventHandler(this.btnExportCsv_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(174, 38);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(414, 23);
            this.progressBar.TabIndex = 5;
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(174, 64);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(0, 13);
            this.lblProgress.TabIndex = 6;
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(600, 450);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btnExportCsv);
            this.Controls.Add(this.rtxtResults);
            this.Controls.Add(this.btnAnalyze);
            this.Controls.Add(this.txtFolderPath);
            this.Controls.Add(this.btnBrowse);
            this.Name = "MainForm";
            this.Text = "Astro Image Tracker";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
