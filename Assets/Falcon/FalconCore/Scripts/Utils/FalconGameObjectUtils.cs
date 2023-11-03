using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UnityEngine;


namespace Falcon.FalconCore.Scripts.Utils
{
    public class FalconGameObjectUtils : MonoBehaviour
    {
        private static FalconGameObjectUtils _instance;

        private static readonly LimitQueue<ActionRequest> WaitingAction = new LimitQueue<ActionRequest>(int.MaxValue);

        private static readonly ConcurrentDictionary<int, List<Action>> GameStopAction = new ConcurrentDictionary<int, List<Action>>();

        private static readonly ConcurrentBag<Type> GameStopAssigned = new ConcurrentBag<Type>();

        private static readonly ConcurrentDictionary<int, List<Action>> GameContinueAction = new ConcurrentDictionary<int, List<Action>>();

        private static readonly ConcurrentBag<Type> GameContinueAssigned = new ConcurrentBag<Type>();

        public bool GameStop { get; private set; }


        public static FalconGameObjectUtils Instance
        {
            get
            {
                if (_instance == null)
                {
                    var gObject = GameObject.Find("Falcon");
                    if (gObject == null) gObject = new GameObject("Falcon");

                    _instance = gObject.GetComponent<FalconGameObjectUtils>() == null
                        ? gObject.AddComponent<FalconGameObjectUtils>()
                        : gObject.GetComponent<FalconGameObjectUtils>();

                    _instance.enabled = true;
                }

                return _instance;
            }
        }

        public static GameObject GameObject
        {
            get { return Instance.gameObject; }
        }

        private static int? MainThread { get; set; }
        public static bool ApplicationRunning { get; private set; }

        private void Awake()
        {
            if (!Application.isPlaying)
            {
                if (Instance != this) DestroyImmediate(this);
            }
            else
            {
                if (Instance != this) Destroy(this);
            }

            try
            {
                GameStop = false;
                _instance = this;
                MainThread = Thread.CurrentThread.ManagedThreadId;
                ApplicationRunning = true;
                if (Application.isPlaying) DontDestroyOnLoad(gameObject);
            }
            catch (System.Exception e)
            {
                FalconLogUtils.Error(e, "#ecbd77");
            }
        }

        public void Update()
        {
            try
            {
                if (!WaitingAction.IsEmpty)
                {
                    var request = WaitingAction.Dequeue();
                    if (request != null)
                        try
                        {
                            request.MainAction.Invoke();
                        }
                        catch (System.Exception e)
                        {
                            if (request.ErrorCallBack != null) request.ErrorCallBack.Invoke(e);
                        }
                }
            }
            catch (System.Exception e)
            {
                FalconLogUtils.Error(e, "#ecbd77");
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (MainThread == null) MainThread = Thread.CurrentThread.ManagedThreadId;

            if (Application.isPlaying && !hasFocus)
            {
                if (!GameStop)
                    GameStop = true;
                else
                    return;

                FalconLogUtils.Info("On Game Stop", "#ecbd77");
                foreach (var entry in new SortedDictionary<int, List<Action>>(GameStopAction))
                foreach (var action in new List<Action>(entry.Value))
                    try
                    {
                        action.Invoke();
                    }
                    catch (System.Exception e)
                    {
                        FalconLogUtils.Error(e, "#ecbd77");
                    }
            }
            else if (Application.isPlaying)
            {
                if (GameStop)
                    GameStop = false;
                else
                    return;
                FalconLogUtils.Info("On Game Continue", "#ecbd77");

                foreach (var entry in new SortedDictionary<int, List<Action>>(GameContinueAction))
                foreach (var action in new List<Action>(entry.Value))
                    try
                    {
                        action.Invoke();
                    }
                    catch (System.Exception e)
                    {
                        FalconLogUtils.Error(e, "#ecbd77");
                    }
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (MainThread == null) MainThread = Thread.CurrentThread.ManagedThreadId;

            if (Application.isPlaying && pauseStatus)
            {
                if (!GameStop)
                    GameStop = true;
                else
                    return;
                // ApplicationRunning = false;
                FalconLogUtils.Info("On Game Stop", "#ecbd77");

                foreach (var entry in new SortedDictionary<int, List<Action>>(GameStopAction))
                foreach (var action in new List<Action>(entry.Value))
                    try
                    {
                        action.Invoke();
                    }
                    catch (System.Exception e)
                    {
                        FalconLogUtils.Error(e, "#ecbd77");
                    }
            }
            else if (Application.isPlaying)
            {
                if (GameStop)
                    GameStop = false;
                else
                    return;
                // ApplicationRunning = true;
                FalconLogUtils.Info("On Game Continue", "#ecbd77");

                foreach (var entry in new SortedDictionary<int, List<Action>>(GameContinueAction))
                foreach (var action in new List<Action>(entry.Value))
                    try
                    {
                        action.Invoke();
                    }
                    catch (System.Exception e)
                    {
                        FalconLogUtils.Error(e, "#ecbd77");
                    }
            }
        }

        private void OnApplicationQuit()
        {
            if (MainThread == null) MainThread = Thread.CurrentThread.ManagedThreadId;
            ApplicationRunning = false;

            if (!GameStop)
                GameStop = true;
            else
                return;

            FalconLogUtils.Info("On Game Quit", "#ecbd77");

            foreach (var entry in GameStopAction)
            foreach (var action in new List<Action>(entry.Value))
                try
                {
                    action.Invoke();
                }
                catch (System.Exception e)
                {
                    FalconLogUtils.Error(e, "#ecbd77");
                }
        }
        
        public void DoCoroutine(IEnumerator enumerator)
        {
            Trigger(() => gameObject.GetComponent<FalconGameObjectUtils>().StartCoroutine(enumerator));
        }

        public void Trigger(Action action, Action<System.Exception> onFail = null)
        {
            try
            {
                WaitingAction.Enqueue(new ActionRequest(action, onFail));
            }
            catch (System.Exception e)
            {
                if (onFail != null)
                    onFail.Invoke(e);
                else
                    FalconLogUtils.Error(e, "#ecbd77");
            }
        }

        public void AddMonoBehaviourIfNotExist<T>() where T : MonoBehaviour
        {
            Trigger(() =>
            {
                if (gameObject.GetComponent<T>() == null) gameObject.AddComponent<T>();
            });
        }

        public void OnGameStop(int priority, Action gameStopAction)
        {
            var callingClass = CallingClass();
            if (!GameStopAssigned.Contains(callingClass))
            {
                GameStopAssigned.Add(callingClass);
                GameStopAction.TryAdd(priority, new List<Action>());
                GameStopAction[priority].Add(gameStopAction);
            }
        }

        public void OnGameContinue(int priority, Action gameStopAction)
        {
            var callingClass = CallingClass();
            if (!GameContinueAssigned.Contains(callingClass))
            {
                GameContinueAssigned.Add(callingClass);
                GameContinueAction.TryAdd(priority, new List<Action>());
                GameContinueAction[priority].Add(gameStopAction);
            }
        }

        private static Type CallingClass()
        {
            var mth = new StackTrace().GetFrame(2).GetMethod();
            if (mth != null && mth.ReflectedType != null)
                return mth.ReflectedType;
            return null;
        }

        private class ActionRequest
        {
            public ActionRequest(Action mainAction, Action<System.Exception> errorCallBack)
            {
                MainAction = mainAction;
                ErrorCallBack = errorCallBack;
            }

            public Action MainAction { get; private set; }
            public Action<System.Exception> ErrorCallBack { get; private set;}
        }
    }
}