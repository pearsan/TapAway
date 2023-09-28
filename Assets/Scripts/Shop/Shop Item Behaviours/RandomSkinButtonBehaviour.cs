using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomSkinButtonBehaviour : MonoBehaviour
{
    public RandomSkinSO randomSkinSO;

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
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClickButton);
    }   
    
    private void UpdateInteractableState()
    {
        //button.interactable = (randomSkinSO.IsUnlock ? true : false);
    }

    public void OnClickButton()
    {
        ShopManager.Instance.Subcribe(randomSkinSO);
    }    
}
