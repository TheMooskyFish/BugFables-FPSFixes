using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using System;
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

        [HarmonyPatch(typeof(BattleControl))]
        internal static class BattleControlReversePatch
        {
            [HarmonyPatch(nameof(BattleControl.FixedUpdate)), HarmonyPrefix]
            private static bool RemoveFixedUpdate() => false;

            [HarmonyPatch(nameof(BattleControl.Update)), HarmonyPrefix]
            private static void AddBattleFixedUpdateToUpdate(BattleControl __instance) => BattleFixedUpdate(__instance);

            [HarmonyPatch(nameof(BattleControl.FixedUpdate)), HarmonyReversePatch]
            private static void BattleFixedUpdate(BattleControl __instance)
            {
                throw new NotImplementedException();

#pragma warning disable CS8321 // Local function is declared but never used
                IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
#pragma warning restore CS8321 // Local function is declared but never used
                {
                    CodeMatcher codeMatcher = new(instructions);
                    for (int i = 0; i < 3; i++) //rotation vines
                    {
                        codeMatcher.MatchForward(false, new CodeMatch(OpCodes.Ldc_R4, 0.15f))
                            .SetOperandAndAdvance((float)codeMatcher.Operand * 50)
                            .Insert(
                                new CodeInstruction(OpCodes.Call,
                                    AccessTools.PropertyGetter(typeof(Time), nameof(Time.deltaTime))),
                                new CodeInstruction(OpCodes.Mul));
                    }

                    codeMatcher.MatchForward(false, new CodeMatch(OpCodes.Ldc_R4, 0.2f)) //cursor
                        .SetOperandAndAdvance((float)codeMatcher.Operand * 50)
                        .Insert(
                            new CodeInstruction(OpCodes.Call,
                                AccessTools.PropertyGetter(typeof(Time), nameof(Time.deltaTime))),
                            new CodeInstruction(OpCodes.Mul));

                    return codeMatcher.InstructionEnumeration();
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
