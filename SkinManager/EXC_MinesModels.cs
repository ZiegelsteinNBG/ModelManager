using MelonLoader;
using ModOverlayGUI;
using RootMotion.FinalIK;
using Rotorz.Games;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UTJ.FrameCapturer.DataPath;

namespace ModelManager
{
    public class EXC_MinesModels
    {
        static List<GameObject> models = new List<GameObject>();
        static List<GameObject> activeModels = new List<GameObject>();

        static Dictionary<String, int> dict_star = new Dictionary<String, int>();
        static GameObject star_root;
        static SkinnedMeshRenderer star_skin;
        static bool star_enabled;

        static Dictionary<String, int> dict_isa = new Dictionary<String, int>();
        static GameObject isa_root;
        static SkinnedMeshRenderer isa_skin;
        static bool isa_enabled;

        static Dictionary<String, int> dict_mnhr = new Dictionary<String, int>();
        static GameObject mnhr_root;
        static SkinnedMeshRenderer mnhr_skin;
        static bool mnhr_enabled;
        public static List<GameObject> loadModels()
        {
            HelperMethodsCM.setChildActive("Star Room/", "Chunk", true);
            GameObject star = HelperMethodsCM.copyObjectDDOL("Star Room/Chunk/STAR-S23??/STAR_Normal_Anim", "STAR_Normal_Anim", true);
            star.SetActive(false);
            star_skin = HelperMethodsCM.destroyAndSkin(star, "STARBody_001");
            dict_star = HelperMethodsCM.dictList(star_skin);
            models.Add(star);

            GameObject eulr = HelperMethodsCM.copyObjectDDOL("Star Room/Chunk/STAR-S23??/Deko/EULR_Normal_Anim", "EULR_Normal_Anim", true);
            eulr.SetActive(false);
            models.Add(eulr);

            HelperMethodsCM.setChildActive("Cutscenes/Isa vs Adler/", "EXC_Fight_3 Wideshot", true);
            GameObject isa_re = HelperMethodsCM.copyObjectDDOL("Cutscenes/Isa vs Adler/EXC_Fight_3 Wideshot/CharacterSpace/isa_cutscene/isa_re_metarig_IK Variant", "isa_re_metarig_IK Variant", true);
            isa_re.SetActive(false);
            dict_isa = HelperMethodsCM.dictList(isa_re.transform.Find("Body").GetComponent<SkinnedMeshRenderer>());
            models.Add(isa_re);

            HelperMethodsCM.setChildActive("Monowire/", "Chunk", true);
            GameObject mnhr = HelperMethodsCM.copyObjectDDOL("Monowire/Chunk/MNHR-S2301/MNHR_Normal_Anim/", "MNHR_Normal_Anim", true);
            mnhr.SetActive(false);
            mnhr_skin = HelperMethodsCM.destroyAndSkin(mnhr, "MNHRBody_001");
            dict_mnhr = HelperMethodsCM.dictList(mnhr_skin);
            GameObject faceShield = mnhr.transform.Find("MNHRFaceplate_001").gameObject;
            SkinnedMeshRenderer smr = faceShield.GetComponent<SkinnedMeshRenderer>();
            MeshFilter mf = faceShield.AddComponent<MeshFilter>();
            mf.mesh = smr.sharedMesh;
            Material mat = smr.material;
            GameObject.Destroy(smr);
            MeshRenderer meshR = faceShield.AddComponent<MeshRenderer>();
            meshR.material = mat;
            faceShield.transform.parent = mnhr.transform.Find("MNHR_Normal/Root/hips/spine/chest/neck/head");
            faceShield.SetActive(false);
            models.Add(mnhr);   

            MelonLogger.Msg("Models from EXC_MinesModels loaded");
            return models;
        }

        public static void updateModels(ModData modData, float diffHeight)
        {

            foreach (GameObject model in activeModels)
            {
                ModelData modelData = modData.FindModelDataByName(model.name);
                switch (modelData.modelName)
                {
                    case "isa_re_metarig_IK Variant":
                        isa_enabled = modelData.active[modelData.bodyIdx];
                        HelperMethodsCM.weaponShowcaseActive("Root_isa", isa_enabled);
                        if (isa_enabled) HelperMethodsCM.weaponScaling("Root_isa", modData.weaponModelSize);
                        GameObject charHeight = GameObject.Find($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/metarig/{isa_root.name}");
                        if (charHeight != null) charHeight.transform.localPosition = charHeight.transform.localPosition - new Vector3(0, 0, diffHeight);
                        break;
                    case "STAR_Normal_Anim":
                        star_enabled = modelData.active[modelData.bodyIdx];
                        HelperMethodsCM.weaponShowcaseActive("Root_STAR", star_enabled);
                        if(star_enabled) HelperMethodsCM.weaponScaling("Root_STAR", modData.weaponModelSize);
                        charHeight = GameObject.Find($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/metarig/{star_root.name}");
                        if (charHeight != null) charHeight.transform.localPosition = charHeight.transform.localPosition - new Vector3(0, 0, diffHeight);
                        break;
                    case "MNHR_Normal_Anim":
                        mnhr_enabled = modelData.active[modelData.bodyIdx];
                        HelperMethodsCM.weaponShowcaseActive("Root_MNHR", mnhr_enabled);
                        if (mnhr_enabled) HelperMethodsCM.weaponScaling("Root_MNHR", modData.weaponModelSize);
                        charHeight = GameObject.Find($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/metarig/{mnhr_root.name}");
                        if (charHeight != null) charHeight.transform.localPosition = charHeight.transform.localPosition - new Vector3(0, 0, diffHeight);
                        break;
                }
                for (int i = 0; i < modelData.modelParts.Length; i++)
                {
                    string part = modelData.modelParts[i];
                    if(part == "Knife")
                    {
                        HelperMethodsCM.setChildActive($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/metarig/Root_isa/hips/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/", part, modelData.active[i]);
                        continue;
                    }else if(part == "MNHRFaceplate_001")
                    {
                        continue;
                    }
                    GameObject gameObject = GameObject.Find($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/{model.name}/{part}");
                    if (gameObject != null)
                    {
                        HelperMethodsCM.setChildActive($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/{model.name}", part, modelData.active[i]);
                    }
                    else MelonLogger.Error($"updateModels failed at EXC_MinesModels: __Prerequisites__/Character Origin/Character Root/Ellie_Default/{model.name}/{modelData.modelParts[i]}");
                }
            }
        }
        public static void updatePose(ModData data)
        {
            if (mnhr_skin == null) return;
            SkinnedMeshRenderer defaultSkin = GameObject.Find("__Prerequisites__/Character Origin/Character Root/Ellie_Default/Normal/Body").GetComponent<SkinnedMeshRenderer>();
            if (star_enabled)HelperMethodsCM.updatePose(defaultSkin, star_skin, dict_star);
            if (isa_enabled)HelperMethodsCM.updatePose(defaultSkin, isa_skin, dict_isa);
            if (mnhr_enabled) HelperMethodsCM.updatePose(defaultSkin, mnhr_skin, dict_mnhr);
            
        }

        public static void updateWeaponModelsManual(ModData modData)
        {
            if (star_enabled) HelperMethodsCM.updateWeaponModelsManual(modData, "Root_STAR");
            if (isa_enabled) HelperMethodsCM.updateWeaponModelsManual(modData, "Root_isa");
            if (mnhr_enabled) HelperMethodsCM.updateWeaponModelsManual(modData, "Root_MNHR");
        }

        public static void updateWeaponModelsDynamic()
        {
            if (star_enabled) HelperMethodsCM.updateWeaponModelsDynamic("Root_STAR");
            if (isa_enabled) HelperMethodsCM.updateWeaponModelsDynamic("Root_isa");
            if (mnhr_enabled) HelperMethodsCM.updateWeaponModelsDynamic("Root_MNHR");
        }

        public static void insertModels(ModData data)
        {
            activeModels = new List<GameObject>();
            HelperMethodsCM.setChildActive("__Prerequisites__/Character Origin/Character Root/Ellie_Default/", "Normal", true);
            GameObject basicCopy = HelperMethodsCM.copyObjectDDOL("__Prerequisites__/Character Origin/Character Root/Ellie_Default/Normal/Body", "basicCopy", false);
            foreach (GameObject model in models)
            {
                // Cleanup
                GameObject existing = GameObject.Find($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/{model.name}");
                if (existing != null) GameObject.Destroy(existing);

                if(model.name == "MNHR_Normal_Anim")
                {
                    mnhr_skin = HelperMethodsCM.insertAlternative(model, "MNHR_Normal_Anim", "MNHR", "MNHRBody_001", "MNHR_Normal", dict_mnhr);
                    activeModels.Add(GameObject.Find("__Prerequisites__/Character Origin/Character Root/Ellie_Default/MNHR_Normal_Anim/"));
                    mnhr_root = GameObject.Find("__Prerequisites__/Character Origin/Character Root/Ellie_Default/metarig/Root_MNHR");
                    continue;
                }
                else if (model.name == "isa_re_metarig_IK Variant")
                {
                    isa_skin = HelperMethodsCM.insertAlternative(model, "isa_re_metarig_IK Variant", "isa", "Body", "metarig", dict_isa);
                    activeModels.Add(GameObject.Find("__Prerequisites__/Character Origin/Character Root/Ellie_Default/isa_re_metarig_IK Variant/"));
                    isa_root = GameObject.Find("__Prerequisites__/Character Origin/Character Root/Ellie_Default/metarig/Root_isa");
                    continue;
                }
                else if(model.name == "STAR_Normal_Anim")
                {
                    star_skin = HelperMethodsCM.insertAlternative(model, "STAR_Normal_Anim", "STAR", "STARBody_001", "STAR_Normal", dict_star);
                    activeModels.Add(GameObject.Find("__Prerequisites__/Character Origin/Character Root/Ellie_Default/STAR_Normal_Anim/"));
                    star_root = GameObject.Find("__Prerequisites__/Character Origin/Character Root/Ellie_Default/metarig/Root_STAR");
                    continue;
                }

                ModelData currModel = data.FindModelDataByName(model.name);
                MelonLogger.Msg(model.name);
                GameObject modelCopy = new GameObject();
                modelCopy.name = model.name;
                HelperMethodsCM.setParent("__Prerequisites__/Character Origin/Character Root/Ellie_Default", modelCopy);
                foreach (string part in currModel.modelParts)
                {
                    MelonLogger.Msg($"Copy {model.name} | {part}");
                    GameObject bodyPart = GameObject.Instantiate(basicCopy);
                    bodyPart.name = part;
                    HelperMethodsCM.copyModelSMR(model.transform.Find(part).gameObject, bodyPart);
                    HelperMethodsCM.setParent(modelCopy, bodyPart);
                    bodyPart.SetActive(false);
                }
                activeModels.Add(modelCopy);
            }
            GameObject.Destroy(basicCopy);
        }
    }
}