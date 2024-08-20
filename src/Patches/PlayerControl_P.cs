using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace FPSFixes.Patches
{
    [HarmonyPatch(typeof(PlayerControl))]
    class PlayerControl_P
    {
        [HarmonyPatch(nameof(PlayerControl.DoActionHold)), HarmonyTranspiler]
        static IEnumerable<CodeInstruction> PlayerActionHold(IEnumerable<CodeInstruction> instructions)
        {
            var entity = AccessTools.Field(typeof(PlayerControl), nameof(PlayerControl.entity));
            return new CodeMatcher(instructions)
            .MatchForward(false,
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Ldfld, entity),
                new CodeMatch(OpCodes.Ldfld),
                new CodeMatch(OpCodes.Ldc_I4_S),
                new CodeMatch(OpCodes.Beq),
                new CodeMatch(OpCodes.Ldarg_0)
            ).Nopify(13)
            .InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, entity),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Utils), nameof(Utils.DigSound)))
            ).InstructionEnumeration();
        }
        [HarmonyPatch(nameof(PlayerControl.LateUpdate)), HarmonyTranspiler]
        static IEnumerable<CodeInstruction> ViFly(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
            .MatchForward(false,
                new CodeMatch(OpCodes.Ldc_R4, 1f),
                new CodeMatch(OpCodes.Add)
            ).SetOperandAndAdvance(-100f)
            .MatchForward(false,
                new CodeMatch(OpCodes.Ldc_R4, 0f))
            .SetAndAdvance(OpCodes.Ldarg_0, null)
            .Insert(
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Utils), nameof(Utils.ViFly)))
            ).InstructionEnumeration();
        }
    }
}
