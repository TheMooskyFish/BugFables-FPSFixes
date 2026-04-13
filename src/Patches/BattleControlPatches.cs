using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace FPSFixes.Patches
{
    [HarmonyPatch(typeof(BattleControl))]
    internal class BattleControlPatches
    {
        [HarmonyPatch(nameof(BattleControl.CounterAnimation), MethodType.Enumerator), HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> CounterAnimationPatch(IEnumerable<CodeInstruction> instructions)
        {
            var codeMatcher = new CodeMatcher(instructions);
            codeMatcher.MatchForward(false, //counter type 0 - adding deltatime to rotate
                new CodeMatch(OpCodes.Ldc_I4, 360)
            );
            for (var i = 0; i < 4; i++)
            {
                codeMatcher.MatchForward(false,
                    new CodeMatch(OpCodes.Ldc_R4),
                    new CodeMatch(OpCodes.Newobj),
                    new CodeMatch(OpCodes.Callvirt)
                ).AddCustomDeltaTime(60f, 1, false, false);
                codeMatcher.Advance(3);
            } // end of counter type 0
            codeMatcher.MatchForward(false, //counter type 1 and 2 - adding deltatime to position and last rotate
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Ldnull)
            );
            for (var i = 0; i < 3; i++)
            {
                codeMatcher.MatchForward(false,
                    new CodeMatch(OpCodes.Ldc_R4),
                    new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Vector3), nameof(Vector3.Lerp)))
                ).AddCustomDeltaTime(60f, 1, false, false);
            }
            codeMatcher.MatchForward(true,
                new CodeMatch(OpCodes.Ldc_R4, 0f),
                new CodeMatch(OpCodes.Ldc_R4, 10f)
            ).AddCustomDeltaTime(60f, 1, false, false); // end of counter type 1 and 2
            return codeMatcher.InstructionEnumeration();
        }
        [HarmonyPatch(nameof(BattleControl.UpdateRotation)), HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> UpdateRotationPatch(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions).MatchForward(false,
                new CodeMatch(OpCodes.Ldc_R4, 0.2f)
            ).AddCustomDeltaTime(50f, 1, false, false).InstructionEnumeration();
        }
        [HarmonyPatch(nameof(BattleControl.FixedUpdate)), HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> RemoveVinesRotationScale(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions).MatchForward(false,
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(BattleControl), nameof(BattleControl.currentaction)))
            ).MatchThenNopify(false,
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(BattleControl), nameof(BattleControl.enemy)))
            ).InstructionEnumeration();
        }
        [HarmonyPatch(nameof(BattleControl.LateUpdate)), HarmonyPrefix]
        private static void LateUpdatePatch(BattleControl __instance)
        {
            if (MainManager.instance.inbattle && MainManager.pausemenu == null && __instance.choicevine != null)
            {
                Transform children = __instance.choicevine.GetChild(0);
                if (__instance.currentaction == BattleControl.Pick.BaseAction)
                {
                    __instance.UpdateRotation(__instance.option);
                    foreach (Transform child in children)
                        child.localScale = Vector3.Lerp(child.localScale, Vector3.one * 2.5f, 
                            Utils.AddDeltaTime(0.15f, 50f, false));
                }
                else
                {
                    __instance.UpdateRotation(__instance.lastoption);
                    foreach (Transform child in children)
                        if (__instance.lastoption == child.GetSiblingIndex())
                            child.localScale = Vector3.Lerp(
                                child.localScale, 
                                new Vector3(2.5f, __instance.currentaction == BattleControl.Pick.SelectPlayer ? 2f : 2.5f, 2.5f),
                                Utils.AddDeltaTime(0.15f, 50f, false));
                        else
                            child.localScale = Vector3.Lerp(child.localScale, new Vector3(2.5f, 1.5f, 2.5f), 
                                Utils.AddDeltaTime(0.15f, 50f, false));
                }
            }
        }
        [HarmonyPatch(nameof(BattleControl.DoCommand), MethodType.Enumerator), HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> RemoveTieFrameRateForBarFill(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions).MatchForward(false,
                    new CodeMatch(OpCodes.Ldsfld, AccessTools.Field(typeof(MainManager), nameof(MainManager.vsync))),
                    new CodeMatch(OpCodes.Brtrue))
                .SetOpcodeAndAdvance(OpCodes.Ldc_I4_1)
                .SetOpcodeAndAdvance(OpCodes.Brfalse_S).MatchForward(false,
                    new CodeMatch(Transpilers.EmitDelegate(MainManager.TieFramerate)))
                .Nopify(0).MatchForward(false,
                    new CodeMatch(OpCodes.Call,
                        AccessTools.PropertyGetter(typeof(Application), nameof(Application.targetFrameRate))))
                .MatchThenNopify(true, new CodeMatch(OpCodes.Mul))
                .InstructionEnumeration();
        }
    }
}
