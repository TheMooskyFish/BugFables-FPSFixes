using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace FPSFixes.Patches
{
    [HarmonyPatch(typeof(MainManager))]
    class MainManager_P
    {
        [HarmonyPatch(nameof(MainManager.Transition), MethodType.Enumerator), HarmonyTranspiler]
        static IEnumerable<CodeInstruction> TransitionPatch(IEnumerable<CodeInstruction> instructions)
        {
            var CodeMatcher = new CodeMatcher(instructions);
            for (var i = 0; i < 2; i++)
            {
                CodeMatcher.MatchForward(true,
                    new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(MainManager), "GetSqrDistance", [typeof(Vector3), typeof(Vector3)]))
                ).AddCustomDeltaTime(60f, 1, false, false);
            }
            return CodeMatcher.InstructionEnumeration();
        }
        [HarmonyPatch(nameof(MainManager.DoClock)), HarmonyTranspiler]
        static IEnumerable<CodeInstruction> RemoveGCCollect(IEnumerable<CodeInstruction> instructions)
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
        [HarmonyPatch(nameof(MainManager.SetText), [typeof(string), typeof(int), typeof(float?), typeof(bool), typeof(bool), typeof(Vector3), typeof(Vector3), typeof(Vector2), typeof(Transform), typeof(NPCControl)])]
        [HarmonyPatch(MethodType.Enumerator), HarmonyTranspiler]
        static IEnumerable<CodeInstruction> DialogueSpeedPatch(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
            .MatchForward(false,
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name.StartsWith("<centra")),
                new CodeMatch(OpCodes.Brfalse)
            ).MatchForward(false,
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Ldfld),
                new CodeMatch(OpCodes.Newobj),
                new CodeMatch(OpCodes.Stfld)
            ).Advance(3).Insert(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Utils), nameof(Utils.DialogueSpeed))))
            .InstructionEnumeration();
        }
        [HarmonyPatch(nameof(MainManager.ApplySettings)), HarmonyPostfix]
        static void VSyncPatch()
        {
            QualitySettings.vSyncCount = MainManager.vsync;
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
            [HarmonyPatch(nameof(MainManager.ResetCamera), [typeof(bool)])]
            [HarmonyTranspiler]
            static IEnumerable<CodeInstruction> ResetCameraPatch(IEnumerable<CodeInstruction> instructions)
            {
                return new CodeMatcher(instructions)
                .MatchForward(false,
                    new CodeMatch(OpCodes.Ldc_R4, 1f)
                ).SetOperandAndAdvance(10f).InstructionEnumeration();
            }
            [HarmonyPatch(nameof(MainManager.RefreshCamera))]
            [HarmonyTranspiler] //adds deltatime to all camspeed
            static IEnumerable<CodeInstruction> Patch3(IEnumerable<CodeInstruction> instructions)
            {
                CorePlugin.Logger.LogInfo("Patching Camera");
                var CodeMatcher = new CodeMatcher(instructions);
                for (var i = 0; i < 7; i++)
                {
                    CodeMatcher.MatchForward(false,
                        new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(MainManager), nameof(MainManager.camspeed)))
                    ).AddCustomDeltaTime(50f, 1, false, true).Advance(3);
                };
                CodeMatcher.MatchBack(false,
                    new CodeMatch(OpCodes.Ldloc_0))
                .AddCustomDeltaTime(50f, 1, true, true)
                .MatchForward(true,
                    new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(MainManager), nameof(MainManager.camanglespeed))),
                    new CodeMatch(OpCodes.Stloc_2)
                ).Advance(1).Insert(
                    new CodeInstruction(OpCodes.Ldloc_2, null),
                    new CodeInstruction(OpCodes.Stloc_2, null)
                ).AddCustomDeltaTime(50f, 1, false, true);
                return CodeMatcher.InstructionEnumeration();
            }
        }
    }
}
