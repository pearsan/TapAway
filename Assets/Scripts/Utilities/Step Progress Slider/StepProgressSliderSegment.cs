using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StepProgressSliderSegment : MonoBehaviour
{
    private GameObject Fill;

    public void OnTriggerSegmentFill()
    {
        Fill = transform.Find("Segment Fill").gameObject;
        Debug.Log(Fill);
        Fill.GetComponent<Image>().DOFillAmount(1, 0.2f).From(0);
    }

    public void OnTriggerSegmentFillnt()
    {
        Fill = transform.Find("Segment Fill").gameObject;
        Debug.Log(Fill);
        Fill.GetComponent<Image>().DOFillAmount(0, 0f);
    }    
}
