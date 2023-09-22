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

    private void Awake()
    {
        /*parent = new GameObject("StarParent");
        child = Resources.Load<GameObject>("Prefabs/star");
        GameObject childClone = Instantiate(child, transform.position, Quaternion.identity);
        childClone.transform.SetParent(parent.transform);
        child = childClone;*/
    }

    private void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity) && Input.GetMouseButtonDown(0))
        {
            if (hit.collider != null)
                if (hit.collider.gameObject.GetComponent<TapCube>() != null)
                    if (!hit.collider.gameObject.GetComponent<TapCube>().IsBlock())
                        hit.collider.gameObject.GetComponent<TapCube>().SetMoving();
        }
    }
}
