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
using HarmonyLib;
using TMPro;
using ModOverlayGUI;
using UnityEditor;
using System.Runtime.InteropServices;


namespace ModelManager
{
    internal class Costumizer: MelonMod
    {
        private static bool loaded = false;
        private static AsyncOperation asyncLoad;
        private static int sceneCounter = 0;
        private static string[] scenesLoading = { "DET_Detention", "RES_Residential", "EXC_Mines" };
        private static List<GameObject> modelList = new List<GameObject>();
        private static bool activeHook = false;
        private static bool finishedLoading = false;
        private static Scene hookScene;

        private static ModData modData;
        private static int version;

        private static FullScreenMode playerScreenMode;

        private static string targetDirectory;
        private static string currentDirectory;
        private static bool guiLoaded = false;
        private static Resolution currentResolution;

        public override void OnApplicationStart()
        {
            currentDirectory = System.Environment.CurrentDirectory;
            targetDirectory = Path.Combine(currentDirectory, "Mods", "ModGUI");
            Directory.SetCurrentDirectory(targetDirectory);
            modData = ModDataManager.LoadModData();
            if (modData != null)
            {
                version = modData.call;
            }
            Directory.SetCurrentDirectory(currentDirectory);
        }


        public override void OnUpdate()
        {
            base.OnUpdate();
            Directory.SetCurrentDirectory(targetDirectory);
            modData = ModDataManager.LoadModData();
            Directory.SetCurrentDirectory(currentDirectory);
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
                if (hookScene.name != SceneManager.GetActiveScene().name) setModels(true);
                if (GameObject.Find("__Prerequisites__/Character Origin/Character Root/Ellie_Default/isa_metarig_IK") == null) setModels(true);
                Ellie_DefaultModels.updateModels(modData, true);
                if (version < modData.call)
                {
                    setModels(false);
                    version = modData.call;
                }
                if (modData.weaponShowCase)Ellie_DefaultModels.updateWeaponModelsDynamic();
                
            }
        }

        
        private static void screeenState()
        {
            if (modData != null)
            {
                if (modData.windowed && Screen.fullScreenMode != FullScreenMode.Windowed)
                {
                    UnityEngine.Application.runInBackground = true;
                    playerScreenMode = Screen.fullScreenMode;
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                }
                else if ( !modData.windowed && Screen.fullScreenMode == FullScreenMode.Windowed)
                {
                    UnityEngine.Application.runInBackground = false;
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

        private static void setModels(bool missing)
        {
            Scene currScene = SceneManager.GetActiveScene();
            if (GameObject.Find("__Prerequisites__/Character Origin/Character Root/Ellie_Default") != null)
            {
                if(hookScene == null || !(hookScene.name == currScene.name))
                {
                    MelonLogger.Msg("Hook: new scene detected, insert models");
                    hookScene = currScene;
                    DET_DetentionModels.insertModels(modData);
                    RES_ResidentialModels.insertModels(modData);
                    EXC_MinesModels.insertModels(modData);
                    Ellie_DefaultModels.localHeight = 0.0f;
                }
                Ellie_DefaultModels.updateModels(modData, true);
                RES_ResidentialModels.updateModels(modData);
                DET_DetentionModels.updateModels(modData);
                EXC_MinesModels.updateModels(modData);
                if (!modData.weaponShowCase) Ellie_DefaultModels.updateWeaponModelsManual(modData);
            }
        }

    }
}