using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
namespace ModOverlayGUI
{
    public class ModDataManager
    {
        private static string FilePath = "CMData.xml";
        private static readonly object lockObject = new object();

        public static void SaveModData(ModData modData)
        {
            lock (lockObject)
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ModData));
                    using (FileStream fs = new FileStream(FilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                    {
                        serializer.Serialize(fs, modData);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error saving data: " + ex.Message);
                }
            }
        }

        public static ModData LoadModData()
        {
            lock (lockObject)
            {
                try
                {
                    if (!File.Exists(FilePath))
                    {
                        ModData modData = iniModData();
                        return modData; // Return default settings if file doesn't exist
                    }

                    XmlSerializer serializer = new XmlSerializer(typeof(ModData));
                    using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        return (ModData)serializer.Deserialize(fs);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error loading settings: " + ex.Message);
                    return null; // Return default settings in case of error
                }
            }
        }
       
        public static ModData iniModData()
        {
            ModData modData = new ModData
            {
                version = 1,
                call = 0,
                playerModelSize = 1.0f,
                modelData = new List<ModelData>(),
                localHeight = 0,

                // WeaponModels
                weaponModelSize = 1.0f,
                weaponShowCase = false,
                weaponName = new string[] { "Nitro Model", "FGun Model", "Shotgun", "Revolver Model", "Pistol", "Machete" },
                windowed = false,
            };
            modData.weaponBool = new bool[modData.weaponName.Length];

            // Elster Models (0-4)
            String[] modelParts_ELNormal = { "Hat", "Body", "Hair", "Tasche", "HairHead"};
            modData.modelData.Add(new ModelData("Normal", modelParts_ELNormal, new bool[] { true, true, true, true, true}));

            String[] modelParts_ELArmor = { "Body", "Hair", "HairHead", "Armor", "TascheArmor" };
            modData.modelData.Add(new ModelData("Armored", modelParts_ELArmor, new bool[modelParts_ELArmor.Length]));

            String[] modelParts_ELEVA = { "Body", "Helmet", "Neck", "Backpack", "TascheArmor", "Visor", "Visor Layer2"};
            modData.modelData.Add(new ModelData("EVA", modelParts_ELEVA, new bool[modelParts_ELEVA.Length]));

            String[] modelParts_ELCrippled = { "Body", "Organs", "Hair", "HairHead"};
            modData.modelData.Add(new ModelData("Crippled",modelParts_ELCrippled, new bool[modelParts_ELCrippled.Length]));

            String[] modelParts_IsaPast = { "Body", "Hair", "HairHead", "Skirt", "Braid"};
            modData.modelData.Add(new ModelData("Isa_Past", modelParts_IsaPast, new bool[modelParts_IsaPast.Length]));

            // DET_Detention (5-10)-1
            String[] modelParts_isa_metarig_IK = { "Body", "Poncho", "Hair_Isa", "HairHead_Isa", "Braid" };
            modData.modelData.Add(new ModelData("isa_metarig_IK", modelParts_isa_metarig_IK, new bool[modelParts_isa_metarig_IK.Length]));

            String[] modelParts_arianeGhost = { "Body", "Hair_Ariane", "HairHead_Ariane", "HairLong", "Skirt" };
            modData.modelData.Add(new ModelData("ariane_metarig_IK_ghost", modelParts_arianeGhost, new bool[modelParts_arianeGhost.Length]));

            String[] modelParts_arianeUniform = { "Body", "Hair_Ariane", "HairHead_Ariane", "Tasche" };
            modData.modelData.Add(new ModelData("ariane_metarig_IK_uniform", modelParts_arianeUniform, new bool[modelParts_arianeUniform.Length]));

            String[] modelParts_alina = { "AlinaBodyDetailsArmor", "Body", "FemaleMilitaryHat", "Hair_Alina", "HairHead_Alina", "PonyTail", "Tasche"};
            modData.modelData.Add(new ModelData("alina_metarig_IK", modelParts_alina, new bool[modelParts_alina.Length]));

            String[] modelParts_isaRe = { "Body", "Braid", "Hair_Isa", "HairHead_Isa", "Isas_Knife", "Skirt_Short", "Tasche" };
            //modData.modelData.Add(new ModelData("isa_re_metarig_IK Variant", modelParts_isaRe, new bool[modelParts_isaRe.Length])); Doesn't work for some reason, ignore that for now

            String[] modelParts_STCR = { "STCR_Baton", "STCR_Belt", "STCR_Body", "STCR_Hair", "STCR_HairHead" };
            modData.modelData.Add(new ModelData("STCR_Normal_Anim", modelParts_STCR, new bool[modelParts_STCR.Length]));

            // RES_Residential (11-13)-1
            String[] modelParts_adler = { "Body", "Hair_Adler" };
            modData.modelData.Add(new ModelData("adler_metarig_IK", modelParts_adler, new bool[modelParts_adler.Length]));

            String[] modelParts_ARAR = { "ARAR_Hair", "ARAR_HairHead", "ARAR_NormalBody", "Amigasa" };
            modData.modelData.Add(new ModelData("ARAR_Normal", modelParts_ARAR, new bool[modelParts_ARAR.Length]));

            String[] modelParts_KLBR = { "KLBR_Armor", "KLBR_Belt", "KLBR_Hair", "KLBR_HairHead", "KLBR_Normal_Body" };
            modData.modelData.Add(new ModelData("KLBR_Normal_Anim", modelParts_KLBR, new bool[modelParts_KLBR.Length]));

            // EXC_Mines (14-15)-1
            String[] modelParts_STAR = { "STAR_Hair", "STAR_HairHead", "STARBaton_001", "STARBody_001" };
            modData.modelData.Add(new ModelData("STAR_Normal_Anim", modelParts_STAR, new bool[modelParts_STAR.Length]));

            String[] modelParts_EULR = { "EULR_Hair", "EULR_HairHead", "EULR_Normal_Body", "FemaleMilitaryHat_001" };
            modData.modelData.Add(new ModelData("EULR_Normal_Anim", modelParts_EULR, new bool[modelParts_EULR.Length]));

            return modData;
        }
    }
    [Serializable]
    public class ModelData
    {
        public string modelName { get; set; }
        public string[] modelParts { get; set; }
        public bool[] active { get; set; }

        public ModelData() { }
        public ModelData(String modelName, String[] modelParts, bool[] active)
        {
            this.modelName = modelName;
            this.modelParts = modelParts;
            this.active = active;
        }

        public override int GetHashCode()
        {
            return modelName.GetHashCode();
        }
    }

    [Serializable]
    public class ModData
    {
        public int version { get; set; }
        // Init
        public int call {  get; set; }
        // PLayModel
        public float playerModelSize { get; set; }

        public List<ModelData> modelData { get; set; }
        public float localHeight { get; set; }

        public bool windowed { get; set; }
        public ModelData FindModelDataByName(string modelName)
        {
            return modelData.FirstOrDefault(md => md.modelName == modelName);
        }

        // WeaponModels
        public float weaponModelSize { get; set; }
        public string[] weaponName { get; set; }
        public bool weaponShowCase { get; set; }
        public bool[] weaponBool { get; set; }
    }
}