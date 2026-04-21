using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace FPSFixes.Patches
{
    [HarmonyPatch(typeof(EntityControl))]
    internal class EntityControlPatches
    {
        [HarmonyPatch(nameof(EntityControl.Start)), HarmonyPostfix]
        private static void InterpolateEntity(EntityControl __instance)
        {
            if (__instance.playerentity && !__instance.battle)
            {
                __instance.rigid.interpolation = RigidbodyInterpolation.Interpolate;
            }
        }

        [HarmonyPatch(nameof(EntityControl.UpdateFlip)), HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> UpdateFlipPatch(IEnumerable<CodeInstruction> instructions)
        {
            FieldInfo spin = AccessTools.Field(typeof(EntityControl), nameof(EntityControl.spin));
            FieldInfo sprite = AccessTools.Field(typeof(EntityControl), nameof(EntityControl.spritetransform));
            CodeMatcher codematcher = new CodeMatcher(instructions)
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

        [HarmonyPatch(nameof(EntityControl.GetFlipSpeed)), HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> GetFlipPatch(IEnumerable<CodeInstruction> instructions)
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

        [HarmonyPatch(typeof(EntityControl))]
        private static class LeifFlyPatch
        {
            [HarmonyPatch("FixedUpdate"), HarmonyTranspiler] // removes leif fly from fixedupdate
            private static IEnumerable<CodeInstruction> FixedUpdatePatch(IEnumerable<CodeInstruction> instructions)
            {
                return new CodeMatcher(instructions)
                    .MatchForward(false,
                        new CodeMatch(OpCodes.Ldarg_0),
                        new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(EntityControl), "leiffly")))
                    .SetOpcodeAndAdvance(OpCodes.Ret)
                    .Insert(new CodeInstruction(OpCodes.Ldarg_0))
                    .InstructionEnumeration();
            }

            [HarmonyPatch("Update"), HarmonyPostfix]
            private static void UpdatePatch(EntityControl __instance)
            {
                if (__instance.leiffly && MainManager.player != null && !MainManager.instance.inevent)
                {
                    __instance.transform.position = Vector3.Lerp(
                        __instance.transform.position,
                        MainManager.player.transform.position +
                        MainManager.player.entity.spritetransform.right.normalized * 1.5f +
                        MainManager.instance.globalcamdir.forward.normalized * 0.2f,
                        Time.deltaTime * 60 * 0.05f);
                }
            }
        }

        [HarmonyPatch(nameof(EntityControl.Drop), MethodType.Enumerator), HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> UseMathfPowForDrop(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions).MatchForward(true,
                    new CodeMatch(OpCodes.Ldfld),
                    new CodeMatch(OpCodes.Ldc_R4, 1.1f),
                    new CodeMatch(OpCodes.Mul))
                .InsertAndAdvance(
                    new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(Time), nameof(Time.deltaTime))),
                    new CodeInstruction(OpCodes.Ldc_R4, 60f),
                    new CodeInstruction(OpCodes.Mul),
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Mathf), nameof(Mathf.Pow))))
                .InstructionEnumeration();
        }
        
        [HarmonyPatch("ShakeSprite", [typeof(Vector3), typeof(float)]), HarmonyPostfix]
        private static void ShakeSpritePatch(EntityControl __instance, Vector3 ___extraoffset)
        {
            __instance.spritetransform.localPosition = Vector3.zero + ___extraoffset;
        }
    }
}
