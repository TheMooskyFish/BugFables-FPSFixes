using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace FPSFixes.Patches
{
    [HarmonyPatch(typeof(NPCControl))]
    class NPCControl_P
    {
        [HarmonyPatch("Update"), HarmonyTranspiler]
        static IEnumerable<CodeInstruction> SavePoint(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
            .MatchForward(true,
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(NPCControl), nameof(NPCControl.internaltransform))),
                new CodeMatch(OpCodes.Ldc_I4_0),
                new CodeMatch(OpCodes.Ldelem_Ref),
                new CodeMatch(OpCodes.Ldc_R4),
                new CodeMatch(OpCodes.Ldc_R4),
                new CodeMatch(OpCodes.Ldc_R4, 0.3f)
            )
            .AddCustomDeltaTime(60f, 1, false, false)
            .InstructionEnumeration();
        }
        [HarmonyPatch("Update"), HarmonyTranspiler]
        static IEnumerable<CodeInstruction> GeizerPatch(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
            .MatchForward(true,
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(NPCControl), nameof(NPCControl.internaltransform))),
                new CodeMatch(OpCodes.Ldc_I4_2),
                new CodeMatch(OpCodes.Ldelem_Ref),
                new CodeMatch(OpCodes.Ldc_R4),
                new CodeMatch(OpCodes.Ldc_R4),
                new CodeMatch(OpCodes.Ldc_R4, 5f)
            )
            .AddCustomDeltaTime(60f, 1, false, false)
            .MatchForward(false, new CodeMatch(OpCodes.Ldc_R4, -5f))
            .AddCustomDeltaTime(60f, 1, false, false)
            .InstructionEnumeration();
        }
    }
}
