using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace WoT_modDownloader
{
    [Serializable]
    public class LocalConfig : SerializableBase
    {
        //public string gameVersion { get; set; }
        //public string modVersion { get; set; }

        private Version _gameVersion;
        private Version _modVersion;

        public string gameVersion
        {
            get { return _gameVersion.ToString(); }
            set { _gameVersion = new Version(value); }
        }
        
        public string modVersion
        {
            get { return _modVersion.ToString();  }
            set { _modVersion = new Version(value); }
        }

        public void Save(string location)
        {
            SerializableBase.PutClassToFile(this, location);
        }

        public static LocalConfig Load(string location)
        {
            return SerializableBase.GetClassFromFile<LocalConfig>(location);
        }

    }
}
