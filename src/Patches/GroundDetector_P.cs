using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace FPSFixes.Patches
{
    [HarmonyPatch(typeof(GroundDetector))]
    class GroundDetector_P
    {
        [HarmonyPatch(nameof(GroundDetector.OnTriggerStay)), HarmonyTranspiler]
        static IEnumerable<CodeInstruction> OnTriggerStayPatch(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
            .MatchForward(true,
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Ldarg_1),
                new CodeMatch(OpCodes.Callvirt)
            ).Advance(2).Insert(
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(GroundDetector), nameof(GroundDetector.parent))),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Utils), nameof(Utils.ChangeInterpolation)))
            ).InstructionEnumeration();
        }
        [HarmonyPatch(nameof(GroundDetector.OnTriggerExit)), HarmonyTranspiler]
        static IEnumerable<CodeInstruction> OnTriggerExitPatch(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
            .MatchForward(true,
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Ldnull),
                new CodeMatch(OpCodes.Stfld)
            ).Advance(1).Insert(
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(GroundDetector), nameof(GroundDetector.parent))),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Utils), nameof(Utils.ChangeInterpolation)))
            ).InstructionEnumeration();
        }
    }
}
