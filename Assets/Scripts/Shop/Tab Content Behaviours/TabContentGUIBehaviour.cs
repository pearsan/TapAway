using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class TabContentGUIBehaviour : MonoBehaviour
{
    [Header("Contents")]
    [Tooltip("Transform where to instantiate shop item")]
    [SerializeField] protected Transform TargetTransforms;

    [Header("Prefabs")]
    [SerializeField] protected GameObject ShopItemPrefab;
    [Space(10f)]
    [SerializeField] protected Sprite AdsLockLayer;


    public List<(GameObject, ShopItemSO)> ShopItems;

    private void Awake()
    {
        ShopItems = new List<(GameObject, ShopItemSO)>();
    }

    private void Update()
    {
        UpdateIcon();
    }

    public virtual void UpdateIcon()
    {
        foreach (var item in ShopItems)
        {
            item.Item1.transform.GetChild(0).GetComponent<Image>().color = (item.Item2).IsUnlock ? new Color(1, 1, 1, 0) : new Color(0, 0, 0, 0.75f);
        }
    }    

    public virtual void CreateItemInShop(ShopItemSO[] data)
    {
        
    }
}
