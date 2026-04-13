using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace FPSFixes.Patches
{
    [HarmonyPatch(typeof(ScrewPlatform))]
    internal class ScrewPlatformPatches
    {
        [HarmonyPatch(nameof(ScrewPlatform.Update)), HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> RotaterPatch(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
                .MatchForward(false,
                    new CodeMatch(OpCodes.Ldloc_0),
                    new CodeMatch(OpCodes.Brfalse)
                ).InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldloc_1)
                ).AddCustomDeltaTime(60f, 0, false, false)
                .InsertAndAdvance(
                    new CodeInstruction(OpCodes.Stloc_1)
                ).InstructionEnumeration();
        }
    }
}
//NOTE: platforms are still very slow but at least won't take long time to start moving