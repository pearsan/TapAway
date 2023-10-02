using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItemSO : ScriptableObject
{
    public string Name;
    public bool IsUnlock;
    public int Price;
    public string Type; //This type will recognize string path of PlayerPrefs
}
