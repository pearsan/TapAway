using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

public class LevelRewardUIManager : MonoBehaviour
{
    public static LevelRewardUIManager Instance;

    [Header("Prefabs")]
    [SerializeField] private GameObject SegmentPrefab;
    [SerializeField] private GameObject RewardPrefab;

    [Space(10f)]
    [SerializeField] private Transform RewardProgressTransform;

    [SerializeField] private UnityEvent GoldRewardEvent;
    [SerializeField] private UnityEvent LevelRewardEvent;
    [SerializeField] private UnityEvent ShutAllButtonEvent;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CreateNewRewardProgress();
    }

    public void ShutAllButtonBehaviour()
    {
        ShutAllButtonEvent.Invoke();
    }    

    public void ExcuteButtonBehaviour()
    {
        if (LevelRewardManager.Instance.OnValidateTriggerClaimRewardEvent())
            LevelRewardEvent.Invoke();
        else
            GoldRewardEvent.Invoke();
    }     

    public void CreateNewRewardProgress()
    {
        StepProgressSliderSegment[] segments = RewardProgressTransform.GetComponentsInChildren<StepProgressSliderSegment>();
        if (segments.Length > 0)
            foreach (var segment in segments)
                Destroy(segment.gameObject);

        int completed = LevelRewardManager.Instance.GetData().Item1;
        int total = LevelRewardManager.Instance.GetData().Item2;

        GameObject reward = Instantiate<GameObject>(RewardPrefab, RewardProgressTransform);
        for(int i = total - 1;i >=1;i--)
        {
            GameObject segment = Instantiate<GameObject>(SegmentPrefab, RewardProgressTransform);
            if (i < completed)
                segment.GetComponent<StepProgressSliderSegment>().OnTriggerFillSegment();
            else if (i == completed)
                segment.GetComponent<StepProgressSliderSegment>().OnTriggerHalfFillSegment();
            else
                segment.GetComponent<StepProgressSliderSegment>().OnTriggerFillntSegment();
        }    
    }

    public IEnumerator CreateNewRewardProgressTween(Transform RewardProgressTweenTransform)
    {
        LevelRewardUIManager.Instance.ShutAllButtonBehaviour();
        StepProgressSliderSegment[] segments = RewardProgressTweenTransform.GetComponentsInChildren<StepProgressSliderSegment>();
        if (segments.Length > 0)
            foreach (var segment in segments)
                Destroy(segment.gameObject);

        int completed = LevelRewardManager.Instance.GetData().Item1 + 1;
        int total = LevelRewardManager.Instance.GetData().Item2;

        GameObject reward = Instantiate<GameObject>(RewardPrefab, RewardProgressTweenTransform);
        for (int i = total - 1; i >= 1; i--)
        {
            GameObject segment = Instantiate<GameObject>(SegmentPrefab, RewardProgressTweenTransform);
            if (i < completed)
                segment.GetComponent<StepProgressSliderSegment>().OnTriggerFillSegment();
            else if (i == completed)
                segment.GetComponent<StepProgressSliderSegment>().OnTriggerHalfFillSegmentTween();
            else
                segment.GetComponent<StepProgressSliderSegment>().OnTriggerFillntSegment();
        }

        yield return new WaitUntil(() => !DOTween.IsTweening("Fill segment"));
        LevelRewardUIManager.Instance.ExcuteButtonBehaviour();
    }    
}
