using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class ShopGUIManager : MonoBehaviour
{
    public static ShopGUIManager Instance;

    [Tooltip("This game object control buy and equip button (SetActive)")]
    public GameObject BuyButton;
    [Tooltip("This game object control buy and equip button")]
    public GameObject EquipButton;

    [Header("Button Behaviours")]
    public EquipButtonBehaviour EquipButtonBehaviour;

    [Header("Currency Behaviours")]
    [SerializeField] private TMP_Text GoldText;
    [SerializeField] private TMP_Text ItemPriceText;

    private List<TabGUIManager> tabGUIManagers;

    private TabGUIManager lastTabSelected;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        UpdateGoldText();
    }

    private void Initialize()
    {
        tabGUIManagers = new List<TabGUIManager>();
        tabGUIManagers = new List<TabGUIManager>(GetComponentsInChildren<TabGUIManager>());

        foreach (var tab in tabGUIManagers)
        {
            tab.GetComponent<Button>().onClick.AddListener(OnTabClick);
        }

        FirstTabSelectedFromStart();
    }

    //---------------------------------Text Behaviour------------------------------------ 
    private void UpdateGoldText()
    {
        if (GoldManager.Instance != null)
            GoldText.text = GoldManager.Instance.GetGold().ToString();
        else
            Debug.LogWarning("Not singleton of Gold Manager");
    }
    //-----------------------------------------------------------------------------------


    #region Button behaviours
    public void OnTabClick()
    {
        BuyButton.SetActive(false);
        EquipButton.SetActive(false);
        foreach (var tab in tabGUIManagers)
        {
            if (tab.gameObject == EventSystem.current.currentSelectedGameObject)
            {
                bool temp = IsAscendingTab(lastTabSelected, tab);
                lastTabSelected = tab;
                tab.OnTabSelect(temp);
            }
            else
                tab.OnTabUnselect();
        }
    }
    public void OnBuyButtonClick()
    {
        if (ShopManager.Instance.SubcriberSO.Price <= GoldManager.Instance.GetGold())
        {
            GoldManager.Instance.ModifyGoldValue(-ShopManager.Instance.SubcriberSO.Price);
            ShopManager.Instance.MarkShopItemIsUnlock();
            ShopManager.Instance.Subcribe();
        }
        else
        {
            Debug.Log("Ban co the xem quang cao");
        }

    }

    public void OnAddGoldBuyADSButton()
    {
        ISHandler.Instance.ShowRewardedVideo("Add gold button", () => { GoldManager.Instance.ModifyGoldValue(200); }, () => { });
    }
    #endregion

    #region Script behaviours
    public void OnShopItemButtonClick(ShopItemSO shopItemSO)
    {
        if (shopItemSO.IsUnlock)
        {
            BuyButton.SetActive(false);
            EquipButton.SetActive(true);
        }
        else
        {
            BuyButton.SetActive(true);
            ItemPriceText.text = shopItemSO.Price.ToString();

            EquipButton.SetActive(false);
        }
    }
    #endregion
    //-----------------------------------------------------------------------------------

    public void OnUpdateAllSelectedItemFeedbacks()
    {
        foreach (var tab in tabGUIManagers)
            tab.OnUpdateItemSelectedFeedback();
    }

    private void FirstTabSelectedFromStart()
    {
        foreach (var tab in tabGUIManagers)
        {
            if (tab == tabGUIManagers[0])
            {
                lastTabSelected = tab;
                tab.OnTabSelect(false, false);
            }
            else
                tab.OnTabUnselect();
        }
    }

    private bool IsAscendingTab(TabGUIManager lastObject, TabGUIManager nextObject)
    {
        int lastIndex = tabGUIManagers.IndexOf(lastObject);
        int nextIndex = tabGUIManagers.IndexOf(nextObject);
        if (lastIndex < nextIndex)
            return true;
        else
            return false;
    }
}
