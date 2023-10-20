using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelRewardUIManager : MonoBehaviour
{
    public static LevelRewardUIManager Instance;

    [Header("Prefabs")]
    [SerializeField] private GameObject SegmentPrefab;

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

        Debug.Log(completed + "/" + total);

        for(int i = 1;i <=total;i++)
        {
            GameObject segment = Instantiate<GameObject>(SegmentPrefab, RewardProgressTransform);
            if (i <= completed)
                segment.GetComponent<StepProgressSliderSegment>().OnTriggerSegmentFill();
            else
                segment.GetComponent<StepProgressSliderSegment>().OnTriggerSegmentFillnt();
        }    
    }    
}
