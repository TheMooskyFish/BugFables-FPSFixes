using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace FPSFixes.Patches
{
    [HarmonyPatch(typeof(FlappyBee))]
    internal class FlappyBeePatches
    {
        [HarmonyPatch(nameof(FlappyBee.TitleShow)), HarmonyPostfix]
        private static IEnumerator EnableInterpolationPatch(IEnumerator __result, FlappyBee __instance)
        {
            while (__result.MoveNext())
                yield return __result.Current;
            __instance.player.rigid.interpolation = RigidbodyInterpolation2D.Interpolate;
        }
    }
}