using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using MelonLoader;
using System.IO;
using System.Diagnostics;
using UnityEngine.SceneManagement;
using System;
using UnityEngine;
using System.Reflection;
using HarmonyLib;
using TMPro;
using ModOverlayGUI;
using UnityEditor;
using System.Runtime.InteropServices;
using Debug = UnityEngine.Debug;
using static PlatformManagement.SaveSystemWorkerBase;



namespace ModelManager
{
    internal class Costumizer: MelonMod
    {
        private static bool loaded = false;
        private static AsyncOperation asyncLoad;
        private static int sceneCounter = 0;
        private static string[] scenesLoading = { "DET_Detention", "RES_Residential", "EXC_Mines", "BOS_Adler" };
        private static List<GameObject> modelList = new List<GameObject>();
        private static bool activeHook = false;
        private static bool finishedLoading = false;
        private static Scene hookScene;

        private static ModData modData;
        private static int version = -1;

        private static FullScreenMode playerScreenMode;

        private static string targetDirectory;
        private static string currentDirectory;
        private static bool guiLoaded = false;
        private static Resolution currentResolution;

        private static bool retryInsert;
        private static bool retryMissing;
        private static float localHeight { get; set; }
        private static int selIdx = -1;
        private static bool started = false;

        private ModDataSets sets;
        private DateTime previousTimestamp = DateTime.MinValue;

        public override void OnApplicationStart()
        {
            try
            {
                startMod(); 
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        public void startMod()
        {
            currentDirectory = System.Environment.CurrentDirectory;
            targetDirectory = Path.Combine(currentDirectory, "Mods", "ModGUI");
            Directory.SetCurrentDirectory(targetDirectory);
            ModDataSets sets = ModDataManager.LoadModDataSet();
            UnityEngine.Application.runInBackground = true;
            modData = sets?.modDatas[sets.aktiv];
            if (modData != null)
            {
                version = modData.call;
                selIdx = sets.aktiv;
            }
            Directory.SetCurrentDirectory(currentDirectory);
            started = true;
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
            if (!started)
            {
                MelonLogger.Log("Restart ModStart");
                startMod();
                return;
            }
            Directory.SetCurrentDirectory(targetDirectory);
            DateTime lastModified = File.GetLastWriteTime("BackupXML.bak");

            // Store this value and compare it later to detect changes
            if (lastModified > previousTimestamp || !File.Exists("BackupXML.bak"))
            {
                Console.WriteLine("The file has been modified.");
                sets = ModDataManager.LoadModDataSet();
                previousTimestamp = lastModified;

            }
            
            if (sets != null)modData = sets.modDatas[sets.aktiv];
            else
            {
                MelonLogger.Msg("Retry loading XML...sets");
                return;
            }
            Directory.SetCurrentDirectory(currentDirectory);
            if(modData == null)
            {
                MelonLogger.Msg("Retry loading XML...data");
                return;
            }
            if(!guiLoaded)modData.windowed = false;
            else if (version < modData.call)
            {
                screeenState();
            }

            Scene scene = SceneManager.GetActiveScene();
            if (!loaded && scene.name == "MainMenu")
            {
                if (asyncLoad == null)
                {
                    ShowLoadingScreen();
                    asyncLoad = SceneManager.LoadSceneAsync(scenesLoading[sceneCounter], LoadSceneMode.Additive);
                }
            }

            if (asyncLoad != null && asyncLoad.isDone && !loaded)
            {
                OnSceneLoaded(scenesLoading[sceneCounter]);
            }
            else if(loaded && asyncLoad.isDone && !finishedLoading)
            {
                finishedLoading = true;
                activeHook = true;
                HideLoadingScreen();
                MelonLogger.Msg("Done Loading Models: " + activeHook);
            }
            else if(loaded && GameObject.Find("__Prerequisites__/Character Origin/Character Root/Ellie_Default") != null)
            {

                if (!guiLoaded)
                {
                    guiLoaded = true;
                    MelonLogger.Msg("Set runInBackground to yes for Overlay...");
                    MelonLogger.Msg("Loading the ModOverlay...");
                    modData.windowed = true;
                    screeenState();
                    runGui();
                }
                if (retryInsert)
                {
                    retryInsert = false;
                    setModels(retryMissing);
                }
                if (GameObject.Find($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/{modData.modelData[modData.modelData.Count-1].modelName}") == null) setModels(true);
                Ellie_DefaultModels.updateModels(modData, true);
                updatePose();
                //MelonLogger.Msg($"Version: {version}    Call: {modData.call}\nSelIdx: {selIdx} Aktiv: {sets.aktiv}");
                if (version < modData.call || selIdx != sets.aktiv)
                {
                    
                    setModels(false);
                    selIdx = sets.aktiv;
                    version = modData.call;
                }
                // TODO ManualUpdate New modData
                if (modData.weaponShowCase)
                {
                    Ellie_DefaultModels.updateWeaponModelsDynamic();
                    BOS_AdlerModels.updateWeaponModelsDynamic();
                    EXC_MinesModels.updateWeaponModelsDynamic();
                    DET_DetentionModels.updateWeaponModelsDynamic();
                    RES_ResidentialModels.updateWeaponModelsDynamic();
                }
                else
                {
                    Ellie_DefaultModels.updateWeaponModelsManual(modData);
                    BOS_AdlerModels.updateWeaponModelsManual(modData);
                    EXC_MinesModels.updateWeaponModelsManual(modData);
                    DET_DetentionModels.updateWeaponModelsManual(modData);
                    RES_ResidentialModels.updateWeaponModelsManual(modData);
                }
                
            }
        }
        
        private static void screeenState()
        {
            if (modData != null)
            {
                if (modData.windowed && Screen.fullScreenMode != FullScreenMode.Windowed)
                {
                    
                    playerScreenMode = Screen.fullScreenMode;
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                }
                else if ( !modData.windowed && Screen.fullScreenMode == FullScreenMode.Windowed)
                {
                    Screen.fullScreenMode = playerScreenMode;
                }
            }
        }
        private static void OnSceneLoaded(string sceneName)
        {
            Scene loadedScene = SceneManager.GetSceneByName(sceneName);
            List<GameObject> list = new List<GameObject>();
            if (loadedScene.IsValid())
            {
                MelonLogger.Msg($"Loaded Scene: {loadedScene.name}. Start loading Models.");
                if (loadedScene.name == "DET_Detention") list = DET_DetentionModels.loadModels();
                else if (loadedScene.name == "RES_Residential") list = RES_ResidentialModels.loadModels();
                else if (loadedScene.name == "EXC_Mines") list = EXC_MinesModels.loadModels();
                else if(loadedScene.name == "BOS_Adler") list   = BOS_AdlerModels.loadModels();
            }
            foreach(GameObject childObject in list)
            {
                modelList.Add(childObject);
            }
            SceneManager.UnloadSceneAsync(sceneName);
            if(sceneCounter == scenesLoading.Length - 1)
            {
                asyncLoad = SceneManager.LoadSceneAsync("MainMenu");
                loaded = true;
                return;
            }
            sceneCounter++;
            asyncLoad = SceneManager.LoadSceneAsync(scenesLoading[sceneCounter], LoadSceneMode.Additive);
        }

        private static void ShowLoadingScreen( )
        {
            HelperMethodsCM.setChildActive("UI", "BookCanvas", true);
            GameObject.Find("Page Indicator").SetActive(false);
            TextMeshProUGUI textMesh = GameObject.Find("Book Content").GetComponent<TextMeshProUGUI>();
            textMesh.m_text = "ModelManager\r\nPreparing models by cycling through scenes, please be patient...";
        }

        private static void HideLoadingScreen()
        {
            GameObject.Find("UI/BookCanvas").SetActive(false);
        }

        private static void runGui()
        {
            // Current directory of the game
            string currentDirectory = System.Environment.CurrentDirectory;
            MelonLogger.Msg($"Current Directory: {currentDirectory}");

            // Navigate to the GUI.exe directory
            targetDirectory = Path.Combine(currentDirectory, "Mods", "ModGUI");
            if (Directory.Exists(targetDirectory))
            {
                Directory.SetCurrentDirectory(targetDirectory);
                MelonLogger.Msg($"Changed Directory to: {targetDirectory}");

                // Execute the EXE inside the Mods/ModGui folder
                string exePath = Path.Combine(targetDirectory, "ModOverlayGUI.exe");
                if (File.Exists(exePath))
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo(exePath);
                    Process process = new Process { StartInfo = startInfo };
                    try
                    {
                        // Default without elevated permissions
                        process.Start();
                        MelonLogger.Msg($"Started executable without elevated permissions: {exePath}");
                    }
                    catch (Exception ex)
                    {
                        // In case it needs elevated permissions
                        MelonLogger.Error($"Failed to start executable without elevated permissions. Please notify the mod-creator about this case: {exePath}. Exception: {ex.Message}");
                        MelonLogger.Msg($"Trying with elevated permissions.");
                        startInfo = new ProcessStartInfo(exePath)
                        {
                            WorkingDirectory = Path.GetDirectoryName(exePath),
                            UseShellExecute = true, 
                            Verb = "runas"
                        };
                        process = new Process { StartInfo = startInfo };
                        try {
                            process.Start();
                            MelonLogger.Msg($"Started executable with elevated permissions: {exePath}");
                        }
                        catch { MelonLogger.Error($"Failed to start executable with elevated permissions: {exePath}. Exception: {ex.Message}"); }
                    }
                    MelonLogger.Msg($"ModOverlay should run by now");
                }
                else
                {
                    MelonLogger.Error($"Executable not found: {exePath}");
                }
            }
            else
            {
                MelonLogger.Error($"Target directory not found: {targetDirectory}");
            }
            // Reset the directory
            Directory.SetCurrentDirectory(currentDirectory);
        }

        private static bool validScene()
        {
            HashSet<string> sceneNames = new HashSet<string> { "PEN_Wreck", "PEN_Hole", "LOV_Reeducation", "DET_Detention", "MED_Medical", "RES_School", "RES_Residential", "EXC_Mines", "LAB_Labyrinth", "MEM_Memory", "BIO_Reeducation", "ROT_Rotfront", "BOS_Adler" };
            Scene currScene = SceneManager.GetActiveScene();
            return sceneNames.Contains(currScene.name);
        }

        private static void updatePose()
        {
            DET_DetentionModels.updatePose(modData);
            RES_ResidentialModels.updatePose(modData);
            EXC_MinesModels.updatePose(modData);
            BOS_AdlerModels.updatePose(modData);
            BOS_AdlerModels.updateFX();
        }
        private static void setModels(bool missing)
        {
            try
            {
                Scene currScene = SceneManager.GetActiveScene();
                if (GameObject.Find("__Prerequisites__/Character Origin/Character Root/Ellie_Default") != null)
                {
                    if (hookScene == null || !(hookScene.name == currScene.name))
                    {
                        MelonLogger.Msg("Hook: new scene detected, insert models");
                        hookScene = currScene;
                        DET_DetentionModels.insertModels(modData);
                        RES_ResidentialModels.insertModels(modData);
                        EXC_MinesModels.insertModels(modData);
                        BOS_AdlerModels.insertModels();
                        
                    }
                    float newHeight = modData.localHeight;
                    float diffHeight = localHeight - newHeight;
                    Ellie_DefaultModels.updateModels(modData, true);
                    RES_ResidentialModels.updateModels(modData, diffHeight);
                    DET_DetentionModels.updateModels(modData, diffHeight);
                    EXC_MinesModels.updateModels(modData, diffHeight);
                    BOS_AdlerModels.updateModels(modData, diffHeight);
                    localHeight = modData.localHeight;
                    if (!modData.weaponShowCase)
                    {
                        Ellie_DefaultModels.updateWeaponModelsManual(modData);
                    }
                }
            }
            catch (Exception e) 
            {
                retryInsert = true;
                retryMissing = missing;
                MelonLogger.Error($"An Error occured insterting the models. Retry\nException: {e.Message}\nStackTrace: {e.StackTrace}");
            }
        }

    }
}