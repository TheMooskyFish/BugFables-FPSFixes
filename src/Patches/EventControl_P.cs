﻿using System.Linq;
using HarmonyLib;

namespace FPSFixes.Patches
{
    [HarmonyPatch(typeof(EventControl))]
    class EventControl_P
    {
        //[HarmonyPatch(nameof(EventControl.Event68)), HarmonyPrefix]
        //static void Event68Patch()
        //{
        //    Utils.ChangeInterpolation(MainManager.map.chompy, false);
        //    foreach (var plr in MainManager.instance.playerdata)
        //        Utils.ChangeInterpolation(plr.entity, false);
        //}
        //[HarmonyPatch(nameof(EventControl.EndEvent), [typeof(bool)]), HarmonyPrefix]
        //static void EndEventPatch()
        //{
        //    switch (MainManager.lastevent)
        //    {
        //        case 68:
        //            Utils.ChangeInterpolation(MainManager.map.chompy, true);
        //            foreach (var plr in MainManager.instance.playerdata)
        //                Utils.ChangeInterpolation(plr.entity, true);
        //            return;
        //        default:
        //            return;
        //    }
        //}
        internal static int[] safeEvents = [85, 200]; //TODO: check more events
        [HarmonyPatch(nameof(EventControl.StartEvent)), HarmonyPostfix]
        static void StartEventPatch()
        {
            var safeEventCheck = safeEvents.Contains(MainManager.lastevent);
            CorePlugin.Logger.LogInfo($"Start Event: {MainManager.lastevent} - Safe Event Check: {safeEventCheck}");
            if (safeEventCheck) return;
            if (MainManager.map?.chompy)
                Utils.ChangeInterpolation(MainManager.map?.chompy, false);
            foreach (var plr in MainManager.instance.playerdata)
                Utils.ChangeInterpolation(plr.entity, false);
        }
        [HarmonyPatch(nameof(EventControl.EndEvent), [typeof(bool)]), HarmonyPostfix]
        static void EndEventPatch()
        {
            var safeEventCheck = safeEvents.Contains(MainManager.lastevent);
            CorePlugin.Logger.LogInfo($"End Event: {MainManager.lastevent} - Safe Event Check: {safeEventCheck}");
            if (MainManager.map?.chompy)
                Utils.ChangeInterpolation(MainManager.map?.chompy, true);
            foreach (var plr in MainManager.instance.playerdata)
                Utils.ChangeInterpolation(plr.entity, true);
        }
    }
}
