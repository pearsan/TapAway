using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PointerAnimation : MonoBehaviour
{
    public Image sprite1;
    public Image sprite2;
    // Start is called before the first frame update

    private void OnEnable()
    {
        StartAnimation();
    }

    void StartAnimation()
    {
        // Create a new Sequence
        Sequence sequence = DOTween.Sequence();
        sequence.Append(sprite1.DOFade(0, 0f)); // Fade in over 1 second
        sequence.Append(sprite2.DOFade(0, 0f)); // Fade in over 1 second
        sequence.AppendInterval(0.5f); // Wait for 1 second

        // Add the first sprite fade in and out to the sequence
        sequence.Append(sprite1.DOFade(1, 0.5f)); // Fade in over 1 second
        sequence.AppendInterval(0.35f); // Wait for 1 second
        sequence.Append(sprite1.DOFade(0, 0f)); // Fade out immediately

        // Add the second sprite fade in and out to the sequence
        sequence.Append(sprite2.DOFade(1, 0f)); // Fade in over 1 second
        sequence.AppendInterval(0.25f); // 
        sequence.Append(sprite2.DOFade(0, 0f)); // Fade out over 1 second

        // Add the first sprite fade in and out again to the sequence
        sequence.Append(sprite1.DOFade(1, 0f)); // Fade in over 1 second
        sequence.AppendInterval(0.35f); // Wait for 1 second
        sequence.Append(sprite1.DOFade(0, 0.5f)); // Fade out over 1 second

        // Loop the sequence indefinitely
        sequence.SetLoops(-1);
    }
}
