using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Falcon.FalconCore.Scripts.Utils
{
    public static class FalconThreadUtils
    {
        // private const int MaxPoolSize = 2;
        // private const int MinPoolSize = 1;
        // private static readonly TimeSpan KeepAliveTime = TimeSpan.FromSeconds(5);
        // private static readonly List<Thread> ThreadPool = new List<Thread>();
        
        private static readonly LimitQueue<ActionWarp> ActionQueue = new LimitQueue<ActionWarp>(Int32.MaxValue);

        private static readonly ConcurrentDictionary<Action, ScheduleAction> ScheduleMap =
            new ConcurrentDictionary<Action, ScheduleAction>();

        static FalconThreadUtils()
        {
            FalconGameObjectUtils.Instance.DoCoroutine(CheckAndExecute());
        }

        private static IEnumerator CheckAndExecute()
        {
            while (FalconGameObjectUtils.ApplicationRunning)
            {
                List<ActionWarp> actions = ActionQueue.DequeueAll();
                foreach (var action in actions)
                {
                    if (action.CanInvoke())
                    {
                        ThreadPool.QueueUserWorkItem(_ =>
                        {
                            action.Invoke();
                            action.OnFinish();
                        });
                    }
                    else
                    {
                        ActionQueue.Enqueue(action);
                    }
                }

                yield return new WaitForSeconds(1);
            }
        }
        
        public static OneTimeAction MainThread(Action action)
        {
            var oneTimeAction = new OneTimeAction(action);
            FalconGameObjectUtils.Instance.Trigger(() =>
            {
                oneTimeAction.Invoke();
                oneTimeAction.OnFinish();
            });
            // CreateThreadIfNeed();
            return oneTimeAction;
        }

        public static DelayAction MainThread(Action action, TimeSpan delay)
        {
            var delayAction = new DelayAction(() => MainThread(action), delay);
            ActionQueue.Enqueue(delayAction);
            // CreateThreadIfNeed();
            return delayAction;
        }

        public static OneTimeAction Execute(Action action)
        {
            var oneTimeAction = new OneTimeAction(action);
            ActionQueue.Enqueue(oneTimeAction);
            // CreateThreadIfNeed();
            return oneTimeAction;
        }

        public static DelayAction Execute(Action action, TimeSpan delay)
        {
            var delayAction = new DelayAction(action, delay);
            ActionQueue.Enqueue(delayAction);
            // CreateThreadIfNeed();
            return delayAction;
        }
        
        // private static void CreateNewThread()
        // {
        //     try
        //     {
        //         new Thread(() =>
        //         {
        //             lock (ThreadPool)
        //             {
        //                 ThreadPool.Add(Thread.CurrentThread);
        //             }
        //
        //             try
        //             {
        //                 long totalSleepMillis = 0;
        //                 while (FalconGameObjectUtils.ApplicationRunning)
        //                 {
        //                     int size = ActionQueue.Count;
        //                     bool didSomething = false;
        //                     for (int i = 0; i < size; i++)
        //                     {
        //                         ActionWarp action = ActionQueue.Dequeue();
        //                         if (action != null)
        //                         {
        //                             if (action.CanInvoke())
        //                             {
        //                                 action.Invoke();
        //                                 action.OnFinish();
        //                                 didSomething = true;
        //                             }
        //                             else
        //                             {
        //                                 ActionQueue.Enqueue(action);
        //                             }
        //                         }
        //                         if(ActionQueue.IsEmpty) break;
        //                     }
        //
        //                     if (didSomething)
        //                     {
        //                         totalSleepMillis = 0;
        //                     }
        //                     else 
        //                     {
        //                         lock (ThreadPool)
        //                         {
        //                             if(totalSleepMillis > KeepAliveTime.TotalMilliseconds && ThreadPool.Count > MinPoolSize) return;
        //                         }
        //                         Thread.Sleep(1000);
        //                         totalSleepMillis += 1000;
        //                     }
        //                 }
        //             }
        //             catch (System.Exception e)
        //             {
        //                 FalconLogUtils.Error(e, "#ecbd77");
        //             }
        //             finally
        //             {
        //                 lock (ThreadPool)
        //                 {
        //                     ThreadPool.Remove(Thread.CurrentThread);
        //                 }
        //             }
        //         }).Start();
        //     }
        //     catch (System.Exception e)
        //     {
        //         FalconLogUtils.Error(e, "#ecbd77");
        //     }
        // }
        
        // private static void CreateThreadIfNeed()
        // {
        //     if (ActionQueue.IsEmpty) return;
        //     lock (ThreadPool)
        //     {
        //         if (ThreadPool.Count < MaxPoolSize)
        //         {
        //             CreateNewThread();
        //         }
        //     }
        // }


        public static ScheduleAction Schedule(Action action, TimeSpan timeSpan)
        {
            try
            {
                if (ScheduleMap.ContainsKey(action))
                {
                    ScheduleMap[action].TimeSpan = timeSpan;
                }
                else
                {
                    ScheduleMap[action] = new ScheduleAction(action, timeSpan);
                    ActionQueue.Enqueue(ScheduleMap[action]);
                    // CreateThreadIfNeed();
                }
                return ScheduleMap[action];
            }
            catch (System.Exception)
            {
                FalconLogUtils.Warning("Schedule fail", "#ecbd77");
                return null;
            }
        }

        public abstract class ActionWarp
        {
            public readonly Action Action;

            protected ActionWarp(Action action)
            {
                Action = action;
                IsDone = false;
                Exception = null;
            }

            public bool IsDone { get; private set; }
            public System.Exception Exception { get; private set; }

            [Obsolete("Do not call this function outside the class FalconThreadUtils, the Action will be completed itself")]
            public void Invoke()
            {
                try
                {
                    Action.Invoke();
                }
                catch (System.Exception e)
                {
                    Exception = e;
                    FalconLogUtils.Warning(e, "#ecbd77");
                }

                IsDone = true;
            }

            public abstract bool CanInvoke();
            public abstract void OnFinish();
        }

        public class OneTimeAction : ActionWarp
        {
            public OneTimeAction(Action action) : base(action)
            {
            }

            public override bool CanInvoke()
            {
                return true;
            }

            public override void OnFinish()
            {
            }
        }

        public class DelayAction : ActionWarp
        {
            public readonly long CreateTime = FalconTimeUtils.CurrentTimeMillis();
            public readonly TimeSpan DelayTime;

            public DelayAction(Action action, TimeSpan delayTime) : base(action)
            {
                DelayTime = delayTime;
            }

            public override bool CanInvoke()
            {
                return CreateTime + DelayTime.TotalMilliseconds < FalconTimeUtils.CurrentTimeMillis();
            }

            public override void OnFinish()
            {
            }
        }

        public class ScheduleAction : ActionWarp
        {
            public ScheduleAction(Action action, TimeSpan timeSpan) : base(action)
            {
                TimeSpan = timeSpan;
                LastInvokeMillis = 0;
            }

            public TimeSpan TimeSpan { get; set; }

            public long LastInvokeMillis { get; private set; }

            public override bool CanInvoke()
            {
                return LastInvokeMillis + TimeSpan.TotalMilliseconds < FalconTimeUtils.CurrentTimeMillis();
            }

            public override void OnFinish()
            {
                LastInvokeMillis = FalconTimeUtils.CurrentTimeMillis();
                ActionQueue.Enqueue(this);
            }
        }
    }
}