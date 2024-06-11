using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
namespace FPSFixes.Patches
{
    class NPCControl_P
    {
        [HarmonyPatch(typeof(NPCControl), "Update")]
        static class SavePointSpinPatch
        {
            [HarmonyTranspiler]
            static IEnumerable<CodeInstruction> Patch(IEnumerable<CodeInstruction> instructions)
            {
                return new CodeMatcher(instructions)
                .MatchForward(true,
                    new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(NPCControl), nameof(NPCControl.internaltransform))),
                    new CodeMatch(OpCodes.Ldc_I4_0),
                    new CodeMatch(OpCodes.Ldelem_Ref),
                    new CodeMatch(OpCodes.Ldc_R4),
                    new CodeMatch(OpCodes.Ldc_R4),
                    new CodeMatch(OpCodes.Ldc_R4, (float)0.3)
                ).Advance(1)
                .InsertAndAdvance(Utils.AddDeltaTimeFloat)
                .InstructionEnumeration();
            }
        }
    }
}
