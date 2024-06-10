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
    public class RES_ResidentialModels
    {
        static List<GameObject> models = new List<GameObject>();
        static List<GameObject> activeModels = new List<GameObject>();
        public static List<GameObject> loadModels()
        {
            HelperMethodsCM.setChildActive("Cutscenes/", "Isa Adler Meeting", true);
            HelperMethodsCM.setChildActive("Cutscenes/Isa Adler Meeting/", "RES_Isa_1", true);
            GameObject adlr = HelperMethodsCM.copyObjectDDOL("Cutscenes/Isa Adler Meeting/RES_Isa_1/CharSpace/adler_metarig_IK", "adler_metarig_IK", true);
            adlr.SetActive(false);
            models.Add(adlr);

            HelperMethodsCM.setChildActive("Service Tunnel/", "Chunk", true);
            GameObject arar = HelperMethodsCM.copyObjectDDOL("Service Tunnel/Chunk/Please stop decompiling our game, it's very disrespectful./ARAR-S2318/ARAR_Normal", "ARAR_Normal", true);
            arar.SetActive(false);
            models.Add(arar);

            HelperMethodsCM.setChildActive("Library/", "Chunk", true);
            GameObject klbr = HelperMethodsCM.copyObjectDDOL("Library/Chunk/KLBR-S2302/KLBR_Normal_Anim", "KLBR_Normal_Anim", true);
            klbr.SetActive(false);
            models.Add(klbr);

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

        public static void insertModels(ModData data)
        {
            activeModels = new List<GameObject>();
            HelperMethodsCM.setChildActive("__Prerequisites__/Character Origin/Character Root/Ellie_Default/", "Normal", true);
            GameObject basicCopy = HelperMethodsCM.copyObjectDDOL("__Prerequisites__/Character Origin/Character Root/Ellie_Default/Normal/Body", "basicCopy", false);
            foreach (GameObject model in models)
            {
                ModelData currModel = data.FindModelDataByName(model.name);
                MelonLogger.Msg(model.name);
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