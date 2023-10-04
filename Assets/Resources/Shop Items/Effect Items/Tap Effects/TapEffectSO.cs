using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapEffect
{
    public string Name;
    public bool IsUnlock;
    public int Price;

    public bool CanUnlockByGold;
    public bool CanUnlockByAds;

    [HideInInspector] public int AdsWatched;
    public int AdsToUnlock;
}

[CreateAssetMenu(fileName = "Tap effect SO", menuName = "Shop/Tap Effect", order = 1)]
public class TapEffectSO : ShopItemSO
{
    private const string TAP_EFFECT_PATH = "Shop_TapEffect_";

    [Header("Inheritance parameters")]
    public Sprite TapIcon;
    public GameObject tapPrefab;

    private const string TYPE = "TapEffect";

    TapEffect tap;
    public void LoadData()
    {
        string jsonData = PlayerPrefs.GetString(TAP_EFFECT_PATH + Name);
        if (string.IsNullOrEmpty(jsonData))
        {
            tap = new TapEffect
            {
                Name = Name,
                IsUnlock = false,
                Price = Price,
                
                CanUnlockByGold = CanUnlockByGold,
                CanUnlockByAds = CanUnlockByAds,

                AdsWatched = 0,
                AdsToUnlock = AdsToUnlock
            };

            string newItemData = JsonUtility.ToJson(tap);
            PlayerPrefs.SetString(TAP_EFFECT_PATH + Name, newItemData);
            Debug.Log($"Not exist so create {Name} first!");
        }

        jsonData = PlayerPrefs.GetString(TAP_EFFECT_PATH + Name);

        TapEffect tapEffect = JsonUtility.FromJson<TapEffect>(jsonData);
        Name = tapEffect.Name;
        Price = tapEffect.Price;
        IsUnlock = tapEffect.IsUnlock;
        Type = TYPE;

        CanUnlockByAds = tapEffect.CanUnlockByAds;
        CanUnlockByGold = tapEffect.CanUnlockByGold;

        AdsWatched = tapEffect.AdsWatched;
        AdsToUnlock = tapEffect.AdsToUnlock;
    }

    public void SaveData(TapEffectSO tap)
    {
        TapEffect tapEffect = new TapEffect { Name = tap.Name, Price = tap.Price, IsUnlock = tap.IsUnlock, CanUnlockByAds = tap.CanUnlockByAds, CanUnlockByGold = tap.CanUnlockByGold, AdsToUnlock = tap.AdsToUnlock, AdsWatched = tap.AdsWatched };

        string jsonData = JsonUtility.ToJson(tapEffect);
        PlayerPrefs.SetString(TAP_EFFECT_PATH + Name, jsonData);
        PlayerPrefs.Save();
    }
}