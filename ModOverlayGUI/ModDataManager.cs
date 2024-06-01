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
        private static string FilePath = "CMData.xml";

        public static void SaveModData(ModData settings)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ModData));
                using (StreamWriter writer = new StreamWriter(FilePath))
                {
                    serializer.Serialize(writer, settings);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving settings: " + ex.Message);
            }
        }

        public static ModData LoadModData()
        {
            try
            {
                if (!File.Exists(FilePath))
                {
                    ModData modData = new ModData();
                    modData.init = true;
                    return modData; // Return default settings if file doesn't exist
                }

                XmlSerializer serializer = new XmlSerializer(typeof(ModData));
                using (StreamReader reader = new StreamReader(FilePath))
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

        public ModData iniModData()
        {
            ModData modData = new ModData();
            // Init
            modData.init = false;
            // Playermodel
            modData.playerModelSize = 1.0f;
            List<ModelData> models = new List<ModelData>();

            // DET_Detention
            String[] modelParts_isaHurt = { "Body", "Braid", "Hair_Isa", "HairHead_Isa", "IsaBodyDetailsArmor", "Isas_Knife",  "Skirt_Short", "Tasche" }; 
            ModelData isa_metarig_hurt = new ModelData("isa_metarig_hurt", modelParts_isaHurt, new bool[modelParts_isaHurt.Length]);
            models.Add(isa_metarig_hurt);

            String[] modelParts_arianeGhost = { "Body", "Hair_Ariane", "HairHead_Ariane", "HairLong ", "Skirt" };
            ModelData ariane_metarig_IK_ghost = new ModelData("ariane_metarig_IK_ghost", modelParts_arianeGhost, new bool[modelParts_arianeGhost.Length]);
            models.Add(ariane_metarig_IK_ghost);

            String[] modelParts_arianeUniform = { "Body", "Hair_Ariane", "HairHead_Ariane", "Tasche" };
            ModelData ariane_metarig_IK_uniform = new ModelData("ariane_metarig_IK_uniform", modelParts_arianeUniform, new bool[modelParts_arianeUniform.Length]);
            models.Add(ariane_metarig_IK_uniform);

            String[] modelParts_alina = { "AlinaBodyDetailsArmor", "Body", "FemaleMilitaryHat", "Hair_Alina", "HairHead_Alina", "PonyTail", "Tasche"};
            ModelData alina_metarig_IK = new ModelData("alina_metarig_IK", modelParts_alina, new bool[modelParts_alina.Length]);
            models.Add(alina_metarig_IK);

            String[] modelParts_isaRe = { "Body", "Braid", "Hair_Isa", "HairHead_Isa", "Isas_Knife", "Skirt_Short", "Tasche" };
            ModelData isa_re_metarig_IK_Variant = new ModelData("isa_re_metarig_IK Variant", modelParts_isaRe, new bool[modelParts_isaRe.Length]);
            models.Add(isa_re_metarig_IK_Variant);

            String[] modelParts_STCR = { "STCR_Baton", "STCR_Belt", "STCR_Body", "STCR_Hair", "STCR_HairHead" };
            ModelData STCR_Normal_Anim = new ModelData("STCR_Normal_Anim", modelParts_STCR, new bool[modelParts_STCR.Length]);
            models.Add (STCR_Normal_Anim);

            // RES_Residential
            String[] modelParts_adler = { "Body", "Hair_Adler" };
            ModelData adler_metarig_IK = new ModelData("adler_metarig_IK", modelParts_adler, new bool[modelParts_adler.Length]);
            models.Add(adler_metarig_IK);

            String[] modelParts_ARAR = { "ARAR_Hair", "ARAR_HairHead", "ARAR_NomalBody", "Amigasa" };
            ModelData ARAR_Normal = new ModelData("ARAR_Normal", modelParts_ARAR, new bool[modelParts_ARAR.Length]);
            models.Add(ARAR_Normal);

            String[] modelParts_KLBR = { "KLBR_Armor", "KLBR_Belt", "KLBR_Hair", "KLBR_HairHead", "KLBR_Normal_Body" };
            ModelData KLBR_Normal_Anim = new ModelData("KLBR_Normal_Anim", modelParts_KLBR, new bool[modelParts_KLBR.Length]);
            models.Add(KLBR_Normal_Anim);

            // EXC_Mines
            String[] modelParts_STAR = { "STAR_Hair", "STAR_HairHead", "STARBaton_001", "STARBody_001" };
            ModelData STAR_Normal_Anim = new ModelData("STAR_Normal_Anim", modelParts_STAR, new bool[modelParts_STAR.Length]);
            models.Add(STAR_Normal_Anim);

            String[] modelParts_EULR = { "EULR_Hair", "EULR_HairHead", "EULR_Normal_Body", "FemaleMilitaryHat_001" };
            ModelData EULR_Normal_Anim = new ModelData("EULR_Normal_Anim", modelParts_EULR, new bool[modelParts_EULR.Length]);
            models.Add (EULR_Normal_Anim);

            modData.modelData = models;
            // WeaponModels
            modData.weaponModelSize = 1.0f;
            modData.weaponShowCase = false;
            modData.weaponBool = new bool[7];
            return modData;
        }
    }

    public class ModelData
    {
        public String modelName { get; }
        public String[] modelParts { get; }
        public bool[] active { get; set; }

        public ModelData(String modelName, String[] modelParts, bool[] active)
        {
            this.modelName = modelName;
            this.modelParts = modelParts;
            this.active = active;
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
        public bool[] weaponBool { get; set; }
    }
}
 