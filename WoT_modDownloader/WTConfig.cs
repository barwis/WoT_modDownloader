using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoT_modDownloader
{
    [Serializable]
    public class WTConfig : SerializableBase
    {
        #region diskOps
        public void Save(string location)
        {
            SerializableBase.PutClassToFile(this, location);
        }

        public static WTConfig Load(string location)
        {
            if (System.IO.File.Exists(location))
                return SerializableBase.GetClassFromFile<WTConfig>(location); //if config exists in location
            //else create sample on disk
            var newConfig = new WTConfig();
            newConfig.Save(location);

            return newConfig;
        }

        #endregion

        public string SomeKindOfPropertyString { get; set; }
        public int SomeKindOfPropertyInt { get; set; }

        public WTConfig()
        {
            //defaulting values
            SomeKindOfPropertyInt = 123;
            SomeKindOfPropertyString = "ValueOfString";
        }
    }
}
