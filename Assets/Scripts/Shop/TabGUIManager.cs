using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TabGUIManager : MonoBehaviour
{
    //Attach this class to any tabs of button
    [Header("Sprites Transition")]
    [SerializeField] private Sprite UnselectedSprite;
    [SerializeField] private Sprite SelectedSprite;

    [Header("Contents")]
    [Tooltip("Content object from this tab when selected")]
    [SerializeField] private GameObject TargetContents;

    private Image _targetImage;

    private void Awake()
    {
        _targetImage = GetComponent<Image>();
    }

    private void Start()
    {
        
    }

    /// <summary>
    /// Handle the tab selection event and perform a tween animation.
    /// </summary>
    /// <param name="IsAscending">If true, it will tween from left to right. If false, it will tween from right to left.</param>
    public void OnTabSelect(bool IsAscending)
    {
        _targetImage.sprite = SelectedSprite;
        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = 3;

        TargetContents.SetActive(true);
    }    

    public void OnTabUnselect()
    {
        _targetImage.sprite = UnselectedSprite;
        Destroy(gameObject.GetComponent<Canvas>());

        //TargetContents.SetActive(false);
    }    
}
