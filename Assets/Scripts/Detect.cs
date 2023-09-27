using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detect : MonoBehaviour
{
    private RaycastHit hit;
    private Ray ray;
    [SerializeField] private GameObject parent;
    [SerializeField] private GameObject child;

    private void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Input.GetMouseButtonUp(0))
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider != null)
                {
                    TapCube tapCube = hit.collider.gameObject.GetComponent<TapCube>();
                    if (tapCube != null && !tapCube.IsBlock())
                    {
                        tapCube.SetMoving();
                    }
                }
            }
        }
    }
}
