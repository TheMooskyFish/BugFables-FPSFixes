using HarmonyLib;
using UnityEngine;

namespace FPSFixes.Patches
{
    [HarmonyPatch(typeof(StartMenu), "SetMenuText")]
    static class StartMenuVersion
    {
        static void Postfix(StartMenu __instance)
        {
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("dev.mooskyfish.modlist"))
                return;
            var y = -3.2473f;
            var menu1 = (Transform)AccessTools.Field(typeof(StartMenu), "menu1").GetValue(__instance);
            MainManager.instance.StartCoroutine(MainManager.SetText($"|size,0.45||halfline||color,4||font,0|FPS Fixes v{CorePlugin.Version}", new Vector3(-8.75f, y, 10f), menu1));
        }
    }
    [HarmonyPatch(typeof(StartMenu), "Start")]
    static class StartPatch // fixes bug caused by camera patches
    {
        static void Postfix(StartMenu __instance)
        {
            __instance.transform.localEulerAngles = Vector3.zero;
            __instance.transform.localPosition = new Vector3(0f, -1f, 10f);
        }
    }
}
