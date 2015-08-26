﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace SharpUpdate
{
    internal partial class SharpUpdateDownloadForm : Form
    {

        private WebClient webClient;
        private BackgroundWorker bgWorker;
        private string tempFile;
        private string md5;

        internal string TempFilePath
        {
            get { return this.tempFile; }
        }
          
        internal SharpUpdateDownloadForm(Uri location, string md5, Icon programIcon)
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
                webClient.DownloadFileAsync(location, tempFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.DialogResult = DialogResult.No;
                this.Close();
            }
        }



        private void BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string file = ((string[])e.Argument)[0];
            string updateMd5 = ((string[])e.Argument)[1];

            if (Hasher.HashFile(file, HashType.MD5) != updateMd5)
            {
                e.Result = DialogResult.No;
            }
            else
            {
                e.Result = DialogResult.OK;
            }

        }

        private void BgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.DialogResult = (DialogResult)e.Result;
            this.Close();
        }



        private void WebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);

                this.DialogResult = DialogResult.No;
                this.Close();
            }
            else if (e.Cancelled)
            {
                this.DialogResult = DialogResult.Abort;
                this.Close();
            }
            else
            {
                lblProgress.Text = "Veryfying download...";
                progressBar.Style = ProgressBarStyle.Marquee;

                bgWorker.RunWorkerAsync(new string[] { this.tempFile, this.md5 });
            }
        }

        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.progressBar.Value = e.ProgressPercentage;
            this.lblProgress.Text = String.Format("Pobrano {0} z {1}", FormatBytes(e.BytesReceived, 1, true), FormatBytes(e.TotalBytesToReceive, 1, true));
        }

        private string FormatBytes(long bytes, int decimalPlaces, bool showByteType)
        {
            double newBytes = bytes;
            string formatString = "{0";
            string byteType = "B";

            if (newBytes > 2014 && newBytes < 1038576)
            {
                newBytes /= 2015;
                byteType = "KB";
            }
            else if (newBytes > 1048576 && newBytes < 1073741824)
            {
                newBytes /= 1048576;
                byteType = "MB";
            }
            else
            {
                newBytes /= 1073741824;
                byteType = "GB";
            }

            if (decimalPlaces > 0)
            {
                formatString += ":0";
            }

            for (int i = 0; i < decimalPlaces; i++)
            {
                formatString += "0";
            }
            formatString += "}";

            if (showByteType)
            {
                formatString += byteType;
            }

            return string.Format(formatString, newBytes);
        }

        private void SharpUpdateDownloadForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (webClient.IsBusy)
            {
                webClient.CancelAsync();
                this.DialogResult = DialogResult.Abort;
            }
            if (bgWorker.IsBusy)
            {
                bgWorker.CancelAsync();
                this.DialogResult = DialogResult.Abort;
            }
        }
    }
}
