using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipButtonBehaviour : MonoBehaviour
{
    [Header("Sprites Transition")]
    [SerializeField] private Sprite EquippedSprite;
    [SerializeField] private Sprite EquipSprite;

    /// <summary>
    /// This function will feedback to player that equipment be selected or not
    /// </summary>
    /// <param name="itemPlayerSelected"></param>
    public void OnStartFeedbackToPlayer(ShopItemSO itemPlayerSelected)
    {
        string EquimentEquipped = ShopReadWriteData.Instance.GetEquippedEquipmentName(itemPlayerSelected);
        if (EquimentEquipped == itemPlayerSelected.Name)
        {
            GetComponent<Image>().sprite = EquippedSprite;
            GetComponent<Button>().interactable = false;
        }
        else
        {
            GetComponent<Image>().sprite = EquipSprite;
            GetComponent<Button>().interactable = true;
        } 
            
    }

    public void OnEquipItem()
    {
        ShopReadWriteData.Instance.SetEquippedEquipment();
        OnStartFeedbackToPlayer(ShopManager.Instance.SubcriberSO);
        ShopGUIManager.Instance.OnUpdateAllSelectedItemFeedbacks();
    }    
}
