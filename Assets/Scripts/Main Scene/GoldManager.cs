using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance;

    public int GOLD_EARN_EACH_ADS;
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public int GetGold()
    {
        return PlayerPrefs.GetInt("Gold");
    }

    public string GetGoldString()
    {
        int gold = PlayerPrefs.GetInt("Gold");
        return gold.ToString("N0").Replace(",", ".");
    }

    public void ModifyGoldValue(int value)
    {
        int currentGold = PlayerPrefs.GetInt("Gold");
        PlayerPrefs.SetInt("Gold", currentGold + value);
    }

    /// <summary>
    /// This function only use for dev func, not use in complete build APK
    /// </summary>
    /// <param name="value"></param>
    public void HardModifyGoldValue (int value)
    {
        PlayerPrefs.SetInt("Gold", value);
    }    
}
