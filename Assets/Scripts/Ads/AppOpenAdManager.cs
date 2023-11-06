using System;
using GoogleMobileAds.Api;
using UnityEngine;

public class AppOpenAdManager
{
#if UNITY_ANDROID
    // private const string AD_UNIT_ID = "ca-app-pub-3940256099942544/3419835294"; //test
    private const string AD_UNIT_ID = "ca-app-pub-3926209354441959/6053344512";
#elif UNITY_IOS
    private const string AD_UNIT_ID = "ca-app-pub-3940256099942544/5662855259";
#else
    private const string AD_UNIT_ID = "unexpected_platform";
#endif

    private static AppOpenAdManager _instance;
    private AppOpenAd _ad;
    private bool _isShowingAd;
    private DateTime _loadTime;

    public static AppOpenAdManager Instance => _instance ??= new AppOpenAdManager();

    private bool IsAdAvailable => _ad != null && (DateTime.UtcNow - _loadTime).TotalHours < 4;

    public void LoadAd()
    {
        if (IsAdAvailable) return;
        
        var request = new AdRequest.Builder().Build();

        // Load an app open ad for portrait orientation
        AppOpenAd.Load(AD_UNIT_ID, request, ((appOpenAd, error) =>
        {
            if (error != null)
            {
                // Handle the error.
                Debug.LogFormat("Failed to load the ad. (reason: {0})", error.GetMessage());
                return;
            }

            // App open ad is loaded.
            _ad = appOpenAd;
            _loadTime = DateTime.UtcNow;
        }));
    }
    
    public void ShowAdIfAvailable()
    {
        if (!IsAdAvailable || _isShowingAd)
        {
            LoadAd();
            return;
        }

        _ad.OnAdFullScreenContentClosed += OnAdFullScreenContentClosed;
        _ad.OnAdFullScreenContentFailed += OnAdFullScreenContentFailed;
        _ad.OnAdFullScreenContentOpened += OnAdFullScreenContentOpened;
        _ad.OnAdImpressionRecorded += OnAdImpressionRecorded;
        _ad.OnAdPaid += OnAdPaid;

        _ad.Show();
    }

    private void OnAdFullScreenContentClosed()
    {
        Debug.LogError("Closed app open ad");
        // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
        _ad = null;
        _isShowingAd = false;
        LoadAd();
    }

    private void OnAdFullScreenContentFailed(AdError adError)
    {
        // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
        _ad = null;
        LoadAd();
    }

    private void OnAdFullScreenContentOpened()
    {
        Debug.LogError("Displayed app open ad");
        _isShowingAd = true;
    }

    private void OnAdImpressionRecorded()
    {
        Debug.LogError("Recorded ad impression");
    }

    private void OnAdPaid(AdValue adValue)
    {
        if (!FireBaseRemote.IsInitialized) return;

        Firebase.Analytics.Parameter[] adParameters =
        {
        new Firebase.Analytics.Parameter("ad_platform", "google_mobile_ad"),
        new Firebase.Analytics.Parameter("ad_source", "n/a"),
        new Firebase.Analytics.Parameter("ad_unit_name", "n/a"),
        new Firebase.Analytics.Parameter("ad_format", "app_open"),
    };
        Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", adParameters);
    }
}
