using System.Collections;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Localization;
using UnityEngine.Tilemaps;

public class EventZoom : MonoBehaviour
{
    private Vector3 Origin;
    private Vector3 Difference;
    private Vector3 ResetCamera;
    private Camera _camera;
    public float zoomMin = 5f;
    public float zoomMax = 8f;
    public float speed = .01f;
    [SerializeField] private Tilemap tileMap;
    private GlobalMapInputManager _inputManager;
    private Coroutine zoomCoroutine;

    private void OnEnable()
    {
        _inputManager = new GlobalMapInputManager();
        _inputManager.Enable();
        _inputManager.MoveZoom.ZoomTouchContact.performed += ZoomStart;
        _inputManager.MoveZoom.ZoomTouchContact.canceled += ZoomStop;
        _inputManager.MoveZoom.MouseWheel.started += CameraZoom;
        // _inputManager.MoveZoom.MouseWheel.canceled += ZoomStop;
        // _inputManager.MoveZoom += MoveMap;
    }

    private void OnDisable()
    {
        _inputManager.MoveZoom.ZoomTouchContact.performed -= ZoomStart;
        _inputManager.MoveZoom.ZoomTouchContact.canceled -= ZoomStop;
        _inputManager.MoveZoom.MouseWheel.started -= CameraZoom;
        // _inputManager.MoveZoom.MouseWheel.canceled -= ZoomStop;
        // _inputManager.MoveZoom -= MoveMap;
        _inputManager.Disable();
    }
    private void Awake()
    {
        _camera = Camera.main;
    }

    void Start()
    {
        ResetCamera = _camera.transform.position;
    }

    // public async void OnClick(InputAction.CallbackContext context)
    // {
    //     if (context.performed)
    //     {
    //     }
    // }

    void zoom(float increment)
    {
        _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize - increment * speed, zoomMin, zoomMax);
    }

    void CameraZoom(InputAction.CallbackContext context)
    {
        zoom(context.ReadValue<Vector2>().y);
    }

    private void ZoomStart(InputAction.CallbackContext context)
    {
        if (!_inputManager.Zooming)
        {
            _inputManager.SetZooming(true);
            zoomCoroutine = StartCoroutine(ZoomDetection());
        }
    }

    private void ZoomStop(InputAction.CallbackContext context)
    {
        // StopCoroutine(zoomCoroutine);
        _inputManager.SetZooming(false);
    }

    private IEnumerator ZoomDetection()
    {
        float prevMagnitude = 0;

        // Vector2 touchOnePrev = Vector2.zero;
        // Vector2 touchTwoPrev = Vector2.zero;
        while (_inputManager.Zooming)
        {
            // Vector2 touchOne = _inputManager.MoveZoom.ZoomTouch1.ReadValue<Vector2>();
            // Vector2 touchTwo = _inputManager.MoveZoom.ZoomTouch2.ReadValue<Vector2>();

            // Vector2 touchOneDelta = touchOne - touchOnePrev;
            // Vector2 touchTwoDelta = touchTwo - touchTwoPrev;

            // Vector2 touchOnePrevPos = touchOne - touchOneDelta;
            // Vector2 touchTwoPrevPos = touchTwo - touchTwoDelta;

            // float prevMagnitude = (touchOnePrevPos - touchTwoPrevPos).magnitude;
            // float currentMagnitude = (touchOne - touchTwo).magnitude;

            // float difference = currentMagnitude - prevMagnitude;

            var magnitude = (_inputManager.MoveZoom.ZoomTouch1.ReadValue<Vector2>()
                - _inputManager.MoveZoom.ZoomTouch2.ReadValue<Vector2>()).magnitude;
            if (prevMagnitude == 0)
                prevMagnitude = magnitude;
            var difference = magnitude - prevMagnitude;
            prevMagnitude = magnitude;

            zoom(difference);
            Debug.Log("Zooming");
            yield return null;
        }
    }
}
