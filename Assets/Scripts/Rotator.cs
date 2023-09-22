
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class Rotator : MonoBehaviour 
{
    [SerializeField] private InputAction pressed, axis;
    [SerializeField] private Transform _targert;

    private Transform cam;
    [SerializeField] private float speed = 1;
    [SerializeField] private bool inverted;
    private Vector2 rotation;
    private bool rotateAllowed;
    private Vector3 previousPosition;
    private void Awake() 
    {
        cam = Camera.main.transform;
        pressed.Enable();
        axis.Enable();
        pressed.performed += _ => { StartCoroutine(Rotate()); };
        pressed.canceled += _ => { rotateAllowed = false; };
        axis.performed += context => { rotation = context.ReadValue<Vector2>(); };	
    }

    private IEnumerator Rotate()
    {
        rotateAllowed = true;
        while(rotateAllowed)
        {
            // apply rotation
            transform.position = _targert.position;

            rotation *= speed;
            transform.Rotate(Vector3.up * (inverted? 1: -1), rotation.x, Space.World);
            transform.Rotate(cam.right * (inverted? -1: 1), rotation.y, Space.World);
            
            transform.Translate(new Vector3(0, 1, -8));

            
            yield return null;
        }
    }
}
