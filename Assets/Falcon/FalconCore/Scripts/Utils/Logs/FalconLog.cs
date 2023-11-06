using System;
using System.Diagnostics;
using Falcon.FalconCore.Scripts.Utils.Singletons;
#if UNITY_EDITOR
using System.Reflection;
using Debug = UnityEngine.Debug;
#endif

namespace Falcon.FalconCore.Scripts.Utils.Logs
{
    public abstract class FalconLog<T> : Singleton<T> where T : FalconLog<T>, new()
    {
        [Conditional("UNITY_EDITOR")]
        [Conditional("FALCON_LOG_DEBUG")]
        public void Info(object info)
        {
#if UNITY_EDITOR || FALCON_LOG_DEBUG
            var color = Instance.GetColor();
            Debug.Log(color != null ? $"<color={color}> {info} </color>" : info);
#endif
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("FALCON_LOG_DEBUG")]
        public void Warning(object info)
        {
#if UNITY_EDITOR || FALCON_LOG_DEBUG
            var exception = info as AggregateException;
            if (exception != null)
            {
                foreach (var innerException in exception.InnerExceptions) Warning(innerException);
                return;
            }

            var invocationException = info as TargetInvocationException;
            if (invocationException != null)
            {
                Warning(invocationException.InnerException);
                return;
            }

            var color = Instance.GetColor();
            Debug.LogWarning(color != null ? $"<color={color}> {info} </color>" : info);
#endif
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("FALCON_LOG_DEBUG")]
        public void Error(object exception)
        {
#if UNITY_EDITOR || FALCON_LOG_DEBUG
            var aggregateException = exception as AggregateException;
            if (aggregateException != null)
            {
                foreach (var innerException in aggregateException.InnerExceptions) Error(innerException);
                return;
            }

            var invocationException = exception as TargetInvocationException;
            if (invocationException != null)
            {
                Error(invocationException.InnerException);
                return;
            }

            var color = Instance.GetColor();
            Debug.LogError($"<color={color}> {exception} </color>");
#endif
        }

        protected abstract string GetColor();
    }
}