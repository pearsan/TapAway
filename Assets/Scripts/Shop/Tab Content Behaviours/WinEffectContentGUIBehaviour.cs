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
            ShopItems.Add((item, win));
            
            item.GetComponent<Image>().sprite = ((WinEffectSO)win).WinIcon;
            item.transform.GetChild(0).GetComponent<Image>().sprite = win.CanUnlockByAds ? AdsLockLayer : GoldLockLayer;
        }
    }
}
