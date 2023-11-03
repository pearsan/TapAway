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
        AppOpenAd.LoadAd(AD_UNIT_ID, ScreenOrientation.Portrait, request, ((appOpenAd, error) =>
        {
            if (error != null)
            {
                // Handle the error.
                Debug.LogFormat("Failed to load the ad. (reason: {0})", error.LoadAdError.GetMessage());
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

        _ad.OnAdDidDismissFullScreenContent += OnAdFullScreenContentClosed;
        _ad.OnAdFailedToPresentFullScreenContent += OnAdFullScreenContentFailed;
        _ad.OnAdDidPresentFullScreenContent += OnAdFullScreenContentOpened;
        _ad.OnAdDidRecordImpression += OnAdImpressionRecorded;
        _ad.OnPaidEvent += OnAdPaid;

        _ad.Show();
    }

    private void OnAdFullScreenContentClosed(object sender, EventArgs args)
    {
        Debug.LogError("Closed app open ad");
        // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
        _ad = null;
        _isShowingAd = false;
        LoadAd();
    }

    private void OnAdFullScreenContentFailed(object sender, AdErrorEventArgs args)
    {
        Debug.LogFormat("Failed to present the ad (reason: {0})", args.AdError.GetMessage());
        // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
        _ad = null;
        LoadAd();
    }

    private void OnAdFullScreenContentOpened(object sender, EventArgs args)
    {
        Debug.LogError("Displayed app open ad");
        _isShowingAd = true;
    }

    private void OnAdImpressionRecorded(object sender, EventArgs args)
    {
        Debug.LogError("Recorded ad impression");
    }

    private void OnAdPaid(object sender, AdValueEventArgs args)
    {
        Debug.LogFormat("Received paid event. (currency: {0}, value: {1}",
            args.AdValue.CurrencyCode, args.AdValue.Value);

        if (!FireBaseRemote.IsInitialized) return;
        
        Firebase.Analytics.Parameter[] adParameters =
        {
            new("ad_platform", "google_mobile_ad"),
            new("ad_source", "n/a"),
            new("ad_unit_name", "n/a"),
            new("ad_format", "app_open"),
            new("currency", args.AdValue.CurrencyCode),
            new("value", args.AdValue.Value)
        };
        Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", adParameters);
    }
}
