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

    private void Initialize()
    {
        tabGUIManagers = new List<TabGUIManager>();
        tabGUIManagers = new List<TabGUIManager>(GetComponentsInChildren<TabGUIManager>());

        foreach(var tab in tabGUIManagers)
        {
            tab.GetComponent<Button>().onClick.AddListener(OnTabClick);
        }

        FirstTabSelectedFromStart();
    }

    //---------------------------------Text Behaviour------------------------------------ 
    //-----------------------------------------------------------------------------------


    //---------------------------------Button Behaviour----------------------------------
    public void OnTabClick()
    {
        BuyButton.SetActive(false);
        EquipButton.SetActive(false);
        foreach(var tab in tabGUIManagers)
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

    public void OnShopItemButtonClick(bool IsUnlock)
    {
        if(IsUnlock)
        {
            BuyButton.SetActive(false);
            EquipButton.SetActive(true);
        }
        else
        {
            BuyButton.SetActive(true);
            EquipButton.SetActive(false);
        }
    }    

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
