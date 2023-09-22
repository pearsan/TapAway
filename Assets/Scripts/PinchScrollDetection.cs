using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PinchScrollDetection : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed = 0.01f;

    private float previousMagnitude = 0;
    private int touchCount = 0;
    public InputAction test;
    void Start()
    {
        var scrollAction = new InputAction(binding: "<Mouse>/scroll");
        scrollAction.Enable();
        scrollAction.performed += ctx =>CameraZoom(ctx.ReadValue<Vector2>().y * speed);
        
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
            CameraZoom(difference * speed);
        };
    }

    // Update is called once per frame
    private void CameraZoom(float increment) =>
        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView + increment, 20, 100);
}
