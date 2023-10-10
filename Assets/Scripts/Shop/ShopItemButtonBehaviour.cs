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
        //button.interactable = (randomSkinSO.IsUnlock ? true : false);
    }

    public void OnClickButton()
    {
        ShopManager.Instance.Subcribe(shopItemSO);
    }    
}
