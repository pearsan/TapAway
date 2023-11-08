using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private void Start()
    {
        GetComponent<DOTweenAnimation>().DOPlay();
    }

    public void Throw(Vector3 hitPoint, float explodeRadius, float explodeForce)
    {
        GetComponent<DOTweenAnimation>().DOComplete();

        transform.SetParent(null);
        Vector3 startPoint = transform.position;
        Vector3 endPoint = hitPoint;
        
        Vector3 middlePoint = (startPoint + endPoint) / 2; // calculate the middle point
        middlePoint.y += 5f; // elevate the middle point by 10 units


        Vector3[] path = new Vector3[]
        {
            startPoint,
            middlePoint,
            endPoint
        };
        
        transform.DOPath(path, 2, PathType.CatmullRom).SetEase(Ease.InSine).SetLookAt(0.01f).OnComplete(() =>
        {
            Explode(hitPoint, explodeRadius, explodeForce);
            Destroy(gameObject);
        }
            );
    }

    private void Explode(Vector3 hitPoint, float explodeRadius, float explodeForce)
    {
        int layerMask = 1 << LayerMask.NameToLayer("Cube");
        Collider[] hitColliders = Physics.OverlapSphere( hitPoint, explodeRadius, layerMask);
        foreach (Collider hitCollider in hitColliders)
        {
            IExplodable cube = hitCollider.GetComponent<IExplodable>();
            if (cube != null) // If the object has a TapCube component
            {
                cube.Explode(hitPoint, explodeForce, explodeRadius);
            }
        }        
    }
}
