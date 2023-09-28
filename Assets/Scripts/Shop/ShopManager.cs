using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    //--------------------Datas----------------
    private PurchaseSkinSO[] _purchaseSkinsData;
    private RandomSkinSO[] _randomSkinsData;
    private TapEffectSO[] _tapEffectsData;
    private WinEffectSO[] _winEffectsData;

    private ShopItemSO SubcriberSO; //recognize last button want to "Observer pattern"

    private void Awake()
    {
        Instance = this;
        FetchData();
    }

    private void Update()
    {
        
    }

    private void FetchData()
    {
        _purchaseSkinsData = Resources.LoadAll<PurchaseSkinSO>("Shop Items/Skin Items/Purchase Skins");
        _randomSkinsData = Resources.LoadAll<RandomSkinSO>("Shop Items/Skin Items/Random Skins");
        _tapEffectsData = Resources.LoadAll<TapEffectSO>("Shop Items/Effect Items/Tap Effects");
        _winEffectsData = Resources.LoadAll<WinEffectSO>("Shop Items/Effect Items/Win Effects");

        foreach (var data in _purchaseSkinsData)
            data.LoadData();
        
        foreach (var data in _randomSkinsData)
            data.LoadData();

        foreach (var data in _tapEffectsData)
            data.LoadData();

        foreach (var data in _winEffectsData)
            data.LoadData();
    }

    public void FetchPurchaseSkinData(GameObject receiver)
    {
        receiver.GetComponent<TabGUIManager>()?.FetchPurchaseSkinDataFromSender(_purchaseSkinsData);
    }

    public void FetchRandomSkinData(GameObject receiver)
    {
        receiver.GetComponent<TabGUIManager>()?.FetchRandomSkinDataFromSender(_randomSkinsData);
    }
    
    public void FetchTapEffectData(GameObject receiver)
    {
        receiver.GetComponent<TabGUIManager>()?.FetchTapEffectDataFromSender(_tapEffectsData);
    }

    public void FetchWinEffectData(GameObject receiver)
    {
        Debug.Log(_winEffectsData);

        receiver.GetComponent<TabGUIManager>()?.FetchWinEffectDataFromSender(_winEffectsData);
    }

    public void Subcribe(ShopItemSO subcriber)
    {
        SubcriberSO = subcriber;
        ShopGUIManager.Instance.OnShopItemButtonClick(subcriber.IsUnlock);
    }    
}