using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace FPSFixes.Patches
{
    class MainManager_P
    {
        [HarmonyPatch(typeof(MainManager), nameof(MainManager.Transition), MethodType.Enumerator)]
        static class TransitionPatch
        {
            [HarmonyTranspiler]
            static IEnumerable<CodeInstruction> Patch(IEnumerable<CodeInstruction> instructions)
            {
                var codepatch = new CodeMatcher(instructions);
                for (var i = 0; i < 2; i++)
                {
                    codepatch.MatchForward(true,
                        new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(MainManager), "GetSqrDistance", [typeof(Vector3), typeof(Vector3)]))
                    ).Advance(1)
                    .Insert(Utils.AddDeltaTimeFloat);
                }
                return codepatch.InstructionEnumeration();
            }
        }
        [HarmonyPatch(typeof(MainManager), nameof(MainManager.DoClock))]
        static class RemoveGCCollectPatch
        {
            [HarmonyTranspiler]
            static IEnumerable<CodeInstruction> Patch(IEnumerable<CodeInstruction> instructions)
            {
                return new CodeMatcher(instructions)
                .MatchForward(false,
                    new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(MainManager), nameof(MainManager.clocksec))),
                    new CodeMatch(OpCodes.Ldc_I4_5)
                ).MatchThenNopify(true,
                    new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(System.GC), nameof(System.GC.Collect)))
                ).InstructionEnumeration();
            }
        }
        [HarmonyPatch(typeof(MainManager), nameof(MainManager.ApplySettings))]
        static class VSyncPatch
        {
            static void Postfix()
            {
                QualitySettings.vSyncCount = MainManager.vsync;
            }
        }
        [HarmonyPatch(typeof(MainManager), nameof(MainManager.Start))]
        static class GUICameraPatch
        {
            static void Postfix(MainManager __instance)
            {
                var GUICamera = __instance.transform.GetChild(0).GetComponent<Camera>();
                GUICamera.orthographicSize = CorePlugin.GUICameraSize.Value;
            }
        }
        //Camera Patches
        [HarmonyPatch(typeof(MainManager))]
        static class CameraPatch
        {
            [HarmonyPatch(nameof(MainManager.FixedUpdate))]
            [HarmonyPrefix]
            static bool Patch1()
            {
                if (MainManager.basicload)
                {
                    MainManager.instance.LoopMusic();
                }
                return false;
            }
            [HarmonyPatch(nameof(MainManager.LateUpdate))]
            [HarmonyPostfix]
            static void Patch2()
            {
                if (MainManager.basicload)
                {
                    MainManager.instance.RefreshCamera();
                }
            }
            [HarmonyPatch(nameof(MainManager.RefreshCamera))]
            [HarmonyTranspiler] //camera patch adds deltatime to all camspeed
            static IEnumerable<CodeInstruction> Patch3(IEnumerable<CodeInstruction> instructions)
            {
                var CodeMatcher = new CodeMatcher(instructions);
                for (var i = 0; i < 7; i++)
                {
                    CodeMatcher.MatchForward(false,
                        new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(MainManager), nameof(MainManager.camspeed)))
                    ).Advance(1)
                    .Insert(Utils.AddDeltaTimeFloat);
                };
                CodeMatcher.MatchForward(true,
                    new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(MainManager), nameof(MainManager.camanglespeed))),
                    new CodeMatch(OpCodes.Stloc_2)
                ).Advance(1)
                .Insert(
                    new CodeInstruction(OpCodes.Ldloc_2, null),
                    Utils.AddDeltaTimeFloat,
                    new CodeInstruction(OpCodes.Stloc_2, null)
                );
                return CodeMatcher.InstructionEnumeration();
            }
        }
    }
}
