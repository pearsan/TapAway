// using System;
// using System.Threading;
// # if !UNITY_EDITOR
// using Falcon.FalconCore.Scripts.Exception;
// #endif
//
// namespace Falcon.FalconCore.Scripts.Utils
// {
//     public static class FalconFreezePreventor
//     {
//
//         private const long MaxWaitTimeTime = 1000;
//         
//         public static void Wait(Func<bool> waitTill, String waitingFor, int checkInterval = 10)
//         {
// # if !UNITY_EDITOR
//             long totalSleepTime = 0;
// #endif
//             while (!waitTill.Invoke())
//             {
//                 Thread.Sleep(checkInterval);
// # if !UNITY_EDITOR
//                 totalSleepTime += checkInterval;
//                 if (totalSleepTime > MaxWaitTimeTime)
//                 {
//                     throw new ThreadException(waitingFor + " not responding");
//                 }
// #endif
//             }
//         }
//
//         // public static void TryTillNotFreeze(Action action, int retryDuration = 1000)
//         // {
//         //     try
//         //     {
//         //         action.Invoke();
//         //     }
//         //     catch (NotRespondingException e)
//         //     {
//         //         FalconLogUtils.Warning(e);
//         //         ManualResetEvent resetEvent = new ManualResetEvent(false);
//         //         FalconGameObjectUtils.Trigger(() =>
//         //         {
//         //             FalconGameObjectUtils.DoCoroutine(Retry(action, retryDuration));
//         //         });
//         //     }
//         // }
//         //
//         // private static IEnumerator Retry(Action action, int retrySec = 1)
//         // {
//         //     while (FalconGameObjectUtils.ApplicationRunning)
//         //     {
//         //         try
//         //         {
//         //             action.Invoke();
//         //             yield break;
//         //         }
//         //         catch (NotRespondingException e)
//         //         {
//         //             FalconLogUtils.Warning(e);
//         //         }
//         //         catch (System.Exception e)
//         //         {
//         //             FalconLogUtils.Error(e, "#ecbd77");
//         //             yield break;
//         //         }
//         //         
//         //         yield return new WaitForSeconds(retrySec);
//         //     }
//         // }
//     }
// }