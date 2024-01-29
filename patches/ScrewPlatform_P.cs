//using System.Reflection;
//using HarmonyLib;
//namespace BugFablesFixes.Patches
//{
//    class ScrewPlatform_P
//    {
//        [HarmonyPatch(typeof(ScrewPlatform), "Update")]
//        class RotaterPatch
//        {
//            [HarmonyPrefix]
//            [HarmonyWrapSafe]
//            static void Patch(ScrewPlatform __instance, float ___a, NPCControl[] ___switches, int ___oneactive)
//            {
//                if (__instance.IsActive())
//                {
//                    float num = (___switches.Length != 1 && ___oneactive <= -1) ? 1f : (1f - ___switches[___oneactive].actioncooldown / ___switches[___oneactive].vectordata[0].z);
//                    BFPluginFixes.Logger.LogMessage($"{___a} - NUM: {num}");
//                }
//            }
//        }
//    }
//}
//TODO: find out how to fix screwplatform being slow in high fps