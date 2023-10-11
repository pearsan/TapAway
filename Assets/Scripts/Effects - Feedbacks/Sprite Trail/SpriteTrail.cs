using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SpriteTrail : MonoBehaviour
{
    [SerializeField] private float TimeBetweenSpawn;
    [SerializeField] private float TimeFading;

    [Space(10f)]
    [SerializeField] private GameObject CoinSegmentationPrefab;

    private Transform TargetTransform;

    private void Start()
    {
        Initialize(); 
    }

    private void Initialize()
    {
        TargetTransform = transform.parent;
    }
    
    public IEnumerator SpawnSpriteTrail()
    {
        GameObject sprite = Instantiate<GameObject>(CoinSegmentationPrefab, TargetTransform);
        sprite.transform.localScale = Vector3.one;
        sprite.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
        FadeSprite(sprite.GetComponent<Image>());
        yield return new WaitForSeconds(TimeBetweenSpawn);
        StartCoroutine(SpawnSpriteTrail());
    }    

    private void FadeSprite(Image target)
    {
        target.DOFade(0, TimeFading).OnComplete(() => Destroy(target.gameObject));
    }    
}
