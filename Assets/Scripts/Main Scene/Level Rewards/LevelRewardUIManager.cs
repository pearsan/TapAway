using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelRewardUIManager : MonoBehaviour
{
    public static LevelRewardUIManager Instance;

    [Header("Prefabs")]
    [SerializeField] private GameObject SegmentPrefab;
    [SerializeField] private GameObject RewardPrefab;

    [Space(10f)]
    [SerializeField] private Transform RewardProgressTransform;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CreateNewRewardProgress();
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
}
