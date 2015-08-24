using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace WoT_modDownloader
{
    [Serializable]
    public class SerializableBase
    {
        public static XmlDocument Serialize<T>(T instance) where T : new()
        {
            XmlDocument xd = new XmlDocument();
            XmlSerializer xs = new XmlSerializer(typeof(T));
            using (var stream = new MemoryStream())
            {
                xs.Serialize(stream, instance);
                stream.Flush();
                stream.Seek(0, SeekOrigin.Begin);
                xd.Load(stream);
            }
            return xd;
        }

        public static T Deserialize<T>(XmlDocument xd) where T : new()
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            string xmlString = xd.OuterXml.ToString();
            byte[] buffer = UTF8Encoding.UTF8.GetBytes(xmlString);
            MemoryStream ms = new MemoryStream(buffer);
            using (XmlReader reader = new XmlTextReader(ms))
            {
                var o = (T)xs.Deserialize(reader);
                return o;
            }
        }

        public static T GetClassFromFile<T>(string location) where T : new()
        {
            XmlDocument xd = new XmlDocument();
            xd.Load(location);
            return Deserialize<T>(xd);
        }

        public static void PutClassToFile<T>(T instance, string location) where T : new()
        {
            XmlDocument xd = Serialize<T>(instance);
            xd.Save(location);
        }

    }
}
