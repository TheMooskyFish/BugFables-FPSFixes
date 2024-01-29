using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;
namespace FPSFixes.Patches
{
    class FixesClass
    {
        [HarmonyPatch(typeof(MainManager), "ApplySettings")]
        class VSyncPatch
        {
            static void Postfix()
            {
                QualitySettings.vSyncCount = MainManager.vsync;
            }
        }
        [HarmonyPatch(typeof(MainManager), "Start")]
        class GUICameraPatch
        {
            static void Postfix(Camera ___MainCamera)
            {
                var GUICamera = ___MainCamera.transform.GetChild(0).GetComponent<Camera>();
                GUICamera.orthographicSize = CorePlugin.GUICameraSize.Value;
            }
        }
        [HarmonyPatch(typeof(PauseMenu), "Update")]
        static class PauseMenuFPSPatch
        {
            [HarmonyTranspiler]
            static IEnumerable<CodeInstruction> Patch(IEnumerable<CodeInstruction> instructions)
            {
                return new CodeMatcher(instructions)
                .MatchForward(true,
                    new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(PauseMenu), "fps")),
                    new CodeMatch(OpCodes.Ldc_I4_2)
                )
                .Set(OpCodes.Ldc_I4_3, null)
                .MatchForward(false,
                    new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch(OpCodes.Ldc_I4_1),
                    new CodeMatch(OpCodes.Stfld, AccessTools.Field(typeof(PauseMenu), "fps"))
                )
                .Advance(1).Set(OpCodes.Ldc_I4_2, null)
                .InstructionEnumeration();
            }
        }
    }
}