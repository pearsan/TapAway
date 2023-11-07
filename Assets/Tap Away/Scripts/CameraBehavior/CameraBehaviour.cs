
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class CameraBehaviour : MonoBehaviour
{
    [SerializeField] private InputAction pressed, axis;
    [SerializeField] private Transform targert;
    [SerializeField] private InputAction clicked;

    private Transform _cam;
    [SerializeField] private float speedRotate = 0.3f;
    [SerializeField] private bool inverted;
    private Vector2 _rotation;
    private bool _rotateAllowed;
    private Vector3 _previousPosition;
    
    [SerializeField] private float speedZoom = 0.01f;
    [SerializeField] private float minZoom = 20f;
    [SerializeField] private float maxZoom = 120f;

    private float _previousMagnitude = 0;
    private int _touchCount = 0;
    
    [SerializeField] private InputAction dragAction;
    [SerializeField] private float dragSpeed = 0.1f;
    private bool _isDragging = false;
    private Vector2 _lastDragPosition;
    public float minCameraX, maxCameraX, minCameraY, maxCameraY;
    
    private Vector2 _touch0StartPos = Vector2.zero;
    private Vector2 _touch1StartPos = Vector2.zero;
    private bool _firstDrag = true;
    private bool _cameraEnable = true;
    private bool _behaviorOn = false;
    [SerializeField] private bool tapCube = true;
    
    [Header("Bombs")]
    [SerializeField] private bool bomb = true;

    [SerializeField] private float explodeRadius = 0.75f;
    [SerializeField] private float explodeForce = 500f;
    
    /// <summary>
    /// EDITOR Cube
    /// </summary>
    /// 
    [SerializeField] private GameObject parent;
    [SerializeField] private GameObject child;
    [FormerlySerializedAs("spawnBlock")] [SerializeField] private bool editCube = false;

    
    private void Awake()
    {
        clicked.Enable();
        clicked.performed += _ =>
        {
            if (editCube)
            {
                EditLevel();
            }
            else if (targert != null && GameplayManager.Instance.GetGameState() == GameplayManager.PLAYING_STATE && _cameraEnable)
            {
                ShootRay();
            }
        };
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void SetEnable()
    {
        SetRotate();
        SetZoom();
        SetDrag();
        _behaviorOn = true;
        _cameraEnable = true;
    }

    public void OnPause()
    {
        _cameraEnable = false;
    }
    public void OnPlay()
    {
        _cameraEnable = true;
    }

    public void SetDisable()
    {
        DisableDrag();
        _rotateAllowed = false;
        _cameraEnable = false;
    }

    private void SetRotate()
    {
        if (Camera.main != null) 
            _cam = Camera.main.transform;
        pressed.Enable();
        axis.Enable();
        pressed.performed += _ =>
        {
            if (_touchCount > 1)
            {
                _rotateAllowed = false;
                return;
            }
            _rotation = Vector2.zero;
            StartCoroutine(Rotate());
        };
        pressed.canceled += _ => { _rotateAllowed = false; };
        axis.performed += context =>
        {
            if (_touchCount > 1)
            {
                _rotateAllowed = false;
                return;
            }
            _rotation = context.ReadValue<Vector2>();
        };	        
    }

    private IEnumerator Rotate()
    {
        _rotateAllowed = true;
        while(_rotateAllowed && targert != null && _cameraEnable)
        {

            _rotation *= speedRotate;
            targert.transform.Rotate(Vector3.up * (inverted? 1: -1), _rotation.x, Space.World);
            targert.transform.Rotate(_cam.right * (inverted? -1: 1), _rotation.y, Space.World);
            
            yield return null;
        }
    }

    private void SetZoom()
    {

        var scrollAction = new InputAction(binding: "<Mouse>/scroll");
        scrollAction.Enable();
        scrollAction.performed += ctx =>CameraZoom(ctx.ReadValue<Vector2>().y * speedZoom);
        
        var touch0Contact = new InputAction( type: InputActionType.Button,
            binding: "<Touchscreen>/touch0/press"
        );
        touch0Contact.Enable();
        
        var touch1contact = new InputAction( type: InputActionType.Button,
            binding: "<Touchscreen>/touch1/press"
        );
        touch1contact.Enable();

        touch0Contact.canceled += _ =>
        {
            _touchCount--;
            _previousMagnitude = 0;
        };
        touch1contact.canceled += _ =>
        {
            _touchCount--;
            _previousMagnitude = 0;
        };
        
        var touch0Pos = new InputAction (type: InputActionType.Value,
            binding: "<TouchScreen>/touch0/position");
        touch0Pos.Enable();
        
        var touch1Pos = new InputAction (type: InputActionType.Value,
            binding: "<TouchScreen>/touch1/position");
        touch1Pos.Enable();
        
        touch0Contact.performed += _ =>
        {
            _touchCount++;
            _touch0StartPos = touch0Pos.ReadValue<Vector2>();
            _touch1StartPos = touch1Pos.ReadValue<Vector2>();
        };
        touch1contact.performed += _ => {
            _touchCount++;
            _touch0StartPos = touch0Pos.ReadValue<Vector2>();
            _touch1StartPos = touch1Pos.ReadValue<Vector2>();
        };
        
        touch1Pos.performed += _ =>
        {
            
            
            //drag
            if (_cameraEnable)
            {
                //zoom
                if (_touchCount  < 2)
                    return;
                var magnitude = (touch0Pos.ReadValue<Vector2>() - touch1Pos.ReadValue<Vector2>()).magnitude;
                if (_previousMagnitude == 0)
                {
                    _previousMagnitude = magnitude;
                }
                var difference = magnitude - _previousMagnitude;
                _previousMagnitude = magnitude;
                CameraZoom(-difference * speedZoom);
                
                Vector2 touch0CurrentPos = touch0Pos.ReadValue<Vector2>();
                Vector2 touch1CurrentPos = touch1Pos.ReadValue<Vector2>();

                Vector2 prevPos = (_touch0StartPos + _touch1StartPos) / 2;
                Vector2 currentPos = (touch0CurrentPos + touch1CurrentPos) / 2;
                if (_firstDrag)
                {
                    prevPos = currentPos;
                    _firstDrag = false;
                }
                Vector2 differenceDrag = currentPos - prevPos;
                Vector3 newPosition = Camera.main.transform.position + new Vector3(-differenceDrag.x * dragSpeed / 10, -differenceDrag.y * dragSpeed / 10, 0) * Time.deltaTime;
                newPosition.x = Mathf.Clamp(newPosition.x, minCameraX, maxCameraX);
                newPosition.y = Mathf.Clamp(newPosition.y, minCameraY, maxCameraY);
                Camera.main.transform.position = newPosition;

                _touch0StartPos = touch0CurrentPos;
                _touch1StartPos = touch1CurrentPos;
            }
        };
    }

    private void CameraZoom(float increment) =>
        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView + increment, minZoom, maxZoom);

    private void SetDrag()
    {
        // For mouse
        dragAction = new InputAction(binding: "<Mouse>/middleButton");
        dragAction.Enable();
        dragAction.started += ctx => { _isDragging = true; };
        dragAction.canceled += ctx => { _isDragging = false; };
    }

    private void DisableDrag()
    {
        dragAction.Disable();
    }

    private void FixedUpdate()
    {
        if (_isDragging)
        {
            Vector2 currentDragPosition = Input.mousePosition;
            if (_lastDragPosition == Vector2.zero)
                _lastDragPosition = currentDragPosition;
            Vector2 difference = currentDragPosition - _lastDragPosition;
            Vector3 localNewPosition = new Vector3(-difference.x * dragSpeed / 10, -difference.y * dragSpeed / 10, 0) * Time.deltaTime;

            // Apply the new position in local space
            Camera.main.transform.Translate(localNewPosition, Space.Self);

            // Get local position
            Vector3 localPos = Camera.main.transform.localPosition;

            // Clamp local X and Y position
            localPos.x = Mathf.Clamp(localPos.x, minCameraX, maxCameraX);
            localPos.y = Mathf.Clamp(localPos.y, minCameraY, maxCameraY);
            localPos.z = -15;
            // Set local position
            Camera.main.transform.localPosition = localPos;
            
            _lastDragPosition = currentDragPosition;
        }
        else
        {
            _lastDragPosition = Vector2.zero;
        }
    }
    private void ShootRay()
    {
        RaycastHit hit;
        Ray ray;
        
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            
            if (hit.collider != null)
            {
                ITappable tapCube = hit.collider.gameObject.GetComponent<ITappable>();
                if (tapCube != null && GameplayManager.Instance.GetMoveAttemps() > 0)
                {
                    if (this.tapCube)
                    {
                        if (GameplayManager.Instance.GetCurrentStage() > 2)
                            GameplayManager.Instance.MinusMoveAttemps();

                        SoundManager.Instance.TapCube();
                        tapCube.Tap();                        
                    }
                    else if (bomb)
                    {
                        int layerMask = 1 << LayerMask.NameToLayer("Cube");
                        Collider[] hitColliders = Physics.OverlapSphere( hit.point, explodeRadius, layerMask);
                        foreach (Collider hitCollider in hitColliders)
                        {
                            IExplodable cube = hitCollider.GetComponent<IExplodable>();
                            if (cube != null) // If the object has a TapCube component
                            {
                                cube.Explode(hit.collider.transform.position, explodeForce, explodeRadius);

                            }
                        }
                    }


                    #region tutorial
                    if (GameplayManager.Instance.GetCurrentStage() == 2)
                    {
                        Transform hitObject = hit.collider.transform;
                        if (hitObject.childCount > 1)
                        {

                            if (hitObject.GetChild(1).gameObject.activeInHierarchy)
                            { 
                                hitObject.GetComponentInChildren<PointerAnimation>().gameObject.SetActive(false);
                                TutorialManager.Instance.ChangeStep(GameplayManager.Instance.GetCurrentStage());
                            }
                            else
                            {
                                TutorialManager.Instance._current.Remove(hitObject.GetChild(1).gameObject);
                            }
                        }
                    }
                    else if (GameplayManager.Instance.GetCurrentStage() < 2)
                    {
                        TutorialManager.Instance.ChangeStep(GameplayManager.Instance.GetCurrentStage());
                    }
                    #endregion
                    
                    if (GameplayManager.Instance.CheckIfLose())
                    {
                        GameplayManager.Instance.OnTriggerLose();
                    }

                    if (GameplayManager.Instance.CheckIfWin())
                    {
                        GameplayManager.Instance.OnTriggerWin();
                    }
                }
            }
        }
    }

    private void EditLevel()
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
                    
                    Vector3 positionToInstantiate = tapCube.transform.position + hit.normal;
                    GameObject newCube = Instantiate(child);
                    newCube.transform.SetParent(parent.transform);
                    newCube.transform.position = positionToInstantiate;
                    newCube.transform.localRotation = tapCube.transform.localRotation;
                    
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
                    Destroy(tapCube.gameObject);
  
                }
            }
        }
    }

    public void SetTargert(Transform targert)
    {
        this.targert = targert;
    }

    public bool CameraIsOn()
    {
        return _behaviorOn;
        
    }
}


