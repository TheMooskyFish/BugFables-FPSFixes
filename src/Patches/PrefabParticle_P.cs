using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace FPSFixes.Patches
{
    [HarmonyPatch(typeof(PrefabParticle))]
    class PrefabParticle_P
    {
        [HarmonyPatch("LateUpdate"), HarmonyTranspiler]
        static IEnumerable<CodeInstruction> UpdatePatch(IEnumerable<CodeInstruction> instructions)
        {
            var CodeMatcher = new CodeMatcher(instructions);
            CodeMatcher.MatchForward(false,
                new CodeMatch(OpCodes.Ldc_R4, 1f)
            ).AddCustomDeltaTime(60f, 1, false, false)
            .MatchForward(false,
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(PrefabParticle), nameof(PrefabParticle.childspin))),
                new CodeMatch(OpCodes.Callvirt)
            ).AddCustomDeltaTime(60f, 1, true, false)
            .MatchForward(false,
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(PrefabParticle), nameof(PrefabParticle.speed)))
            ).AddCustomDeltaTime(60f, 1, false, false);
            return CodeMatcher.InstructionEnumeration();
        }
    }
}
