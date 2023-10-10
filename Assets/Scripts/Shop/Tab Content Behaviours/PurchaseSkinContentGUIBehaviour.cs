using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseSkinContentGUIBehaviour : TabContentGUIBehaviour
{
    public override void CreateItemInShop(ShopItemSO[] data)
    {
        foreach (var skin in data)
        {
            GameObject item = Instantiate<GameObject>(ShopItemPrefab, TargetTransforms);
            ShopItemButtonBehaviour shopItemButtonBehaviour = item.AddComponent<ShopItemButtonBehaviour>();
            shopItemButtonBehaviour.shopItemSO = skin;
            ShopItems.Add((item, skin));

            item.GetComponent<Image>().sprite = ((PurchaseSkinSO)skin).SkinIcon;
            item.transform.GetChild(0).GetComponent<Image>().sprite = skin.CanUnlockByAds ? AdsLockLayer : GoldLockLayer;       
        }
    }
}
