using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class TapCube : MonoBehaviour
{
    [SerializeField] private bool _moving = false;

    [SerializeField] private float speed;

    [SerializeField] private bool isHidden = false;


    public bool drawRay;
    private Vector3 startPosition;


    private void Update()
    {
        if (_moving)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }
    
    public bool isMoving()
    {
        return _moving;
    }

    public void SetMoving()
    {
        _moving = true;
        StartCoroutine(DestroyByTime());
    }

    private IEnumerator DestroyByTime()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    public bool IsBlock()
    {
        float maxDistance = 100f;
        RaycastHit hit;
        bool isHit = Physics.Raycast(transform.position, transform.forward, out hit, maxDistance);
        if (isHit)
        {
            if (hit.collider.gameObject.GetComponent<TapCube>().isMoving())
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        return isHit;
    }
   

    private void OnDrawGizmos()
    {
        if (drawRay)
        {
            float maxDistance = 5f;
            RaycastHit hit;

            bool isHit = Physics.Raycast(transform.position, transform.forward, out hit, maxDistance);

            if (isHit)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position, transform.forward * hit.distance);
            }
            else
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(transform.position, transform.forward * maxDistance);
            }            
        }
    }

    public void HiddenCube()
    {
        isHidden = true;
        gameObject.GetComponent<Collider>().enabled = false;
        drawRay = false;
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
