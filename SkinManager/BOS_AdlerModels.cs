using LlockhamIndustries.Decals;
using MelonLoader;
using ModelManager;
using ModOverlayGUI;
using RootMotion.FinalIK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;
using static BrightnessSlider;
using static RootMotion.Demos.Turret;
using static UTJ.FrameCapturer.DataPath;

namespace ModelManager
{
    public class BOS_AdlerModels
    {
        private static GameObject models;
        private static GameObject activeModels;

        private static Dictionary<String, int> dict_fklr = new Dictionary<String, int>();
        private static GameObject fklr_root;
        private static SkinnedMeshRenderer fklr_skin;
        private static bool fklr_enabled, corrupted_enabled, normal_enabled;

        public static List<GameObject> loadModels()
        {
            // FKLR
            HelperMethodsCM.setChildActive("Arena/", "Chunk", true);
            GameObject fklr = HelperMethodsCM.copyObjectDDOL("Arena/Enemy Manager/Falke_Manager/FKLR/FKLR/", "FKLR_Anim", true);
            GameObject.Destroy(fklr.transform.Find("Camera_Origin").gameObject);
            GameObject.Destroy(fklr.transform.Find("Point Light").gameObject);
            //GameObject.Destroy(fklr.transform.Find("Corrupted/FKLR_Body_CorruptedFX").gameObject);
            //GameObject.Destroy(fklr.transform.Find("Normal/FKLR_Normal_1").gameObject);
            //GameObject.Destroy(fklr.transform.Find("Normal/STCR_Hair_1").gameObject);
            //GameObject.Destroy(fklr.transform.Find("Normal/STCR_HairHead_1").gameObject); //FKLR_Normal_002
            //GameObject.Destroy(fklr.transform.Find("Normal/FKLR_Normal_002").gameObject);
            GameObject.Destroy(fklr.GetComponent<Animator>());
            GameObject.Destroy(fklr.GetComponent<EnemyFXMeshOption>());
            GameObject.Destroy(fklr.transform.Find("FKLR").GetComponent<AimIK>());
            dict_fklr = HelperMethodsCM.dictList(fklr.transform.Find("Normal/FKLR_Normal").GetComponent<SkinnedMeshRenderer>());
            fklr.SetActive(false);
            models = fklr;
            List<GameObject> list = new List<GameObject>();
            list.Add(models);
            MelonLogger.Msg("Models from BOS_AdlerModels loaded");
            return list;
        }

        public static void updateModels(ModData data, float diffHeight)
        {
            try
            {
                ModelData modelData = data.FindModelDataByName("FKLR_Anim");
                fklr_enabled = modelData.active[1] || modelData.active[2];
                HelperMethodsCM.weaponShowcaseActive(fklr_root.name, fklr_enabled);
                if (fklr_enabled) HelperMethodsCM.weaponScaling(fklr_root.name, data.weaponModelSize);
                if (modelData != null)
                {
                    bool normal = false;
                    for (int i = 0; i < modelData.modelParts.Length; i++)
                    {
                        String part = modelData.modelParts[i];
                        if (i == 0)//STARShield
                        {
                            HelperMethodsCM.setChildActive($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/FKLR_Anim/", part, modelData.active[i]);
                        }
                        else if (i == 1)// FKLR_Body_Corrupted
                        {
                            HelperMethodsCM.setChildActive($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/FKLR_Anim/Corrupted/", part, modelData.active[i]);
                            corrupted_enabled = modelData.active[i];
                        }
                        else if (i >= 2 && i <= 5)// FKLR_Normal/FKLR_Normal_001/STCR_Hair/STCR_HairHead
                        {
                            HelperMethodsCM.setChildActive($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/FKLR_Anim/Normal/", part, modelData.active[i]);
                            normal = normal || modelData.active[i];
                        }
                        else if (i >= 6 && i <= 11)// SpearR-1-3/SpearL-1-3
                        {
                            HelperMethodsCM.setChildActive($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/metarig/Root_FKLR/hips/", part, modelData.active[i]);
                        }
                        else if (i == 12)// Halo
                        {
                            HelperMethodsCM.setChildActive($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/metarig/Root_FKLR/hips/spine/chest/", part, modelData.active[i]);
                        }
                        else if (i >= 13 && i <= 14)
                        {
                            HelperMethodsCM.setChildActive($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/metarig/Root_FKLR/hips/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/Pivot/", part, modelData.active[i]);
                        }
                    }
                    normal_enabled = normal;
                    GameObject charHeight = GameObject.Find($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/metarig/{fklr_root.name}");
                    if (charHeight != null) charHeight.transform.localPosition = charHeight.transform.localPosition - new Vector3(0, 0, diffHeight);

                }
            }catch (Exception e)
            {
                MelonLogger.Error($"BOS_AdlerModels/ updateModels\nException: {e.Message}\nStacktrace: {e.StackTrace}");
            }
        }

        public static void updateFX()
        {
            GameObject Room = PlayerState.currentRoom.gameObject;
            bool enemies = Room.transform.Find("Enemy Manager") != null;
            if (corrupted_enabled)
            {
                HelperMethodsCM.setChildActive($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/FKLR_Anim/Corrupted/", "FKLR_Body_CorruptedFX", enemies);
            }else HelperMethodsCM.setChildActive($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/FKLR_Anim/Corrupted/", "FKLR_Body_CorruptedFX", false);
            if (normal_enabled)
            {
                HelperMethodsCM.setChildActive($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/FKLR_Anim/Normal/", "FKLR_Normal_1", enemies);
                HelperMethodsCM.setChildActive($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/FKLR_Anim/Normal/", "STCR_Hair_1", enemies);
                HelperMethodsCM.setChildActive($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/FKLR_Anim/Normal/", "STCR_HairHead_1", enemies);
                HelperMethodsCM.setChildActive($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/FKLR_Anim/Normal/", "FKLR_Normal_002", enemies);
            }
            else
            {
                HelperMethodsCM.setChildActive($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/FKLR_Anim/Normal/", "FKLR_Normal_1", false);
                HelperMethodsCM.setChildActive($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/FKLR_Anim/Normal/", "STCR_Hair_1", false);
                HelperMethodsCM.setChildActive($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/FKLR_Anim/Normal/", "STCR_HairHead_1", false);
                HelperMethodsCM.setChildActive($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/FKLR_Anim/Normal/", "FKLR_Normal_002", false);

            }
        }

        public static void updatePose(ModData data)
        {
            SkinnedMeshRenderer defaultSkin = GameObject.Find("__Prerequisites__/Character Origin/Character Root/Ellie_Default/Normal/Body").GetComponent<SkinnedMeshRenderer>();
            HelperMethodsCM.updatePose(defaultSkin, fklr_skin, dict_fklr);
        }

        public static void updateWeaponModelsManual(ModData modData)
        {
            if (!fklr_enabled) return;
            HelperMethodsCM.updateWeaponModelsManual(modData, fklr_root.name);
        }

        public static void updateWeaponModelsDynamic()
        {
            if (!fklr_enabled) return;
            HelperMethodsCM.updateWeaponModelsDynamic(fklr_root.name);
        }

        public static void insertModels()
        {
            models.transform.Find("Normal").gameObject.SetActive(true);
            fklr_skin = HelperMethodsCM.insertAlternative(models, "FKLR_Anim", "FKLR", "Normal/FKLR_Normal", "FKLR", dict_fklr);
            models.transform.Find("Normal").gameObject.SetActive(false);
            fklr_root = GameObject.Find("__Prerequisites__/Character Origin/Character Root/Ellie_Default/metarig/Root_FKLR");
            activeModels = (GameObject.Find("__Prerequisites__/Character Origin/Character Root/Ellie_Default/FKLR/"));
}
    }
}
