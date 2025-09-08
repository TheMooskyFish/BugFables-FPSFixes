using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace FPSFixes.Patches
{
    internal class EntityControl_P
    {
        [HarmonyPatch(typeof(EntityControl), "Start")]
        private class InterpolateEntity
        {
            private static void Postfix(EntityControl __instance)
            {
                if (__instance.playerentity && !__instance.battle)
                {
                    __instance.rigid.interpolation = RigidbodyInterpolation.Interpolate;
                }
            }
        }
        [HarmonyPatch(typeof(EntityControl), "UpdateFlip")]
        private class UpdateFlipPatch
        {
            [HarmonyTranspiler]
            private static IEnumerable<CodeInstruction> Patch(IEnumerable<CodeInstruction> instructions)
            {
                var spin = AccessTools.Field(typeof(EntityControl), nameof(EntityControl.spin));
                var sprite = AccessTools.Field(typeof(EntityControl), nameof(EntityControl.spritetransform));
                var codematcher = new CodeMatcher(instructions)
                .MatchForward(false,
                    new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch(OpCodes.Ldfld, sprite),
                    new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch(OpCodes.Ldfld, spin)
                )
                .Nopify(3)
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                .Set(OpCodes.Call, AccessTools.Method(typeof(Utils), nameof(Utils.EntitySpin)));
                return codematcher.InstructionEnumeration();
            }
        }
        [HarmonyPatch(typeof(EntityControl), "GetFlipSpeed")]
        private class GetFlipPatch
        {
            [HarmonyTranspiler]
            private static IEnumerable<CodeInstruction> Patch(IEnumerable<CodeInstruction> instructions)
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
        
        [HarmonyPatch(typeof(EntityControl))]
        private static class LeifFlyPatch
        {
            [HarmonyPatch("FixedUpdate"), HarmonyTranspiler] // removes leif fly from fixedupdate
            private static IEnumerable<CodeInstruction> FixedUpdatePatch(IEnumerable<CodeInstruction> instructions)
            {
                return new CodeMatcher(instructions)
                    .MatchForward(false,
                        new CodeMatch(OpCodes.Ldarg_0),
                        new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(EntityControl), "leiffly"))
                    ).MatchThenNopify(true,
                        new CodeMatch(OpCodes.Call),
                        new CodeMatch(OpCodes.Callvirt))
                    .InstructionEnumeration();
            }

            [HarmonyPatch("Update"), HarmonyPostfix]
            private static void UpdatePatch(EntityControl __instance)
            {
                if (__instance.leiffly && MainManager.player != null && !MainManager.instance.inevent)
                {
                    __instance.transform.position = Vector3.Lerp(__instance.transform.position,
                        MainManager.player.transform.position +
                        MainManager.player.entity.spritetransform.right.normalized * 1.5f +
                        MainManager.instance.globalcamdir.forward.normalized * 0.2f, MainManager.framestep * 0.05f);
                }
            }
        }
        [HarmonyPatch(typeof(EntityControl))]
        [HarmonyPatch("ShakeSprite", [typeof(Vector3), typeof(float)])]
        private static class ShakeSpritePatch
        {
            [HarmonyPostfix]
            private static void Patch(EntityControl __instance, Vector3 ___extraoffset)
            {
                __instance.spritetransform.localPosition = Vector3.zero + ___extraoffset;
            }
        }
    }
}
