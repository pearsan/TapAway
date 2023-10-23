using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GachaEffect : MonoBehaviour
{
    [Header("Gacha Animation Prefab")]
    [SerializeField] private GameObject AnimationPrefab;

    [Header("Camera Behaviours")]
    //Control camera behaviours by set active game object contain them
    [SerializeField] private GameObject MainCamera;

    private GameObject _animationCamera;
    private MeshRenderer _animationCubeMesh;
    private MeshFilter _animationCubeMeshFilter;

    public IEnumerator OnTriggerGachaAnimation (ShopItemSO obtainedObject)
    {
        MainCamera.SetActive(false);

        if (_animationCamera != null)
            Destroy(_animationCamera);

        _animationCamera = Instantiate<GameObject>(AnimationPrefab);
        _animationCubeMesh = _animationCamera.transform.Find("Box").GetComponentInChildren<MeshRenderer>();
        _animationCubeMeshFilter = _animationCamera.transform.Find("Box").GetComponentInChildren<MeshFilter>();

        _animationCubeMesh.material = obtainedObject.ShopItemPrefab.GetComponent<MeshRenderer>().sharedMaterial;
        _animationCubeMeshFilter.mesh = obtainedObject.ShopItemPrefab.GetComponentInChildren<MeshFilter>().sharedMesh;

        yield return new WaitForSeconds(5f);
    }

    public void OnExitGachaAnimation()
    {
        Destroy(_animationCamera);
        MainCamera.SetActive(true);
    }
}
