using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;
namespace FPSFixes
{
    public static class Utils
    {
        public static float AddDeltaTime(float number)
        {
            if (CorePlugin.ToggleUtilsdeltaTime.Value)
                return number * 60 * Time.deltaTime;
            else
                return number;
        }

        public static Vector3 AddDeltaTime(Vector3 vector)
        {
            if (CorePlugin.ToggleUtilsdeltaTime.Value)
            {
                vector.x = vector.x * 60 * Time.deltaTime;
                vector.y = vector.y * 60 * Time.deltaTime;
                vector.z = vector.z * 60 * Time.deltaTime;
                return vector;
            }
            else
                return vector;
        }

        public static void EntitySpin(EntityControl entityControl) => entityControl.spritetransform.Rotate(AddDeltaTime(entityControl.spin));

        public static void DigSound(EntityControl entity)
        {
            if (entity.animstate != 101 && !MainManager.player.digging && MainManager.SoundIsPlaying("Dig") == -1)
                MainManager.PlaySound("Dig", -1, 1f, 0.7f);
        }
        public static CodeMatcher Nopify(this CodeMatcher codeMatcher, int instsnumber)
        {
            foreach (var _ in codeMatcher.InstructionsWithOffsets(0, instsnumber))
            {
                codeMatcher.SetAndAdvance(OpCodes.Nop, null);
            }
            return codeMatcher;
        }
    }
}
