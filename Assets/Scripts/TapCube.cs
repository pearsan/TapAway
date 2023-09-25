using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class TapCube : MonoBehaviour
{
    [SerializeField] private bool _moving = false;

    [SerializeField]
    private float speed;

    private int attemp = 0;
    [SerializeField] private int maxAttemp = 10;
    [SerializeField] private Vector3 _initialPos;
    // Start is called before the first frame update
    private List<GameObject> path = new List<GameObject>();
    
    public List<Vector3> directions = new List<Vector3>()
    {
        Vector3.forward,
        Vector3.back,
        Vector3.left,
        Vector3.right,
        Vector3.up,
        Vector3.down
    };

    private void Start()
    {
        while (ShootRaycast(this.path, gameObject, transform, transform.forward) && attemp < maxAttemp)
        {
            attemp++;
        }
    }

    // Update is called once per frame
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
        Shoot1Raycast();


    }

    public bool ShootRaycast(List<GameObject> path, GameObject firstGameObject , Transform origin, Vector3 direction)
    {
        float maxDistance = 5f;
        RaycastHit hit;

        // If the raycast hits nothing, return false immediately
        if (!Physics.Raycast(origin.position, direction, out hit, maxDistance))
        {
            return false;
        }

        GameObject hitObject = hit.collider.gameObject;
        if (hitObject.GetComponent<TapCube>() != null)
        {
            // Do something when a cube is hit
            if (hitObject.Equals(firstGameObject) || path.Contains(hitObject))
            {
                ChangeDirection(firstGameObject.transform.forward, firstGameObject);
                return true;
            }
            path.Add(hitObject);

            return ShootRaycast(path, firstGameObject, hitObject.transform, hitObject.transform.forward);
        }

        return false;
    }
    
    public void Shoot1Raycast()
    {
        float maxDistance = 5f;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance))
        {        
            RaycastHit hit2;
            GameObject hitObject = hit.collider.gameObject;
            if (hitObject.GetComponent<TapCube>() != null)
            {
                if (Physics.Raycast(hitObject.transform.position, hitObject.transform.forward, out hit2, maxDistance))
                {

                    if (hit2.collider.gameObject == gameObject)
                    {
                    ChangeDirection(transform.forward, gameObject);                        
                    }
                }

            }
        }
    }

    
    private void ChangeDirection(Vector3 originalDirection , GameObject hitObject)
    {
        // Remove the original direction from the list
        directions.RemoveAll(dir => Vector3.Dot(dir, originalDirection) > 0.99f); 
        
        // Get the list of possible directions from the cube's script
        
        /*
        Debug.Log("Remaining Directions Count: " + directions.Count);
        */

        if (directions.Count > 0)
        {
            // Choose a random direction from the remaining directions
            Vector3 newDirection = directions[Random.Range(0, directions.Count)];

            // Convert the direction vector to a quaternion
            Quaternion rotation =  Quaternion.LookRotation(newDirection);

            // Set the cube's rotation
            hitObject.transform.localRotation = rotation;
        }
    }

    public bool GetBlocked()
    {
        float maxDistance = 5f;
        RaycastHit hit;

        bool isHit = Physics.Raycast(transform.position, transform.forward, out hit, maxDistance);

        if (isHit)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool isMoving()
    {
        return _moving;
    }

    public Quaternion RandomDirection()
    {
        Quaternion randomRotation = Quaternion.Euler(
            Random.Range(0, 4) * 90f,
            Random.Range(0, 4) * 90f,
            Random.Range(0, 4) * 90f
        );
        return randomRotation;
    }

    public Vector3 GetDirection()
    {
        return transform.forward;
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
