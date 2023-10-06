using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _targert;
    [SerializeField] private Vector3 _offset;
    private Vector3 previousPosition;
    // Start is called before the first frame update
    void Start()
    {
        _offset = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            previousPosition = _camera.ScreenToViewportPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 direction = previousPosition - _camera.ScreenToViewportPoint(Input.mousePosition);
            _camera.transform.position = _targert.position;
            _camera.transform.Rotate(new Vector3(1, 0, 0), direction.y * 180);
            _camera.transform.Rotate(new Vector3(0, 1, 0), -direction.x * 180, Space.World);
            _camera.transform.Translate(new Vector3(0, 0, -8));

            previousPosition = _camera.ScreenToViewportPoint(Input.mousePosition);
        }
    }
}
