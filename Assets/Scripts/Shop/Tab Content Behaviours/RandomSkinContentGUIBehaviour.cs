using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomSkinContentGUIBehaviour : TabContentGUIBehaviour
{
    [SerializeField] private Sprite LockSprite;

    public override void CreateItemInShop(ShopItemSO[] data)
    {
        foreach (var skin in data)
        {
            GameObject item = Instantiate<GameObject>(ShopItemPrefab, TargetTransforms);
            ShopItems.Add((item, skin));
            ShopItemButtonBehaviour shopItemButtonBehaviour = item.AddComponent<ShopItemButtonBehaviour>();
            shopItemButtonBehaviour.shopItemSO = skin;

            item.GetComponent<Image>().sprite = ((RandomSkinSO)skin).IsUnlock ? ((RandomSkinSO)skin).SkinIcon : LockSprite;
        }
    }

    public override void UpdateIcon()
    {
        foreach (var item in ShopItems)
        {
            item.Item1.GetComponent<Image>().sprite = ((RandomSkinSO)item.Item2).IsUnlock ?((RandomSkinSO)item.Item2).SkinIcon : LockSprite ;
        }
    }
}
