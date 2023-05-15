using System;

using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager
{
    public delegate void ArenaClickEvent(InputAction.CallbackContext context);
    public event ArenaClickEvent OnClickArena;
    private static ArenaInputController _input = new();

    public void Enable()
    {
        _input.ArenaActions.Enable();
        _input.ArenaEntity.Enable();
    }

    public void Disable()
    {
        _input.ArenaActions.Disable();
        _input.ArenaEntity.Disable();
    }

    public event Action<InputAction.CallbackContext> Click
    {
        add
        {
            _input.ArenaActions.Click.performed += value;
        }
        remove
        {
            _input.ArenaActions.Click.performed -= value;
        }
    }
    public event Action<InputAction.CallbackContext> ClickEntity
    {
        add
        {
            _input.ArenaEntity.Click.performed += value;
        }
        remove
        {
            _input.ArenaEntity.Click.performed -= value;
        }
    }
    public Vector2 clickPosition()
    {
        return _input.ArenaEntity.Position.ReadValue<Vector2>();
    }
    // void Start()
    // {
    //     _input.ArenaActions.Click.started += ctx => ArenaClick(ctx);
    // }


    // private void ArenaClick(InputAction.CallbackContext context)
    // {
    //     if (OnClickArena != null)
    //     {
    //         // if (context.performed)
    //         // {
    //         //     var rayHit = Physics2D.GetRayIntersection(_camera.ScreenPointToRay(context.ReadValue<Vector2>()));
    //         //     if (!rayHit.collider) return;

    //         //     Debug.Log($"OnClick::: {rayHit.collider.gameObject.name}");
    //         //     OnClickArena(context);
    //         //     // if (rayHit.collider.gameObject == gameObject)
    //         //     // {
    //         //     // }
    //         // }
    //         Debug.Log($"OnClick Arena");
    //         OnClickArena(context);
    //     }
    // }
}
