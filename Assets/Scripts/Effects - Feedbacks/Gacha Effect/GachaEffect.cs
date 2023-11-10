using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class GachaEffect : MonoBehaviour
{
    [Header("Gacha Animation Prefab")]
    [SerializeField] private GameObject AnimationPrefab;
    [SerializeField] private GameObject FXAnimationPrefab;

    [Header("Texts")]
    [SerializeField] private TMP_Text RewardText;

    [Header("Camera Behaviours")]
    //Control camera behaviours by set active game object contain them
    [SerializeField] private GameObject MainCamera;

    private GameObject _animationCamera;
    private MeshRenderer _animationCubeMesh;
    private MeshFilter _animationCubeMeshFilter;

    public IEnumerator OnTriggerGachaAnimation (ShopItemSO obtainedObject)
    {
        Debug.Log(obtainedObject);
        if (obtainedObject.Type == "Skin")
        {
            RewardText.text = "You got new skin!";

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
        else
        {
            RewardText.text = "You got new FX!!";

            MainCamera.SetActive(false);

            if (_animationCamera != null)
                Destroy(_animationCamera);

            _animationCamera = Instantiate<GameObject>(FXAnimationPrefab);

            _animationCamera.transform.Find("Box").gameObject.SetActive(false);
            GameObject FX = Instantiate<GameObject>(obtainedObject.ShopItemPrefab, _animationCamera.transform);

            yield return new WaitForSeconds(5f);
        } 
            
    }

    public void OnExitGachaAnimation()
    {
        Destroy(_animationCamera);
        MainCamera.SetActive(true);
    }
}
