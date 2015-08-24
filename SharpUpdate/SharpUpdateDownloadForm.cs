using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace SharpUpdate
{
    public partial class SharpUpdateDownloadForm : Form
    {

        private WebClient webClient;
        private BackgroundWorker bgWorker;
        private string tempFile;
        private string md5;

        internal string TempFilePath
        {
            get { return this.tempFile; }
        }
          
        public SharpUpdateDownloadForm(Uri location, string md5, Icon programIcon)
        {
            InitializeComponent();
            if (programIcon != null)
                this.Icon = programIcon;

            tempFile = Path.GetTempFileName();
            this.md5 = md5;

            webClient = new WebClient();
            webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
            webClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;

            bgWorker = new BackgroundWorker();
            bgWorker.DoWork += BgWorker_DoWork;
            bgWorker.RunWorkerCompleted += BgWorker_RunWorkerCompleted;
            try
            {
                webClient.DownloadFileAsync(location, this.tempFile);
            }
            catch
            {
                this.DialogResult = DialogResult.No; this.Close();
            }
        }

        private void BgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            throw new NotImplementedException();
        }



        private void WebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.progressBar.Value = e.ProgressPercentage;
            this.lblProgress.Text = String.Format("Pobrano {0} z {1}", e.BytesReceived, e.TotalBytesToReceive);
        }
    }
}
