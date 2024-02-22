using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
namespace FPSFixes.Patches
{
    class ScrewPlatform_P
    {
        [HarmonyPatch(typeof(ScrewPlatform), "Update")]
        static class RotaterPatch
        {
            [HarmonyTranspiler]
            static IEnumerable<CodeInstruction> Patch(IEnumerable<CodeInstruction> instructions)
            {
                return new CodeMatcher(instructions)
                .MatchForward(true,
                    new CodeMatch(OpCodes.Sub),
                    new CodeMatch(OpCodes.Br),
                    new CodeMatch(OpCodes.Ldc_R4),
                    new CodeMatch(OpCodes.Stloc_1)
                ).Advance(1).InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Utils), nameof(Utils.AddDeltaTime), new[] { typeof(float) })),
                    new CodeInstruction(OpCodes.Stloc_1)
                ).InstructionEnumeration();
            }
        }
    }
}
//NOTE: platforms are still very slow but at least won't take long time to start moving
