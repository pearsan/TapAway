using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinEffectContentGUIBehaviour : TabContentGUIBehaviour
{
    public override void CreateItemInShop(ShopItemSO[] data)
    {
        foreach (var win in data)
        {
            GameObject item = Instantiate<GameObject>(ShopItemPrefab, TargetTransforms);
            ShopItemButtonBehaviour shopItemButtonBehaviour = item.AddComponent<ShopItemButtonBehaviour>();
            shopItemButtonBehaviour.shopItemSO = win;
            ShopItems.Add((item,win));
            if (((WinEffectSO)win).IsUnlock)
                item.GetComponent<Image>().sprite = ((WinEffectSO)win).WinIcon;
        }
    }
}
