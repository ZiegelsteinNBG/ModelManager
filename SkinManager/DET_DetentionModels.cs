using MelonLoader;
using ModOverlayGUI;
using Rotorz.Games;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace ModelManager
{
    
    public class DET_DetentionModels
    {
        static List<GameObject> models = new List<GameObject>();
        static List<GameObject> activeModels = new List<GameObject>();
        public static List<GameObject> loadModels()
        {
            string[] rigs = { "isa_metarig_IK", "ariane_metarig_IK_ghost", "ariane_metarig_IK_uniform", "alina_metarig_IK" }; //, "isa_re_metarig_IK Variant"
            // Set Path active
            if ((HelperMethodsCM.setChildActive("Cutscenes/Alina Vision Smiling 1/", "DET_Alina_1", true) && HelperMethodsCM.setChildActive("Cutscenes/Alina Vision Smiling 1/DET_Alina_1", "CharSpace", true)))
            {
                foreach (string r in rigs)
                {
                    HelperMethodsCM.setChildActive("Cutscenes/Alina Vision Smiling 1/DET_Alina_1/CharSpace/", r, true);
                    GameObject metarig = HelperMethodsCM.copyObjectDDOL($"Cutscenes/Alina Vision Smiling 1/DET_Alina_1/CharSpace/{r}/", r, true);
                    metarig.SetActive(true);
                    switch (r)
                    {
                        case "isa_metarig_IK":
                            HelperMethodsCM.setChildActive(metarig, "Poncho");
                            break;
                        case "alina_metarig_IK":
                            HelperMethodsCM.setChildActive(metarig, "AlinaBodyDetailsArmor");
                            break;
                        case "isa_re_metarig_IK Variant":
                            HelperMethodsCM.setChildActive(metarig, "IsaBodyDetailsArmor");
                            HelperMethodsCM.setChildActive(metarig, "Tasche");
                            break;
                    }
                    metarig.SetActive(false);
                    models.Add(metarig);
                }
                HelperMethodsCM.setChildActive("Rationing/", "Chunk", true);
                GameObject stcr = HelperMethodsCM.copyObjectDDOL("Rationing/Chunk/STCR-S2307/STCR_Normal_Anim", "STCR_Normal_Anim", true);
                HelperMethodsCM.setChildActive(stcr, "STCR_Baton");
                stcr.SetActive(false);
                models.Add(stcr);
            }
            else
            {
                MelonLogger.Error("failed loadModels: Path isn't active");
            }
            MelonLogger.Msg("Models from DET_DetentionModels loaded");
            return models;
        }
        public static void updateModels(ModData modData)
        {
            foreach (GameObject model in activeModels)
            {
                ModelData modelData = modData.FindModelDataByName(model.name);
                for (int i = 0; i < modelData.modelParts.Length; i++)
                {
                    string part = modelData.modelParts[i];
                    GameObject gameObject = GameObject.Find($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/{model.name}/{part}");
                    if (gameObject != null)
                    {
                        HelperMethodsCM.setChildActive($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/{model.name}", part, modelData.active[i]);
                    }
                    else MelonLogger.Error($"updateModels failed at EllieDefault: __Prerequisites__/Character Origin/Character Root/Ellie_Default/{model.name}/{modelData.modelParts[i]}");
                }
            }
        }

        public static void insertModels(ModData data)
        {
            activeModels = new List<GameObject>();
            HelperMethodsCM.setChildActive("__Prerequisites__/Character Origin/Character Root/Ellie_Default/", "Normal", true);
            GameObject basicCopy = HelperMethodsCM.copyObjectDDOL("__Prerequisites__/Character Origin/Character Root/Ellie_Default/Normal/Body", "basicCopy", false);
            foreach (GameObject model in models)
            {
                    GameObject existing = GameObject.Find($"__Prerequisites__/Character Origin/Character Root/Ellie_Default/{model.name}");
                    if (existing != null) GameObject.Destroy(existing);
                    ModelData currModel = data.FindModelDataByName(model.name);
                    MelonLogger.Msg(model.name);
                    GameObject modelCopy = new GameObject();
                    modelCopy.name = model.name;
                    HelperMethodsCM.setParent("__Prerequisites__/Character Origin/Character Root/Ellie_Default", modelCopy);
                    foreach(string part in currModel.modelParts)
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