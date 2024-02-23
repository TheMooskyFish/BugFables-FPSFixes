using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace FPSFixes
{
    [BepInPlugin("dev.mooskyfish.FPSFixes", "FPS Fixes", "0.3.0")]
    [BepInProcess("Bug Fables.exe")]
    public class CorePlugin : BaseUnityPlugin
    {
        public static new ManualLogSource Logger;
        public static readonly string Version = MetadataHelper.GetMetadata(typeof(CorePlugin)).Version.ToString() + "-DEV";
        public static ConfigEntry<float> GUICameraSize;
        public static ConfigEntry<bool> ToggleUtilsdeltaTime;
        public static ConfigEntry<bool> ToggleFixShakeSprite;

        public void Awake()
        {
            Logger = base.Logger;
            SetUpConfig();
            var harmony = new Harmony("dev.mooskyfish.FPSFixes");
            harmony.PatchAll();
        }

        private void SetUpConfig()
        {
            ToggleUtilsdeltaTime = Config.Bind("Patches", "Toggle deltaTime Patches", true, "");
            ToggleFixShakeSprite = Config.Bind("Patches", "Toggle Fix Shake Sprite", true, "");
            GUICameraSize = Config.Bind("Patches", "GUI Camera Size", 5f, "Changes GUI Camera Size (Requires quitting to main menu or restart)");
        }
    }
}
