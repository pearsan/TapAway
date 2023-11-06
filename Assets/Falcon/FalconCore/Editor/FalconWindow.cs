using System;
using System.Collections;
using System.IO;
using Falcon.FalconCore.Editor.FPlugins;
using Falcon.FalconCore.Editor.Utils;
using Falcon.FalconCore.Scripts;
using Falcon.FalconCore.Scripts.Utils;
using Falcon.FalconCore.Scripts.Utils.Sequences.Editor;
using UnityEditor;
using UnityEngine;

namespace Falcon.FalconCore.Editor
{
    namespace Falcon
    {
        public class FalconWindow : EditorWindow
        {
            private ExecState loginState = ExecState.NotStarted;

            private Texture2D trashIcon;

            private void Awake()
            {
                if (ExecStates.CanStart(loginState))
                    loginState = FKeyManager.Instance.HasFalconKey() ? ExecState.Succeed : ExecState.Failed;

                trashIcon = new Texture2D(2, 2);
                trashIcon.LoadImage(File.ReadAllBytes(FalconCoreFileUtils.GetFalconPluginFolder() +
                                                      @"/FalconCore/Editor/images/trash.png"));
            }

            private void OnDisable()
            {
                if (!Application.isPlaying) DestroyImmediate(FalconMain.Instance.gameObject);
            }

            private void OnGUI()
            {
                if (ExecStates.CanStart(loginState))
                    RenderLoginMenu();
                else if (loginState.Equals(ExecState.Processing))
                    RenderWaitingMenu("Logging In, please wait");
                else
                    RenderPluginMenu();
            }

            [MenuItem("Falcon/FalconMenu", priority = 0)]
            public static void ShowWindow()
            {
                var window = GetWindow<FalconWindow>("Falcon Settings", true);
                window.minSize = new Vector2(460, 600);
                window.maxSize = new Vector2(460, 800);

                window.Show();
            }
            
            private void RenderWaitingMenu(string message)
            {
                GUIVertical(() =>
                {
                    GUILayout.Space(20);

                    GUILayout.Label(message);
                });
            }

            private void RenderPluginMenu()
            {
                if (FPlugin.PullState.Equals(ExecState.Processing))
                {
                    RenderWaitingMenu("Plugin are being Loaded. please wait!!!");
                }
                else
                {
                    var plugins = FPlugin.GetAllSync();

                    GUIVertical(() =>
                    {
                        GUILayout.Space(20);
                        GUILayout.Label("Loaded " + plugins.Count + "/" + FPlugin.RemotePluginCount +
                                        " plugin, some may are still being loaded");
                    });

                    GUIHorizon(() =>
                    {
                        if (GUILayout.Button("Refresh", GUILayout.Width(100), GUILayout.Height(20)))
                        {
                            FPlugin.Clear();
                            new EditorSequence(FPlugin.Init()).Start();
                        }

                        GUILayout.Space(20);
                        if (GUILayout.Button("LogOut", GUILayout.Width(100), GUILayout.Height(20)))
                        {
                            loginState = ExecState.NotStarted;
                            FKeyManager.Instance.DeleteFalconKey();
                        }
                    });

                    foreach (var plugin in plugins) RenderPluginItem(plugin);
                }
            }

            private void RenderPluginItem(FPlugin plugin)
            {
                GUIVertical(() => { GUILayout.Space(20); });

                GUIHorizon(() =>
                {
                    if (!plugin.Installed)
                    {
                        RenderUninstalledPlugin(plugin);
                    }
                    else
                    {
                        if (!plugin.InstalledNewest())
                        {
                            RenderOldPlugin(plugin);
                        }
                        else
                        {
                            RenderNewestPlugin(plugin);
                        }
                    }
                });
            }

            private void RenderUninstalledPlugin(FPlugin plugin)
            {
                GUILayout.Label(plugin.PluginName + " v" + plugin.RemoteConfig.Version);
                if (GUILayout.Button("Install", GUILayout.Width(100), GUILayout.Height(20)))
                    new EditorSequence(plugin.Install()).Start();

                GUI.enabled = false;
                GUILayout.Button(trashIcon, GUILayout.Width(20), GUILayout.Height(20));

                GUI.enabled = true;
            }

            private void RenderOldPlugin(FPlugin plugin)
            {
                GUILayout.Label(plugin.PluginName + " v" + plugin.RemoteConfig.Version +
                                "(installing v" +
                                plugin.InstalledConfig.Version + ")");
                GUI.enabled = true;
                if (GUILayout.Button("Update", GUILayout.Width(100), GUILayout.Height(20)))
                    new EditorSequence(plugin.Install()).Start();

                GUI.enabled = !plugin.IsFalconCore();

                if (GUILayout.Button(trashIcon, GUILayout.Width(20), GUILayout.Height(20)))
                    plugin.UnInstall();

                GUI.enabled = true;
            }

            private void RenderNewestPlugin(FPlugin plugin)
            {
                GUILayout.Label(plugin.PluginName + " v" + plugin.RemoteConfig.Version);
                GUI.enabled = false;
                GUILayout.Button("Install", GUILayout.Width(100), GUILayout.Height(20));

                GUI.enabled = !plugin.IsFalconCore();

                if (GUILayout.Button(trashIcon, GUILayout.Width(20), GUILayout.Height(20)))
                    plugin.UnInstall();

                GUI.enabled = true;
            }

            private void GUIHorizon(Action action)
            {
                GUILayout.BeginHorizontal();
                action.Invoke();
                GUILayout.EndHorizontal();
            }

            private void GUIVertical(Action action)
            {
                GUILayout.BeginVertical();
                action.Invoke();
                GUILayout.EndVertical();
            }


            #region Login

            private string userInputFalconKey;

            private void RenderLoginMenu()
            {
                //Module Login
                GUIVertical(() =>
                {
                    GUILayout.Space(20);
                    GUILayout.Label("Falcon Key : ");
                    userInputFalconKey = GUILayout.TextField(userInputFalconKey);
                    GUILayout.Space(5);
                    if (GUILayout.Button("Login", GUILayout.Width(100), GUILayout.Height(20)))
                    {
                        loginState = ExecState.Processing;
                        new EditorSequence(ValidateLogin(userInputFalconKey)).Start();
                    }
                });
                GUILayout.BeginVertical();


                GUILayout.EndVertical();
            }

            private IEnumerator ValidateLogin(string key)
            {
                var validation = FKeyManager.Instance.ValidateFalconKey(key);
                yield return validation;
                if (validation.Current)
                {
                    loginState = ExecState.Succeed;
                }
                else
                {
                    loginState = ExecState.Failed;
                    EditorUtility.DisplayDialog("Notification", "Information invalid, please retry!", "Ok");
                }
            }

            #endregion
        }
    }
}