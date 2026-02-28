using System.Linq;
using HarmonyLib;

namespace FPSFixes.Patches
{
    [HarmonyPatch(typeof(EventControl))]
    internal class EventControl_P
    {
        private static readonly int[] SafeEvents = [2, 11, 14, 85, 200]; //TODO: check more events
        [HarmonyPatch(nameof(EventControl.StartEvent)), HarmonyPostfix]
        private static void StartEventPatch()
        {
            var safeEventCheck = SafeEvents.Contains(MainManager.lastevent);
            CorePlugin.Logger.LogInfo($"Start Event: {MainManager.lastevent} - Safe Event Check: {safeEventCheck}");
            if (safeEventCheck) return;
            if (MainManager.map?.chompy)
                Utils.ChangeInterpolation(MainManager.map?.chompy, false);
            foreach (var plr in MainManager.instance.playerdata)
                Utils.ChangeInterpolation(plr.entity, false);
        }
        [HarmonyPatch(nameof(EventControl.EndEvent), typeof(bool)), HarmonyPostfix]
        private static void EndEventPatch()
        {
            var safeEventCheck = SafeEvents.Contains(MainManager.lastevent);
            CorePlugin.Logger.LogInfo($"End Event: {MainManager.lastevent} - Safe Event Check: {safeEventCheck}");
            if (MainManager.map?.chompy)
                Utils.ChangeInterpolation(MainManager.map?.chompy, true);
            foreach (var plr in MainManager.instance.playerdata)
                Utils.ChangeInterpolation(plr.entity, true);
        }
    }
}
