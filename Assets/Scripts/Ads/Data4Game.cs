using System;
using Falcon;
using Falcon.FalconAnalytics.Scripts.Enum;
using Falcon.FalconCore.Scripts;
using UnityEngine;

public class Data4Game : MonoBehaviour
{
    private void Start()
    {
        FalconMain.Init();
    }

    public static void LevelLog(int level, TimeSpan duration, int wave, string difficulty, LevelStatus status)
    {
        if(!FalconMain.InitComplete) return;
        DWHLog.Log.LevelLog(level, duration, wave, difficulty, status);
    }

    public static void AdsLog(int maxPassedLevel, AdType type, GameMode mode, string adWhere)
    {
        if(!FalconMain.InitComplete) return;
        DWHLog.Log.AdsLog(maxPassedLevel, type, mode + "_" + adWhere);
    }

    public static void ResourceLog(int level, FlowType flowType, string itemType, string itemId, string currency, int amount)
    {
        if (!FalconMain.InitComplete) return;
        DWHLog.Log.ResourceLog(level, flowType, itemType, itemId, currency, amount);
    }
}
