using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Falcon.FalconCore.Editor.Utils;
using Falcon.FalconCore.Scripts.Exception;
using Falcon.FalconCore.Scripts.Utils;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using KeyNotFoundException = Falcon.FalconCore.Scripts.Exception.KeyNotFoundException;

namespace Falcon.FalconCore.Editor
{
    public class FalconPlugin
    {
        private const String UnityPackageExtension = ".unitypackage";
        
        public string PluginName { get; private set; }

        private JObject LatestConfig { get; set; }

        private string PluginUrl { get; set; }

        public bool Installed { get; private set; }

        public string InstalledDirectory { get; private set; }
        
        private JObject installedConfig;

        public FalconPlugin(string name, string latestVersionConfig, HashSet<string> urls)
        {
            PluginName = name;
            LatestConfig = JObject.Parse(latestVersionConfig);
            var newestVersion = LatestConfigValue<String>("version");

            foreach (var url in urls)
            {
                if (url.EndsWith(newestVersion + UnityPackageExtension))
                {
                    PluginUrl = url;
                    break;
                }
            }
            if (PluginUrl == null)
            {
                PluginUrl = urls.First();
            }

            var directory = Directory.GetDirectories(FalconEditorInfo.AppDataPath, name, SearchOption.AllDirectories);

            if (directory.Length == 0) Installed = false;
            else
                try
                {
                    Installed = true;
                    InstalledDirectory = directory[0].Contains("Release") ? directory[1] : directory[0];
                    installedConfig =
                        JObject.Parse(File.ReadAllText(InstalledDirectory + Path.DirectorySeparatorChar +
                                                       "config.txt"));
                }
                catch (Exception)
                {
                    Installed = false;
                }
        }
        
        public void Install()
        {
            try{
                var requireJson = LatestConfigValue<JObject>("require");
                var requirePlugins = new List<FalconPlugin>();
                foreach (var json in requireJson)
                {
                    var plugin = FalconPluginsManager.Instance.GetPlugin(json.Key);
                    if (string.CompareOrdinal(plugin.InstalledConfigValue<String>("version"), json.Value.Value<String>()) < 0)
                        requirePlugins.Add(plugin);
                }

                if (requirePlugins.Count == 0)
                {
                    UnsafeInstall();
                    return;
                }

                var requirePluginString = new StringBuilder();
                requirePluginString
                    .Append("The following plugins are required for" + PluginName + ":").AppendLine();
                foreach (var plugin in requirePlugins)
                    requirePluginString.Append("  - ").Append(plugin.PluginName).AppendLine();

                requirePluginString.Append("Please install/update them first!");

                EditorUtility.DisplayDialog("Additional plugin require!!!",
                    requirePluginString.ToString(), "Ok");
            } catch(KeyNotFoundException){
                UnsafeInstall();
            }
        }

        private void UnsafeInstall()
        {
            var tempFolder = Application.dataPath + "/../Temp/" + PluginName + UnityPackageExtension;

            installedConfig = LatestConfig;
            Installed = true;
            InstalledDirectory = FalconCoreFileUtils.GetFalconPluginFolder() + Path.DirectorySeparatorChar + PluginName;

            new Thread(() =>
            {
                FalconLogUtils.Info("Downloading " + PluginName, "#ecbd77");
                FalconNetUtils.GetFile(PluginUrl, tempFolder);
                FalconLogUtils.Info("Downloading complete, preparing to import","#ecbd77");
                FalconGameObjectUtils.Instance.Trigger(() =>
                {
                    AssetDatabase.ImportPackage(tempFolder, true);
                });
            }).Start();
        }

        public void UnInstall()
        {
            FalconLogUtils.Info("Start uninstalling","#ecbd77");
            FileUtil.DeleteFileOrDirectory(InstalledDirectory);
            Installed = false;
            installedConfig = null;
            InstalledDirectory = null;
        }

        public bool IsFalconCore()
        {
            return string.CompareOrdinal(PluginName, "FalconCore") == 0;
        }

        public T InstalledConfigValue<T>(string valueName)
        {
            if (Installed)
            {
                var token = installedConfig[valueName];
                if (token == null)
                    throw new KeyNotFoundException("No config found under the name : " + valueName);
                return token.Value<T>();
            }
            else
            {
                throw new InvalidActionException("Plugin uninstall");
            }
        }

        public T LatestConfigValue<T>(string valueName)
        {
            var token = LatestConfig[valueName];
            if (token == null)
                throw new KeyNotFoundException("No config found under the name : " + valueName);
            return token.Value<T>();
        }

        public bool InstalledNewest()
        {
            return String.CompareOrdinal(InstalledConfigValue<String>("version"),
                LatestConfigValue<String>("version")) >= 0;
        }

        // public bool SetConfigValue(string name, string value)
        // {
        //     installedConfig[name] = value;
        //
        //     var configFilePath = InstalledDirectory + Path.DirectorySeparatorChar + "config.txt";
        //     if (File.Exists(configFilePath))
        //     {
        //         File.WriteAllText(configFilePath, installedConfig.ToString());
        //         return true;
        //     }
        //
        //     return false;
        // }

        // public bool CurrentVersionAtLeast(String requireVersion)
        // {
        //     return String.CompareOrdinal(InstalledConfigValue("version"), requireVersion) >= 0;
        // }
    }
}