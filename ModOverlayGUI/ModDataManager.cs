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
                    ModData modData = new ModData();
                    modData.init = true;
                    return modData; // Return default settings if file doesn't exist
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
                ModData modData = new ModData();
                modData.init = true;
                return modData; // Return default settings in case of error
            }
        }
    }
    [Serializable]
    public class ModData
    {
        // Init
        public bool init {  get; set; }
        // PLayModel
        public float playerModelSize { get; set; }

        public List<ModelData> modelData { get; set; }

        // WeaponModels
        public float weaponModelSize { get; set; }
        public bool weaponShowCase { get; set; }



    }

    public class ModelData
    {
        public String modelName { get; }
        public String[] modelParts {  get; }
        public bool[] active {  get; set; }

        public ModelData(String modelName, String[] modelParts, bool[] active) { 
            this.modelName = modelName;
            this.modelParts = modelParts;
            this.active = active;
        }
    }

}
 