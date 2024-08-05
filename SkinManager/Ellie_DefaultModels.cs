using MelonLoader;
using ModOverlayGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModelManager
{
    public class Ellie_DefaultModels
    {
        private static string[] modelNames = new string[]{ "Normal", "Armored", "Crippled", "EVA","Isa_Past"};
        private static string[] equipedWeapons = new string[] { "Rifle", "Pistol", "Revolver", "Shotgun", "FlareGun", "Machete"};
        public static float localHeight { get; set; }

        public static void updateModels(ModData modData, bool prepare)
        {
            string modelEx = "";
            string modelPartEx = "";
            try { 
                foreach (string model in modelNames)
                {
                    modelEx = model;
                    ModelData modelData = modData.FindModelDataByName(model);

                    for (int i = 0; i < modelData.modelParts.Length; i++)
                    {
                        if (prepare) HelperMethodsCM.setChildActive("__Prerequisites__/Character Origin/Character Root/Ellie_Default/", model, true);
                        
                        string part = modelData.modelParts[i];
                        modelPartEx = part;
                        GameObject gameObject = GameObject.Find($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/{model}/{part}");
                        if (part == "Hat")
                        {
                            HelperMethodsCM.setChildActive($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/", part, modelData.active[i]);
                            continue;
                        }else if (model == "Normal" && part == "Body")
                        {
                            gameObject.SetActive(true);
                            SkinnedMeshRenderer meshRendererOrigin = gameObject.GetComponent<SkinnedMeshRenderer>();
                            meshRendererOrigin.GetComponent<Renderer>().castShadows = modelData.active[i];
                            Material material = meshRendererOrigin.material;
                            if (modelData.active[i])
                            {
                                material.color = new Color(1, 1, 1, 1);
                            }
                            else
                            {
                                material.color = new Color(0, 0, 0, -1);
                            }
                            continue;
                        }
                     
                        if (prepare) gameObject.SetActive(false);
                        else HelperMethodsCM.setChildActive($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/{model}", part, modelData.active[i]);
                    }
                }
                if (!prepare)
                {
                    AnWeapon equipedWeapon = InventoryManager.EquippedWeapon;
                    if (equipedWeapon != null)
                    {
                        GameObject weaponSize = GameObject.Find($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/metarig/Root/hips/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/WeaponMount/Weapons/{equipedWeapon.parentItem.name}");
                        if (weaponSize != null) weaponSize.transform.localScale = new Vector3(modData.weaponModelSize, modData.weaponModelSize, modData.weaponModelSize);
                    }

                    GameObject modelSize = GameObject.Find("__Prerequisites__/Character Origin/Character Root/Ellie_Default/");
                    if (modData != null) modelSize.transform.localScale = new Vector3(modData.playerModelSize, modData.playerModelSize, modData.playerModelSize);

                    // TODO: Remove Height due to bug causes
                    float newHeight = modData.localHeight;
                    float diffHeight = localHeight - newHeight;
                    GameObject charHeight = GameObject.Find($"__Prerequisites__/Character Origin/");
                    if (charHeight != null) charHeight.transform.localPosition = charHeight.transform.localPosition + new Vector3(0, 0, diffHeight);
                    localHeight = modData.localHeight;
                }
                else
                {
                    updateModels(modData, false);
                }
            }
            catch(Exception ex){
                MelonLogger.Error($"updateModels {prepare} failed at EllieDefault: __Prerequisites__/Character Origin/Character Root/Ellie_Default/{modelEx}/{modelPartEx}");
                MelonLogger.Error($"Exception: {ex.Message}");
                MelonLogger.Error($"Stack Trace: {ex.StackTrace}");
            }
        }

        public static void updateWeaponModelsManual(ModData modData)
        {
            for (int i = 0; i < modData.weaponBool.Length; i++) {
                if (modData.weaponName[i] == "Nitro Model")
                {
                    HelperMethodsCM.setChildActive("__Prerequisites__/Character Origin/Character Root/Ellie_Default/metarig/Root/hips/spine/chest/", "Nitro Model", modData.weaponBool[i]);
                }
                else
                {
                    HelperMethodsCM.setChildActive($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/metarig/Root/hips/VisibleEquip/", modData.weaponName[i], modData.weaponBool[i]);
                }
            }    
        }

        // __Prerequisites__/Character Origin/Character Root/Ellie_Default/metarig/Root/hips/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/WeaponMount/Weapons/ Size -> localeScale
        // __Prerequisites__/Character Origin/Character Root/Ellie_Default/

        public static void updateWeaponModelsDynamic()
        {
            try
            {
                foreach (AnItem item in InventoryManager.elsterItems.keys)
                {
                    bool elsterInventory = equipedWeapons.Contains(item.name);
                    if (elsterInventory)
                    {
                        bool active = false;
                        if (InventoryManager.EquippedWeapon != null && !InventoryManager.EquippedWeapon.parentItem.Equals(item)) active = true;
                        if (item.name == "Rifle")
                        {
                            HelperMethodsCM.setChildActive("__Prerequisites__/Character Origin/Character Root/Ellie_Default/metarig/Root/hips/spine/chest/", "Nitro Model", active);
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
                            HelperMethodsCM.setChildActive($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/metarig/Root/hips/VisibleEquip/", visibleName, active);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex.Message);
            }
        }
    }
}
