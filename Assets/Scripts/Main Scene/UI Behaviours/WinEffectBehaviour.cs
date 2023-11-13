using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinEffectBehaviour : MonoBehaviour
{
    public static WinEffectBehaviour Instance;

    [SerializeField] private ParticleSystem _winEffect;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _winEffect = Instantiate(_winEffect);
    }

    public IEnumerator OnAnimateWinEffect() 
    {
        _winEffect.Play();
        yield return new WaitForSeconds(2f);
        _winEffect.Stop();
    }

    public void ChangeWinEffect(ShopItemSO newWinEffect)
    {
        Destroy(_winEffect);
        _winEffect = newWinEffect.ShopItemPrefab.GetComponent<ParticleSystem>();
        _winEffect = Instantiate(_winEffect);
    }    
}
