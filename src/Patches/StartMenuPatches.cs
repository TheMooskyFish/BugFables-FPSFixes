using HarmonyLib;
using UnityEngine;

namespace FPSFixes.Patches
{
    [HarmonyPatch(typeof(StartMenu))]
    internal class StartMenuPatches
    {
        [HarmonyPatch(nameof(StartMenu.SetMenuText)), HarmonyPostfix]
        private static void StartMenuVersion(StartMenu __instance)
        {
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("dev.mooskyfish.modlist"))
                return;
            MainManager.instance.StartCoroutine(
                MainManager.SetText(
                    $"|size,0.45||halfline||color,4||font,0|FPS Fixes v{CorePlugin.Version}",
                    new Vector3(-8.75f, 5.9f, 10f),
                    __instance.menu1));
        }

        [HarmonyPatch(nameof(StartMenu.Start)), HarmonyPostfix]
        private static void StartPatch(StartMenu __instance)
        {
            __instance.transform.localEulerAngles = Vector3.zero;
            __instance.transform.localPosition = new Vector3(0f, -1f, 10f);
        }
    }
}