using MelonLoader;
using Rotorz.Games;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SkinManager
{
    public class DET_DetentionModels
    {
        static List<GameObject> models = new List<GameObject>();
        public static List<GameObject> loadModels()
        {
            string[] rigs = { "isa_metarig_IK", "ariane_metarig_IK_ghost", "ariane_metarig_IK_uniform", "alina_metarig_IK", "isa_re_metarig_IK Variant" };
            // Set Path active
            if ((HelperMethodsCM.setChildActive("Cutscenes/Alina Vision Smiling 1/", "DET_Alina_1") && HelperMethodsCM.setChildActive("Cutscenes/Alina Vision Smiling 1/DET_Alina_1", "CharSpace")))
            {
                foreach (string r in rigs)
                { 
                    HelperMethodsCM.setChildActive("Cutscenes/Alina Vision Smiling 1/DET_Alina_1/CharSpace/", r);
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
                    models.Add(metarig);
                }
                HelperMethodsCM.setChildActive("Rationing/", "Chunk");
                GameObject stcr = HelperMethodsCM.copyObjectDDOL("Rationing/Chunk/STCR-S2307/STCR_Normal_Anim", "STCR_Normal_Anim", true);
                HelperMethodsCM.setChildActive(stcr, "STCR_Baton");
                models.Add(stcr);
            }
            else
            {
                MelonLogger.Error("failed loadModels: Path isn't active");
            }
            MelonLogger.Msg("Models from DET_DetentionModels loaded");
            return models;
        }
    }
}
