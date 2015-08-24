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
using KnightsWarriorAutoupdater;
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
        string localConfigPath;
        string gameVersion;
        string buildNo;
        string fileToDownload;

        public string ApplicationName
        {
            get
            {
                return this.ApplicationName;
            }
        }

        public string ApplicationID
        {
            get
            {
                return this.ApplicationName;
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
                return new Uri("");
            }
        }

        public Form Context
        {
            get
            {
                return this;
            }
        }

        public FrmMain()
        {
            InitializeComponent();
            this.Text += " v." + this.ApplcationAssembly.GetName().Version.ToString();
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
                    AddToLog(complex.Message);
                progress.Value = (int)complex.Percent;
                lblProgress.Text = string.Format("{0:0.00}%", complex.Percent);
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
                AddToLog("Pobieranie ukonczone.");
            }
            btnDownload.Enabled = true;
            btnCancel.Enabled = false;
        }

        private void btnDownload_Click(object sender, EventArgs e)
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            bwAsync.CancelAsync();
            btnDownload.Enabled = true;
            btnCancel.Enabled = false;
        }

        private void btnCreateSample_Click(object sender, EventArgs e)
        {
            remoteConfig = new RemoteConfig
            {
                Mods = new[] {
                     new Mod{ GameVersion = "0.9.8" , ModVersion = "0.26.0" },
                     new Mod{ GameVersion = "0.9.9" , ModVersion = "0.27.0" },
                     new Mod{ GameVersion = "0.9.9" , ModVersion = "0.27.1" },
                     new Mod{ GameVersion = "0.9.9" , ModVersion = "0.27.2" },

                 }
            };
        }



        private void loadRemoteConfig()
        {
            try
            {
                remoteConfig = RemoteConfig.Load(@"http://behindpixels.co.uk/wot/config.xml");
                AddToLog("Wczytano zdalna konfiguracje...");
            }
            catch
            {
                AddToLog("Nie udalo sie wczytac zdalnej konfiguracji");
            }
        }

        private void loadLocalConfig()
        {
            try
            {
                localConfig = LocalConfig.Load(localBase + "\\wmd_config.xml");
                AddToLog("Wczytano lokalna konfiguracje...");
            }
            catch
            {
                  AddToLog("Nie udalo sie wczytac lokalnej konfiguracji. Prawdopodobnie uruchamiasz ten program po raz pierwszy; Tworzenie...");

                  localConfig = new LocalConfig
                  {
                      gameVersion = gameVersion,
                      modVersion = "0.0.0"
                  };

            }
            finally
            {
                localConfig.Save(localBase + "\\wmd_config.xml");
            }

        }

        private void btnSaveConfig_Click(object sender, EventArgs e)
        {
            remoteConfig.Save(@"C:\\Games\config.xml");
        }

        private void btnTest_Click(object sender, EventArgs e)
        {

            var selection = from x in remoteConfig.Mods where x.GameVersion.Equals(gameVersion) || (new Version(x.GameVersion).Major == 1)  select x;// select all where GaMeVerSion is 1.2.3.4
            var asList = selection.ToList();

            foreach (var m in asList)
            {
                if (localConfig.modVersion.CompareTo(m.ModVersion) < 0) 
                    AddToLog("Znaleziono nowa wersje moda. Wersja gry: " + m.GameVersion + "; werjsa moda: " + m.ModVersion);
            }
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
                    gameVersion = values[0].Trim().Substring(2);
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
            progress.Value = 0;

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

        private void FrmMain_Load(object sender, EventArgs e)
        {
            if (getGameLoc())
            {
                AddToLog("Gra zainstalowana w: " + localBase);
            }
            if (getGameVersion())
            {
                AddToLog("Wersja gry: " + gameVersion + " #" + buildNo);

            }
            loadRemoteConfig();
            loadLocalConfig();
            AddToLog("Wersja moda (lokalna): " + localConfig.modVersion);
        }
    }

    
}
