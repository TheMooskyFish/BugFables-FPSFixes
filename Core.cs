using System.Collections;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace FPSFixes
{
    [BepInPlugin("dev.mooskyfish.FPSFixes", "FPS Fixes", "0.3.4.1")]
    [BepInProcess("Bug Fables.exe")]
    public class CorePlugin : BaseUnityPlugin
    {
        public new static ManualLogSource Logger;
        public static string Version = MetadataHelper.GetMetadata(typeof(CorePlugin)).Version.ToString();
        private Harmony _harmony;
        //public static ConfigEntry<bool> ToggleCameraPatches;
        //public static ConfigEntry<bool> ToggledeltaTime;
        private ConfigEntry<bool> _toggleUpdateCheck;

        public void Awake()
        {
            Logger = base.Logger;
            _harmony = new Harmony("dev.mooskyfish.FPSFixes");
            SetUpConfig();
            if (_toggleUpdateCheck.Value)
                StartCoroutine(UpdateChecker.CheckUpdate());
            _harmony.PatchAll();
        }
        private void SetUpConfig()
        {
            //ToggledeltaTime = Config.Bind("Patches", "Toggle deltaTime Patches", true, "");
            _toggleUpdateCheck = Config.Bind("Update Checker", "Toggle Update Checker", true, "");
        }
#if DEBUG
        private bool _mode;
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (MainManager.map?.chompy)
                    Utils.ChangeInterpolation(MainManager.map?.chompy, _mode);
                foreach (var plr in MainManager.instance.playerdata)
                    Utils.ChangeInterpolation(plr.entity, _mode);
                _mode = !_mode;
            }
        }
        [HarmonyPatch(typeof(MainManager), "SetVariables")]
        private class SetVariablesPatch
        {
            private static void Postfix()
            {
                if (!MainManager.instance.GetComponent<FPSText>())
                    MainManager.instance.gameObject.AddComponent<FPSText>();
            }
        }
    }
    public class FPSText : MonoBehaviour
    {
        private DynamicFont _fps;
        public void Awake()
        {
            if (_fps == null)
                _fps = DynamicFont.SetUp(true, 1, 2, 100, new Vector2(0.5f, 0.5f), MainManager.GUICamera.transform, new Vector3(-8.9f, 4.65f, 1f));
            _fps.name = "FPSText";
            StartCoroutine(UpdateFPS());
        }

        private IEnumerator UpdateFPS()
        {
            while (true)
            {
                _fps.text = $"FPS: {Mathf.Round(1f / Time.unscaledDeltaTime)}";
                yield return new WaitForSecondsRealtime(0.25f);
            }
        }
#endif
    }
}
