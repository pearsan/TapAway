using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class ShopGUIManager : MonoBehaviour
{
    public static ShopGUIManager Instance;

    [Header("Buy behaviours")]
    [Tooltip("This game object control buy and equip button (SetActive)")]
    [SerializeField] private GameObject BuyButton;   
    [SerializeField] private Button BuyByGoldButton;
    [SerializeField] private TMP_Text ItemPriceText;
    [Space(10f)]
    [SerializeField] private Button BuyByAdsButton;
    [SerializeField] private TMP_Text AdsPriceText;

    [Header("Equip behaviours")]
    [Tooltip("This game object control buy and equip button")]
    [SerializeField] private GameObject EquipButton;

    [Header("Button Behaviours")]
    public EquipButtonBehaviour EquipButtonBehaviour;

    [Header("Currency Behaviours")]
    [SerializeField] private TMP_Text GoldText;

    private List<TabGUIManager> tabGUIManagers;

    private TabGUIManager lastTabSelected;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Initialize();
        OnRandomTapClick();
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
        if(EventSystem.current.currentSelectedGameObject.GetComponent<RandomSkinContentGUIBehaviour>() != null)
        {
            OnRandomTapClick();
            return;
        }    

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

    private void OnRandomTapClick()
    {
        BuyButton.SetActive(false);
        EquipButton.SetActive(false);
        foreach (var tab in tabGUIManagers)
        {
            if (tab.gameObject == EventSystem.current.currentSelectedGameObject)
            {
                ShopItemButtonBehaviour randomSkin = OnRandomSkinToSubcribe(tab.GetComponent<RandomSkinContentGUIBehaviour>().TargetTransforms.GetComponentsInChildren<ShopItemButtonBehaviour>());

                if (randomSkin != null)
                    ShopManager.Instance.Subcribe(randomSkin.shopItemSO);

                bool temp = IsAscendingTab(lastTabSelected, tab);
                lastTabSelected = tab;
                tab.OnTabSelect(temp);
            }
            else
                tab.OnTabUnselect();
        }
    }

    private ShopItemButtonBehaviour OnRandomSkinToSubcribe(ShopItemButtonBehaviour[] behaviours)
    {
        List<ShopItemButtonBehaviour> itemLockeds = new List<ShopItemButtonBehaviour>();
        for (int i = 0; i < behaviours.Length; i++)
        {
            if (behaviours[i].shopItemSO.IsUnlock == false)
                itemLockeds.Add(behaviours[i]);
        }

        if (itemLockeds.Count == 0)
            return null;
        else
        {
            var randomIndex = Random.Range(0, itemLockeds.Count);
            return itemLockeds[randomIndex];
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

    public void OnBuyByAdsButtonClick()
    {
        ISHandler.Instance.ShowRewardedVideo("Buy by Ads button", ShopManager.Instance.SetSubcriberSOAdsUnlockProgressSuccess, () => { });

        ShopManager.Instance.Subcribe();
    }    

    public void OnAddGoldBuyADSButton()
    {
        ISHandler.Instance.ShowRewardedVideo("Add gold button", () => { GameUIManager.Instance.OnAddGoldFeedbackAnimation(GoldManager.Instance.GOLD_EARN_EACH_ADS); }, () => { });
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
            EquipButton.SetActive(false);

            ItemPriceText.text = (shopItemSO.CanUnlockByGold) ? shopItemSO.Price.ToString() : "Not available";
            BuyByGoldButton.interactable = (shopItemSO.CanUnlockByGold) ? true : false;

            AdsPriceText.text = (shopItemSO.CanUnlockByAds) ? $"By ads ({shopItemSO.AdsWatched}/{shopItemSO.AdsToUnlock})" : "Not available";
            BuyByAdsButton.interactable = (shopItemSO.CanUnlockByAds) ? true : false;
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
