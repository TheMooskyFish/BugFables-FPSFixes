using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace FPSFixes
{
    [BepInPlugin("dev.mooskyfish.FPSFixes", "FPS Fixes", "0.3.3")]
    [BepInProcess("Bug Fables.exe")]
    public class CorePlugin : BaseUnityPlugin
    {
        public static new ManualLogSource Logger;
        public static string Version = MetadataHelper.GetMetadata(typeof(CorePlugin)).Version.ToString();
        internal static Harmony Harmony;
        public static ConfigEntry<float> GUICameraSize;
        public static ConfigEntry<bool> ToggleUtilsdeltaTime;
        public static ConfigEntry<bool> ToggleFixShakeSprite;
        public static ConfigEntry<bool> ToggleUpdateCheck;

        public void Awake()
        {
            Logger = base.Logger;
            SetUpConfig();
            if (ToggleUpdateCheck.Value)
            {
                StartCoroutine(new UpdateChecker().CheckUpdate());
            }
            Harmony = new Harmony("dev.mooskyfish.FPSFixes");
            Harmony.PatchAll();
        }

        private void SetUpConfig()
        {
            GUICameraSize = Config.Bind("Patches", "GUI Camera Size", 5f, "Changes GUI Camera Size (Requires quitting to main menu or restart)");
            ToggleUtilsdeltaTime = Config.Bind("Patches", "Toggle deltaTime Patches", true, "");
            ToggleFixShakeSprite = Config.Bind("Patches", "Toggle Fix Shake Sprite", true, "");
            ToggleUpdateCheck = Config.Bind("Update Checker", "Toggle Update Checker", true, "");
        }
    }
}
