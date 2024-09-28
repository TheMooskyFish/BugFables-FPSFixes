using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace FPSFixes.Patches
{
    [HarmonyPatch(typeof(EventControl))]
    class EventControl_P
    {
        [HarmonyPatch(nameof(EventControl.Event68)), HarmonyPrefix]
        static void Event68Patch()
        {
            Utils.ChangeInterpolation(MainManager.map.chompy, false);
            foreach (var plr in MainManager.instance.playerdata)
                Utils.ChangeInterpolation(plr.entity, false);
        }
        [HarmonyPatch(nameof(EventControl.EndEvent), [typeof(bool)]), HarmonyPrefix]
        static void EndEventPatch()
        {
            switch (MainManager.lastevent)
            {
                case 68:
                    Utils.ChangeInterpolation(MainManager.map.chompy, true);
                    foreach (var plr in MainManager.instance.playerdata)
                        Utils.ChangeInterpolation(plr.entity, true);
                    return;
                default:
                    return;
            }
        }
    }
}
