using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ModOverlayGUI
{
    public class ModDataManager
    {
        private static string settingsFilePath = "CMData.xml";

        public static void SaveSettings(ModData settings)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ModData));
                using (StreamWriter writer = new StreamWriter(settingsFilePath))
                {
                    serializer.Serialize(writer, settings);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving settings: " + ex.Message);
            }
        }

        public static ModData LoadSettings()
        {
            try
            {
                if (!File.Exists(settingsFilePath))
                {
                    return new ModData(); // Return default settings if file doesn't exist
                }

                XmlSerializer serializer = new XmlSerializer(typeof(ModData));
                using (StreamReader reader = new StreamReader(settingsFilePath))
                {
                    return (ModData)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading settings: " + ex.Message);
                return new ModData(); // Return default settings in case of error
            }
        }
    }

    public class ModData
    {
        public bool IsCheckboxChecked { get; set; }
    }
}
 