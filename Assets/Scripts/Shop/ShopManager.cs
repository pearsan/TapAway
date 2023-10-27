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

    /// <summary>
    ///  Recognize last button want to "Observer pattern"
    /// </summary>
    [HideInInspector] public ShopItemSO SubcriberSO;

    private void Awake()
    {
        Instance = this;
        FetchData();
    }

    private void Update()
    {
        
    }

    #region Data Read/Write
    private void FetchData()
    {
        _purchaseSkinsData = Resources.LoadAll<PurchaseSkinSO>("Shop Items/Skin Items/Purchase Skins");
        _randomSkinsData = Resources.LoadAll<RandomSkinSO>("Shop Items/Skin Items/Random Skins");
        _tapEffectsData = Resources.LoadAll<TapEffectSO>("Shop Items/Effect Items/Tap Effects");
        _winEffectsData = Resources.LoadAll<WinEffectSO>("Shop Items/Effect Items/Win Effects");

        //SaveData();
        LoadData();

        _randomSkinsData[5].IsUnlock = true;
        _randomSkinsData[3].IsUnlock = true;
        _purchaseSkinsData[0].IsUnlock = true;
        _tapEffectsData[0].IsUnlock = true;
        _tapEffectsData[2].IsUnlock = true;
        _winEffectsData[0].IsUnlock = true;
    }

    private void SaveData()
    {
        foreach (var data in _purchaseSkinsData)
            data.SaveData(data);

        foreach (var data in _randomSkinsData)
            data.SaveData(data);

        foreach (var data in _tapEffectsData)
            data.SaveData(data);

        foreach (var data in _winEffectsData)
            data.SaveData(data);
    }    
    private void LoadData()
    {
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
        receiver.GetComponent<TabGUIManager>()?.FetchWinEffectDataFromSender(_winEffectsData);
    }


    public void MarkShopItemIsUnlock()
    {
        SubcriberSO.IsUnlock = true;
        SaveData();
        Subcribe(SubcriberSO);
        GameUIManager.Instance.OnGachaFeedbackAnimation(SubcriberSO);
    }

    public void MarkShopItemIsUnlock(ShopItemSO target)
    {
        target.IsUnlock = true;
        SaveData();
    }    

    public void SetSubcriberSOAdsUnlockProgressSuccess()
    {
        int adsWatched = SubcriberSO.AdsWatched;
        SubcriberSO.AdsWatched = adsWatched + 1;
        if (SubcriberSO.AdsWatched >= SubcriberSO.AdsToUnlock)
        {
            SubcriberSO.IsUnlock = true;
            GameUIManager.Instance.OnGachaFeedbackAnimation(SubcriberSO);
        }
        SaveData();
        Subcribe(SubcriberSO);
    }    
    #endregion


    /// <summary>
    /// 
    /// </summary>
    /// <param name="subcriber"></param>
    /// <param name="FeedbackToPlayer">If this bool set to false, player don't know feedback of their choice</param>
    public void Subcribe(ShopItemSO subcriber, bool FeedbackToPlayer = true)
    {
        SubcriberSO = subcriber;
        ShopGUIManager.Instance.OnShopItemButtonClick(subcriber);

        if(FeedbackToPlayer)
            ShopGUIManager.Instance.EquipButtonBehaviour.OnStartFeedbackToPlayer(subcriber);
    }

    public void Subcribe()
    {
        ShopGUIManager.Instance.OnShopItemButtonClick(SubcriberSO);
        ShopGUIManager.Instance.EquipButtonBehaviour.OnStartFeedbackToPlayer(SubcriberSO);
    }
}
