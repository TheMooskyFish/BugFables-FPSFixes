using HarmonyLib;
using UnityEngine;

namespace FPSFixes.Patches
{
    [HarmonyPatch(typeof(DialogueAnim))]
    class DialogueAnim_P
    {
        [HarmonyPatch("FixedUpdate"), HarmonyPrefix]
        static void DisableUpdate(DialogueAnim __instance)
        {
            if (!__instance.GetComponent<AnimUpdate>())
            {
                __instance.gameObject.AddComponent<AnimUpdate>().dialogueAnim = __instance;
                __instance.enabled = false;
            }
        }
        class AnimUpdate : MonoBehaviour
        {
            internal DialogueAnim dialogueAnim;
            public void Update()
            {
                Vector3 vector = dialogueAnim.targetscale * dialogueAnim.multiplier;

                if (dialogueAnim.flipx)
                    vector.x = 0f;
                else if (dialogueAnim.flipy)
                    vector.y = 0f;
                else if (dialogueAnim.shrink)
                    vector = Vector3.zero;

                transform.localScale = Vector3.Lerp(transform.localScale, vector, dialogueAnim.shrinkspeed * 50f * Time.smoothDeltaTime);

                if (dialogueAnim.targetpos != Vector3.zero)
                    transform.localPosition = Vector3.Lerp(transform.localPosition, dialogueAnim.targetpos, dialogueAnim.speed * 50f * Time.smoothDeltaTime);
            }
        }
    }
}
