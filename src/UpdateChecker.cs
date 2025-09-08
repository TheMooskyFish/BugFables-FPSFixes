using System;
using System.Collections;
using System.Text;
using BepInEx;
using UnityEngine;
using UnityEngine.Networking;

namespace FPSFixes
{   
    internal static class UpdateChecker
    {
        [Serializable]
        private class GhRelease
        {
            public string tag_name = string.Empty;
        }
        private const string URL = "https://api.github.com/repos/TheMooskyFish/BugFables-FPSFixes/releases/latest";
        internal static IEnumerator CheckUpdate()
        {
            var web = UnityWebRequest.Get(URL);
            yield return web.SendWebRequest();
            if (web.isNetworkError || web.isHttpError)
            {
                CorePlugin.Logger.LogError(web.error);
                yield break;
            }
            var data = JsonUtility.FromJson<GhRelease>(Encoding.UTF8.GetString(web.downloadHandler.GetData()));
            if (data.tag_name is null)
                yield break;
            var pluginVersion = MetadataHelper.GetMetadata(typeof(CorePlugin)).Version;
            var ghVersion = new Version(data.tag_name.Substring(1));
            if (pluginVersion < ghVersion)
            {
                CorePlugin.Version += $" - Update Available: {data.tag_name}";
            }
            yield return null;
        }
    }
}
