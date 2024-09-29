using System;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace FPSFixes
{
    public static class Utils
    {
        public static float AddDeltaTime(float number, float fps, bool smooth)
        {
            if (CorePlugin.ToggledeltaTime.Value)
            {
                var delta = smooth ? Time.smoothDeltaTime : Time.deltaTime;
                return number * fps * delta;
            }
            return number;
        }
        public static Vector3 AddDeltaTime(Vector3 vector, float fps, bool smooth)
        {
            if (CorePlugin.ToggledeltaTime.Value)
            {
                var delta = smooth ? Time.smoothDeltaTime : Time.deltaTime;
                vector.x = vector.x * fps * delta;
                vector.y = vector.y * fps * delta;
                vector.z = vector.z * fps * delta;
                return vector;
            }
            return vector;
        }
        public static CodeMatcher AddCustomDeltaTime(this CodeMatcher codeMatcher, float fps, int advance, bool vector, bool smooth)
        {
            Type[] type = vector ? [typeof(Vector3), typeof(float), typeof(bool)] : [typeof(float), typeof(float), typeof(bool)];
            return codeMatcher
            .Advance(advance).InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldc_R4, fps),
                new CodeInstruction(smooth ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Utils), nameof(AddDeltaTime), type))
            );
        }
        public static void EntitySpin(EntityControl entityControl) => entityControl.spritetransform.Rotate(AddDeltaTime(entityControl.spin, 60f, false));

        public static void DigSound(EntityControl entity)
        {
            if (entity.animstate != 101 && !MainManager.player.digging && MainManager.SoundIsPlaying("Dig") == -1)
                MainManager.PlaySound("Dig", -1, 1f, 0.7f);
        }
        public static float ViFly(PlayerControl plr)
        {
            if (plr.transform.position.y < plr.startheight.Value + 0.99f)
                return 1.25f;
            return 0f;
        }
        public static float DialogueSpeed(float speed)
        {
            if (speed == 0.02f)
                return 0.0275f;
            return speed;
        }
        public static void ChangeInterpolation(EntityControl entity, bool mode)
        {
            if (entity == null) return;
            entity.rigid.interpolation = mode ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;
        }
        public static CodeMatcher Nopify(this CodeMatcher codeMatcher, int instsnumber)
        {
            foreach (var _ in codeMatcher.InstructionsWithOffsets(0, instsnumber))
                codeMatcher.SetAndAdvance(OpCodes.Nop, null);
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
