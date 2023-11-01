using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemButtonBehaviour : MonoBehaviour
{
    public ShopItemSO shopItemSO;

    private Button button;

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        UpdateInteractableState();
    }

    private void Initialize()
    {
        button = GetComponentInChildren<Button>();
        button.onClick.AddListener(OnClickButton);
    }   
    
    private void UpdateInteractableState()
    {
        if(shopItemSO is RandomSkinSO)
        {
            button.interactable = shopItemSO.IsUnlock ? true : false;
        }    
    }

    public void OnClickButton()
    { 
        ShopManager.Instance.Subcribe(shopItemSO);
        if (shopItemSO is RandomSkinSO)
        {
            ShopGUIManager.Instance.ChooseNewRandomSkin();
        }
    }    
}
