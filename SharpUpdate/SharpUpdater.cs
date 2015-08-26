using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace SharpUpdate
{
    public class SharpUpdater
    {
        private ISharpUpdatable applcationInfo;
        private BackgroundWorker bgWorker;

        public SharpUpdater(ISharpUpdatable applcationInfo)
        {
            this.applcationInfo = applcationInfo;

            this.bgWorker = new BackgroundWorker();
            this.bgWorker.DoWork += BgWorker_DoWork;
            this.bgWorker.RunWorkerCompleted += BgWorker_RunWorkerCompleted;
    
        }

        public void DoUpdate()
        {
            if (!this.bgWorker.IsBusy)
                this.bgWorker.RunWorkerAsync(this.applcationInfo);
        }

        private void BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ISharpUpdatable applcation = (ISharpUpdatable)e.Argument;

            if (!SharpUpdateXml.ExistsOnServer(applcationInfo.UpdateXmlLocation))
                e.Cancel = true;
            else
            {
                e.Result = SharpUpdateXml.Parse(applcationInfo.UpdateXmlLocation, applcation.ApplicationID);
            }

        }

        private void BgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                SharpUpdateXml update = (SharpUpdateXml)e.Result;

                if (update != null && update.IsNewerThan(this.applcationInfo.ApplcationAssembly.GetName().Version))
                {
                    if (new SharpUpdateAcceptForm(this.applcationInfo, update).ShowDialog(this.applcationInfo.Context) == DialogResult.Yes)
                        this.DownloadUpdate(update);
                }
            }
        }

        private void DownloadUpdate(SharpUpdateXml update)
        {
            SharpUpdateDownloadForm form = new SharpUpdateDownloadForm(update.Uri, update.MD5, this.applcationInfo.ApplcationIcon);
            DialogResult result = form.ShowDialog(this.applcationInfo.Context);

            if (result == DialogResult.OK)
            {
                string currentPath = this.applcationInfo.ApplcationAssembly.Location;
                string newPath = Path.GetDirectoryName(currentPath) + "\\" + update.FileName;

                UpdateApplication(form.TempFilePath, currentPath, newPath, update.LaunchArgs);

                Application.Exit();
            } else if (result == DialogResult.Abort)
            {
                MessageBox.Show("Anulowano pobieranie.\nProgram nie został zaktualizowany.", "Anulowano pobieranie.", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } else
            {
                MessageBox.Show("Wystąpił problem z pobieraniem. Spróbuj ponownie.", "Błąd aktualizacji.", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void UpdateApplication(string tempFilePath, string currentPath, string newPath, string launchArgs)
        {
            // send arguments to /C command promp
            // show Choice of Y(es) - /N(o) is hidden
            // /D(efault) choice is Y(es)
            // /T(imeout) after which default option will be chosen is 4
            string argument = "/C Choice /C Y /N /D Y /T 4 & Del /F /Q \"{0}\" & Choice /C Y /N /D Y /T 2 & Move /Y \"{1}\" \"{2}\" & Start \"\" /D \"{3}\" \"{4}\" {5}";

            ProcessStartInfo info = new ProcessStartInfo();
            info.Arguments = string.Format(argument, currentPath, tempFilePath, newPath, Path.GetDirectoryName(newPath), Path.GetFileName(newPath), launchArgs);
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.CreateNoWindow = true; // just in case?
            info.FileName = "cmd.exe";
            Process.Start(info);
        }
    }
}
