
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class CameraBehaviour : MonoBehaviour 
{
    [SerializeField] private InputAction pressed, axis;
    [SerializeField] private Transform _targert;
    [SerializeField] private InputAction clicked;

    private Transform cam;
    [SerializeField] private float speedRotate = 0.3f;
    [SerializeField] private bool inverted;
    private Vector2 rotation;
    private bool rotateAllowed;
    private Vector3 previousPosition;
    
    [SerializeField] private float speedZoom = 0.01f;
    [SerializeField] private float minZoom = 20f;
    [SerializeField] private float maxZoom = 120f;

    private float previousMagnitude = 0;
    private int touchCount = 0;
    
    [SerializeField] private InputAction dragAction;
    [SerializeField] private float dragSpeed = 0.1f;
    private bool isDragging = false;
    private Vector2 lastDragPosition;
    
    private Vector2 touch0StartPos = Vector2.zero;
    private Vector2 touch1StartPos = Vector2.zero;
    private bool firstDrag = true;
    
    /// <summary>
    /// EDITOR Cube
    /// </summary>
    [SerializeField] private GameObject parent;
    [SerializeField] private GameObject child;
    [FormerlySerializedAs("spawnBlock")] [SerializeField] private bool editCube = false;


    public InputAction test;

    private void Awake()
    {
        SetZoom();
        SetRotate();
        SetDrag();
        clicked.Enable();
        clicked.performed += _ =>
        {
            ShootRay();
        };
    
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
            rotation = Vector2.zero;
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
            /*
            transform.position = _targert.position;
            */

            rotation *= speedRotate;
            _targert.transform.Rotate(Vector3.up * (inverted? 1: -1), rotation.x, Space.World);
            _targert.transform.Rotate(cam.right * (inverted? -1: 1), rotation.y, Space.World);
            
            /*
            transform.Translate(new Vector3(0, 1, -8));
            */

            
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
        
        touch0contact.performed += _ =>
        {
            touchCount++;
            touch0StartPos = touch0pos.ReadValue<Vector2>();
            touch1StartPos = touch1pos.ReadValue<Vector2>();
        };
        touch1contact.performed += _ => {
            touchCount++;
            touch0StartPos = touch0pos.ReadValue<Vector2>();
            touch1StartPos = touch1pos.ReadValue<Vector2>();
        };
        
        touch1pos.performed += _ =>
        {
            //zoom
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
            
            //drag
            Vector2 touch0CurrentPos = touch0pos.ReadValue<Vector2>();
            Vector2 touch1CurrentPos = touch1pos.ReadValue<Vector2>();

            Vector2 prevPos = (touch0StartPos + touch1StartPos) / 2;
            Vector2 currentPos = (touch0CurrentPos + touch1CurrentPos) / 2;
            if (firstDrag)
            {
                prevPos = currentPos;
                firstDrag = false;
            }
            Vector2 differenceDrag = currentPos - prevPos;

            Camera.main.transform.Translate(new Vector3(-differenceDrag.x * dragSpeed, -differenceDrag.y * dragSpeed, 0) * Time.deltaTime, Space.Self);

            touch0StartPos = touch0CurrentPos;
            touch1StartPos = touch1CurrentPos;
            
        };
    }
    
    private void CameraZoom(float increment) =>
        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView + increment, minZoom, maxZoom);
    
    private void SetDrag()
    {
        // For mouse
        dragAction = new InputAction(binding: "<Mouse>/middleButton");
        dragAction.Enable();
        dragAction.started += ctx => { isDragging = true; };
        dragAction.canceled += ctx => { isDragging = false; };
    }

    private void Update()
    {
        if (isDragging)
        {
            Vector2 currentDragPosition = Input.mousePosition;
            if (lastDragPosition == Vector2.zero)
                lastDragPosition = currentDragPosition;
            Vector2 difference = currentDragPosition - lastDragPosition;
            Camera.main.transform.Translate(new Vector3(-difference.x * dragSpeed, -difference.y * dragSpeed, 0) * Time.deltaTime, Space.Self);

            lastDragPosition = currentDragPosition;
        }
        else
        {
            lastDragPosition = Vector2.zero;
        }
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
                        else if (tapCube.IsBlock())
                        {
                            tapCube.TryMove();
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


