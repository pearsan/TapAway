using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public float speed = 10f;
    public float raycastDistance = 100f;
    private int _layerMask;

    private void Start()
    {
        CameraBehaviour.Instance.OnPause();
        _layerMask = 1 << LayerMask.NameToLayer("Cube");
    }

    private void Update()
    {
        // Cast a ray forward from the rocket's current position
        

        bool isHit = Physics.Raycast(transform.position, transform.forward,
            out var hit, raycastDistance, _layerMask);

        // If no hit detected, continue moving the rocket forward
        if (isHit)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
        else
        {
            transform.position += transform.forward * speed * Time.deltaTime;
            CameraBehaviour.Instance.OnPlay();
            Destroy(gameObject, 0.25f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IExplodable cube = other.GetComponent<IExplodable>();
        if (cube != null)
        {
            other.transform.parent = null;
            cube.Explode(transform.position, 500f, 2f);
        }
    }
}
