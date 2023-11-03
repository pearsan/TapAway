using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using Falcon.FalconCore.Scripts.Utils;
using Newtonsoft.Json.Linq;

namespace Falcon.FalconCore.Editor
{
    public class FalconPluginsManager
    {
        private static FalconPluginsManager _instance;

        public static FalconPluginsManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FalconPluginsManager();
                    // _instance.Init();
                }

                return _instance;
            }
        }

        private const String SdkUrl = "https://api.bitbucket.org/2.0/repositories/falcongame/falcon-unity-sdk/src/master/Assets/Falcon/Release/";

        private readonly List<FalconPlugin> plugins = new List<FalconPlugin>() ;

        public List<FalconPlugin> Plugins
        {
            get
            {
                return plugins;
            }
        }

        public int RemotePluginCount { get; private set; }

        private String initID;

        public void Init()
        {

            plugins.Clear();
            List<JToken> bitBucketLinks = new List<JToken>(GetJTokenFromBitBucketLink(SdkUrl));
            RemotePluginCount = bitBucketLinks.Count;
            initID = Guid.NewGuid().ToString();
            foreach (var value in bitBucketLinks)
            {
                new Thread(() =>
                {
                    String id = initID;
                    String[] tokens = value.SelectToken("path").Value<String>().Split('/');
                    String pluginName = tokens[tokens.Length - 1];
                    String pluginFolderLink = value.SelectToken("links").SelectToken("self").SelectToken("href")
                        .Value<String>();
                    HashSet<String> pluginLink = new HashSet<string>();

                    String configLink = null;
                    foreach (var val in GetJTokenFromBitBucketLink(pluginFolderLink))
                    {
                        String href = val["links"]["self"]["href"].Value<String>();
                        if (href != null && href.EndsWith("config.txt"))
                        {
                            configLink = href;

                        }
                        else if (href != null)
                        {
                            pluginLink.Add(href);
                        }
                    }

                    String versionConfig = FalconNetUtils.DoRequest(HttpMethod.Get, configLink);
                    FalconPlugin plugin = new FalconPlugin(pluginName, versionConfig, pluginLink);
                    if (initID.Equals(id))
                    {
                        plugins.Add(plugin);
                    }
                }).Start();
            }

        }

        public FalconPlugin GetPlugin(String pluginName)
        {
            foreach (var plugin in plugins)
            {
                if (plugin.PluginName.Equals(pluginName))
                {
                    return plugin;
                }
            }

            return null;
        }

        private static IEnumerable<JToken> GetJTokenFromBitBucketLink(String url)
        {
            JObject jsonData = JObject.Parse(FalconNetUtils.DoRequest(HttpMethod.Get, url));

            foreach (var value in jsonData["values"])
            {
                if (!value.SelectToken("path").Value<String>().Contains(".meta"))
                {
                    yield return value;
                }
            }
        }
    }
}