using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

[RequireComponent(typeof(TabContentGUIBehaviour))]
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
    [Space(10f)]
    [SerializeField] private GameObject ItemSelectedPrefab;
    

    [Header("Events")]
    [SerializeField] private UnityEvent FetchDataBehaviour;

    private Image _targetTab;
    private Image _targetIcon;

    private GameObject _itemSelectedFeedback;
    public TabContentGUIBehaviour _tabContentGUIBehaviour;

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
        _itemSelectedFeedback = Instantiate<GameObject>(ItemSelectedPrefab, transform);
        _itemSelectedFeedback.SetActive(false);
        OnUpdateItemSelectedFeedback();

        _tabContentGUIBehaviour = GetComponent<TabContentGUIBehaviour>();
    }    

    #region Tab behaviour
    /// <summary>
    /// Handle the tab selection event and perform a tween animation.
    /// </summary>
    /// <param name="IsAscending">If true, it will tween from left to right. If false, it will tween from right to left.</param>
    public void OnTabSelect(bool IsAscending, bool ActiveTween = true)
    {
        _targetTab.sprite = SelectedSprite;
        _targetIcon.color = ColorConverting(SELECTED_COLOR);
        GetComponent<RectTransform>().sizeDelta = new Vector2(240, 96);
        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = 3;

        TargetContents.SetActive(true);

        if (ActiveTween)
        {
            TargetContents.GetComponent<RectTransform>().anchoredPosition = new Vector3(1080 * (IsAscending ? 1 : -1), 0);
            TargetContents.transform.DOLocalMoveX(540, 0.2f);
        }
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
        tabContentBehaviours += () => GetComponent<PurchaseSkinContentGUIBehaviour>().CreateItemInShop(data);
        tabContentBehaviours.Invoke();
    }

    public void FetchRandomSkinDataFromSender(ShopItemSO[] data)
    {
        tabContentBehaviours = null;
        tabContentBehaviours += () => GetComponent<RandomSkinContentGUIBehaviour>().CreateItemInShop(data);
        tabContentBehaviours.Invoke();
    }    

    public void FetchTapEffectDataFromSender(ShopItemSO[] data)
    {
        tabContentBehaviours = null;
        tabContentBehaviours += () => GetComponent<TapEffectContentGUIBehaviour>().CreateItemInShop(data);
        tabContentBehaviours.Invoke();
    }

    public void FetchWinEffectDataFromSender(ShopItemSO[] data)
    {
        tabContentBehaviours = null;
        tabContentBehaviours += () => GetComponent<WinEffectContentGUIBehaviour>().CreateItemInShop(data);
        tabContentBehaviours.Invoke();
    }

    public void OnUpdateItemSelectedFeedback()
    {
        bool IsThisTabHasItemSelected = false;
        foreach(var item in _tabContentGUIBehaviour.ShopItems)
        {
            if(item.Item2.Name == ShopReadWriteData.Instance.GetEquippedEquipmentName(item.Item2))
            {
                _itemSelectedFeedback.SetActive(true);
                _itemSelectedFeedback.transform.SetParent(item.Item1.transform);
                _itemSelectedFeedback.GetComponent<RectTransform>().offsetMin = Vector2.zero;
                _itemSelectedFeedback.GetComponent<RectTransform>().offsetMax = Vector2.zero;
                Debug.Log(item.Item2.ShopItemPrefab);
                GameplayManager.Instance.ChangeCurrentSkin(item.Item2.ShopItemPrefab);
                IsThisTabHasItemSelected = true;
            }
        }
        if (!IsThisTabHasItemSelected)
            _itemSelectedFeedback.SetActive(false);
    }    
    #endregion
}
