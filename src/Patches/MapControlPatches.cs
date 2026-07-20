using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace FPSFixes.Patches
{
    [HarmonyPatch(typeof(MapControl))]
    internal class MapControlPatches
    {
        private static readonly int Rotation = Shader.PropertyToID("_Rotation");

        [HarmonyPatch(nameof(MapControl.LateUpdate)), HarmonyPrefix]
        private static void SetRotationSkyBox(MapControl __instance)
        {
            if (!__instance.overrideskybox && RenderSettings.skybox != null)
                RenderSettings.skybox.SetFloat(Rotation, 180f + MainManager.MainCamera.transform.position.x);
        }

        [HarmonyPatch(nameof(MapControl.FixedUpdate)), HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> RemoveRotationFixedUpdate(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
            .MatchForward(false,
                new CodeMatch(OpCodes.Call, AccessTools.PropertyGetter(typeof(RenderSettings), "skybox")),
                new CodeMatch(OpCodes.Ldstr))
            .MatchThenNopify(true, new CodeMatch(OpCodes.Add), new CodeMatch(OpCodes.Callvirt))
            .InstructionEnumeration();
        }
    }
}