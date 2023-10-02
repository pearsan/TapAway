using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinEffect
{
    public string Name;
    public bool IsUnlock;
    public int Price;
}

[CreateAssetMenu(fileName = "Win effect SO", menuName = "Shop/Win Effect", order = 1)]
public class WinEffectSO : ShopItemSO
{
    private const string Win_EFFECT_PATH = "Shop_WinEffect_";

    public Sprite WinIcon;
    public GameObject winPrefab;

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
                Price = Price
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
    }

    public void SaveData(WinEffectSO Win)
    {
        WinEffect winEffect = new WinEffect { Name = Win.Name, Price = Win.Price, IsUnlock = Win.IsUnlock };

        string jsonData = JsonUtility.ToJson(winEffect);
        PlayerPrefs.SetString(Win_EFFECT_PATH + Name, jsonData);
        PlayerPrefs.Save();
    }
}