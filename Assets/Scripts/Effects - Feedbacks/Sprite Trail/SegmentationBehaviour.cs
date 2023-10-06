using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentationBehaviour : MonoBehaviour
{
    public Transform Target;
    public float LerpSpeed;
    private void Update()
    {
        if (Target != null)
            transform.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(transform.GetComponent<RectTransform>().anchoredPosition, Target.GetComponent<RectTransform>().anchoredPosition, LerpSpeed);
        else
            Destroy(gameObject);
    }

}
