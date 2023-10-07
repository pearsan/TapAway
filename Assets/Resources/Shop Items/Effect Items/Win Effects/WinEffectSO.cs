using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinEffect
{
    public string Name;
    public bool IsUnlock;
    public int Price;

    public bool CanUnlockByGold;
    public bool CanUnlockByAds;

    [HideInInspector] public int AdsWatched;
    public int AdsToUnlock;
}

[CreateAssetMenu(fileName = "Win effect SO", menuName = "Shop/Win Effect", order = 1)]
public class WinEffectSO : ShopItemSO
{
    private const string Win_EFFECT_PATH = "Shop_WinEffect_";

    [Header("Inheritance parameters")]
    public Sprite WinIcon;

    private const string TYPE = "WinEffect";

    WinEffect win;
    public void LoadData()
    {
        string jsonData = PlayerPrefs.GetString(Win_EFFECT_PATH + Name);
        if (string.IsNullOrEmpty(jsonData))
        {
            win = new WinEffect
            {
                Name = Name,
                IsUnlock = false,
                Price = Price,

                CanUnlockByGold = CanUnlockByGold,
                CanUnlockByAds = CanUnlockByAds,

                AdsWatched = 0,
                AdsToUnlock = AdsToUnlock
            };

            string newItemData = JsonUtility.ToJson(win);
            PlayerPrefs.SetString(Win_EFFECT_PATH + Name, newItemData);
            Debug.Log($"Not exist so create {Name} first!");
        }

        jsonData = PlayerPrefs.GetString(Win_EFFECT_PATH + Name);

        WinEffect winEffect = JsonUtility.FromJson<WinEffect>(jsonData);
        Name = winEffect.Name;
        Price = winEffect.Price;
        IsUnlock = winEffect.IsUnlock;
        Type = TYPE;

        CanUnlockByAds = winEffect.CanUnlockByAds;
        CanUnlockByGold = winEffect.CanUnlockByGold;

        AdsWatched = winEffect.AdsWatched;
        AdsToUnlock = winEffect.AdsToUnlock;
    }

    public void SaveData(WinEffectSO Win)
    {
        WinEffect winEffect = new WinEffect { Name = Win.Name, Price = Win.Price, IsUnlock = Win.IsUnlock, CanUnlockByAds = Win.CanUnlockByAds, CanUnlockByGold = Win.CanUnlockByGold, AdsToUnlock = Win.AdsToUnlock, AdsWatched = Win.AdsWatched };

        string jsonData = JsonUtility.ToJson(winEffect);
        PlayerPrefs.SetString(Win_EFFECT_PATH + Name, jsonData);
        PlayerPrefs.Save();
    }
}