using MelonLoader;
using ModOverlayGUI;
using RootMotion.FinalIK;
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

        static Dictionary<String, int> dict_star = new Dictionary<String, int>();
        static GameObject star_root;
        static SkinnedMeshRenderer star_skin;

        static Dictionary<String, int> dict_isa = new Dictionary<String, int>();
        static GameObject isa_root;
        static SkinnedMeshRenderer isa_skin;

        static Dictionary<String, int> dict_mnhr = new Dictionary<String, int>();
        static GameObject mnhr_root;
        static SkinnedMeshRenderer mnhr_skin;
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
            models.Add(mnhr);   

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
        public static void updatePose(ModData data)
        {
            if (mnhr_skin == null) return;
            if (data.FindModelDataByName("STAR_Normal_Anim").active[3])
            {
                SkinnedMeshRenderer defaultSkin = GameObject.Find("__Prerequisites__/Character Origin/Character Root/Ellie_Default/Normal/Body").GetComponent<SkinnedMeshRenderer>();
                HelperMethodsCM.updatePose(defaultSkin, star_skin, dict_star);
            }

            if (data.FindModelDataByName("isa_re_metarig_IK Variant").active[0])
            {
                SkinnedMeshRenderer defaultSkin = GameObject.Find("__Prerequisites__/Character Origin/Character Root/Ellie_Default/Normal/Body").GetComponent<SkinnedMeshRenderer>();
                HelperMethodsCM.updatePose(defaultSkin, isa_skin, dict_isa);
            }

            if (data.FindModelDataByName("MNHR_Normal_Anim").active[0])
            {
                SkinnedMeshRenderer defaultSkin = GameObject.Find("__Prerequisites__/Character Origin/Character Root/Ellie_Default/Normal/Body").GetComponent<SkinnedMeshRenderer>();
                HelperMethodsCM.updatePose(defaultSkin, mnhr_skin, dict_mnhr);
            }


            //TODO Height Weapons etc
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
                    continue;
                }
                else if (model.name == "isa_re_metarig_IK Variant")
                {
                    isa_skin = HelperMethodsCM.insertAlternative(model, "isa_re_metarig_IK Variant", "isa", "Body", "metarig", dict_isa);
                    activeModels.Add(GameObject.Find("__Prerequisites__/Character Origin/Character Root/Ellie_Default/isa_re_metarig_IK Variant/"));
                    continue;
                }
                else if(model.name == "STAR_Normal_Anim")
                {
                    star_skin = HelperMethodsCM.insertAlternative(model, "STAR_Normal_Anim", "STAR", "STARBody_001", "STAR_Normal", dict_star);
                    activeModels.Add(GameObject.Find("__Prerequisites__/Character Origin/Character Root/Ellie_Default/STAR_Normal_Anim/"));
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