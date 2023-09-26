using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGUIManager : MonoBehaviour
{
    //Attach this class to any tabs of button
    [Header("Sprites Trasition")]
    [SerializeField] private Sprite UnselectedSprite;
    [SerializeField] private Sprite SelectedSprite;

    private Image _targetImage;

    private void Start()
    {
        _targetImage = GetComponent<Image>();
    }

    public void OnTabSelect()
    {
        _targetImage.sprite = SelectedSprite;
        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = 3;
    }    

    public void OnTabUnselect()
    {
        _targetImage.sprite = UnselectedSprite;
        Destroy(gameObject.GetComponent<Canvas>());
    }    
}
