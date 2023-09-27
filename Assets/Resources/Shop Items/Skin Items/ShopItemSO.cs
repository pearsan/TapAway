using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItemSO : ScriptableObject
{
    public string Name;
    [HideInInspector] public bool IsUnlock;
}
