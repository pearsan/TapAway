using UnityEngine;
using System;
using GoogleMobileAds;
using GoogleMobileAds.Api;

public class IntersititialAds : MonoBehaviour
{
    bool CanAds = false;

    float timer = 0;
    public void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            CanAds = true;
            LoadInterstitialAd();
            RegisterReloadHandler(_interstitialAd); 
            // This callback is called once the MobileAds SDK is initialized.
        });
    }

#if UNITY_ANDROID
    private string _interstitialAdID = "ca-app-pub-8301791769525290/1240624626";
#elif UNITY_IPHONE
  private string _interstitialAdID = "ca-app-pub-8301791769525290/1240624626";
#else
  private string _interstitialAdID = "unused";
#endif

    private InterstitialAd _interstitialAd;

    /// <summary>
    /// Loads the interstitial ad.
    /// </summary>
    public void LoadInterstitialAd()
    {
        // Clean up the old ad before loading a new one.
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        Debug.Log("Loading the interstitial ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        InterstitialAd.Load(_interstitialAdID, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Interstitial ad loaded with response : "
                          + ad.GetResponseInfo());

                _interstitialAd = ad;
            });
    }

    /// <summary>
    /// Shows the interstitial ad.
    /// </summary>
    public void ShowInterstitialAd()
    {
        RegisterReloadHandler(_interstitialAd);
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            _interstitialAd.Show();
        }
        else
        {
            Debug.LogWarning("Interstitial ad is not ready yet.");
        }
    }

    /*private void Update()
    {
        timer += Time.deltaTime;
        if (Input.GetMouseButtonDown(0) && CanAds)
        {
            ShowInterstitialAd();
        }
    }*/

    private void RegisterReloadHandler(InterstitialAd interstitialAd)
    {
        // Raised when the ad closed full screen content.
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.LogWarning("Interstitial Ad full screen content closed.");

            // Reload the ad so that we can show another as soon as possible.
            LoadInterstitialAd();
        };
        // Raised when the ad failed to open full screen content.
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogWarning("Interstitial ad failed to open full screen content " +
                           "with error : " + error);

            // Reload the ad so that we can show another as soon as possible.
            LoadInterstitialAd();
        };
    }
}