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
        
        private static bool bodyActive;
        private static String root = "Root";

        public static void updateModels(ModData modData, bool prepare)
        {
            string modelEx = "";
            string modelPartEx = "";
            try {
                bodyActive = bodyActiveM(modData);
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
                    // TODO Implementation for M2
                    if (bodyActive) HelperMethodsCM.weaponScaling(root, modData.weaponModelSize);

                    GameObject modelSize = GameObject.Find("__Prerequisites__/Character Origin/Character Root/Ellie_Default/");
                    if (modData != null) modelSize.transform.localScale = new Vector3(modData.playerModelSize, modData.playerModelSize, modData.playerModelSize);

                    // TODO Implementation for M2
                    HelperMethodsCM.weaponShowcaseActive(root, bodyActive);
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
            if (bodyActive)  HelperMethodsCM.updateWeaponModelsManual(modData, root);    
        }

        private static bool bodyActiveM(ModData data)
        {
            foreach(ModelData model in data.modelData)
            {
                if (model.M1 && model.active[model.bodyIdx])return true;
            }
            return false;
        }

        public static void updateWeaponModelsDynamic()
        {
            if (bodyActive) HelperMethodsCM.updateWeaponModelsDynamic(root);
        }
    }
}
