using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TabContentGUIBehaviour : MonoBehaviour
{
    [Header("Contents")]
    [Tooltip("Transform where to instantiate shop item")]
    [SerializeField] protected Transform TargetTransforms;

    [Header("Prefabs")]
    [SerializeField] protected GameObject ShopItemPrefab;
    public virtual void CreateItemInShop(ShopItemSO[] data)
    {

    }    
}
