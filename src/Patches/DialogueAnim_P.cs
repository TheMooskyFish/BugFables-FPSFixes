using HarmonyLib;
using UnityEngine;

namespace FPSFixes.Patches
{
    [HarmonyPatch(typeof(DialogueAnim))]
    internal class DialogueAnim_P
    {
        [HarmonyPatch("FixedUpdate"), HarmonyPrefix]
        private static bool DisableUpdate(DialogueAnim __instance)
        {
            if (!__instance.GetComponent<AnimUpdate>())
            {
                __instance.gameObject.AddComponent<AnimUpdate>()._dialogueAnim = __instance;
            }
            return false;
        }
        private class AnimUpdate : MonoBehaviour
        {
            internal DialogueAnim _dialogueAnim;
            public void Update()
            {
                if (!_dialogueAnim.enabled) return;
                var vector = _dialogueAnim.targetscale * _dialogueAnim.multiplier;

                if (_dialogueAnim.flipx)
                    vector.x = 0f;
                else if (_dialogueAnim.flipy)
                    vector.y = 0f;
                else if (_dialogueAnim.shrink)
                    vector = Vector3.zero;

                transform.localScale = Vector3.Lerp(transform.localScale, vector, _dialogueAnim.shrinkspeed * 50f * Time.smoothDeltaTime);

                if (_dialogueAnim.targetpos != Vector3.zero)
                    transform.localPosition = Vector3.Lerp(transform.localPosition, _dialogueAnim.targetpos, _dialogueAnim.speed * 50f * Time.smoothDeltaTime);
            }
        }
    }
}
