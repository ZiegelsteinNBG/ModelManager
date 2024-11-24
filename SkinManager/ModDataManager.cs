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
        private static string TempFilePath = "CMData2_tmp.xml";

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
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine("Inner exception: " + ex.InnerException.Message);
                    }
                    return null;
                }
            }
        }


        public static void SaveModDataSets(ModDataSets modData)
        {
            lock (lockObject)
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ModDataSets));

                    // Save to temporary file
                    using (FileStream fs = new FileStream(TempFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        serializer.Serialize(fs, modData);
                        fs.Close();
                    }

                    // Replace the original file
                    if (File.Exists(FilePath))
                    {
                        ReplaceFileWithRetry(TempFilePath, FilePath, "BackupXML.bak"); 
                    }
                    else
                    {
                        File.Move(TempFilePath, FilePath); // Move temp file to destination if original doesn't exist
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error saving data: " + ex.Message);
                }
            }
        }


        static void ReplaceFileWithRetry(string source, string destination, string backup = null)
        {
            const int maxAttempts = 3;
            const int delayBetweenAttempts = 100; // Milliseconds
            int attempts = 0;

            while (attempts < maxAttempts)
            {
                try
                {
                    File.Replace(source, destination, backup);
                    return;
                }
                catch (IOException)
                {
                    attempts++;
                    Task.Delay(delayBetweenAttempts).Wait();
                }
            }

            throw new IOException("Unable to replace file after multiple attempts.");
        }

        public static ModDataSets LoadModDataSet()
        {
            lock (lockObject)
            {
                int attempts = 3;
                while (attempts > 0)
                {
                    try
                    {
                        if (!File.Exists(FilePath))
                        {
                            return new ModDataSets(); // Return default if file doesn't exist
                        }

                        XmlSerializer serializer = new XmlSerializer(typeof(ModDataSets));
                        using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            ModDataSets set = (ModDataSets)serializer.Deserialize(fs);
                            fs.Close();
                            return set;
                        }
                    }
                    catch (IOException)
                    {
                        attempts--;
                        Task.Delay(100).Wait(); // Wait briefly before retrying
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error loading settings: " + ex.Message);
                        ModDataSets backup = LoadBackupFile("BackupXML.bak");
                        if(backup != null) Console.WriteLine("Loading Backup successfull");
                        return backup;
                    }
                }
                return null;
                /*
                try
                {
                    if (!File.Exists(FilePath))
                    {
                        
                        return new ModDataSets(); // Return default settings if file doesn't exist
                    }

                    XmlSerializer serializer = new XmlSerializer(typeof(ModDataSets));
                    using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        return (ModDataSets)serializer.Deserialize(fs);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error loading settings: " + ex.Message);
                    return new ModDataSets(); // Return default settings in case of error
                }
                */
            }
        }

        static ModDataSets LoadBackupFile(string backupFilePath)
        {
            try
            {
                if (!File.Exists(backupFilePath))
                {
                    Console.WriteLine("Backup file not found: " + backupFilePath);
                    return null;
                }
                ModDataSets set = null;
                XmlSerializer serializer = new XmlSerializer(typeof(ModDataSets));
                using (FileStream fs = new FileStream(backupFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    set = (ModDataSets)serializer.Deserialize(fs);

                }
                if (set != null)
                {
                    using (FileStream fs = new FileStream(TempFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        serializer.Serialize(fs, set);
                        fs.Close();
                    }
                    ReplaceFileWithRetry(TempFilePath, FilePath);
                }
                return set;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading backup file: " + ex.Message);
                return null;
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
            modData.modelData.Add(new ModelData("Normal", modelParts_ELNormal, new bool[] { true, true, true, true, true}, 1, true));  
            String[] modelParts_ELArmor = { "Body", "Hair", "HairHead", "Armor", "TascheArmor" };                      
            modData.modelData.Add(new ModelData("Armored", modelParts_ELArmor, new bool[modelParts_ELArmor.Length], 0, true));   

            String[] modelParts_ELEVA = { "Body", "Helmet", "Neck", "Backpack", "TascheArmor", "Visor", "Visor Layer2"};    
            modData.modelData.Add(new ModelData("EVA", modelParts_ELEVA, new bool[modelParts_ELEVA.Length], 0, true));               

            String[] modelParts_ELCrippled = { "Body", "Organs", "Hair", "HairHead"};                                           
            modData.modelData.Add(new ModelData("Crippled", modelParts_ELCrippled, new bool[modelParts_ELCrippled.Length], 0, true));   

            String[] modelParts_IsaPast = { "Body", "Hair", "HairHead", "Skirt", "Braid"};                              
            modData.modelData.Add(new ModelData("Isa_Past", modelParts_IsaPast, new bool[modelParts_IsaPast.Length], 0, true));     

            // DET_Detention (5-9)
            String[] modelParts_isa_metarig_IK = { "Body", "Poncho", "Hair_Isa", "HairHead_Isa", "Braid" };                                
            modData.modelData.Add(new ModelData("isa_metarig_IK", modelParts_isa_metarig_IK, new bool[modelParts_isa_metarig_IK.Length], 0, true)); 
            String[] modelParts_arianeGhost = { "Body", "Hair_Ariane", "HairHead_Ariane", "HairLong", "Skirt" };                               
            modData.modelData.Add(new ModelData("ariane_metarig_IK_ghost", modelParts_arianeGhost, new bool[modelParts_arianeGhost.Length], 0, true));  

            String[] modelParts_arianeUniform = { "Body", "Hair_Ariane", "HairHead_Ariane", "Tasche" };                                             
            modData.modelData.Add(new ModelData("ariane_metarig_IK_uniform", modelParts_arianeUniform, new bool[modelParts_arianeUniform.Length], 0, true));

            String[] modelParts_alina = { "AlinaBodyDetailsArmor", "Body", "FemaleMilitaryHat", "Hair_Alina", "HairHead_Alina", "PonyTail", "Tasche"};  
            modData.modelData.Add(new ModelData("alina_metarig_IK", modelParts_alina, new bool[modelParts_alina.Length], 1, true));                              

            String[] modelParts_STCR = { "STCR_Baton", "STCR_Belt", "STCR_Body", "STCR_Hair", "STCR_HairHead" };           
            modData.modelData.Add(new ModelData("STCR_Normal_Anim", modelParts_STCR, new bool[modelParts_STCR.Length], 2, false));    

            // RES_Residential (10-12)
            String[] modelParts_adler = { "Body", "Hair_Adler" };                                                           
            modData.modelData.Add(new ModelData("adler_metarig_IK", modelParts_adler, new bool[modelParts_adler.Length], 0, true));  

            String[] modelParts_ARAR = { "ARAR_Hair", "ARAR_HairHead", "ARAR_NormalBody", "Amigasa" };              
            modData.modelData.Add(new ModelData("ARAR_Normal", modelParts_ARAR, new bool[modelParts_ARAR.Length], 2, true));

            String[] modelParts_KLBR = { "KLBR_Armor", "KLBR_Belt", "KLBR_Hair", "KLBR_HairHead", "KLBR_Normal_Body" };     
            modData.modelData.Add(new ModelData("KLBR_Normal_Anim", modelParts_KLBR, new bool[modelParts_KLBR.Length], 4, false));    

            // EXC_Mines (13-16)
            String[] modelParts_STAR = { "STAR_Hair", "STAR_HairHead", "STARBaton_001", "STARBody_001" };                   
            modData.modelData.Add(new ModelData("STAR_Normal_Anim", modelParts_STAR, new bool[modelParts_STAR.Length], 3, false));

            String[] modelParts_EULR = { "EULR_Hair", "EULR_HairHead", "EULR_Normal_Body", "FemaleMilitaryHat_001" };     
            modData.modelData.Add(new ModelData("EULR_Normal_Anim", modelParts_EULR, new bool[modelParts_EULR.Length], 2, true));   

            String[] modelParts_isaRe = { "Body", "Braid", "Hair_Isa", "HairHead_Isa", "Isas_Knife", "Skirt_Short", "Tasche", "Knife" };    
            modData.modelData.Add(new ModelData("isa_re_metarig_IK Variant", modelParts_isaRe, new bool[modelParts_isaRe.Length], 0, false)); 

            String[] modelParts_MNHR = { "MNHRBody_001" };//"MNHRFaceplate_001"                                             
            modData.modelData.Add (new ModelData("MNHR_Normal_Anim", modelParts_MNHR, new bool[modelParts_MNHR.Length], 0, false));

            // BOS_Adler
            // >> FKLR
            // STARShield

            // FKLR>>Corrupted
            // FKLR_Body_Corrupted

            // FKLR>>Normal
            // FKLR_Normal
            // FKLR_Normal_001
            // STCR_Hair
            // STCR_HairHead

            // metarig>>Root_FKLR>>hips>>SpearL-1 / -3 / SpearR-1 / -3
            // metarig>>Root_FKLR>>hips>>spine>>chest>>Halo
            // metarig/Root_FKLR/hips/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/Pivot
            // FKLR_SpearHand
            // FKLR_Sword

            String[] modelParts_FKLR = { "STARShield", "FKLR_Body_Corrupted", "FKLR_Normal", "FKLR_Normal_001", "STCR_Hair", "STCR_HairHead", "SpearL-1", "SpearL-2", "SpearL-3", "SpearR-1", "SpearR-2", "SpearR-3", "Halo", "FKLR_SpearHand", "FKLR_Sword"};//, "Point Light"                                    
            modData.modelData.Add(new ModelData("FKLR_Anim", modelParts_FKLR, new bool[modelParts_FKLR.Length], -1, false));   

            return modData;
        }
    }
    [Serializable]
    public class ModelData
    {
        public string modelName { get; set; }
        [XmlArray("modelParts")]
        [XmlArrayItem("string")]
        public string[] modelParts { get; set; }
        [XmlArray("active")]
        [XmlArrayItem("boolean")]
        public bool[] active { get; set; }
        public int bodyIdx { get; set; }
        public bool M1 { get; set; }

        public ModelData() { }
        public ModelData(String modelName, String[] modelParts, bool[] active, int bodyIdx, bool M1)
        {
            this.modelName = modelName;
            this.modelParts = modelParts;
            this.active = active;
            this.bodyIdx = bodyIdx;
            this.M1 = M1;
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
        [XmlElement("ModelData")]
        public List<ModelData> modelData { get; set; }
        public float localHeight { get; set; }

        public bool windowed { get; set; }
        public ModelData FindModelDataByName(string modelName)
        {
            return modelData.FirstOrDefault(md => md.modelName == modelName);
        }

        // WeaponModels
        public float weaponModelSize { get; set; }
        [XmlArray("weaponName")]
        [XmlArrayItem("string")]
        public string[] weaponName { get; set; }
        public bool weaponShowCase { get; set; }
        [XmlArray("weaponBool")]
        [XmlArrayItem("boolean")]
        public bool[] weaponBool { get; set; }
    }
    [Serializable]
    public class ModDataSets
    {
        public int aktiv { get; set; }
        public ModData[] modDatas { get; set; }

        public ModDataSets()
        {
            aktiv = 0;
            modDatas = new ModData[5];
            for (int i = 0; i < modDatas.Length; i++)
            {
                modDatas[i] = ModDataManager.iniModData();
            }
        }
    }
}