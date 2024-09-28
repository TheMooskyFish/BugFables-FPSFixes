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
                if (__instance.playerentity && !__instance.battle)
                {
                    __instance.rigid.interpolation = RigidbodyInterpolation.Interpolate;
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
                .AddCustomDeltaTime(60f, 0, false, false)
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
                ).Nopify(0)
                .AddCustomDeltaTime(50f, 1, false, false)
                .Nopify(0)
                .InstructionEnumeration();
            }
        }
        [HarmonyPatch(typeof(EntityControl))]
        [HarmonyPatch("ShakeSprite", [typeof(Vector3), typeof(float)])]
        static class ShakeSpritePatch
        {
            [HarmonyPostfix]
            static void Patch(EntityControl __instance, Vector3 ___extraoffset)
            {
                __instance.spritetransform.localPosition = Vector3.zero + ___extraoffset;
            }
        }
    }
}
