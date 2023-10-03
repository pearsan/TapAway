using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;


public class TapCube : MonoBehaviour
{

    [SerializeField] private bool isHidden = false;
    [SerializeField] private Transform cubeMesh;
    [SerializeField] private float speed;
    private const float TweenDuration = 0.5f;
    private bool _moving = false;

    private bool _canDoMove = true;
    public bool drawRay;
    

    private void Update()
    {
        if (_moving)
        {
            transform.position += transform.forward * (speed * Time.deltaTime);
        }
    }
    
    public bool IsMoving()
    {
        return _moving;
    }

    public void SetMoving()
    {
        transform.parent = null;
        gameObject.GetComponent<Collider>().enabled = false;
        _moving = true;
        StartCoroutine(DestroyByTime());
    }

    private IEnumerator DestroyByTime()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public bool IsBlock()
    {
        float maxDistance = 100f;
        RaycastHit hit;
        bool isHit = Physics.Raycast(transform.position, transform.forward, out hit, maxDistance);
        if (isHit)
        {
            if (hit.collider.gameObject.GetComponent<TapCube>().IsMoving())
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        return false;
    }

    public void TryMove()
    {
        float maxDistance = 100f;
        RaycastHit hit;
        bool isHit = Physics.Raycast(transform.position, transform.forward,
            out hit, maxDistance);
        if (isHit && _canDoMove)
        {
            float distance = (hit.collider.transform.position - transform.position).sqrMagnitude;
            if (distance >= 4)
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
        float maxDistance = 100f;
        bool isHit = Physics.Raycast(transform.position, direction,
            out var hit, maxDistance);
        bool firstLoopDone = false;
        
        if (isHit && _canDoMove)
        {
            float distance = (hit.transform.position - transform.position).sqrMagnitude;
            if (distance < 4)
            {
                _canDoMove = false;
                Vector3 localTargetPosition = transform.InverseTransformPoint(transform.position + direction / 20);
                cubeMesh.DOLocalMove(localTargetPosition, TweenDuration / 4).SetEase(Ease.InOutSine).SetLoops(2, LoopType.Yoyo).OnStepComplete(() => {
                    if (!firstLoopDone)
                    {
                        firstLoopDone = true;
                        hit.collider.gameObject.GetComponent<TapCube>().TryMoveShort(direction);
                    }
                }).OnComplete(() => _canDoMove = true);
            }
        }
        else
        {
            // move the cube
            _canDoMove = false;
            Vector3 localTargetPosition = transform.InverseTransformPoint(transform.position + direction / 20);
            cubeMesh.DOLocalMove(localTargetPosition, TweenDuration / 4).SetEase(Ease.InOutSine).SetLoops(2, LoopType.Yoyo)
                .OnComplete(() => _canDoMove = true);
        }
    } 

    private void OnDrawGizmos()
    {
        if (!drawRay) return;
        float maxDistance = 10f;
        RaycastHit hit;

        bool isHit = Physics.BoxCast(transform.position, Vector3.one / 100 * 80 / 2, transform.forward,
            out hit, transform.localRotation, maxDistance);

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

}
