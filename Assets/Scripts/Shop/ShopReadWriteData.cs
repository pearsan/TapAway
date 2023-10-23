using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopReadWriteData : MonoBehaviour
{
    public static ShopReadWriteData Instance;

    private const string EQUIP_PATH = "Equipped_";

    private void Awake()
    {
        Instance = this;
    }

    public string GetEquippedEquipmentName(ShopItemSO input)
    {
        string TypeEquipInput = EQUIP_PATH + input.Type;
        //Debug.Log(TypeEquipInput + "/" + PlayerPrefs.GetString(TypeEquipInput));
        return PlayerPrefs.GetString(TypeEquipInput);
    }   
    
    public void SetEquippedEquipment()
    {
        string TypeEquipInput = EQUIP_PATH + ShopManager.Instance.SubcriberSO.Type;
        PlayerPrefs.SetString(TypeEquipInput, ShopManager.Instance.SubcriberSO.Name);

    }  
}
