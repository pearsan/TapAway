using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TapEffectContentGUIBehaviour : TabContentGUIBehaviour
{
    public override void CreateItemInShop(ShopItemSO[] data)
    {
        foreach (var skin in data)
        {
            GameObject item = Instantiate<GameObject>(ShopItemPrefab, TargetTransforms);
            if (((TapEffectSO)skin).IsUnlock)
                item.GetComponent<Image>().sprite = ((TapEffectSO)skin).TapIcon;
        }
    }
}
