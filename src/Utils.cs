using UnityEngine;
namespace FPSFixes
{
    public class Utils
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
    }
}