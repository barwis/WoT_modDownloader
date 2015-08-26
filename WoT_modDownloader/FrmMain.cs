using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using SharpUpdate;
using System.Reflection;

namespace WoT_modDownloader
{




    public partial class FrmMain : Form, ISharpUpdatable
    {
        RemoteConfig remoteConfig = new RemoteConfig();//creating sample config
        //WTConfig mainProgCfg = new WTConfig();
        LocalConfig localConfig = new LocalConfig();

        string remoteBase = @"http://behindpixels.co.uk/wot/";
        string localBase;
        Version gameVersion;
        string buildNo;
        string fileToDownload;

        private SharpUpdater updater;

        #region UpdaterVariables

        public string ApplicationName
        {
            get
            {
                return "WoT_modDownloader";
            }
        }

        public string ApplicationID
        {
            get
            {
                return "WoT_modDownloader";
            }
        }

        public Assembly ApplcationAssembly
        {
            get
            {
                return Assembly.GetExecutingAssembly();
            }
        }

        public Icon ApplcationIcon
        {
            get
            {
                return this.Icon;
            }
        }

        public Uri UpdateXmlLocation
        {
            get
            {
                return new Uri("http://behindpixels.co.uk/wot_modDownloader/update.xml");
            }
        }

        public Form Context
        {
            get
            {
                return this;
            }
        }

        #endregion

        public FrmMain()
        {
            InitializeComponent();
            this.Text = ApplicationID +  " v." + this.ApplcationAssembly.GetName().Version.ToString() + " by Fowler";
            updater = new SharpUpdater(this);
        }

        private delegate void AddToLogDelegate(string message);

        private void AddToLog(string message)
        {
            if (this.InvokeRequired)
            {
                var d = new AddToLogDelegate(AddToLog);
                this.Invoke(d, message);
                return;
            }
            txtLog.Text += message + Environment.NewLine;
            txtLog.SelectionStart = txtLog.Text.Length - 1; 
            txtLog.SelectionLength = 0;
            txtLog.ScrollToCaret();
        }

        private void InstallUpdates() {

        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            txtLog.Clear();
            if (!getGameLoc())
            {
                AddToLog("Nie znaleziono zainstalowanej gry.");
                return;

            }
            else
            {
                AddToLog("Gra zainstalowana w: " + localBase);
            }

            if (!getGameVersion())
                return;


            if (!loadRemoteConfig())
                return;

            loadLocalConfig();

            AddToLog("Wersja gry: " + localConfig.gameVersion);


            //AddToLog("Wersja moda (lokalna): " + localConfig.modVersion);

            var updates = new List<Mod>();

            var selection = from x in remoteConfig.Mods where x.GameVersion.Equals(gameVersion.ToString()) &&  localConfig.modVersion.CompareTo(x.ModVersion) <= 0  /* || (new Version(x.GameVersion).Major == 1) */ select x;// select all where GaMeVerSion is 1.2.3.4
            var asList = selection.ToList();

            foreach (var m in asList)
            {
                //if (localConfig.modVersion.CompareTo(m.ModVersion) < 0)
                    AddToLog(string.Format("Znaleziono aktualizacje modow do obecnej wersji gry. [ {0} -> {1} ]", localConfig.modVersion, m.ModVersion));
                    updates.Add(m);
            }

            var c = updates.Max(m => new Version(m.ModVersion));

            if (c == null)
            {
                AddToLog("Twoja wersja moda jest aktualna.");
                return;
            } else {
                AddToLog(c.ToString());
                fileToDownload = remoteBase + localConfig.gameVersion + "/mods/" + c.ToString() + "/mods.zip";
                AddToLog(fileToDownload);

                DialogResult result = MessageBox.Show(string.Format("Dostępna jest nowa wersja moda ({0}). Kliknij OK aby zainstalować", c.ToString()), "Dostępna aktualizacja!", MessageBoxButtons.OK);
                if (result == DialogResult.OK)
                {
                    if (!bwAsync.IsBusy)
                    {
                        bwAsync.RunWorkerAsync();
                        btnDownload.Enabled = false;
                        btnCancel.Enabled = true;
                    }
                    else
                    {
                        MessageBox.Show("Pobieranie w toku!");
                    }
                }
            }




            //DialogResult result = MessageBox.Show()

            //if (!bwAsync.IsBusy)
            //{
            //    bwAsync.RunWorkerAsync();
            //    btnDownload.Enabled = false;
            //    btnCancel.Enabled = true;
            //}
            //else
            //{
            //    MessageBox.Show("Pobieranie w toku!");
            //}
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            bwAsync.CancelAsync();
            btnDownload.Enabled = true;
            btnCancel.Enabled = false;
        }


        private void bwAsync_DoWork(object sender, DoWorkEventArgs e)
        {

            //AddToLog("Downloading to " + KnownFolders.GetPath(KnownFolder.Downloads) + "...");

            string destinationPath = KnownFolders.GetPath(KnownFolder.Downloads) + "//vehicles.zip";
            //string destinationPath = @"d:\\vehicles.zip";
            //string sourceURL = @"http://behindpixels.co.uk/wot/0.9.9/mods/0.27.1/vehicles.zip";
            string sourceURL = remoteBase + gameVersion + "/mods/0.27.1/vehicles.zip";
            //string sourceURL = @"http://behindpixels.co.uk/wot/0.9.9/mods/0.27.1/vehicles.zip";
            DownloadHelper.Download(destinationPath, sourceURL, sender as BackgroundWorker);
        }


       
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage >= 0)
                progress.Value = e.ProgressPercentage < 100 ? e.ProgressPercentage: 100;
            
            var complex = e.UserState as ComplexResponse;
            if (complex != null)
            {
                if (complex.IsError)
                    MessageBox.Show(this, complex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else if (!string.IsNullOrWhiteSpace(complex.Message))
                    //AddToLog(complex.Message);
                progress.Value = (int)complex.Percent;
                lblProgress.Text = string.Format("{0:0.00} %", complex.Percent);
                lblAdditionalInfo.Text = string.Format("{0:0.00}KB / {1:0.00}KB ({2:0.00}KB)", complex.CurrentBytes / 1024.0, complex.TotalBytes / 1024.0, complex.FileSize / 1024.0);
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                MessageBox.Show("Anulowano pobieranie!!");
            }
            else if (e.Error != null)
            {
                MessageBox.Show("Error: " + e.Error.Message);
            }
            else
            {
                lblProgress.Text = "...";
                lblAdditionalInfo.Text = "...";
                AddToLog("Pobieranie ukonczone.");
            }

            btnDownload.Enabled = true;
            btnCancel.Enabled = false;
        }

        private bool getGameVersion()
        {

            if (localBase == null)
            {
                return false;
            }

            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(localBase + "\\version.xml");
                if (doc != null)
                {
                    XmlNode node = doc.DocumentElement.SelectSingleNode("/version.xml/version");
                    string[] values = node.InnerText.Split('#');
                    gameVersion = new Version(values[0].Trim().Substring(2));
                    //gameVersion = "0.9.10";
                    buildNo = values[1].Trim();
                }
                return true;
            }
            catch (System.IO.FileNotFoundException)
            {
                AddToLog("Nie mozna wczytac pliku wersji");
                return false;
            }
        }

        private bool getGameLoc()
        {

            string registry_key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            using (Microsoft.Win32.RegistryKey key = Registry.LocalMachine.OpenSubKey(registry_key))
            {
                foreach (string subkey_name in key.GetSubKeyNames())
                {
                    using (RegistryKey subkey = key.OpenSubKey(subkey_name))
                    {
                        if (subkey.GetValue("DisplayName") != null && subkey.GetValue("DisplayName").ToString().Trim() == "World of Tanks")
                        {
                            localBase = subkey.GetValue("InstallLocation").ToString().Trim();
                            return true;
                        }
                    }
                }
            }
            // couldn't find game in registry, checking most common install locations
            List<string> locs = new List<string>()
            {
                "C:\\Games\\World_of_Tanks",
                "D:\\Games\\World_of_Tanks",
                "E:\\Games\\World_of_Tanks",
                "F:\\Games\\World_of_Tanks",
                "G:\\Games\\World_of_Tanks",
            };

            foreach (string loc in locs)
            {
                if (Directory.Exists(loc))
                {
                    localBase = loc;
                    return true;
                }
            }

            return false;
        }

        private bool loadRemoteConfig()
        {
            try
            {
                remoteConfig = RemoteConfig.Load(@"http://behindpixels.co.uk/wot/config.xml");
                AddToLog("Wczytano zdalna konfiguracje...");
                return true;
            }
            catch
            {
                AddToLog("Nie udalo sie wczytac zdalnej konfiguracji");
                return false;
            }
        }

        private bool loadLocalConfig()
        {
            try
            {
                localConfig = LocalConfig.Load(localBase + "\\wmd_config.xml");
                AddToLog("Wczytano lokalna konfiguracje...");
                return true;
            }
            catch
            {
                  AddToLog("Nie udalo sie wczytac lokalnej konfiguracji. Prawdopodobnie uruchamiasz ten program po raz pierwszy; Tworzenie...");

                localConfig = new LocalConfig
                {
                    gameVersion = gameVersion.ToString(),
                    modVersion = "0.0.0"
                  };

            }
            finally
            {
                localConfig.Save(localBase + "\\wmd_config.xml");
            }
            return true;

        }

        //private void btnCreateSample_Click(object sender, EventArgs e)
        //{
        //    remoteConfig = new RemoteConfig
        //    {
        //        Mods = new[] {
        //             new Mod{ GameVersion = "0.9.8" , ModVersion = "0.26.0" },
        //             new Mod{ GameVersion = "0.9.9" , ModVersion = "0.27.0" },
        //             new Mod{ GameVersion = "0.9.9" , ModVersion = "0.27.1" },
        //             new Mod{ GameVersion = "0.9.9" , ModVersion = "0.27.2" },

        //         }
        //    };
        //}


        //private void btnSaveConfig_Click(object sender, EventArgs e)
        //{
        //    remoteConfig.Save(@"C:\\Games\config.xml");
        //}



        private void FrmMain_Load(object sender, EventArgs e)
        {
            updater.DoUpdate();
        }

        private void btnLoadConfig_Click(object sender, EventArgs e)
        {

        }
    }

    
}
