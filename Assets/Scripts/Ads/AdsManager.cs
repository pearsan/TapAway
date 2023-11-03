using Puzzle.UI;
using System;
using System.Collections;
using UnityEngine;

public class AdsManager : SingletonMonoBehavior<AdsManager>
{
    [SerializeField] private GameObject adsBreakCanvas;
    [SerializeField] private float delayShowAdsBreak;
    [SerializeField] private float timer;
    [SerializeField] private bool active;

    public static event Action OnShowInter;
    public static event Action OnHideInter;

    public void ShowReward(string adsWhere, Action onSucceed = null, Action onFail = null)
    {
        Debug.Log("Get Reward");

#if UA_BUILD
        onSucceed?.Invoke();
#else
        ISHandler.Instance.ShowRewardedVideo(adsWhere, onSucceed, onFail);
#endif
    }

    public void ShowInter(string adsWhere)
    {
        Debug.Log("Show Inter");
#if !UA_BUILD
        ISHandler.Instance.ShowInterstitial(adsWhere);
#endif
    }

    private void Update()
    {
        if (!active) { return; }

        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                StartCoroutine(IEShowAdsBreak());
                timer = delayShowAdsBreak;
            }
        }
    }

    private IEnumerator IEShowAdsBreak()
    {
        OnShowInter?.Invoke();
        Hub.Show(adsBreakCanvas);
        yield return new WaitForSeconds(2);
        ShowInter("AdsBreak");
        OnHideInter?.Invoke();
        Hub.Hide(adsBreakCanvas);
        Debug.Log("Ads break");
    }

    public void StartShowAdsBreak()
    {
        timer = delayShowAdsBreak;
        active = true;
    }

    public void StopShowAdsBreak()
    {
        timer = 0f;
        active = false;
    }

    public void SetActiveAdsBreak(bool active)
    {
        this.active = active;
    }
}