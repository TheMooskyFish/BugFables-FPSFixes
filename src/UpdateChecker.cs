using System;
using System.Collections;
using System.Text;
using BepInEx;
using UnityEngine;
using UnityEngine.Networking;

namespace FPSFixes
{
    public class UpdateChecker
    {
        [Serializable]
        private class GHRelease
        {
            public string tag_name = null;
        }
        private const string _url = "https://api.github.com/repos/TheMooskyFish/BugFables-FPSFixes/releases/latest";
        public IEnumerator CheckUpdate()
        {
            var web = UnityWebRequest.Get(_url);
            yield return web.SendWebRequest();
            if (web.isNetworkError || web.isHttpError)
            {
                CorePlugin.Logger.LogError(web.error);
                yield break;
            }
            var jsondata = JsonUtility.FromJson<GHRelease>(Encoding.UTF8.GetString(web.downloadHandler.GetData()));
            if (jsondata.tag_name is null)
                yield break;
            var pluginVersion = MetadataHelper.GetMetadata(typeof(CorePlugin)).Version;
            var ghVersion = new Version(jsondata.tag_name.Substring(1));
            if (pluginVersion < ghVersion)
            {
                CorePlugin.Version += $" - Update Available: {jsondata.tag_name}";
            }
            yield return null;
        }
    }
}
