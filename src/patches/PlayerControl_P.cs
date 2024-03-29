using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
namespace FPSFixes.Patches
{
    class PlayerControl_P
    {
        [HarmonyPatch(typeof(PlayerControl))]
        [HarmonyPatch("DoActionHold")]
        static class DoActionPatch
        {
            [HarmonyTranspiler]
            static IEnumerable<CodeInstruction> Patch(IEnumerable<CodeInstruction> instructions)
            {
                var entity = AccessTools.Field(typeof(PlayerControl), nameof(PlayerControl.entity));
                var codepatch = new CodeMatcher(instructions)
                .MatchForward(false,
                    new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch(OpCodes.Ldfld, entity),
                    new CodeMatch(OpCodes.Ldfld),
                    new CodeMatch(OpCodes.Ldc_I4_S),
                    new CodeMatch(OpCodes.Beq),
                    new CodeMatch(OpCodes.Ldarg_0)
                ).RemoveInstructions(14)
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, entity))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Utils), nameof(Utils.DigSound))));
                return codepatch.InstructionEnumeration();
            }

        }
    }
}