using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoT_modDownloader
{
    [Serializable]
    public class RemoteConfig: SerializableBase
    {
        public Mod[] Mods { get; set; }

        public RemoteConfig()
        {
            Mods = new Mod[] { };
        }

        public void Save(string location)
        {
            SerializableBase.PutClassToFile(this, location); 
        }

        public static RemoteConfig Load(string location)
        {
            return SerializableBase.GetClassFromFile<RemoteConfig>(location);
        }
    }

    [Serializable]
    public class Mod
    {
        private Version _gameVersion = new Version();
        private Version _modVersion = new Version();

        public string GameVersion { get { return _gameVersion.ToString(); } set { _gameVersion = new Version(value); } }
        public string ModVersion { get { return _modVersion.ToString(); } set { _modVersion = new Version(value); } }
    }

    

}
