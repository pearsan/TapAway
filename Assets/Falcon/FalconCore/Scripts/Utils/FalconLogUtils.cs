using System;
using Debug = UnityEngine.Debug;

namespace Falcon.FalconCore.Scripts.Utils
{
    public static class FalconLogUtils
    {
        private static bool _enableLog = true;

        public static bool EnableLog
        {
            get { return _enableLog; }
            set { _enableLog = value; }
        }

        public static void Info(object info, String color)
        {
            if (EnableLog)
            {
                if (color != null)
                {
                    Debug.Log(string.Format("<color={0}> {1} </color>", color, info));
                }
                else
                {
                    Debug.Log(info);
                }
            }
        }

        public static void Warning(object info, String color)
        {
            if (EnableLog)
            {
                if (color != null)
                {
                    Debug.LogWarning(string.Format("<color={0}> {1} </color>", color, info));
                }
                else
                {
                    Debug.LogWarning(info);
                }
            }
        }
        
        public static void Error(System.Exception exception, String color)
        {
            if (exception is AggregateException)
            {
                foreach (var innerException in ((AggregateException)exception).InnerExceptions) {
                    Error(innerException, color);}   
            }
            else
            {
                if (EnableLog)
                {
// #if UNITY_EDITOR
//                     throw exception;
// #else
                    if (color != null)
                    {
                        Debug.LogError(string.Format("<color={0}> {1} </color>", color, exception));
                    }
                    else
                    {
                        Debug.LogError(exception);
                    }
// #endif
                } 
            }
        }
    }
}