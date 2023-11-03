using System;
using System.IO;
using System.Linq;
using Falcon.FalconCore.Editor.Utils;
using Falcon.FalconCore.Scripts.Utils;
using UnityEditor;
using UnityEngine;

namespace Falcon.FalconCore.Editor
{
    [InitializeOnLoad]
    public class FalconPluginAssetImporter
    {
        static FalconPluginAssetImporter()
        {
            // Debug.Log("FalconPluginAssetImporter: s_ctor");
            // AssetDatabase.importPackageStarted += OnImportPackageStarted;
            AssetDatabase.importPackageCompleted += OnImportPackageCompleted;
            AssetDatabase.importPackageFailed += OnImportPackageFailed;
        }

        private static void OnImportPackageCompleted(string packageName)
        {
            string importPackageLocation = Path.Combine(FalconCoreFileUtils.GetFalconPluginFolder(), packageName);
            if (Directory.Exists(Path.Combine(Application.dataPath, "Falcon", packageName)))
            {
                try
                {
                    FileUtil.MoveFileOrDirectory(Application.dataPath + Path.DirectorySeparatorChar + "Falcon" +
                                                 Path.DirectorySeparatorChar + packageName, importPackageLocation);
                    AssetDatabase.Refresh();
                }
                catch (Exception e)
                {
                    FalconLogUtils.Warning(e, "#ecbd77");
                }
            }

            CleanUp();

            PluginInstallResponder responder = LookUpInstallResponders(packageName);
            if (responder != null) responder.OnPluginInstalled(importPackageLocation);
            
            FalconLogUtils.Info("ImportPackageCompleted(" + packageName + ")","#ecbd77");
        }

        private static void OnImportPackageFailed(string packageName, string errormessage)
        {
            CleanUp();
        }


        private static void CleanUp()
        {
            if (Directory.Exists(Application.dataPath + Path.DirectorySeparatorChar + "Falcon"))
            {
                if (IsDirectoryEmpty(Application.dataPath + Path.DirectorySeparatorChar + "Falcon"))
                {
                    FileUtil.DeleteFileOrDirectory(Application.dataPath + Path.DirectorySeparatorChar + "Falcon");
                }
            }
        }

        private static bool IsDirectoryEmpty(string directoryPath)
        {
            if (Directory.GetFiles(directoryPath).Length > 0)
            {
                return false;
            }

            foreach (var path in Directory.GetDirectories(directoryPath))
            {
                if (!IsDirectoryEmpty(path)) return false;
            }

            return true;
        }
        
        private static PluginInstallResponder LookUpInstallResponders(string packageName)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(t => t.IsSubclassOf(typeof(PluginInstallResponder)) && !t.IsAbstract)
                .Select(t => (PluginInstallResponder) Activator.CreateInstance(t));

            foreach (PluginInstallResponder responder in types)
            {
                if (responder.GetPackageName().Equals(packageName))
                {
                    return responder;
                }
            }

            return null;
        }

        // private static bool IsFile(String path)
        // {
        //     // get the file attributes for file or directory
        //     FileAttributes attr = File.GetAttributes(path);
        //
        //     return !attr.HasFlag(FileAttributes.Directory);
        // }
    }
}