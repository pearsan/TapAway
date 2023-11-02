using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

[SelectionBase]
public class TapCube : MonoBehaviour, ITappable
{

    [SerializeField] private bool isHidden = false;
    [SerializeField] private Transform cubeMesh;
    [SerializeField] private float speed;
    private const float TweenDuration = 0.25f;
    private bool _moving = false;
    private Material _material;
    private bool _canDoMove = true;
    public bool drawRay;
    

    private void FixedUpdate()
    {
        if (_moving)
        {
            transform.position += transform.forward * (speed * Time.deltaTime);
        }
    }

    private void SetMoving()
    {
        transform.parent = null;
        gameObject.GetComponent<Collider>().enabled = false;
        _moving = true;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public bool IsBlock()
    {
        int layerMask = 1 << LayerMask.NameToLayer("Cube");

        float maxDistance = 100f;
        bool isHit = Physics.Raycast(transform.position, transform.forward,
            out var hit, maxDistance, layerMask);
        return isHit;
    }

    public void TryMove()
    {
        if (!_canDoMove)
        {
            DOTween.Kill(cubeMesh);
            _canDoMove = true;
        }
        cubeMesh.position = transform.position;
        float maxDistance = 100f;
        RaycastHit hit;
        bool isHit = Physics.Raycast(transform.position, transform.forward,
            out hit, maxDistance);
        if (isHit && _canDoMove)
        {
            if (hit.distance >= 1)
            {
                _canDoMove = false;
                Vector3 localTargetPosition = transform.InverseTransformPoint(hit.collider.transform.position - transform.forward);
                cubeMesh.DOLocalMove(localTargetPosition, TweenDuration).SetEase(Ease.InOutSine).SetLoops(2, LoopType.Yoyo).OnComplete(() => _canDoMove = true);    
            }
            else
            {

                TryMoveShort(transform.forward);
            }
        }
    }

    private void TryMoveShort(Vector3 direction)
    {
        // move the next one if have
        float maxDistance = 5f;
        bool isHit = Physics.Raycast(transform.position, direction,
            out var hit, maxDistance);
        bool firstLoopDone = false;
        if (isHit && _canDoMove)
        {
            _canDoMove = false;
            Vector3 localTargetPosition = transform.InverseTransformPoint(transform.position + direction / 10);
            cubeMesh.DOLocalMove(localTargetPosition, TweenDuration / 4).SetEase(Ease.InOutSine).SetLoops(2, LoopType.Yoyo).OnStepComplete(() => {
                if (!firstLoopDone)
                {
                    firstLoopDone = true;
                    if (hit.collider.gameObject.GetComponent<TapCube>()._canDoMove && hit.distance < 1) 
                        hit.collider.gameObject.GetComponent<TapCube>().TryMoveShort(direction);
                }
            }).OnComplete(() => _canDoMove = true);
        }
        else if (_canDoMove)
        {
            // move the cube
            _canDoMove = false;
            Vector3 localTargetPosition = transform.InverseTransformPoint(transform.position + direction / 10);
            cubeMesh.DOLocalMove(localTargetPosition, TweenDuration / 4).SetEase(Ease.InOutSine).SetLoops(2, LoopType.Yoyo)
                .OnComplete(() => _canDoMove = true);
        }
    } 

    private void OnDrawGizmos()
    {
        if (!drawRay) return;
        float maxDistance = 10f;

        bool isHit = Physics.Raycast(transform.position, transform.forward,
            out var hit, maxDistance);

        if (isHit)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position,  transform.forward * hit.distance);
                
            Gizmos.matrix = Matrix4x4.TRS(hit.transform.position - transform.forward, transform.rotation, Vector3.one / 100 * 80);
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, transform.forward * maxDistance);
        }
    }

    public void HiddenCube()
    {
        isHidden = true;
        gameObject.GetComponent<Collider>().enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);
    }
    
    public void ShowCube()
    {
        isHidden = false;
        gameObject.GetComponent<Collider>().enabled = true;
        transform.GetChild(0).gameObject.SetActive(true);
    }
    

    public bool IsHidden()
    {
        return isHidden;
    }

    public void SetCanDoMove(bool canMove)
    {
        _canDoMove = canMove;
    }

    public void Tap()
    {
        if (!IsBlock())
        {
            StartCoroutine(FadeOut());
            SetMoving();
            GameplayManager.Instance.SpawnRewardCube();           
        }
        else
        {
            TryMove();
        }
    }

    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(0.3f);
        _material = new Material(gameObject.GetComponentInChildren<Renderer>().sharedMaterial);
        _material.shader = Shader.Find("Standard");
        ToFadeMode(_material);
        gameObject.GetComponentInChildren<Renderer>().sharedMaterial = _material;
        _material.DOFade(0f, 1f).OnComplete((() => Destroy(gameObject)));
    }
    
    private void ToFadeMode(Material material)
    {
        material.SetOverrideTag("RenderType", "Transparent");
        material.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = (int) UnityEngine.Rendering.RenderQueue.Transparent;
    }

    private void OnDestroy()
    {
        Destroy(_material);
    }
}
