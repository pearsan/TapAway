using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Falcon.FalconCore.Scripts.Exception;
using Falcon.FalconCore.Scripts.Utils;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditorInternal;
#endif

namespace Falcon.FalconCore.Scripts
{
    public static class FalconMain
    {
        private static readonly SortedDictionary<int, List<IFalconInit>> PriorityToInstances = new SortedDictionary<int, List<IFalconInit>>();

        private static bool InitStarted { get; set; }

        public static bool InitComplete { get; private set; }

        public static int? MainThreadId { get; private set; }

        public static void Init()
        {
            if (!InitStarted)
            {
#if UNITY_EDITOR
                if (!InternalEditorUtility.CurrentThreadIsMainThread())
                    throw new System.Exception("FalconMain.Init() can only be called from the main thread");
#endif
                try
                {
                    FalconLogUtils.Info("Sdk Initialize Started", "#ecbd77");
                    MainThreadId = Thread.CurrentThread.ManagedThreadId;
                    InitComplete = false;
                    
                    var init = FalconGameObjectUtils.Instance;

                    
                    GetFalconInitInstances();

                    CallInit();
                    InitStarted = true;
                }
                catch (System.Exception e)
                {
                    FalconLogUtils.Error(e, "#ecbd77");
                }
            }
        }

        private static void GetFalconInitInstances()
        {
            PriorityToInstances.Clear();
            var types = from t in AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                where t.GetInterfaces().Contains(typeof(IFalconInit))
                      && t.GetConstructor(Type.EmptyTypes) != null
                select t;

            foreach (var type in types)
                try
                {
                    IFalconInit instance = GetFalconInitInstance(type);

                    if (instance != null)
                    {
                        if (!PriorityToInstances.ContainsKey(instance.GetPriority()))
                        {
                            PriorityToInstances.Add(instance.GetPriority(), new List<IFalconInit>());
                        }
                        PriorityToInstances[instance.GetPriority()].Add(instance);
                    }
                }
                catch (System.Exception e)
                {
                    FalconLogUtils.Error(e, "#ecbd77");
                }
        }

        private static IFalconInit GetFalconInitInstance(Type falconInitType)
        {
            IFalconInit instance;
            if (falconInitType.IsSubclassOf(typeof(MonoBehaviour)))
                if (FalconGameObjectUtils.GameObject.GetComponent(falconInitType) != null)
                    instance = (IFalconInit)FalconGameObjectUtils.GameObject.GetComponent(falconInitType);
                else
                    instance = FalconGameObjectUtils.GameObject.AddComponent(falconInitType) as IFalconInit;

            else
                instance = Activator.CreateInstance(falconInitType) as IFalconInit;

            return instance;
        }

        private static void CallInit()
        {
            FalconLogUtils.Info("Falcon Main start initializing", "#ecbd77");

            FalconThreadUtils.Execute(() =>
            {
                try
                {
                    foreach (var keyValuePair in PriorityToInstances)
                    foreach (var instance in keyValuePair.Value)
                        try
                        {
                            while (true)
                            {
                                if (instance.InitInMainThread())
                                {
                                
                                    FalconThreadUtils.OneTimeAction action = FalconThreadUtils.MainThread(() =>
                                    {
                                        instance.Init();
                                        FalconLogUtils.Info(instance.GetType() + " init complete", "#ecbd77");
                                    });
                                    
                                    while(!action.IsDone) Thread.Sleep(100);
                                }
                                else
                                {
                                    instance.Init();
                                    FalconLogUtils.Info(instance.GetType() + " init complete", "#ecbd77");
                                }
                                break;
                            }
                            
                        }
                        catch (System.Exception e)
                        {
                            FalconLogUtils.Error(e, "#ecbd77");
                        }

                    FalconLogUtils.Info("Initialize complete", "#ecbd77");
                    InitComplete = true;
                }
                catch (System.Exception e)
                {
                    FalconLogUtils.Error(e, "#ecbd77");
                    InitStarted = false;
                }

            });
        }
    }
}