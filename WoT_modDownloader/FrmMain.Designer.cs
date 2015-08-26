namespace WoT_modDownloader
{
    partial class FrmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.progress = new System.Windows.Forms.ProgressBar();
            this.bwAsync = new System.ComponentModel.BackgroundWorker();
            this.btnDownload = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblProgress = new System.Windows.Forms.Label();
            this.lblAdditionalInfo = new System.Windows.Forms.Label();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // progress
            // 
            this.progress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progress.Location = new System.Drawing.Point(12, 92);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(523, 23);
            this.progress.Step = 100;
            this.progress.TabIndex = 0;
            // 
            // bwAsync
            // 
            this.bwAsync.WorkerReportsProgress = true;
            this.bwAsync.WorkerSupportsCancellation = true;
            this.bwAsync.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwAsync_DoWork);
            this.bwAsync.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.bwAsync.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // btnDownload
            // 
            this.btnDownload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDownload.Location = new System.Drawing.Point(12, 295);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(75, 23);
            this.btnDownload.TabIndex = 1;
            this.btnDownload.Text = "Download";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.Enabled = false;
            this.btnCancel.Location = new System.Drawing.Point(93, 295);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblProgress
            // 
            this.lblProgress.Location = new System.Drawing.Point(12, 117);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(103, 23);
            this.lblProgress.TabIndex = 4;
            this.lblProgress.Text = "...";
            // 
            // lblAdditionalInfo
            // 
            this.lblAdditionalInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAdditionalInfo.Location = new System.Drawing.Point(314, 117);
            this.lblAdditionalInfo.Name = "lblAdditionalInfo";
            this.lblAdditionalInfo.Size = new System.Drawing.Size(221, 13);
            this.lblAdditionalInfo.TabIndex = 5;
            this.lblAdditionalInfo.Text = "...";
            this.lblAdditionalInfo.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtLog
            // 
            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.Location = new System.Drawing.Point(12, 143);
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.Size = new System.Drawing.Size(523, 146);
            this.txtLog.TabIndex = 8;
            this.txtLog.Text = "";
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(547, 327);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.lblAdditionalInfo);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnDownload);
            this.Controls.Add(this.progress);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Downloader";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar progress;
        private System.ComponentModel.BackgroundWorker bwAsync;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.Label lblAdditionalInfo;
        private System.Windows.Forms.RichTextBox txtLog;
    }
}

