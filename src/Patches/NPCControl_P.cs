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
        [HarmonyPatch(typeof(NPCControl))]
        static class PushableIceCube
        {
            [HarmonyPatch("FixedUpdate"), HarmonyPrefix]
            static void FixedUpdatePos(NPCControl __instance)
            {
                if (MainManager.instance.pause || MainManager.instance.minipause) return;
                if (__instance.objecttype == NPCControl.ObjectTypes.PushRock && __instance.data.Length == 3 && __instance.hit)
                {
                    __instance.internalvector[1] = __instance.entity.rigid.position;
                    __instance.entity.rigid.position += __instance.internalvector[0] * (__instance.vectordata[0].z * 1.25f);
                }
            }
            [HarmonyPatch("Update"), HarmonyTranspiler]
            static IEnumerable<CodeInstruction> RemoveUpdatePos(IEnumerable<CodeInstruction> instructions)
            {
                var codematcher = new CodeMatcher(instructions);
                codematcher.MatchForward(false,
                    new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(NPCControl), nameof(NPCControl.ColliderNotThis)))
                ).MatchForward(false,
                    new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(NPCControl), nameof(NPCControl.internalvector))),
                    new CodeMatch(OpCodes.Ldc_I4_1),
                    new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch(OpCodes.Call)
                ).Nopify(6)
                .MatchForward(false,
                    new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch(OpCodes.Call),
                    new CodeMatch(OpCodes.Dup)
                ).Nopify(17);
                return codematcher.InstructionEnumeration();
            }
        }
    }
}
