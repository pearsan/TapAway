using System;
using System.Collections.Generic;
using UnityEngine;

public class ISHandler : MonoBehaviour
{
    [SerializeField] private string androidKey;
    [SerializeField] private string iosKey;

    [Tooltip("If the banner fire onAdLoadFailedEvent then try again after this duration")] [SerializeField]
    private bool loadBanner = true, loadInter = true, loadRewarded = true;

#if UNITY_EDITOR
    [Header("Only works on editor")]
    [SerializeField]
    private bool _isRewardedSuccessed = true;
#endif


    private bool _isInitSuccessful, _rewardedHasClosed, _rewardedHasRewarded;
    private Action _onRewardedSuccess, _onRewardedFail;

    public static ISHandler Instance { get; private set; }

    protected void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;
            return;
        }

        DestroyImmediate(gameObject);
    }

    private void RegisterEvents()
    {
        IronSourceEvents.onImpressionDataReadyEvent += ImpressionDataReadyEvent; //Register the ImpressionData Listener

        if (loadBanner) RegisterBannerEvents(); //Add AdInfo Banner Events
        if (loadInter) RegisterInterstitialEvents(); //Add AdInfo Interstitial Events
        if (loadRewarded) RegisterRewardedEvents(); //Add AdInfo Rewarded Video Events
    }

    private void UnregisterEvents()
    {
        IronSourceEvents.onImpressionDataReadyEvent -=
            ImpressionDataReadyEvent; //Unregister the ImpressionData Listener
        IronSourceEvents.onSdkInitializationCompletedEvent -= SdkInitializationCompletedEvent;

        if (loadBanner) UnregisterBannerEvents(); //Remove AdInfo Banner Events
        if (loadInter) UnregisterInterstitialEvents(); //Remove Interstitial Events
        if (loadRewarded) UnregisterRewardedEvents(); //Remove RewardedVideo Events
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (!_isInitSuccessful) return;
        IronSource.Agent.onApplicationPause(pauseStatus);
    }

    public void Init(bool setConsent)
    {
        IronSource.Agent.validateIntegration();
        IronSource.Agent.setConsent(setConsent);
        IronSource.Agent.shouldTrackNetworkState(true);

#if UNITY_ANDROID
        if (loadBanner)
            IronSource.Agent.init(androidKey, IronSourceAdUnits.BANNER);
        if (loadInter)
            IronSource.Agent.init(androidKey, IronSourceAdUnits.INTERSTITIAL);
        if (loadRewarded)
            IronSource.Agent.init(androidKey, IronSourceAdUnits.REWARDED_VIDEO);
#elif UNITY_IPHONE
        if(loadBanner) 
            IronSource.Agent.init(iosKey, IronSourceAdUnits.BANNER);
        if(loadInter) 
            IronSource.Agent.init(iosKey, IronSourceAdUnits.INTERSTITIAL);
        if(loadRewarded)
            IronSource.Agent.init(iosKey, IronSourceAdUnits.REWARDED_VIDEO);
#endif

    }

    private void SdkInitializationCompletedEvent()
    {
        _isInitSuccessful = true;
        RegisterEvents();

        if (loadBanner) LoadBanner();

        //Recommend requesting an Interstitial Ad a short while before you plan on showing it to your users as the loading process can take time
        // If you’d like to serve several Interstitial in your app, you must repeat this step after you’ve shown and closed the previous Interstitial.
        // Once the InterstitialAdClosedEvent function is fired, you will be able to load a new Interstitial ad.
        if (loadInter) LoadInterstitial();
    }

    #region Banner

    private void RegisterBannerEvents()
    {
        IronSourceBannerEvents.onAdLoadedEvent += BannerOnAdLoadedEvent;
        IronSourceBannerEvents.onAdLoadFailedEvent += BannerOnAdLoadFailedEvent;
        IronSourceBannerEvents.onAdClickedEvent += BannerOnAdClickedEvent;
        IronSourceBannerEvents.onAdScreenPresentedEvent += BannerOnAdScreenPresentedEvent;
        IronSourceBannerEvents.onAdScreenDismissedEvent += BannerOnAdScreenDismissedEvent;
        IronSourceBannerEvents.onAdLeftApplicationEvent += BannerOnAdLeftApplicationEvent;
    }

    private void UnregisterBannerEvents()
    {
        IronSourceBannerEvents.onAdLoadedEvent -= BannerOnAdLoadedEvent;
        IronSourceBannerEvents.onAdLoadFailedEvent -= BannerOnAdLoadFailedEvent;
        IronSourceBannerEvents.onAdClickedEvent -= BannerOnAdClickedEvent;
        IronSourceBannerEvents.onAdScreenPresentedEvent -= BannerOnAdScreenPresentedEvent;
        IronSourceBannerEvents.onAdScreenDismissedEvent -= BannerOnAdScreenDismissedEvent;
        IronSourceBannerEvents.onAdLeftApplicationEvent -= BannerOnAdLeftApplicationEvent;
    }

    private void LoadBanner()
    {
        IronSource.Agent.loadBanner(IronSourceBannerSize.SMART, IronSourceBannerPosition.BOTTOM);
        IronSource.Agent.hideBanner();
    }

    public void ShowBanner()
    {
        if (!_isInitSuccessful) return;

        IronSource.Agent.displayBanner();
    }

    public void HideBanner()
    {
        if (!_isInitSuccessful) return;
        IronSource.Agent.hideBanner();
    }

    /************* Banner AdInfo Delegates *************/
    //Invoked once the banner has loaded
    private void BannerOnAdLoadedEvent(IronSourceAdInfo adInfo)
    {
#if! UA_BUILD
        ISHandler.Instance.ShowBanner();
#endif
    }

    //Invoked when the banner loading process has failed.
    private void BannerOnAdLoadFailedEvent(IronSourceError ironSourceError)
    {
        LoadBanner();
        ISHandler.Instance.ShowBanner();
    }

    // Invoked when end user clicks on the banner ad
    private void BannerOnAdClickedEvent(IronSourceAdInfo adInfo)
    {

    }

    //Notifies the presentation of a full screen content following user click
    private void BannerOnAdScreenPresentedEvent(IronSourceAdInfo adInfo)
    {

    }

    //Notifies the presented screen has been dismissed
    private void BannerOnAdScreenDismissedEvent(IronSourceAdInfo adInfo)
    {

    }

    //Invoked when the user leaves the app
    private void BannerOnAdLeftApplicationEvent(IronSourceAdInfo adInfo)
    {

    }

    #endregion

    #region Interstitial

    private void RegisterInterstitialEvents()
    {
        IronSourceInterstitialEvents.onAdReadyEvent += InterstitialOnAdReadyEvent;
        IronSourceInterstitialEvents.onAdLoadFailedEvent += InterstitialOnAdLoadFailed;
        IronSourceInterstitialEvents.onAdOpenedEvent += InterstitialOnAdOpenedEvent;
        IronSourceInterstitialEvents.onAdClickedEvent += InterstitialOnAdClickedEvent;
        IronSourceInterstitialEvents.onAdShowSucceededEvent += InterstitialOnAdShowSucceededEvent;
        IronSourceInterstitialEvents.onAdShowFailedEvent += InterstitialOnAdShowFailedEvent;
        IronSourceInterstitialEvents.onAdClosedEvent += InterstitialOnAdClosedEvent;
    }

    private void UnregisterInterstitialEvents()
    {
        IronSourceInterstitialEvents.onAdReadyEvent -= InterstitialOnAdReadyEvent;
        IronSourceInterstitialEvents.onAdLoadFailedEvent -= InterstitialOnAdLoadFailed;
        IronSourceInterstitialEvents.onAdOpenedEvent -= InterstitialOnAdOpenedEvent;
        IronSourceInterstitialEvents.onAdClickedEvent -= InterstitialOnAdClickedEvent;
        IronSourceInterstitialEvents.onAdShowSucceededEvent -= InterstitialOnAdShowSucceededEvent;
        IronSourceInterstitialEvents.onAdShowFailedEvent -= InterstitialOnAdShowFailedEvent;
        IronSourceInterstitialEvents.onAdClosedEvent -= InterstitialOnAdClosedEvent;
    }

    private void LoadInterstitial()
    {

        IronSource.Agent.loadInterstitial();
    }

    private bool IsInterstitialReady()
    {
        return _isInitSuccessful && IronSource.Agent.isInterstitialReady();
    }

    public void ShowInterstitial(string adsWhere)
    {
        if (!IsInterstitialReady()) return;
        IronSource.Agent.showInterstitial();
    }

    private Action interCall;

    public void ShowInterstitial(Action callback)
    {
        if (IsInterstitialReady())
        {
            interCall = callback;
            IronSource.Agent.showInterstitial();
        }
        else
        {
            if (callback != null)
            {
                callback.Invoke();
            }
        }
    }

    /************* Interstitial AdInfo Delegates *************/
    //Invoked when the interstitial ad was loaded successfully.
    private void InterstitialOnAdReadyEvent(IronSourceAdInfo adInfo)
    {

    }

    //Invoked when the initialization process has failed.
    private void InterstitialOnAdLoadFailed(IronSourceError ironSourceError)
    {
        Debug.Log("Trying to load intersitial ads again");
        LoadInterstitial();
        interCall?.Invoke();
    }

    //Invoked when the Interstitial Ad Unit has opened. This is the impression indication. 
    private void InterstitialOnAdOpenedEvent(IronSourceAdInfo adInfo)
    {

    }

    //Invoked when end user clicked on the interstitial ad
    private void InterstitialOnAdClickedEvent(IronSourceAdInfo adInfo)
    {

    }

    //Invoked when the ad failed to show.
    private void InterstitialOnAdShowFailedEvent(IronSourceError ironSourceError, IronSourceAdInfo adInfo)
    {
        Debug.Log("Failed to show intersitial ads, show again");
        LoadInterstitial();
        ShowInterstitial("");
        interCall?.Invoke();
    }

    //Invoked when the interstitial ad closed and the user went back to the application screen.
    private void InterstitialOnAdClosedEvent(IronSourceAdInfo adInfo)
    {
        LoadInterstitial();
        interCall?.Invoke();
    }

    //Invoked before the interstitial ad was opened, and before the InterstitialOnAdOpenedEvent is reported.
    //This callback is not supported by all networks, and we recommend using it only if  
    //it's supported by all networks you included in your build. 
    private void InterstitialOnAdShowSucceededEvent(IronSourceAdInfo adInfo)
    {

    }

    #endregion

    #region RewardedVideo

    private void RegisterRewardedEvents()
    {
        IronSourceRewardedVideoEvents.onAdOpenedEvent += RewardedVideoOnAdOpenedEvent;
        IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoOnAdClosedEvent;
        IronSourceRewardedVideoEvents.onAdAvailableEvent += RewardedVideoOnAdAvailable;
        IronSourceRewardedVideoEvents.onAdLoadFailedEvent += RewardedVideoOnAdFailedLoad;
        IronSourceRewardedVideoEvents.onAdUnavailableEvent += RewardedVideoOnAdUnavailable;
        IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoOnAdShowFailedEvent;
        IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
        IronSourceRewardedVideoEvents.onAdClickedEvent += RewardedVideoOnAdClickedEvent;
    }

    private void UnregisterRewardedEvents()
    {
        IronSourceRewardedVideoEvents.onAdOpenedEvent -= RewardedVideoOnAdOpenedEvent;
        IronSourceRewardedVideoEvents.onAdClosedEvent -= RewardedVideoOnAdClosedEvent;
        IronSourceRewardedVideoEvents.onAdAvailableEvent -= RewardedVideoOnAdAvailable;
        IronSourceRewardedVideoEvents.onAdUnavailableEvent -= RewardedVideoOnAdUnavailable;
        IronSourceRewardedVideoEvents.onAdShowFailedEvent -= RewardedVideoOnAdShowFailedEvent;
        IronSourceRewardedVideoEvents.onAdRewardedEvent -= RewardedVideoOnAdRewardedEvent;
        IronSourceRewardedVideoEvents.onAdClickedEvent -= RewardedVideoOnAdClickedEvent;
    }

    private void LoadRewardedVideo()
    {
        //It's automatically. If you want to load it manually:
        //Following the doc: https://developers.is.com/ironsource-mobile/unity/rewarded-video-manual-integration-unity/#step-3
    }

    public bool IsRewardedVideoReady()
    {
        return _isInitSuccessful && IronSource.Agent.isRewardedVideoAvailable();
    }

    public void ShowRewardedVideo(string adsWhere, Action onSuccess, Action onFail)
    {
        #if UNITY_EDITOR
            if(_isRewardedSuccessed)
                onSuccess?.Invoke();
            else
                onFail?.Invoke();;
        #endif
        
        if (!_isInitSuccessful) return;

        _onRewardedSuccess = onSuccess;
        _onRewardedFail = onFail;
        _rewardedHasClosed = false;
        _rewardedHasRewarded = false;
        IronSource.Agent.showRewardedVideo();
    }


    /************* RewardedVideo AdInfo Delegates *************/
    //Indicates that there’s an available ad.
    //The adInfo object includes information about the ad that was loaded successfully
    //This replaces the RewardedVideoAvailabilityChangedEvent(true) event
    private void RewardedVideoOnAdAvailable(IronSourceAdInfo adInfo)
    {

    }

    private void RewardedVideoOnAdFailedLoad(IronSourceError errorInfo)
    {
        Debug.Log("Failed to load rewarded video but trying to load again");
        IronSource.Agent.loadRewardedVideo();
    }

    //Indicates that no ads are available to be displayed
    //This replaces the RewardedVideoAvailabilityChangedEvent(false) event
    private void RewardedVideoOnAdUnavailable()
    {
        Debug.Log("No available ads to show but trying to load again");
        LoadRewardedVideo();
    }

    //The Rewarded Video ad view has opened. Your activity will loose focus.
    private void RewardedVideoOnAdOpenedEvent(IronSourceAdInfo adInfo)
    {
        var eventValues = new Dictionary<string, string>
        {
            { "af_rewarded_displayed", "af_rewarded_displayed" },
        };

    }

    //The Rewarded Video ad view is about to be closed. Your activity will regain its focus.
    //Note:  The onRewardedVideoAdRewardedEvent and onRewardedVideoAdClosedEvent are asynchronous.
    //Make sure to set up your listener to grant rewards even in cases where onRewardedVideoAdRewarded is fired after the onRewardedVideoAdClosedEvent.
    private void RewardedVideoOnAdClosedEvent(IronSourceAdInfo adInfo)
    {
        _rewardedHasClosed = true;
        CheckFireRewardedEvent();

    }

    //The user completed to watch the video, and should be rewarded.
    //The placement parameter will include the reward data.
    //When using server-to-server callbacks, you may ignore this event and wait for the ironSource server callback.
    private void RewardedVideoOnAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
    {
        _rewardedHasRewarded = true;
        CheckFireRewardedEvent();
    }

    private void CheckFireRewardedEvent()
    {
        if (!_rewardedHasClosed || !_rewardedHasRewarded) return;
        _onRewardedSuccess?.Invoke();
        _onRewardedSuccess = null;
    }

    //The rewarded video ad was failed to show.
    private void RewardedVideoOnAdShowFailedEvent(IronSourceError error, IronSourceAdInfo adInfo)
    {
        Debug.Log("Show failed, but trying to show again");
        ShowInterstitial("");
        _onRewardedFail?.Invoke();
        _onRewardedFail = null;
    }

    //Invoked when the video ad was clicked.
    //This callback is not supported by all networks, and we recommend using it only if
    //it’s supported by all networks you included in your build.
    private void RewardedVideoOnAdClickedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
    {

    }

    #endregion

    private void ImpressionDataReadyEvent(IronSourceImpressionData impressionData)
    {
        
    }
}