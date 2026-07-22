using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace FPSFixes.Patches
{
    [HarmonyPatch(typeof(Wind))]
    internal class WindPatches
    {
        [HarmonyPatch(typeof(Wind))]
        internal static class WindReversePatch
        {
            [HarmonyPatch(nameof(Wind.FixedUpdate)), HarmonyPrefix]
            private static bool RemoveFixedUpdate() => false;

            [HarmonyPatch(nameof(Wind.LateUpdate)), HarmonyPostfix]
            private static void AddWindUpdateToLateUpdate(Wind __instance) => WindUpdate(__instance);

            [HarmonyPatch(nameof(Wind.FixedUpdate)), HarmonyReversePatch]
            private static void WindUpdate(Wind __instance)
            {
                throw new NotImplementedException();

#pragma warning disable CS8321 // Local function is declared but never used
                IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
#pragma warning restore CS8321 // Local function is declared but never used
                {
                    CodeMatcher codeMatcher = new(instructions);
                    CodeInstruction vector3Multiply = new(OpCodes.Call,
                        AccessTools.Method(typeof(Vector3), "op_Multiply", [typeof(Vector3), typeof(float)]));
                    
                    for (int i = 0; i < 2; i++)
                    {
                        codeMatcher.MatchForward(true,
                            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Wind), nameof(Wind.speed))),
                            new CodeMatch(OpCodes.Call),
                            new CodeMatch(OpCodes.Call))
                        .InsertAndAdvance(
                            new CodeInstruction(OpCodes.Ldc_R4, 50f),
                            vector3Multiply,
                            new CodeInstruction(OpCodes.Call,
                                AccessTools.PropertyGetter(typeof(Time), nameof(Time.deltaTime))),
                            vector3Multiply);
                    }

                    codeMatcher.MatchForward(true, 
                        new CodeMatch(OpCodes.Ldfld), 
                        new CodeMatch(OpCodes.Ldc_R4, 1f))
                    .SetOperandAndAdvance(50f)
                    .InsertAndAdvance(
                        new CodeInstruction(OpCodes.Call, 
                            AccessTools.PropertyGetter(typeof(Time), nameof(Time.deltaTime))), 
                        new CodeInstruction(OpCodes.Mul));
                    return codeMatcher.InstructionEnumeration();
                }
            }
        }
    }
}