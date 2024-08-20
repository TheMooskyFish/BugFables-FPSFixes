using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace FPSFixes.Patches
{
    [HarmonyPatch(typeof(BattleControl))]
    class BattleControl_P
    {
        [HarmonyPatch(nameof(BattleControl.CounterAnimation), MethodType.Enumerator), HarmonyTranspiler]
        static IEnumerable<CodeInstruction> CounterAnimation_Patch(IEnumerable<CodeInstruction> instructions)
        {
            var CodeMatcher = new CodeMatcher(instructions);
            CodeMatcher.MatchForward(false, //counter type 0 - adding deltatime to rotate
                new CodeMatch(OpCodes.Ldc_I4, 360)
            );
            for (var i = 0; i < 4; i++)
            {
                CodeMatcher.MatchForward(false,
                    new CodeMatch(OpCodes.Ldc_R4),
                    new CodeMatch(OpCodes.Newobj),
                    new CodeMatch(OpCodes.Callvirt)
                ).AddCustomDeltaTime(60f, 1, false, false);
                CodeMatcher.Advance(3);
            } // end of counter type 0
            CodeMatcher.MatchForward(false, //counter type 1 and 2 - adding deltatime to position and last rotate
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Ldnull)
            );
            for (var i = 0; i < 3; i++)
            {
                CodeMatcher.MatchForward(false,
                    new CodeMatch(OpCodes.Ldc_R4),
                    new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Vector3), nameof(Vector3.Lerp)))
                ).AddCustomDeltaTime(60f, 1, false, false);
            }
            CodeMatcher.MatchForward(true,
                new CodeMatch(OpCodes.Ldc_R4, 0f),
                new CodeMatch(OpCodes.Ldc_R4, 10f)
            ).AddCustomDeltaTime(60f, 1, false, false); // end of counter type 1 and 2
            return CodeMatcher.InstructionEnumeration();
        }
    }
}
