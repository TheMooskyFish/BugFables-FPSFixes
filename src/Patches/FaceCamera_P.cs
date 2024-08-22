using HarmonyLib;
using UnityEngine;

namespace FPSFixes.Patches
{
    [HarmonyPatch(typeof(FaceCamera))]
    class FaceCamera_P
    {
        [HarmonyPatch("Start"), HarmonyPostfix]
        static void Start(FaceCamera __instance)
        {
            if (__instance.gameObject.name == "player") // only for mite knight
            {
                __instance.gameObject.AddComponent<FaceCameraUpdate>().FaceCamera = __instance;
                __instance.enabled = false;
            }
        }
        class FaceCameraUpdate : MonoBehaviour
        {
            internal FaceCamera FaceCamera;
            public void Update()
            {
                FaceCamera.FixedUpdate();
            }
        }
    }
}
