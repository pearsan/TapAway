using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopGUIManager : MonoBehaviour
{
    private List<TabGUIManager> tabGUIManagers;

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
            tab.GetComponent<Button>().onClick.AddListener(OnTapClick);
        }

        FirstTabSelectedFromStart();
    }

    public void OnTapClick()
    {
        foreach(var tab in tabGUIManagers)
        {
            if (tab.gameObject == EventSystem.current.currentSelectedGameObject)
                tab.OnTabSelect(true);
            else
                tab.OnTabUnselect();
        }    
    }    

    private void FirstTabSelectedFromStart()
    {
        foreach (var tab in tabGUIManagers)
        {
            if (tab == tabGUIManagers[0])
                tab.OnTabSelect(false);
            else
                tab.OnTabUnselect();
        }
    }    
}
