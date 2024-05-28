using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;
namespace FPSFixes.Patches
{
    class MainManager_P
    {
        [HarmonyPatch(typeof(MainManager), nameof(MainManager.Transition), MethodType.Enumerator)]
        static class TransitionPatch
        {
            [HarmonyTranspiler]
            static IEnumerable<CodeInstruction> Patch(IEnumerable<CodeInstruction> instructions)
            {
                var codepatch = new CodeMatcher(instructions);
                for (var i = 0; i < 2; i++)
                {
                    codepatch.MatchForward(true,
                        new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(MainManager), "GetSqrDistance", [typeof(Vector3), typeof(Vector3)]))
                    ).Advance(1).Insert(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Utils), nameof(Utils.AddDeltaTime), [typeof(float)])));
                }
                return codepatch.InstructionEnumeration();
            }
        }
    }
}
