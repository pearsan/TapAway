using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquipButtonBehaviour : MonoBehaviour
{
    public static EquipButtonBehaviour Instance;

    private void Awake()
    {
        Instance = this;
    }

    [Header("Sprites Transition")]
    [SerializeField] private Sprite EquippedSprite;
    [SerializeField] private Sprite EquipSprite;

    [Header("Texts")]
    [SerializeField] private TMP_Text EquipButtonText;

    /// <summary>
    /// This function will feedback to player that equipment be selected or not
    /// </summary>
    /// <param name="itemPlayerSelected"></param>
    public void OnStartFeedbackToPlayer(ShopItemSO itemPlayerSelected)
    {
        string EquipmentEquipped = ShopReadWriteData.Instance.GetEquippedEquipmentName(itemPlayerSelected);
        if (EquipmentEquipped == itemPlayerSelected.Name)
        {
            GetComponent<Image>().sprite = EquippedSprite;
            GetComponent<Button>().interactable = false;

            if(itemPlayerSelected.Type == "Skin")
                GameplayManager.Instance.ChangeCurrentSkin(itemPlayerSelected.ShopItemPrefab);
            EquipButtonText.text = "Equipped";
        }
        else
        {
            GetComponent<Image>().sprite = EquipSprite;
            GetComponent<Button>().interactable = true;
            EquipButtonText.text = "Equip";
        } 
            
    }

    public void OnEquipItem()
    {
        ShopReadWriteData.Instance.SetEquippedEquipment();
        OnStartFeedbackToPlayer(ShopManager.Instance.SubcriberSO);
        ShopGUIManager.Instance.OnUpdateAllSelectedItemFeedbacks();

        if (ShopManager.Instance.SubcriberSO.Type == "Skin")
            GameplayManager.Instance.ChangeCurrentSkin(ShopManager.Instance.SubcriberSO.ShopItemPrefab);
        else if (ShopManager.Instance.SubcriberSO.Type == "TapEffect")
            ClickEffect.Instance.ChangeEffect(ShopManager.Instance.SubcriberSO.ShopItemPrefab.GetComponent<ParticleSystem>());
        else if (ShopManager.Instance.SubcriberSO.Type == "WinEffect")
            WinEffectBehaviour.Instance.ChangeWinEffect(ShopManager.Instance.SubcriberSO);
    }    
}
