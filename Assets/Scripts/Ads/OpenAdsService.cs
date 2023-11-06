using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine;

public class OpenAdsService : MonoBehaviour
{
    public static bool isInitialized = false;
    
    private void Awake()
    {
        // Listen to application foreground and background events.
        AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
    }

    private void OnDestroy()
    {
        AppStateEventNotifier.AppStateChanged -= OnAppStateChanged;
    }

    void Start()
    {
        MobileAds.Initialize(initStatus =>
        {
            isInitialized = true;
            AppOpenAdManager.Instance.LoadAd();
            //NativeAdsService.RequestNativeAd();
        });
    }
    
    // In order to be notified of app foregrounding events, we recommend listening to the AppStateEventNotifier singleton.
    // By implementing the AppStateEventNotifier.AppStateChanged delegate, your app will be alerted to app launch and foregrounding events and will be able to show the ad.
    // AppStateEventNotifier is recommended because OnApplicationPause will give false foreground and background signals when the app shows a full screen ad.
    private void OnAppStateChanged(AppState state)
    {
        Debug.Log("AppOpenAdsService === App State is " + state);
        // Display the app open ad when the app is foregrounded.
        if (state == AppState.Foreground)
        {
            AppOpenAdManager.Instance.ShowAdIfAvailable();
        }
    }
}
