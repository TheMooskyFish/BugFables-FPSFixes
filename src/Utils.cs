using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;
namespace FPSFixes
{
    public static class Utils
    {
        public static CodeInstruction AddDeltaTimeFloat = new(OpCodes.Call, AccessTools.Method(typeof(Utils), nameof(AddDeltaTime), [typeof(float)]));
        public static CodeInstruction AddDeltaTimeVector = new(OpCodes.Call, AccessTools.Method(typeof(Utils), nameof(AddDeltaTime), [typeof(Vector3)]));
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
        public static CodeMatcher MatchThenNopify(this CodeMatcher codeMatcher, bool useEnd, params CodeMatch[] codeMatches)
        {
            var oldPos = codeMatcher.Pos;
            var currentPos = codeMatcher.MatchForward(useEnd, codeMatches).Advance(useEnd ? 0 : -1).Pos;
            codeMatcher.Advance(oldPos - currentPos);
            codeMatcher.Nopify(currentPos - oldPos);
            return codeMatcher;
        }
    }
}
