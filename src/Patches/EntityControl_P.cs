using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;
namespace FPSFixes.Patches
{
    class EntityControl_P
    {
        [HarmonyPatch(typeof(EntityControl), "Start")]
        static class InterpolateEntity
        {
            static void Postfix(EntityControl __instance)
            {
                if (__instance.playerentity)
                {
                    __instance.rigid.interpolation = RigidbodyInterpolation.Extrapolate; //change to Interpolate after fixing vi's fly
                }
            }
        }
        [HarmonyPatch(typeof(EntityControl), "UpdateFlip")]
        static class UpdateFlipPatch
        {
            [HarmonyTranspiler]
            static IEnumerable<CodeInstruction> Patch(IEnumerable<CodeInstruction> instructions)
            {
                var spin = AccessTools.Field(typeof(EntityControl), nameof(EntityControl.spin));
                var sprite = AccessTools.Field(typeof(EntityControl), nameof(EntityControl.spritetransform));
                var codepatch = new CodeMatcher(instructions)
                .MatchForward(false,
                    new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch(OpCodes.Ldfld, sprite),
                    new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch(OpCodes.Ldfld, spin)
                )
                .Nopify(3)
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                .Set(OpCodes.Call, AccessTools.Method(typeof(Utils), nameof(Utils.EntitySpin)));
                return codepatch.InstructionEnumeration();
            }
        }
        [HarmonyPatch(typeof(EntityControl), "GetFlipSpeed")]
        static class GetFlipPatch
        {
            [HarmonyTranspiler]
            static IEnumerable<CodeInstruction> Patch(IEnumerable<CodeInstruction> instructions)
            {
                return new CodeMatcher(instructions)
                .MatchForward(true,
                    new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch(OpCodes.Ldfld),
                    new CodeMatch(OpCodes.Ret)
                )
                .Insert(
                    new CodeInstruction(OpCodes.Ldc_R4, (float)60),
                    new CodeInstruction(OpCodes.Mul),
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Time), "get_deltaTime")),
                    new CodeInstruction(OpCodes.Mul))
                .InstructionEnumeration();
            }
        }
        [HarmonyPatch(typeof(EntityControl), "FixedUpdate")]
        static class LeifFlyPatch
        {
            [HarmonyTranspiler]
            static IEnumerable<CodeInstruction> Patch(IEnumerable<CodeInstruction> instructions)
            {
                return new CodeMatcher(instructions)
                .MatchForward(false,
                    new CodeMatch(OpCodes.Ldsfld, AccessTools.Field(typeof(MainManager), "framestep"))
                ).SetAndAdvance(OpCodes.Nop, null)
                .Advance(1).SetInstruction(Utils.AddDeltaTimeFloat)
                .InstructionEnumeration();
            }
        }
        [HarmonyPatch(typeof(EntityControl))]
        [HarmonyPatch("ShakeSprite", new[] { typeof(Vector3), typeof(float) })]
        static class ShakeSpritePatch
        {
            [HarmonyPostfix]
            static void Post(EntityControl __instance, Vector3 ___extraoffset)
            {
                if (CorePlugin.ToggleFixShakeSprite.Value)
                {
                    __instance.spritetransform.localPosition = Vector3.zero + ___extraoffset;
                }
            }
        }
    }
}
