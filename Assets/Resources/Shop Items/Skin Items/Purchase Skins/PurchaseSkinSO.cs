using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseSkin
{
    public string Name;
    public bool IsUnlock;
    public int Price;

    public bool CanUnlockByGold;
    public bool CanUnlockByAds;

    [HideInInspector] public int AdsWatched;
    public int AdsToUnlock;
}

[CreateAssetMenu(fileName = "Purchase skin SO", menuName = "Shop/Purchase Skin", order = 0)]
public class PurchaseSkinSO : ShopItemSO
{
    private const string PURCHASE_SKIN_PATH = "Shop_PurchaseSkin_";

    [Header("Inheritance parameters")]
    public Sprite SkinIcon;

    private const string TYPE = "Skin";

    PurchaseSkin cube;
    public void LoadData()
    {
        string jsonData = PlayerPrefs.GetString(PURCHASE_SKIN_PATH + Name);
        if(string.IsNullOrEmpty(jsonData))
        {
            cube = new PurchaseSkin
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
            PlayerPrefs.SetString(PURCHASE_SKIN_PATH + Name, newItemData);
            Debug.Log($"Not exist so create {Name} first!");
        }

        jsonData = PlayerPrefs.GetString(PURCHASE_SKIN_PATH + Name);

        PurchaseSkin purchaseSkin = JsonUtility.FromJson<PurchaseSkin>(jsonData);
        Name = purchaseSkin.Name;
        Price = purchaseSkin.Price;
        IsUnlock = purchaseSkin.IsUnlock;
        Type = TYPE;

        CanUnlockByAds = purchaseSkin.CanUnlockByAds;
        CanUnlockByGold = purchaseSkin.CanUnlockByGold;

        AdsWatched = purchaseSkin.AdsWatched;
        AdsToUnlock = purchaseSkin.AdsToUnlock;
    }

    public void SaveData(PurchaseSkinSO skin)
    {
        PurchaseSkin purcSkin = new PurchaseSkin { Name = skin.Name, Price = skin.Price, IsUnlock = skin.IsUnlock, CanUnlockByAds = skin.CanUnlockByAds, CanUnlockByGold = skin.CanUnlockByGold, AdsToUnlock = skin.AdsToUnlock, AdsWatched = skin.AdsWatched };

        string jsonData = JsonUtility.ToJson(purcSkin);
        PlayerPrefs.SetString(PURCHASE_SKIN_PATH + Name, jsonData);
        PlayerPrefs.Save();
    }    
}
