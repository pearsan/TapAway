using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    //--------------------Datas----------------
    private PurchaseSkinSO[] _purchaseSkinsData;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        _purchaseSkinsData = Resources.LoadAll<PurchaseSkinSO>("Shop Items/Skin Items/Purchase Skins");
    }

    public void FetchPurchaseSkinData(GameObject receiver)
    {
        receiver.GetComponent<TabGUIManager>()?.FetchPurchaseSkinDataFromSender(_purchaseSkinsData);
    }
}
