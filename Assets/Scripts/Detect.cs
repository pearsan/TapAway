using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Detect : MonoBehaviour
{
    
    [SerializeField] private GameObject parent;
    [SerializeField] private GameObject child;
    [FormerlySerializedAs("spawnBlock")] [SerializeField] private bool editCube = false;

    private void Update()
    {
        ShootRay();
    }

    private void ShootRay()
    {
        RaycastHit hit;
        Ray ray;
        
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Input.GetMouseButtonUp(0))
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider != null)
                {
                    TapCube tapCube = hit.collider.gameObject.GetComponent<TapCube>();
                    if (!editCube)
                    {
                        if (tapCube != null && !tapCube.IsBlock())
                        {
                            tapCube.SetMoving();
                        }    
                    }
                    else
                    {
                        Vector3 positionToInstantiate = tapCube.transform.position + hit.normal;
                        GameObject newCube = Instantiate(child);
                        newCube.transform.SetParent(parent.transform);
                        newCube.transform.position = positionToInstantiate;
                        newCube.transform.localRotation = tapCube.transform.localRotation;
                    }
                    
                }
            }
        }
        else if (Input.GetMouseButtonUp(1))
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider != null)
                {
                    TapCube tapCube = hit.collider.gameObject.GetComponent<TapCube>();
                    if (editCube)
                    {
                        if (tapCube != null)
                        {
                            Destroy(tapCube.gameObject);
                        }    
                    }
                }
            }
        }
    }
}
