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

    [SerializeField] private Vector3 _initialPos;

    public bool drawRay = true;
    

    private void Update()
    {
        if (_moving)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }

        if (Vector3.Distance(transform.position, transform.parent.position) >= 10)
        {
            Destroy(gameObject);
        }

    }
    
    public bool isMoving()
    {
        return _moving;
    }

    public void SetMoving()
    {
        _moving = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<TapCube>())
        {
            _moving = false;
            transform.position = _initialPos;
        }
    }

    public bool IsBlock()
    {
        float maxDistance = 5f;
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
        drawRay = true;
        transform.GetChild(0).gameObject.SetActive(true);
    }

}
