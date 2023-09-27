using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ShopGUIManager : MonoBehaviour
{
    public static ShopGUIManager Instance;

    [SerializeField] private TMP_Text HeadingText;

    private List<TabGUIManager> tabGUIManagers;

    private TabGUIManager lastTabSelected;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Initialize();

        PurchaseSkinSO[] purchaseSkins = Resources.LoadAll<PurchaseSkinSO>("Shop Items/Skin Items/Purchase Skins");
        foreach(var skin in purchaseSkins)
        {
            Debug.Log(skin.LoadData().Name);
        }    
    }

    private void Initialize()
    {
        tabGUIManagers = new List<TabGUIManager>();
        tabGUIManagers = new List<TabGUIManager>(GetComponentsInChildren<TabGUIManager>());

        foreach(var tab in tabGUIManagers)
        {
            tab.GetComponent<Button>().onClick.AddListener(OnTapClick);
        }

        FirstTabSelectedFromStart();
    }

    //---------------------------------Text Behaviour------------------------------------
    public void OnUpdateHeadingText(string newText)
    {
        HeadingText.text = newText;
    }    
    //-----------------------------------------------------------------------------------


    //---------------------------------Button Behaviour----------------------------------
    public void OnTapClick()
    {
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
    //-----------------------------------------------------------------------------------

    private void FirstTabSelectedFromStart()
    {
        foreach (var tab in tabGUIManagers)
        {
            if (tab == tabGUIManagers[0])
            {
                lastTabSelected = tab;
                tab.OnTabSelect(false);
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
