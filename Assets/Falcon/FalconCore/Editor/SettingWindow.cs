using System;
using System.IO;
using System.Threading;
using Falcon.FalconCore.Editor.enums;
using Falcon.FalconCore.Editor.Utils;
using Falcon.FalconCore.Scripts.Utils;
using UnityEditor;
using UnityEngine;

namespace Falcon.FalconCore.Editor
{
    namespace Falcon
    {
        public class SettingWindow : EditorWindow
        {
            private static ExecuteState _pluginInit = ExecuteState.UnDone;
            private static ExecuteState _loginState = ExecuteState.UnDone;
            private Texture2D trashIcon;

            private string userInputFalconKey;

            [MenuItem("Falcon/FalconCore", priority = 1)]
            private static void ShowWindow()
            {
                var window = GetWindow<SettingWindow>("Falcon Settings", true);
                window.minSize = new Vector2(460, 1000);

                window.Show();
                
            }
            
            [MenuItem("Falcon/FalconCore/ClearData")]
            private static void ClearData()
            {
                DataSaveLoad.Save("{}",FalconData.DataFolder , FalconData.DataFile);
            }

            private void Awake()
            {
                FalconEditorInfo.Init();
                if (_loginState.Equals(ExecuteState.UnDone))
                {
                    _loginState = FalconKeyManager.Instance.HasFalconKey() ? ExecuteState.Done : ExecuteState.UnDone;
                }

                if (_pluginInit.Equals(ExecuteState.UnDone))
                {
                    _pluginInit = ExecuteState.Doing;
                    new Thread(() =>
                    {
                        try
                        {
                            FalconPluginsManager.Instance.Init();
                        }
                        finally
                        {
                            _pluginInit = ExecuteState.Done;
                        }
                    }).Start();
                }

                trashIcon = new Texture2D(2, 2);
                trashIcon.LoadImage(File.ReadAllBytes(FalconCoreFileUtils.GetFalconPluginFolder() +
                                                      @"/FalconCore/Editor/images/trash.png"));
            }

            private void OnGUI()
            {
                if (_loginState.Equals(ExecuteState.UnDone))
                    RenderLoginMenu();
                else if (_loginState.Equals(ExecuteState.Doing))
                    RenderWaitingMenu("Logging In, please wait");
                else
                    RenderPluginMenu();
                
                if (!Application.isPlaying && Event.current.type == EventType.Repaint)
                {
                    try
                    {
                        FalconGameObjectUtils.Instance.Update();
                    }
                    catch (Exception e)
                    {
                        FalconLogUtils.Error(e, "#ecbd77");
                    }
                }
            }

            private void RenderLoginMenu()
            {
                //Module Login
                GUILayout.BeginVertical();
                GUILayout.Space(20);
                GUILayout.Label("Falcon Key : ");
                userInputFalconKey = GUILayout.TextField(userInputFalconKey);
                GUILayout.Space(5);
                if (GUILayout.Button("Login", GUILayout.Width(100), GUILayout.Height(20)))
                {
                    _loginState = ExecuteState.Doing;
                    new Thread(() =>
                    {
                        if (FalconKeyManager.Instance.ValidateFalconKey(userInputFalconKey))
                        {
                            _loginState = ExecuteState.Done;
                        }
                        else
                        {
                            _loginState = ExecuteState.UnDone;
                            FalconGameObjectUtils.Instance.Trigger(() =>
                            {
                                EditorUtility.DisplayDialog("Notification", "Information invalid, please retry!",
                                    "Ok");
                            });
                        }
                    }).Start();
                }

                GUILayout.EndVertical();
            }

            private void RenderWaitingMenu(string message)
            {
                GUILayout.BeginVertical();
                
                GUILayout.Space(20);
                
                GUILayout.Label(message);
                
                GUILayout.EndVertical();
            }

            private void RenderPluginMenu()
            {
                if (_pluginInit.Equals(ExecuteState.Doing) || FalconPluginsManager.Instance.Plugins.Count == 0)
                {
                    RenderWaitingMenu("Plugin are being Loaded. please wait!!!");
                }
                else
                {
                    var plugins = FalconPluginsManager.Instance.Plugins;
                    GUILayout.BeginVertical();
                    GUILayout.Space(20);
                    GUILayout.Label("Loaded "+ FalconPluginsManager.Instance.Plugins.Count +"/" + FalconPluginsManager.Instance.RemotePluginCount + " plugin, some may are still being loaded");
                    GUILayout.EndVertical();
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Refresh", GUILayout.Width(100), GUILayout.Height(20)))
                        FalconPluginsManager.Instance.Init();

                    GUILayout.Space(20);
                    if (GUILayout.Button("LogOut", GUILayout.Width(100), GUILayout.Height(20)))
                    {
                        _loginState = ExecuteState.UnDone;
                        FalconKeyManager.Instance.DeleteFalconKey();
                    }
                    GUILayout.EndHorizontal();

                    foreach (var plugin in plugins)
                    {
                        RenderPluginItem(plugin);
                    }
                }
            }

            private void RenderPluginItem(FalconPlugin plugin)
            {
                GUILayout.BeginVertical();
                GUILayout.Space(20);
                GUILayout.EndVertical();
                GUILayout.BeginHorizontal();
                if (!plugin.Installed)
                {
                    GUILayout.Label(plugin.PluginName + " v" + plugin.LatestConfigValue<string>("version"));
                    if (GUILayout.Button("Install", GUILayout.Width(100), GUILayout.Height(20)))
                        plugin.Install();

                    GUI.enabled = false;
                    if (GUILayout.Button(trashIcon, GUILayout.Width(20), GUILayout.Height(20)))
                    {
                    }

                    GUI.enabled = true;
                }
                else
                {
                    if (!plugin.InstalledNewest())
                    {
                        GUILayout.Label(plugin.PluginName + " v" + plugin.LatestConfigValue<string>("version") +
                                        "(installing v" +
                                        plugin.InstalledConfigValue<string>("version") + ")");
                        GUI.enabled = true;
                        if (GUILayout.Button("Update", GUILayout.Width(100), GUILayout.Height(20)))
                        {
                            // plugin.UnInstall();
                            plugin.Install();
                        }

                        GUI.enabled = !plugin.IsFalconCore();

                        if (GUILayout.Button(trashIcon, GUILayout.Width(20), GUILayout.Height(20)))
                            plugin.UnInstall();

                        GUI.enabled = true;
                    }
                    else
                    {
                        GUILayout.Label(plugin.PluginName + " v" + plugin.LatestConfigValue<string>("version"));
                        GUI.enabled = false;
                        if (GUILayout.Button("Install", GUILayout.Width(100), GUILayout.Height(20)))
                        {
                        }

                        GUI.enabled = !plugin.IsFalconCore();

                        if (GUILayout.Button(trashIcon, GUILayout.Width(20), GUILayout.Height(20)))
                            plugin.UnInstall();

                        GUI.enabled = true;
                    }
                }
                
                GUILayout.EndHorizontal();

            }

            private void OnDisable()
            {
                if (!Application.isPlaying)
                {
                    DestroyImmediate(FalconGameObjectUtils.Instance.gameObject);
                }
            }
        }
    }
}