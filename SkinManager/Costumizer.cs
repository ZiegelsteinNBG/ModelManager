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
using Il2CppSystem.Threading;

namespace SkinManager
{
    internal class Costumizer: MelonMod
    {

        public override void OnApplicationStart()
        {
            MelonLogger.Msg("Set runInBackground to yes for Overlay...");
            UnityEngine.Application.runInBackground = true;
            MelonLogger.Msg("Loading the ModOverlay...");
            runGui();
        }

        private bool loaded = false;
        private AsyncOperation asyncLoad;
        private int sceneCounter = 0;
        private string[] scenesLoading = { "DET_Detention", "RES_Residential", "EXC_Mines" };
        private List<GameObject> modelList = new List<GameObject>();
        public override void OnUpdate()
        {
            base.OnUpdate();
            Scene scene = SceneManager.GetActiveScene();
            if (!loaded && scene.name == "MainMenu")
            {
                if(asyncLoad == null)asyncLoad = SceneManager.LoadSceneAsync(scenesLoading[sceneCounter], LoadSceneMode.Additive);
            }
            if (asyncLoad != null && asyncLoad.isDone && !loaded)
            {
                OnSceneLoaded(scenesLoading[sceneCounter]);
            }
        }
        private void OnSceneLoaded(string sceneName)
        {
            Scene loadedScene = SceneManager.GetSceneByName(sceneName);
            List<GameObject> list = new List<GameObject>();
            if (loadedScene.IsValid())
            {
                MelonLogger.Msg($"Loaded Scene: {loadedScene.name}. Start loading Models.");
                if (loadedScene.name == "DET_Detention")
                {
                    list = DET_DetentionModels.loadModels();
                }
                //else if (loadedScene.name == "RES_Residential") ;
                //else if (loadedScene.name == "EXC_Mines") ;
            }
            foreach(GameObject childObject in list)
            {
                modelList.Add(childObject);
            }
            SceneManager.UnloadSceneAsync(sceneName);
            if(sceneCounter == scenesLoading.Length - 1)
            {
                SceneManager.LoadScene("MainMenu");
                loaded = true;
                return;
            }
            sceneCounter++;
            asyncLoad = SceneManager.LoadSceneAsync(scenesLoading[sceneCounter], LoadSceneMode.Additive);
        }

        private void runGui()
        {
            // Current directory of the game
            string currentDirectory = System.Environment.CurrentDirectory;
            MelonLogger.Msg($"Current Directory: {currentDirectory}");

            // Navigate to the GUI.exe directory
            string targetDirectory = Path.Combine(currentDirectory, "Mods", "ModGUI");
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
    }
}
    
