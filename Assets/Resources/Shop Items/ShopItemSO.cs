using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItemSO : ScriptableObject
{
    [Header("Data")]
    public string Name;
    public bool IsUnlock;
    [HideInInspector] public int Price;
    public string Type; //This type will recognize string path of PlayerPrefs
    public GameObject ShopItemPrefab;

    [Header("Unlock types")]
    public bool CanUnlockByGold = true;
    public bool CanUnlockByAds = true;

    [HideInInspector] public int AdsWatched = 0;
    [HideInInspector] public int AdsToUnlock;
}
