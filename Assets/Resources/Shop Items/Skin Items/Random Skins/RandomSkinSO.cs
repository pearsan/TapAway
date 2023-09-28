using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSkin
{
    public string Name;
    public bool IsUnlock;
    public float Price;
}

[CreateAssetMenu(fileName = "Random skin SO", menuName = "Shop/Random Skin", order = 0)]
public class RandomSkinSO : ShopItemSO
{
    private const string RANDOM_SKIN_PATH = "Shop_RandomSkin_";

    public Sprite SkinIcon;
    public float Price;
    public GameObject CubePrefab;

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
                Price = Price
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
    }

    public void SaveData(RandomSkinSO skin)
    {
        RandomSkin randSkin = new RandomSkin { Name = skin.Name, Price = skin.Price, IsUnlock = skin.IsUnlock };

        string jsonData = JsonUtility.ToJson(randSkin);
        PlayerPrefs.SetString(RANDOM_SKIN_PATH + Name, jsonData);
        PlayerPrefs.Save();
    }
}