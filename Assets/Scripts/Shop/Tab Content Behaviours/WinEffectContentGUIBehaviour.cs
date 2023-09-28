using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinEffectContentGUIBehaviour : TabContentGUIBehaviour
{
    public override void CreateItemInShop(ShopItemSO[] data)
    {
        foreach (var skin in data)
        {
            GameObject item = Instantiate<GameObject>(ShopItemPrefab, TargetTransforms);
            if (((WinEffectSO)skin).IsUnlock)
                item.GetComponent<Image>().sprite = ((WinEffectSO)skin).WinIcon;
        }
    }
}
