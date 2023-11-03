using UnityEngine;

public static class MLogger
{
    public static void Log(params string[] values)
    {
#if ENABLE_LOGGER || UNITY_EDITOR
        Debug.Log(GetFinalString(values));
#endif
    }

    public static void LogError(params string[] values)
    {
#if ENABLE_LOGGER || UNITY_EDITOR
        Debug.LogError(GetFinalString(values));
#endif
    }

    public static void LogWarning(params string[] values)
    {
#if ENABLE_LOGGER || UNITY_EDITOR
        Debug.LogWarning(GetFinalString(values));
#endif
    }

    private static string GetFinalString(params string[] values)
    {
        string finalStr = null;
        foreach (var str in values)
        {
            if (finalStr is null)
            {
                finalStr = str;
                continue;
            }
            finalStr += ", " + str;
        }

        return finalStr;
    }
}
