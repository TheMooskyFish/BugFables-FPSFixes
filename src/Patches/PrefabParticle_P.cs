using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace FPSFixes.Patches
{
    [HarmonyPatch(typeof(PrefabParticle))]
    internal class PrefabParticle_P
    {
        [HarmonyPatch("LateUpdate"), HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> UpdatePatch(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
            .MatchForward(false,
                new CodeMatch(OpCodes.Ldc_R4, 1f)
            ).AddCustomDeltaTime(60f, 1, false, false)
            .MatchForward(false,
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(PrefabParticle), nameof(PrefabParticle.childspin))),
                new CodeMatch(OpCodes.Callvirt)
            ).AddCustomDeltaTime(60f, 1, true, false)
            .MatchForward(false,
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(PrefabParticle), nameof(PrefabParticle.speed)))
            ).AddCustomDeltaTime(60f, 1, false, false)
            .InstructionEnumeration();
        }
    }
}
