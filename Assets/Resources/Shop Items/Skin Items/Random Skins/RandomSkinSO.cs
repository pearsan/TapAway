using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSkin
{
    public string Name;
    public bool IsUnlock;
    public int Price;

    public bool CanUnlockByGold;
    public bool CanUnlockByAds;

    [HideInInspector] public int AdsWatched;
    public int AdsToUnlock;
}

[CreateAssetMenu(fileName = "Random skin SO", menuName = "Shop/Random Skin", order = 0)]
public class RandomSkinSO : ShopItemSO
{
    private const string RANDOM_SKIN_PATH = "Shop_RandomSkin_";

    [Header("Inheritance parameters")]
    public Sprite SkinIcon;

    private const string TYPE = "Skin"; 

    RandomSkin cube;
    public void LoadData()
    {
        string jsonData = PlayerPrefs.GetString(RANDOM_SKIN_PATH + Name);
        if (string.IsNullOrEmpty(jsonData))
        {
            cube = new RandomSkin
            {
                Name = Name,
                IsUnlock = false,
                Price = Price,

                CanUnlockByGold = CanUnlockByGold,
                CanUnlockByAds = CanUnlockByAds,

                AdsWatched = 0,
                AdsToUnlock = AdsToUnlock
            };

            string newItemData = JsonUtility.ToJson(cube);
            PlayerPrefs.SetString(RANDOM_SKIN_PATH + Name, newItemData);
            Debug.Log($"Not exist so create {Name} first!");
        }

        jsonData = PlayerPrefs.GetString(RANDOM_SKIN_PATH + Name);
        
        RandomSkin randomSkin = JsonUtility.FromJson<RandomSkin>(jsonData);
        Name = randomSkin.Name;
        Price = randomSkin.Price;
        IsUnlock = randomSkin.IsUnlock;
        Type = TYPE;

        CanUnlockByAds = randomSkin.CanUnlockByAds;
        CanUnlockByGold = randomSkin.CanUnlockByGold;

        AdsWatched = randomSkin.AdsWatched;
        AdsToUnlock = randomSkin.AdsToUnlock;
    }

    public void SaveData(RandomSkinSO skin)
    {
        RandomSkin randSkin = new RandomSkin { Name = skin.Name, Price = skin.Price, IsUnlock = skin.IsUnlock, CanUnlockByAds = skin.CanUnlockByAds, CanUnlockByGold = skin.CanUnlockByGold, AdsToUnlock = skin.AdsToUnlock, AdsWatched = skin.AdsWatched };

        string jsonData = JsonUtility.ToJson(randSkin);
        PlayerPrefs.SetString(RANDOM_SKIN_PATH + Name, jsonData);
        PlayerPrefs.Save();
    }
}
