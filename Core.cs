using System.Collections;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace FPSFixes
{
    [BepInPlugin("dev.mooskyfish.FPSFixes", "FPS Fixes", "0.3.3.1")]
    [BepInProcess("Bug Fables.exe")]
    public class CorePlugin : BaseUnityPlugin
    {
        public static new ManualLogSource Logger;
        public static string Version = MetadataHelper.GetMetadata(typeof(CorePlugin)).Version.ToString();
        internal static Harmony Harmony;
        public static ConfigEntry<bool> ToggleCameraPatches;
        public static ConfigEntry<bool> ToggledeltaTime;
        public static ConfigEntry<bool> ToggleUpdateCheck;

        public void Awake()
        {
            Logger = base.Logger;
            Harmony = new Harmony("dev.mooskyfish.FPSFixes");
            SetUpConfig();
            if (ToggleUpdateCheck.Value)
                StartCoroutine(new UpdateChecker().CheckUpdate());
            Harmony.PatchAll();
        }
        private void SetUpConfig()
        {
            ToggledeltaTime = Config.Bind("Patches", "Toggle deltaTime Patches", true, "");
            ToggleUpdateCheck = Config.Bind("Update Checker", "Toggle Update Checker", true, "");
        }

        //[HarmonyPatch(typeof(MainManager), "SetVariables")]
        //public class SetVariablesPatch
        //{
        //    static void Postfix()
        //    {
        //        if (!MainManager.instance.GetComponent<FPSText>())
        //            MainManager.instance.gameObject.AddComponent<FPSText>();
        //    }
        //}
    }
    //public class FPSText : MonoBehaviour
    //{
    //    public static DynamicFont FPS;
    //    public void Awake()
    //    {
    //        if (FPS == null)
    //            FPS = DynamicFont.SetUp(true, 1, 2, 100, new Vector2(0.5f, 0.5f), MainManager.GUICamera.transform, new Vector3(-8.9f, 4.65f, 1f));
    //        FPS.name = "FPSText";
    //        StartCoroutine(UpdateFPS());
    //    }
    //    public IEnumerator UpdateFPS()
    //    {
    //        while (true)
    //        {
    //            FPS.text = $"FPS: {Mathf.Round(1f / Time.unscaledDeltaTime)}";
    //            yield return new WaitForSecondsRealtime(0.25f);
    //        }
    //
    //    }
    //}
}
