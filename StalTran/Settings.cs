using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace StalTran
{
    [Serializable]
    public class Settings
    {
        public string xmlLocation { get; set; }
        public bool toEng { get; set; }
        public bool ignoreIns { get; set; }
        public string lastSelectedFile { get; set; }
        public string lastSelectedStringId { get; set; }
        public int scd1 { get; set; }
        public int scd2 { get; set; }
        public int scd3 { get; set; }
        public int scd4 { get; set; }
        public decimal fontSize { get; set; }
        public System.Drawing.Size winSize { get; set; }
        public System.Drawing.Point location { get; set; }
        public List<string> contextReplacements { get; set; }

        public static void Store(Settings settings)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
            StreamWriter writer = new StreamWriter("settings.xml");
            serializer.Serialize(writer, settings);
            writer.Close();
        }

        public static Settings Load()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
            try
            {
                FileStream fileStream = new FileStream("settings.xml", FileMode.Open);
                Settings result = (Settings)serializer.Deserialize(fileStream);
                fileStream.Close();
                return result;
            }
            catch (FileNotFoundException)
            {
                return new Settings();
            }
        }
    }
}
