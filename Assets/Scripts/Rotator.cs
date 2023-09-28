
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Rotator : MonoBehaviour 
{
    [SerializeField] private InputAction pressed, axis;
    [SerializeField] private Transform _targert;

    private Transform cam;
    [SerializeField] private float speedRotate = 0.3f;
    [SerializeField] private bool inverted;
    private Vector2 rotation;
    private bool rotateAllowed;
    private Vector3 previousPosition;
    
    [SerializeField] public float speedZoom = 0.01f;

    private float previousMagnitude = 0;
    private int touchCount = 0;
    private void Awake() 
    {
        SetZoom();
        SetRotate();
    }

    private void SetRotate()
    {
        cam = Camera.main.transform;
        pressed.Enable();
        axis.Enable();
        pressed.performed += _ =>
        {
            if (touchCount > 1)
            {
                rotateAllowed = false;
                return;
            }
            StartCoroutine(Rotate());
        };
        pressed.canceled += _ => { rotateAllowed = false; };
        axis.performed += context =>
        {
            if (touchCount > 1)
            {
                rotateAllowed = false;
                return;
            }
            rotation = context.ReadValue<Vector2>();
        };	        
    }

    private IEnumerator Rotate()
    {
        rotateAllowed = true;
        while(rotateAllowed)
        {
            // apply rotation
            transform.position = _targert.position;

            rotation *= speedRotate;
            transform.Rotate(Vector3.up * (inverted? 1: -1), rotation.x, Space.World);
            transform.Rotate(cam.right * (inverted? -1: 1), rotation.y, Space.World);
            
            transform.Translate(new Vector3(0, 1, -8));

            
            yield return null;
        }
    }

    private void SetZoom()
    {
        var scrollAction = new InputAction(binding: "<Mouse>/scroll");
        scrollAction.Enable();
        scrollAction.performed += ctx =>CameraZoom(ctx.ReadValue<Vector2>().y * speedZoom);
        
        var touch0contact = new InputAction( type: InputActionType.Button,
            binding: "<Touchscreen>/touch0/press"
        );
        touch0contact.Enable();
        
        var touch1contact = new InputAction( type: InputActionType.Button,
            binding: "<Touchscreen>/touch1/press"
        );
        touch1contact.Enable();

        touch0contact.performed += _ => touchCount++;
        touch1contact.performed += _ => touchCount++;

        touch0contact.canceled += _ =>
        {
            touchCount--;
            previousMagnitude = 0;
        };
        touch1contact.canceled += _ =>
        {
            touchCount--;
            previousMagnitude = 0;
        };
        
        var touch0pos = new InputAction (type: InputActionType.Value,
            binding: "<TouchScreen>/touch0/position");
        touch0pos.Enable();
        
        var touch1pos = new InputAction (type: InputActionType.Value,
            binding: "<TouchScreen>/touch1/position");
        touch1pos.Enable();
        touch1pos.performed += _ =>
        {
            if (touchCount  < 2)
                return;
            var magnitude = (touch0pos.ReadValue<Vector2>() - touch1pos.ReadValue<Vector2>()).magnitude;
            if (previousMagnitude == 0)
            {
                previousMagnitude = magnitude;
            }
            var difference = magnitude - previousMagnitude;
            previousMagnitude = magnitude;
            CameraZoom(-difference * speedZoom);
        };
    }
    
    private void CameraZoom(float increment) =>
        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView + increment, 20, 100);
}
