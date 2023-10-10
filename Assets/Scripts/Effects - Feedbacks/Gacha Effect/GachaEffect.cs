using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GachaEffect : MonoBehaviour
{
    [SerializeField] private CameraBehaviour gachaCubeRotater;

    [Header("Gacha Materials")]
    [SerializeField] private Material FlashMaterial;

    private GameObject _obtainedObject;

    public void OnGachaEffect (ShopItemSO obtainedObject)
    {
        if (_obtainedObject != null)
            Destroy(_obtainedObject);

        gachaCubeRotater.enabled = true;

        _obtainedObject = Instantiate<GameObject>(obtainedObject.ShopItemPrefab);
        StartCoroutine(OnGachaAnimation());
    }

    public void OnExitGachaAnimation()
    {
        Destroy(_obtainedObject);
        gachaCubeRotater.SetTargert(null);
        gachaCubeRotater.enabled = false;
    }    

    private IEnumerator OnGachaAnimation()
    {
        Material temp = _obtainedObject.GetComponentInChildren<MeshRenderer>().material;
        Vector3 tempScale = _obtainedObject.transform.localScale;

        _obtainedObject.GetComponentInChildren<MeshRenderer>().material = FlashMaterial;
        _obtainedObject.transform.localScale = new Vector3(200, 200, 200);
        _obtainedObject.transform.DOMoveX(_obtainedObject.transform.position.x - 0.1f, 0f);
        _obtainedObject.transform.DOScale(tempScale, 0.3f).SetEase(Ease.InSine);
        yield return new WaitForSeconds(0.6f);
        _obtainedObject.GetComponentInChildren<MeshRenderer>().material = temp;
        _obtainedObject.transform.DOMoveX(_obtainedObject.transform.position.x + 0.1f, 0.5f);
        gachaCubeRotater.SetTargert(_obtainedObject.transform);
    }    
}
