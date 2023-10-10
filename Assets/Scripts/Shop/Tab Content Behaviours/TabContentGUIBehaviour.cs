using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class TabContentGUIBehaviour : MonoBehaviour
{
    [Header("Contents")]
    [Tooltip("Transform where to instantiate shop item")]
    [SerializeField] protected Transform TargetTransforms;

    [Header("Prefabs")]
    [SerializeField] protected GameObject ShopItemPrefab;
    [Space(10f)]
    [SerializeField] protected Sprite AdsLockLayer;
    [SerializeField] protected Sprite GoldLockLayer;


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
            item.Item1.transform.GetChild(0).GetComponent<Image>().color = (item.Item2).IsUnlock ? new Color(1, 1, 1, 0) : new Color(1, 1, 1, 1f);

            if (item.Item2.IsUnlock || item.Item2.CanUnlockByAds == false)
                item.Item1.GetComponentInChildren<TMP_Text>().gameObject.SetActive(false);
            else
            {
                item.Item1.GetComponentInChildren<TMP_Text>().gameObject.SetActive(true);
                item.Item1.GetComponentInChildren<TMP_Text>().text = $"{item.Item2.AdsWatched}/{item.Item2.AdsToUnlock}";
            } 
                
        }
    }    

    public virtual void CreateItemInShop(ShopItemSO[] data)
    {
        
    }
}
