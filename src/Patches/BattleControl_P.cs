using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace FPSFixes.Patches
{
    [HarmonyPatch(typeof(BattleControl))]
    internal class BattleControl_P
    {
        [HarmonyPatch(nameof(BattleControl.CounterAnimation), MethodType.Enumerator), HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> CounterAnimation_Patch(IEnumerable<CodeInstruction> instructions)
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
            Utils.UpdateVines(__instance);
        }
    }
}
