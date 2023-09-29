using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapEffect
{
    public string Name;
    public bool IsUnlock;
    public float Price;
}

[CreateAssetMenu(fileName = "Tap effect SO", menuName = "Shop/Tap Effect", order = 1)]
public class TapEffectSO : ShopItemSO
{
    private const string TAP_EFFECT_PATH = "Shop_TapEffect_";

    public Sprite TapIcon;
    public float Price;
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
                Price = Price
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
    }

    public void SaveData(TapEffect tap)
    {
        TapEffect tapEffect = new TapEffect { Name = tap.Name, Price = tap.Price, IsUnlock = tap.IsUnlock };

        string jsonData = JsonUtility.ToJson(tapEffect);
        PlayerPrefs.SetString(TAP_EFFECT_PATH + Name, jsonData);
        PlayerPrefs.Save();
    }
}