using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StepProgressSliderSegment : MonoBehaviour
{
    private GameObject Fill;

    public void OnTriggerFillSegment()
    {
        Fill = transform.Find("Segment Fill").gameObject;
        Fill.GetComponent<Image>().DOFillAmount(1, 0f);
    }

    /// <summary>
    /// When the next level has not been completed, this segment only stops at displaying itself without connecting to the next segment.
    /// </summary>
    public void OnTriggerHalfFillSegment()
    {
        Fill = transform.Find("Segment Fill").gameObject;
        Fill.GetComponent<Image>().DOFillAmount(0.44f, 0f);
    }

    public void OnTriggerHalfFillSegmentTween()
    {
        Fill = transform.Find("Segment Fill").gameObject;
        Fill.GetComponent<Image>().DOFillAmount(0.44f, 1.5f).From(0).SetId("Fill segment");
    }

    public void OnTriggerFullFillSegmentTween()
    {
        Fill = transform.Find("Segment Fill").gameObject;
        Fill.GetComponent<Image>().DOFillAmount(1f, 1.5f).From(0).SetId("Fill segment");
    }    

    public void OnTriggerFillntSegment()
    {
        Fill = transform.Find("Segment Fill").gameObject;
        Fill.GetComponent<Image>().DOFillAmount(0, 0f);
    }    
}
