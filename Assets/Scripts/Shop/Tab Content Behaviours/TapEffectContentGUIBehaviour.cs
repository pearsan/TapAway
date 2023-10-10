using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TapEffectContentGUIBehaviour : TabContentGUIBehaviour
{
    public override void CreateItemInShop(ShopItemSO[] data)
    {
        foreach (var tap in data)
        {
            GameObject item = Instantiate<GameObject>(ShopItemPrefab, TargetTransforms);
            ShopItemButtonBehaviour shopItemButtonBehaviour = item.AddComponent<ShopItemButtonBehaviour>();
            shopItemButtonBehaviour.shopItemSO = tap;
            ShopItems.Add((item, tap));
            
            item.GetComponent<Image>().sprite = ((TapEffectSO)tap).TapIcon;
            item.transform.GetChild(0).GetComponent<Image>().sprite = tap.CanUnlockByAds ? AdsLockLayer : GoldLockLayer;
        }
    }
}
