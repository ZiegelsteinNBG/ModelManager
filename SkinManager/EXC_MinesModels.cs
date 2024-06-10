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
    public class EXC_MinesModels
    {
        static List<GameObject> models = new List<GameObject>();
        static List<GameObject> activeModels = new List<GameObject>();
        public static List<GameObject> loadModels()
        {
            HelperMethodsCM.setChildActive("Star Room/", "Chunk", true);
            GameObject star = HelperMethodsCM.copyObjectDDOL("Star Room/Chunk/STAR-S23??/STAR_Normal_Anim", "STAR_Normal_Anim", true);
            star.SetActive(false);
            models.Add(star);

            GameObject eulr = HelperMethodsCM.copyObjectDDOL("Star Room/Chunk/STAR-S23??/Deko/EULR_Normal_Anim", "EULR_Normal_Anim", true);
            eulr.SetActive(false);
            models.Add(eulr);

            //HelperMethodsCM.setChildActive("Cutscenes/Isa vs Adler/", "EXC_Fight_3 Wideshot");
            //GameObject isa_re = HelperMethodsCM.copyObjectDDOL("Cutscenes/Isa vs Adler/EXC_Fight_3 Wideshot/CharacterSpace/isa_cutscene/isa_re_metarig_IK Variant", "isa_re_metarig_IK Variant", true);
            //isa_re.SetActive(false);
            //models.Add(isa_re);

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