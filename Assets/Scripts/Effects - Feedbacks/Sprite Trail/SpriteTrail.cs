using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteTrail : MonoBehaviour
{
    [SerializeField] private GameObject SpritePrefab;

    [Tooltip("How many sprites to show sprite trail")]
    [SerializeField] private int SpriteQuantity;
    [SerializeField] private float LerpSpeed;

    [HideInInspector] public Vector3 Direction;

    private Transform _targetTransform;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        _targetTransform = transform.parent;
    } 
    
    public void SpawnSpriteTrail()
    {
        for (int i = 1; i <= SpriteQuantity; i++)
        {
            GameObject sprite = Instantiate<GameObject>(SpritePrefab, _targetTransform);
            sprite.GetComponent<RectTransform>().anchoredPosition = (Vector3) GetComponent<RectTransform>().anchoredPosition + Direction * i*50;
            Debug.Log(sprite.GetComponent<RectTransform>().anchoredPosition);
            sprite.transform.localScale = (1 / (float)(SpriteQuantity + 1) * (float)(SpriteQuantity + 1 - i)) * Vector3.one;
            sprite.GetComponent<Image>().color = new Color(sprite.GetComponent<Image>().color.r, sprite.GetComponent<Image>().color.g, sprite.GetComponent<Image>().color.b, (1 / (float)(SpriteQuantity + 1) * (float)(SpriteQuantity + 1 - i)));
            SegmentationBehaviour segmentationBehaviour = sprite.AddComponent<SegmentationBehaviour>();
            segmentationBehaviour.LerpSpeed = LerpSpeed;
            segmentationBehaviour.Target = transform;
        }
    }    
}
