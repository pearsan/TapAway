using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomSkinContentGUIBehaviour : TabContentGUIBehaviour
{
    public override void CreateItemInShop(ShopItemSO[] data)
    {
        foreach (var skin in data)
        {
            GameObject item = Instantiate<GameObject>(ShopItemPrefab, TargetTransforms);
            ShopItems.Add((item, skin));
            ShopItemButtonBehaviour shopItemButtonBehaviour = item.AddComponent<ShopItemButtonBehaviour>();
            shopItemButtonBehaviour.shopItemSO = skin;
            if (((RandomSkinSO)skin).IsUnlock)
                item.GetComponent<Image>().sprite = ((RandomSkinSO)skin).SkinIcon;
        }
    }
}
