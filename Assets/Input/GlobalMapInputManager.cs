using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GlobalMapInputManager
{
    public delegate void ArenaClickEvent(InputAction.CallbackContext context);
    private static GlobalMapInputController _input = new();
    public float timeClickNotDrag = .05f;
    public static float _timeDragging = 0.0f;
    public float TimeDragging => _timeDragging;
    private static bool _isDragging;
    public bool Dragging => _isDragging;
    private static bool _isZooming;
    public bool Zooming => _isZooming;
    public void SetZooming(bool status)
    {
        _isZooming = status;
    }
    public void SetDragging(bool status)
    {
        _isDragging = status;
    }
    public void SetTimeDragging(float value)
    {
        _timeDragging = value;
    }

    public GlobalMapInputController.MoveZoomActions MoveZoom => _input.MoveZoom;
    public void Enable()
    {
        _input.MapActions.Enable();
        _input.MoveZoom.Enable();
        Debug.Log("Enable InputSystem global map!");
    }

    public void Disable()
    {
        _input.MapActions.Disable();
        _input.MoveZoom.Disable();
        Debug.Log("Disable InputSystem global map!");
    }

    public event Action<InputAction.CallbackContext> Drag
    {
        add
        {
            _input.MapActions.Drag.performed += value;
            _input.MapActions.Drag.canceled += value;
        }
        remove
        {
            _input.MapActions.Drag.performed -= value;
            _input.MapActions.Drag.canceled -= value;
        }
    }
    public event Action<InputAction.CallbackContext> Click
    {
        add
        {
            _input.MapActions.Click.performed += value;
        }
        remove
        {
            _input.MapActions.Click.performed -= value;
        }
    }

    public Vector2 clickPosition()
    {
        return _input.MapActions.Position.ReadValue<Vector2>();
    }

    public bool ClickedOnUi()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = clickPosition();
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        foreach (var item in results)
        {
            if (item.gameObject.name == "Panel Settings")
            {
                return true;
            }
        }
        return false;
    }
}
