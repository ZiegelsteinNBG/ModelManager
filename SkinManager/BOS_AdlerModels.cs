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

namespace ModelManager
{
    public class BOS_AdlerModels
    {
        static GameObject models;
        static GameObject activeModels;

        static Dictionary<String, int> dict_fklr = new Dictionary<String, int>();
        static GameObject fklr_root;
        static SkinnedMeshRenderer fklr_skin;

        public static List<GameObject> loadModels()
        {
            // FKLR
            HelperMethodsCM.setChildActive("Arena/", "Chunk", true);
            GameObject fklr = HelperMethodsCM.copyObjectDDOL("Arena/Enemy Manager/Falke_Manager/FKLR/FKLR/", "FKLR_Anim", true);
            GameObject.Destroy(fklr.transform.Find("Camera_Origin").gameObject);
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

        public static void updatePose(ModData data)
        {
 
            SkinnedMeshRenderer defaultSkin = GameObject.Find("__Prerequisites__/Character Origin/Character Root/Ellie_Default/Normal/Body").GetComponent<SkinnedMeshRenderer>();
            HelperMethodsCM.updatePose(defaultSkin, fklr_skin, dict_fklr);

            //TODO Height Weapons etc
        }

        public static void insertModels()
        {
            models.transform.Find("Normal").gameObject.SetActive(true);
            fklr_skin = HelperMethodsCM.insertAlternative(models, "FKLR_Anim", "FKLR", "Normal/FKLR_Normal", "FKLR", dict_fklr);
            models.transform.Find("Normal").gameObject.SetActive(false);
            activeModels = (GameObject.Find("__Prerequisites__/Character Origin/Character Root/Ellie_Default/FKLR/"));
        }
    }
}
