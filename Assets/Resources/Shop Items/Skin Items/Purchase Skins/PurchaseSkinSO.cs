using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseSkin
{
    public string Name;
    public bool IsUnlock;
    public float Price;
}

[CreateAssetMenu(fileName = "Purchase skin SO", menuName = "Shop/Purchase Skin", order = 0)]
public class PurchaseSkinSO : ShopItemSO
{
    private const string PURCHASE_SKIN_PATH = "Shop_PurchaseSkin_";

    public Sprite SkinIcon;
    public float Price;
    public GameObject CubePrefab;

    PurchaseSkin cube;
    public PurchaseSkin LoadData()
    {
        string jsonData = PlayerPrefs.GetString(PURCHASE_SKIN_PATH + Name);
        if(string.IsNullOrEmpty(jsonData))
        {
            cube = new PurchaseSkin
            {
                Name = Name,
                IsUnlock = false,
                Price = Price
            };

            string newItemData = JsonUtility.ToJson(cube);
            PlayerPrefs.SetString(PURCHASE_SKIN_PATH + Name, newItemData);
            Debug.Log($"Not exist so create {Name} first!");
        }

        jsonData = PlayerPrefs.GetString(PURCHASE_SKIN_PATH + Name);
        return JsonUtility.FromJson<PurchaseSkin>(jsonData);
    }

    public void SaveData(PurchaseSkinSO skin)
    {
        PurchaseSkin purcSkin = new PurchaseSkin { Name = skin.Name, Price = skin.Price, IsUnlock = skin.IsUnlock };

        string jsonData = JsonUtility.ToJson(purcSkin);
        PlayerPrefs.SetString(PURCHASE_SKIN_PATH + Name, jsonData);
        PlayerPrefs.Save();
    }    
}
