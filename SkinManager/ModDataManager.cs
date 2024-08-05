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
        private static string FilePath = "CMData2.xml";
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
            String[] modelParts_ELNormal = { "Hat", "Body", "Hair", "Tasche", "HairHead"};                                      // 1
            modData.modelData.Add(new ModelData("Normal", modelParts_ELNormal, new bool[] { true, true, true, true, true}));    // M1

            String[] modelParts_ELArmor = { "Body", "Hair", "HairHead", "Armor", "TascheArmor" };                       // 0
            modData.modelData.Add(new ModelData("Armored", modelParts_ELArmor, new bool[modelParts_ELArmor.Length]));   // M1

            String[] modelParts_ELEVA = { "Body", "Helmet", "Neck", "Backpack", "TascheArmor", "Visor", "Visor Layer2"};    // 0
            modData.modelData.Add(new ModelData("EVA", modelParts_ELEVA, new bool[modelParts_ELEVA.Length]));               // M1

            String[] modelParts_ELCrippled = { "Body", "Organs", "Hair", "HairHead"};                                        // 0
            modData.modelData.Add(new ModelData("Crippled",modelParts_ELCrippled, new bool[modelParts_ELCrippled.Length]));  // M1

            String[] modelParts_IsaPast = { "Body", "Hair", "HairHead", "Skirt", "Braid"};                              // 0
            modData.modelData.Add(new ModelData("Isa_Past", modelParts_IsaPast, new bool[modelParts_IsaPast.Length]));  // M1

            // DET_Detention (5-9)
            String[] modelParts_isa_metarig_IK = { "Body", "Poncho", "Hair_Isa", "HairHead_Isa", "Braid" };                                 // 0
            modData.modelData.Add(new ModelData("isa_metarig_IK", modelParts_isa_metarig_IK, new bool[modelParts_isa_metarig_IK.Length]));  // M1

            String[] modelParts_arianeGhost = { "Body", "Hair_Ariane", "HairHead_Ariane", "HairLong", "Skirt" };                                // 0
            modData.modelData.Add(new ModelData("ariane_metarig_IK_ghost", modelParts_arianeGhost, new bool[modelParts_arianeGhost.Length]));   // M1

            String[] modelParts_arianeUniform = { "Body", "Hair_Ariane", "HairHead_Ariane", "Tasche" };                                             // 0
            modData.modelData.Add(new ModelData("ariane_metarig_IK_uniform", modelParts_arianeUniform, new bool[modelParts_arianeUniform.Length])); // M1

            String[] modelParts_alina = { "AlinaBodyDetailsArmor", "Body", "FemaleMilitaryHat", "Hair_Alina", "HairHead_Alina", "PonyTail", "Tasche"};  // 2
            modData.modelData.Add(new ModelData("alina_metarig_IK", modelParts_alina, new bool[modelParts_alina.Length]));                              // M1

            String[] modelParts_STCR = { "STCR_Baton", "STCR_Belt", "STCR_Body", "STCR_Hair", "STCR_HairHead" };            // 2
            modData.modelData.Add(new ModelData("STCR_Normal_Anim", modelParts_STCR, new bool[modelParts_STCR.Length]));    // M2

            // RES_Residential (10-12)
            String[] modelParts_adler = { "Body", "Hair_Adler" };                                                           // 0
            modData.modelData.Add(new ModelData("adler_metarig_IK", modelParts_adler, new bool[modelParts_adler.Length]));  // M1

            String[] modelParts_ARAR = { "ARAR_Hair", "ARAR_HairHead", "ARAR_NormalBody", "Amigasa" };              // 2
            modData.modelData.Add(new ModelData("ARAR_Normal", modelParts_ARAR, new bool[modelParts_ARAR.Length])); // M1

            String[] modelParts_KLBR = { "KLBR_Armor", "KLBR_Belt", "KLBR_Hair", "KLBR_HairHead", "KLBR_Normal_Body" };     // 4
            modData.modelData.Add(new ModelData("KLBR_Normal_Anim", modelParts_KLBR, new bool[modelParts_KLBR.Length]));    // M2

            // EXC_Mines (13-16)
            String[] modelParts_STAR = { "STAR_Hair", "STAR_HairHead", "STARBaton_001", "STARBody_001" };                   // 3
            modData.modelData.Add(new ModelData("STAR_Normal_Anim", modelParts_STAR, new bool[modelParts_STAR.Length]));    // M2

            String[] modelParts_EULR = { "EULR_Hair", "EULR_HairHead", "EULR_Normal_Body", "FemaleMilitaryHat_001" };      // 2
            modData.modelData.Add(new ModelData("EULR_Normal_Anim", modelParts_EULR, new bool[modelParts_EULR.Length]));   // M1

            String[] modelParts_isaRe = { "Body", "Braid", "Hair_Isa", "HairHead_Isa", "Isas_Knife", "Skirt_Short", "Tasche" };     // 0
            modData.modelData.Add(new ModelData("isa_re_metarig_IK Variant", modelParts_isaRe, new bool[modelParts_isaRe.Length])); // M2

            String[] modelParts_MNHR = { "MNHRBody_001", "MNHRFaceplate_001" };                                             // 0
            modData.modelData.Add (new ModelData("MNHR_Normal_Anim", modelParts_MNHR, new bool[modelParts_MNHR.Length]));   // M2

            // BOS_Adler
            String[] modelParts_FKLR = { "MNHRBody_001", "MNHRFaceplate_001" };                                            
            modData.modelData.Add(new ModelData("FKLR", modelParts_FKLR, new bool[modelParts_FKLR.Length]));   

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