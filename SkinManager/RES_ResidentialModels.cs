using MelonLoader;
using ModOverlayGUI;
using RootMotion.FinalIK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static RootMotion.Demos.Turret;

namespace ModelManager
{
    public class RES_ResidentialModels
    {
        static List<GameObject> models = new List<GameObject>();
        static List<GameObject> activeModels = new List<GameObject>();

        static Dictionary<String, int> dict_klbr = new Dictionary<String, int>();
        static GameObject klbr_root;
        static SkinnedMeshRenderer klbr_skin;
        static bool klbr_enabled;
        public static List<GameObject> loadModels()
        {
            // ADLR
            HelperMethodsCM.setChildActive("Cutscenes/", "Isa Adler Meeting", true);
            HelperMethodsCM.setChildActive("Cutscenes/Isa Adler Meeting/", "RES_Isa_1", true);
            GameObject adlr = HelperMethodsCM.copyObjectDDOL("Cutscenes/Isa Adler Meeting/RES_Isa_1/CharSpace/adler_metarig_IK", "adler_metarig_IK", true);
            adlr.SetActive(false);
            models.Add(adlr);

            //ARAR
            HelperMethodsCM.setChildActive("Service Tunnel/", "Chunk", true);
            GameObject arar = HelperMethodsCM.copyObjectDDOL("Service Tunnel/Chunk/Please stop decompiling our game, it's very disrespectful./ARAR-S2318/ARAR_Normal", "ARAR_Normal", true);
            arar.SetActive(false);
            models.Add(arar);

            //KLBR
            HelperMethodsCM.setChildActive("Library/", "Chunk", true);
            GameObject klbr = HelperMethodsCM.copyObjectDDOL("Library/Chunk/KLBR-S2302/KLBR_Normal_Anim", "KLBR_Normal_Anim", true);
            klbr_skin = HelperMethodsCM.destroyAndSkin(klbr, "KLBR_Normal_Body");
            dict_klbr = HelperMethodsCM.dictList(klbr_skin);
            klbr.SetActive(false);
            models.Add(klbr);

            MelonLogger.Msg("Models from DET_DetentionModels loaded");
            return models;
        }

        public static void updateModels(ModData modData, float diffHeight)
        {
            foreach (GameObject model in activeModels)
            {
                ModelData modelData = modData.FindModelDataByName(model.name);
                if(modelData.modelName == "KLBR_Normal_Anim")
                {
                    klbr_enabled = modelData.active[modelData.bodyIdx];
                    HelperMethodsCM.weaponShowcaseActive("Root_KLBR", klbr_enabled);
                    if (klbr_enabled) HelperMethodsCM.weaponScaling("Root_KLBR", modData.weaponModelSize);
                    GameObject charHeight = GameObject.Find($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/metarig/{klbr_root.name}");
                    if (charHeight != null) charHeight.transform.localPosition = charHeight.transform.localPosition - new Vector3(0, 0, diffHeight);

                }
                for (int i = 0; i < modelData.modelParts.Length; i++)
                {
                    string part = modelData.modelParts[i];
                    if (part == "Amigasa")
                    {
                        HelperMethodsCM.setChildActive("__Prerequisites__/Character Origin/Character Root/Ellie_Default/metarig/Root/hips/spine/chest/neck/head/", part, modelData.active[i]);
                        continue;
                    }
                    GameObject gameObject = GameObject.Find($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/{model.name}/{part}");
                    if (gameObject != null)
                    {
                        HelperMethodsCM.setChildActive($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/{model.name}", part, modelData.active[i]);
                    }
                    else MelonLogger.Error($"updateModels failed at EllieDefault: __Prerequisites__/Character Origin/Character Root/Ellie_Default/{model.name}/{modelData.modelParts[i]}");
                }
            }
        }
        public static void updatePose(ModData data) {
            if(klbr_skin == null)return;
            if (data.FindModelDataByName("KLBR_Normal_Anim").active[4])
            {
                SkinnedMeshRenderer defaultSkin = GameObject.Find("__Prerequisites__/Character Origin/Character Root/Ellie_Default/Normal/Body").GetComponent<SkinnedMeshRenderer>();
                HelperMethodsCM.updatePose(defaultSkin, klbr_skin, dict_klbr);
            }

        }

        public static void updateWeaponModelsManual(ModData modData)
        {
            if (klbr_enabled) HelperMethodsCM.updateWeaponModelsManual(modData, "Root_KLBR");
        }

        public static void updateWeaponModelsDynamic()
        {
            if (klbr_enabled) HelperMethodsCM.updateWeaponModelsDynamic("Root_KLBR");
        }

        public static void insertModels(ModData data)
        {
            activeModels = new List<GameObject>();
            HelperMethodsCM.setChildActive("__Prerequisites__/Character Origin/Character Root/Ellie_Default/", "Normal", true);
            GameObject basicCopy = HelperMethodsCM.copyObjectDDOL("__Prerequisites__/Character Origin/Character Root/Ellie_Default/Normal/Body", "basicCopy", false);
            foreach (GameObject model in models)
            {
                //Clean Up
                GameObject existing = GameObject.Find($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/{model.name}");
                if(existing!=null)GameObject.Destroy(existing);

                ModelData currModel = data.FindModelDataByName(model.name);
                MelonLogger.Msg(model.name);
                //KLBR CASE 
                if (model.name == "KLBR_Normal_Anim")
                {
                    klbr_skin = HelperMethodsCM.insertAlternative(model, "KLBR_Normal_Anim", "KLBR", "KLBR_Normal_Body", "KLBR_Normal", dict_klbr);
                    activeModels.Add(GameObject.Find("__Prerequisites__/Character Origin/Character Root/Ellie_Default/KLBR_Normal_Anim/"));
                    klbr_root = GameObject.Find("__Prerequisites__/Character Origin/Character Root/Ellie_Default/metarig/Root_KLBR");
                    continue;
                }
                GameObject modelCopy = new GameObject();
                modelCopy.name = model.name;
                HelperMethodsCM.setParent("__Prerequisites__/Character Origin/Character Root/Ellie_Default", modelCopy);
                foreach (string part in currModel.modelParts)
                {
                    MelonLogger.Msg($"Copy {model.name} | {part}");
                    if (part == "Amigasa")
                    {
                        model.SetActive(true);
                        GameObject amigasa = GameObject.Instantiate(GameObject.Find("ARAR_Normal/ARAR_Normal/Root/hips/spine/chest/neck/head/Amigasa"));
                        model.SetActive(false);
                        HelperMethodsCM.setParent("__Prerequisites__/Character Origin/Character Root/Ellie_Default/metarig/Root/hips/spine/chest/neck/head/", amigasa);
                        amigasa.transform.localPosition = new Vector3(-1.4f, 0, 0);
                        amigasa.transform.localEulerAngles = new Vector3(0, 90, 270);
                        amigasa.name = "Amigasa";
                        amigasa.SetActive(false);
                        continue;
                    }
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