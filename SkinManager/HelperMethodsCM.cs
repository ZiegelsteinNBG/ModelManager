using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SkinManager
{
    public class HelperMethodsCM
    {
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

        public static GameObject copyObjectDDOL(String originPath, String newName, bool dontDestroy)
        {
            GameObject originObject = GameObject.Find(originPath);
            if (originObject == null)
            {
                MelonLogger.Error($"Method copyObjectDDOL failed at originPath: {originPath}");
                return null;
            }
            GameObject copy = GameObject.Instantiate(originObject);

            copy.name = newName;
            if(dontDestroy)GameObject.DontDestroyOnLoad(copy);
            return copy;
        }

        public static bool copyModelSMR(String originPath, GameObject destinationObject)
        {
            GameObject originObject = GameObject.Find(originPath);
            if (originObject == null)
            {
                MelonLogger.Error($"copyModel failed at originPath: {originPath}");
                return false;
            }
            if( destinationObject == null)return false;

            SkinnedMeshRenderer meshRendererOrigin = originObject.GetComponent<SkinnedMeshRenderer>();
            SkinnedMeshRenderer meshRendererDestination = destinationObject.GetComponent<SkinnedMeshRenderer>();
            if (meshRendererOrigin == null)
            {
                MelonLogger.Error($"copyModel failed Mesh: {originPath}");
                return false;
            }
            if (meshRendererDestination == null)
            {
                MelonLogger.Error($"copyModel failed Mesh: {destinationObject.name}");
                return false;
            }
            meshRendererDestination.materials = meshRendererOrigin.materials;
            meshRendererDestination.sharedMesh = meshRendererOrigin.sharedMesh;
            return true;
        }
        public static void setParent(String parentPath, String childPath)
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

        public static bool setChildActive(String parentPath, String childActivate)
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
                    return true;
                }
                else
                {
                    MelonLogger.Error($"setChildActive: Child GameObject '{childActivate}' not found within '{parentObject.name}'.");
                    return false;
                }
            }
            else
            {
                MelonLogger.Error($"setChildActive: Parent GameObject '{parentPath}' not found.");
                return false;
            }
        }

        public static bool setChildActive(GameObject parent, String childActivate)
        {

            if (parent != null)
            {
                // Find Child
                Transform childTransform = parent.transform.Find(childActivate);
                if (childTransform != null)
                {
                    // Set ChildObject Active
                    GameObject child = childTransform.gameObject;
                    child.SetActive(true);
                    return true;
                }
                else
                {
                    MelonLogger.Error($"setChildActive: Child GameObject '{childActivate}' not found within '{parent.name}'.");
                    return false;
                }
            }
            else
            {
                MelonLogger.Error($"setChildActive: Parent GameObject '{parent}' not found.");
                return false;
            }
        }
    }
}
