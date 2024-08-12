using Harmony;
using MelonLoader;
using ModOverlayGUI;
using RootMotion.FinalIK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnhollowerBaseLib;
using UnityEngine;

namespace ModelManager
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

        public static bool copyModelSMR(GameObject originObject, GameObject destinationObject)
        { 
            if (originObject == null)
            {
                MelonLogger.Error($"copyModel failed with OriginObject: {originObject}");
                return false;
            }
            if( destinationObject == null)return false;

            SkinnedMeshRenderer meshRendererOrigin = originObject.GetComponent<SkinnedMeshRenderer>();
            SkinnedMeshRenderer meshRendererDestination = destinationObject.GetComponent<SkinnedMeshRenderer>();
            
            if (meshRendererOrigin == null)
            {
                MelonLogger.Error($"copyModel failed Mesh: {originObject}");
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

        public static void setParent(String parentPath, GameObject child)
        {
            GameObject parentObject = GameObject.Find(parentPath);
            if (parentObject != null)
            {
                if (child != null)
                {
                    child.transform.SetParent(parentObject.transform);
                }
                else
                {
                    MelonLogger.Error($"setParent: Child GameObject '{child}' not found.");
                }
            }
            else
            {
                MelonLogger.Error($"setParent: Parent GameObject '{parentPath}' not found.");
            }
        }

        public static void setParent(GameObject parentObject, GameObject child)
        {
            if (parentObject != null)
            {
                if (child != null)
                {
                    child.transform.SetParent(parentObject.transform);
                }
                else
                {
                    MelonLogger.Error($"setParent: Child GameObject '{child}' not found.");
                }
            }
            else
            {
                MelonLogger.Error($"setParent: Parent GameObject '{parentObject}' not found.");
            }
        }

        public static bool setChildActive(String parentPath, String childActivate, bool active)
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
                    child.SetActive(active);
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
        public static Dictionary<String, int> dictList(SkinnedMeshRenderer skin)
        {
            if(skin == null)
            {
                MelonLogger.Error("boneList: Invalid param->null");
                return null;
            }
            Dictionary<String, int> keyValuePairs = new Dictionary<String, int>();
            int idx = 0;
            foreach (Transform bone in skin.bones)
            {
                keyValuePairs.Add(bone.name, idx++);
            }
            return keyValuePairs;
        }

        public static void updatePose(SkinnedMeshRenderer origin, SkinnedMeshRenderer dest, Dictionary<String,int> dict)
        {
            try
            {
                foreach(Transform bone in origin.bones)
                {

                    Transform dest_Transform = dest.bones[dict[bone.name]];
                    if (dest.name == "MNHRBody_001") {
                        if (bone.name == "neck")
                        {
                            Vector3 eul = bone.localEulerAngles;
                            //eul.x = 0;
                            eul.y = dest_Transform.localEulerAngles.y;
                            dest_Transform.localEulerAngles = eul;
                            continue;
                        }else if((bone.name == "shoulder_R" || bone.name == "shoulder_L")&& InventoryManager.EquippedWeapon != null && InventoryManager.EquippedWeapon.name != "Shotgun")
                        {
                            Vector3 eul = bone.localEulerAngles;
                            
                            if (PlayerState.aiming)
                            {
                                eul.y = eul.y * 1.08f;
                                if(bone.name == "shoulder_L") eul.z = eul.z * 0.885f;
                                else eul.z = eul.z * 0.995f;
                            }
                            else
                            {
                                eul.z = eul.z * 1.08f;
                            }
                            dest_Transform.localEulerAngles = eul;
                            continue;
                        }
                    }else if((bone.name == "shoulder_R" || bone.name == "shoulder_L") && (dest.name == "FKLR_Normal" || dest.name == "FKLR_Body_Corrupted")&& PlayerState.aiming && InventoryManager.EquippedWeapon.name != "Shotgun")
                    {
                        Vector3 eul = bone.localEulerAngles;
                        eul.y = eul.y * 1.065f;
                        eul.z = eul.z * 0.99f;
                        dest_Transform.localEulerAngles = eul;
                        continue;
                    }
                    dest_Transform.localRotation = bone.localRotation;
                    if(bone.name == "hips") dest_Transform.localPosition = bone.localPosition;
                }
            }
            catch (Exception e)
            {
                MelonLogger.Error($"updatePost: failed updating Bones \nException: {e.Message}");
            }
        }

        public static void copyComponent(GameObject dest, String comp_or, String name, bool setNull)
        {
            GameObject comp_copy = copyObjectDDOL(comp_or, name, false);
            setParent(dest, comp_copy);
            if (setNull)
            {
                comp_copy.transform.localPosition = Vector3.zero;
                comp_copy.transform.localRotation = new Quaternion(0, 0, 0, 0);
            }
            else
            {
                comp_copy.transform.localPosition = GameObject.Find(comp_or).transform.localPosition;
                comp_copy.transform.localRotation = GameObject.Find(comp_or).transform.localRotation;
            }
        }

        public static SkinnedMeshRenderer insertAlternative(GameObject model_or, String model_s, String name, String body, String normal, Dictionary<String, int> dict)
        {
            model_or.SetActive(true);
            GameObject model = copyObjectDDOL(model_s, model_s, false);
            model_or.SetActive(false);
            setParent("__Prerequisites__/Character Origin/Character Root/Ellie_Default", model);

            GameObject root = GameObject.Find($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/{model_s}/{normal}/Root/");
            root.name = $"Root_{name}";
            setParent("__Prerequisites__/Character Origin/Character Root/Ellie_Default/metarig/", root);
            root.transform.localPosition = Vector3.zero;
            root.transform.localRotation = new Quaternion(0, 0, 0.7071f, 0.7071f);

            SkinnedMeshRenderer skin = model.transform.Find(body).GetComponent<SkinnedMeshRenderer>();
            skin.bones[dict["hips"]].localPosition = Vector3.zero;

            copyComponent(skin.bones[dict["hand_R"]].gameObject, "__Prerequisites__/Character Origin/Character Root/Ellie_Default/metarig/Root/hips/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/WeaponMount/", "WeaponMount", true);
            copyComponent(skin.bones[dict["hips"]].gameObject, "__Prerequisites__/Character Origin/Character Root/Ellie_Default/metarig/Root/hips/VisibleEquip/", "VisibleEquip", true);
            copyComponent(skin.bones[dict["chest"]].gameObject, "__Prerequisites__/Character Origin/Character Root/Ellie_Default/metarig/Root/hips/spine/chest/Nitro Model/", "Nitro Model", false);
            copyComponent(skin.bones[dict["chest"]].gameObject, "__Prerequisites__/Character Origin/Character Root/Ellie_Default/metarig/Root/hips/spine/chest/FlashLightFlare/", "FlashLightFlare", true);

            return skin;
        }

        public static SkinnedMeshRenderer destroyAndSkin(GameObject obj, String body)
        {
            GameObject.Destroy(obj.transform.Find("Camera_Origin").gameObject);
            GameObject.Destroy(obj.transform.Find("LookTarget").gameObject);
            GameObject.Destroy(obj.GetComponent<LookAtIK>());
            GameObject.Destroy(obj.GetComponent<Animator>());
            GameObject.Destroy(obj.GetComponent<CapsuleCollider>());
            return obj.transform.Find(body).GetComponent<SkinnedMeshRenderer>();
        }

        public static void weaponShowcaseActive(String root, bool active)
        {
            if (!active) setChildActive($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/metarig/{root}/hips/spine/chest/", "Nitro Model", active);
            setChildActive($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/metarig/{root}/hips/", "VisibleEquip", active);
            setChildActive($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/metarig/{root}/hips/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/WeaponMount/", "Weapons", active);
        }

        public static void updateWeaponModelsManual(ModData modData, String root)
        {
            for (int i = 0; i < modData.weaponBool.Length; i++)
            {
                if (modData.weaponName[i] == "Nitro Model")
                {
                    HelperMethodsCM.setChildActive($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/metarig/{root}/hips/spine/chest/", "Nitro Model", modData.weaponBool[i]);
                }
                else
                {
                    HelperMethodsCM.setChildActive($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/metarig/{root}/hips/VisibleEquip/", modData.weaponName[i], modData.weaponBool[i]);
                }
            }
        }

        public static void updateWeaponModelsDynamic(String root)
        {
            try
            {
                string[] equipedWeapons = new string[] { "Rifle", "Pistol", "Revolver", "Shotgun", "FlareGun", "Machete" };
                foreach (AnItem item in InventoryManager.elsterItems.keys)
                {
                    bool elsterInventory = equipedWeapons.Contains(item.name);
                    if (elsterInventory)
                    {
                        bool active = false;
                        if ((InventoryManager.EquippedWeapon != null && !InventoryManager.EquippedWeapon.parentItem.Equals(item)) || InventoryManager.EquippedWeapon == null) active = true;
                        if (item.name == "Rifle")
                        {
                            HelperMethodsCM.setChildActive($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/metarig/{root}/hips/spine/chest/", "Nitro Model", active);
                        }
                        else
                        {
                            string visibleName = item.name;
                            switch (visibleName)
                            {
                                case ("FlareGun"):
                                    visibleName = "FGun Model";
                                    break;
                                case ("Revolver"):
                                    visibleName = "Revolver Model";
                                    break;
                            }
                            HelperMethodsCM.setChildActive($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/metarig/{root}/hips/VisibleEquip/", visibleName, active);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex.Message);
            }
        }

        public static void weaponScaling(String root, float size)
        {
            AnWeapon equipedWeapon = InventoryManager.EquippedWeapon;
            if (equipedWeapon != null)
            {
                GameObject weaponSize = GameObject.Find($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/metarig/{root}/hips/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/WeaponMount/Weapons/{equipedWeapon.parentItem.name}");
                if (weaponSize != null) weaponSize.transform.localScale = new Vector3(size, size, size);
            }
        }
    }
}