using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MelonLoader;
using System.IO;
using System.Diagnostics;
using UnityEngine.SceneManagement;
using System;
using UnityEngine;

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
        bool loaded = false;
        public override void OnUpdate()
        {
            base.OnUpdate();
            if (!loaded)
            {
                //loaded = true;
            }
            if (false)
            {
                Scene scene = SceneManager.GetActiveScene();
                MelonLogger.Msg($"Loaded {scene.name}");
                if(scene.name == "LOV_Reeducation")
                {
                    runGui();
                }
            }

        }
        public static string getFullPath(GameObject gameObject)
        {
            string path = "/" + gameObject.name;
            Transform current = gameObject.transform;
            while (current.parent != null)
            {
                current = current.parent;
                path = "/" + current.name + path;
            }
            return path;
        }

        private GameObject copyObjectDDOL(String originPath, String newName)
        {
            GameObject originObject = GameObject.Find(originPath);
            if(originObject == null)
            {
                MelonLogger.Error($"Method copyObjectDDOL failed at originPath: {originPath}");
                return null;
            }
            GameObject copy = GameObject.Instantiate(originObject);
            copy.name = newName;
            GameObject.DontDestroyOnLoad(copy);
            return copy;
        }

        private void copyModelSMR(String originPath, String destinationPath)
        {
            GameObject originObject = GameObject.Find(originPath);
            GameObject destinationObject = GameObject.Find(destinationPath);
            if (originObject == null)
            {
                MelonLogger.Error($"copyModel failed at originPath: {originPath}");
                return;
            }
            if (destinationObject == null)
            {
                MelonLogger.Error($"copyModel failed at originPath: {destinationPath}");
                return;
            }

            SkinnedMeshRenderer meshRendererOrigin = originObject.GetComponent<SkinnedMeshRenderer>();
            SkinnedMeshRenderer meshRendererDestination = destinationObject.GetComponent<SkinnedMeshRenderer>();
            if (meshRendererOrigin == null)
            {
                MelonLogger.Error($"copyModel failed Mesh: {originPath}");
                return;
            }
            if (meshRendererDestination == null)
            {
                MelonLogger.Error($"copyModel failed Mesh: {destinationPath}");
                return;
            }
            meshRendererDestination.materials = meshRendererOrigin.materials;
            meshRendererDestination.sharedMesh = meshRendererOrigin.sharedMesh;
        }
        private void setParent(String parentPath, String childPath)
        {
            GameObject parentObject = GameObject.Find(parentPath);
            if (parentObject != null)
            {
                GameObject childObject = GameObject.Find(childPath);
                if (childObject != null)
                {
                    childObject.transform.SetParent(parentObject.transform);
                }
                else
                {
                    MelonLogger.Error($"setParent: Child GameObject '{childPath}' not found.");
                }
            }
            else
            {
                MelonLogger.Error($"setParent: Parent GameObject '{parentPath}' not found.");
            }
        }

        private void setChildActive(String parentPath, String childActivate)
        {
            // Find Parent
            GameObject parentObject = GameObject.Find(parentPath);
            if (parentObject != null)
            {
                // Find Child
                Transform childTransform = parentObject.transform.Find(childActivate);
                if (childTransform != null)
                {
                    // Set ChildObject Active
                    GameObject child = childTransform.gameObject;
                    child.SetActive(true);
                }
                else
                {
                    MelonLogger.Error($"setChildActive: Child GameObject '{childActivate}' not found within '{parentObject.name}'.");
                }
            }
            else
            {
                MelonLogger.Error($"setChildActive: Parent GameObject '{parentPath}' not found.");
            }
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
    
