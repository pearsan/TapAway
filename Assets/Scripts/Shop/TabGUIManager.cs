using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class TabGUIManager : MonoBehaviour
{
    [SerializeField] private string TabName;

    [Space(20f)]

    //Attach this class to any tabs of button
    [Header("Sprites Transition")]
    [SerializeField] private Sprite UnselectedSprite;
    [SerializeField] private Sprite SelectedSprite;

    [Header("Contents")]
    [Tooltip("Content object from this tab when selected")]
    [SerializeField] private GameObject TargetContents;
    [Tooltip("Transform where to instantiate shop item")]
    [SerializeField] private Transform TargetTransforms;

    [Header("Prefabs")]
    [SerializeField] private GameObject ShopItemPrefab;

    [Header("Events")]
    [SerializeField] private UnityEvent FetchDataBehaviour;

    private Image _targetTab;
    private Image _targetIcon;

    private const string UNSELETECTED_COLOR = "#73778E";
    private const string SELECTED_COLOR = "#FFFFFF";

    private delegate void TabContentBehaviours();
    private TabContentBehaviours tabContentBehaviours;

    private void Awake()
    {
        _targetTab = GetComponent<Image>();
        _targetIcon = transform.Find("Icon").GetComponent<Image>();
    }

    private void Start()
    {
        FetchDataBehaviour.Invoke();
        Initialize();
    }

    private void Initialize()
    {
    }    

    #region Tab behaviour
    /// <summary>
    /// Handle the tab selection event and perform a tween animation.
    /// </summary>
    /// <param name="IsAscending">If true, it will tween from left to right. If false, it will tween from right to left.</param>
    public void OnTabSelect(bool IsAscending)
    {
        _targetTab.sprite = SelectedSprite;
        _targetIcon.color = ColorConverting(SELECTED_COLOR);
        GetComponent<RectTransform>().sizeDelta = new Vector2(240, 96);
        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = 3;

        TargetContents.SetActive(true);

        TargetContents.GetComponent<RectTransform>().anchoredPosition = new Vector3(1080 * (IsAscending ? -1 : 1), 0);
        TargetContents.transform.DOLocalMoveX(540, 0.1f);
    }    

    public void OnTabUnselect()
    {
        _targetTab.sprite = UnselectedSprite;
        _targetIcon.color = ColorConverting(UNSELETECTED_COLOR);
        GetComponent<RectTransform>().sizeDelta = new Vector2(240, 80);
        Destroy(gameObject.GetComponent<Canvas>());

        TargetContents.SetActive(false);
        /*TargetContents.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0);
        TargetContents.transform.DOLocalMoveX(1080 * (IsAscending ? -1 : 1), 0.1f);*/
    }


    private Color ColorConverting(string hexColor)
    {
        if (ColorUtility.TryParseHtmlString(hexColor, out Color color))
            return color;
        return color;
    }
    #endregion

    #region Tab contents behaviour
    public void FetchPurchaseSkinDataFromSender(ShopItemSO[] data)
    {
        tabContentBehaviours = null;
        tabContentBehaviours += () => CreatePurchaseSkinItemsInShop(data);
        tabContentBehaviours.Invoke();
    }
    #endregion

    #region Each type of contents behaviour (Purchase Skins, Random Skins, ...)
    private void CreatePurchaseSkinItemsInShop(ShopItemSO[] data)
    {
        foreach(var skin in data)
        {
            GameObject item = Instantiate<GameObject>(ShopItemPrefab, TargetTransforms);
            if(((PurchaseSkinSO)skin).IsUnlock)
                item.GetComponent<Image>().sprite = ((PurchaseSkinSO)skin).SkinIcon;
        }  
    }    
    #endregion
}
