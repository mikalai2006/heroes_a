using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using System;

[RequireComponent(typeof(EventTrigger))]
public class EventArena : MonoBehaviour
{
    [SerializeField] private Tilemap _arenaGrid;
    [SerializeField] private Camera _camera;
    [SerializeField] private ArenaManager _arenaManager;

    // public void OnClick(InputAction.CallbackContext context)
    // {
    //     // if (!context.started) return;

    //     if (context.performed)
    //     {
    //         var rayHit = Physics2D.GetRayIntersection(_camera.ScreenPointToRay(inputController.Player.TouchPosition.ReadValue<Vector2>()));

    //         if (!rayHit.collider) return;

    //         if (rayHit.collider.gameObject == _arenaGrid.gameObject)
    //         {
    //             Debug.Log($"Click grid Arena::: phase {context.phase} / {rayHit.collider.gameObject.name}");
    //         }
    //     }
    // }

    public void ClickTilemap(PointerEventData data)
    {
        if (data.dragging)
        {
            //Debug.Log($"Dragging {Input.touchCount}");
            return;
        }
        else
        {
            // if (Time.realtimeSinceStartup - timeClickPrev < LevelManager.Instance.ConfigGameSettings.deltaDoubleClick)
            // {
            //     // move active hero.
            //     GameManager.Instance.ChangeState(GameState.StartMoveHero);
            // }
            // else
            // {
            Vector2 posMouse = _camera.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int tilePos = _arenaGrid.WorldToCell(posMouse);

            Debug.Log($"Click node::: {tilePos}");
            _arenaManager.ClickArena(tilePos);
            // timeClickPrev = Time.realtimeSinceStartup;
            // }
        }
    }

    // private void LateUpdate()
    // {

    //     if (Input.touchCount == 2)
    //     {
    //         Touch touchZero = Input.GetTouch(0);
    //         Touch touchOne = Input.GetTouch(1);

    //         Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
    //         Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

    //         float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
    //         float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

    //         float difference = currentMagnitude - prevMagnitude;
    //     }
    //     else if (Input.mouseScrollDelta != Vector2.zero)
    //     {
    //         var newPosCamera = _camera.transform.position;
    //         if (_camera.transform.position != newPosCamera)
    //         {
    //             _camera.transform.position = newPosCamera;
    //         }
    //     }
    // }

}
