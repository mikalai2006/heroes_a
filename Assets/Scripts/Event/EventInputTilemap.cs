using System.Collections;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Localization;
using UnityEngine.Tilemaps;

public class EventInputTilemap : MonoBehaviour
{
    private Vector3 Origin;
    private Vector3 Difference;
    private Vector3 ResetCamera;
    private Camera _camera;
    private float timeDragging = 0.0f;
    private float timeClickNotDrag = .05f;
    private float timeDragDelay = .1f;
    private int SizeEmptyEdge = 3;
    private float timeClickPrev;
    [SerializeField] private Tilemap tileMap;
    private GlobalMapInputManager _inputManager;
    private Coroutine dragCoroutine;
    [SerializeField] private UIGameAside ignoreUI;

    private void OnEnable()
    {
        _inputManager = new GlobalMapInputManager();
        _inputManager.Enable();
        _inputManager.Click += OnClick;
        _inputManager.Drag += DragMap;
    }

    private void OnDisable()
    {
        _inputManager.Click -= OnClick;
        _inputManager.Drag -= DragMap;
        _inputManager.Disable();
    }
    private void Awake()
    {
        _camera = Camera.main;

    }

    void Start()
    {
        ResetCamera = _camera.transform.position;
        timeClickPrev = Time.realtimeSinceStartup;


    }

    public Bounds GetTilemapBounds()
    {
        return tileMap.localBounds;
    }
    private Vector3 ClampCamera(Vector3 targetPosition)
    {
        Vector3 newTransformPosition = new Vector3(0, 0, -10);

        var yDelta = targetPosition.y;

        yDelta = Mathf.Clamp(yDelta, GetBottomMapBoundary(), GetTopMapBoundary());

        newTransformPosition.y = yDelta;

        var xDelta = targetPosition.x;

        xDelta = Mathf.Clamp(xDelta, GetLeftMapBoundary(), GetRightMapBoundary());

        newTransformPosition.x = xDelta;

        return newTransformPosition;
    }

    private float GetEmptySpace()
    {
        return _camera.aspect * SizeEmptyEdge;
    }
    public float GetLeftMapBoundary()
    {
        return GetTilemapBounds().min.x + GetCameraHorizontalExtent() - GetEmptySpace();
    }
    public float GetRightMapBoundary()
    {
        return GetTilemapBounds().max.x - GetCameraHorizontalExtent() + GetEmptySpace();
    }
    public float GetBottomMapBoundary()
    {
        return GetTilemapBounds().min.y + GetCameraVerticalExtent() - GetEmptySpace();
    }
    public float GetTopMapBoundary()
    {
        return GetTilemapBounds().max.y - GetCameraVerticalExtent() + GetEmptySpace();
    }
    private float GetCameraVerticalExtent()
    {

        return _camera.orthographicSize;
    }
    private float GetCameraHorizontalExtent()
    {
        return _camera.aspect * GetCameraVerticalExtent();
    }


    public void OnClick(InputAction.CallbackContext context)
    {
        if (
            context.performed
            && timeDragging < timeClickNotDrag
            && !_inputManager.ClickedOnUi()
            && !_inputManager.Zooming
            && !_inputManager.Dragging
            )
        {
            var pos = _inputManager.clickPosition();//context.ReadValue<Vector2>();
            var rayHit = Physics2D.GetRayIntersection(_camera.ScreenPointToRay(pos));

            if (!rayHit.collider) return;

            Vector2 posMouse = _camera.ScreenToWorldPoint(pos);
            Vector3Int tilePos = tileMap.WorldToCell(posMouse);

            if (rayHit.collider.gameObject == tileMap.gameObject)
            {

                if (context.interaction is PressInteraction || context.interaction is TapInteraction)
                {
                    Debug.Log($"{context.interaction},{tilePos}");
                    GameManager.Instance.MapManager.ChangePath(tilePos);
                    // if (LevelManager.Instance.ActivePlayer.ActiveHero != null)
                    // {
                    //     if (prevClickPosition == tilePos)
                    //     {
                    //         // move active hero.
                    //         GameManager.Instance.ChangeState(GameState.StartMoveHero);
                    //         prevClickPosition = Vector3Int.zero;
                    //     }
                    //     else
                    //     {
                    //         // find path.
                    //         GameManager.Instance.MapManager.ChangePath(tilePos);
                    //         prevClickPosition = tilePos;
                    //     }
                    // }
                    // else
                    // {
                    //     var dialogData = new DataDialogHelp()
                    //     {
                    //         Header = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "Help").GetLocalizedString(),
                    //         Description = new LocalizedString(Constants.LanguageTable.LANG_TABLE_ADVENTURE, "needchoosehero").GetLocalizedString(),
                    //     };

                    //     var dialogWindow = new DialogHelpProvider(dialogData);
                    //     await dialogWindow.ShowAndHide();
                    // }
                }
                else if (context.interaction is HoldInteraction)
                {
                    Debug.Log($"Hold action of ::: {gameObject.name}");
                    // GameManager.Instance.ChangeState(GameState.StartMoveHero);
                }
            }
        }
    }

    private void DragMap(InputAction.CallbackContext context)
    {
        if (
            context.performed
            && !_inputManager.Dragging
            && !_inputManager.Zooming
            && !_inputManager.ClickedOnUi()
            )
        {
            var pos = _inputManager.clickPosition();
            var rayHit = Physics2D.GetRayIntersection(_camera.ScreenPointToRay(pos));

            if (!rayHit.collider) return;

            // Vector2 posMouse = _camera.ScreenToWorldPoint(pos);
            // Vector3Int tilePos = tileMap.WorldToCell(posMouse);

            if (rayHit.collider.gameObject == tileMap.gameObject)
            {
                _inputManager.SetDragging(true);
                // Debug.Log("Start Dragging!");
                OnStartDragTilemap();
                dragCoroutine = StartCoroutine(Drag());
            }
        }

        if ((context.canceled && _inputManager.Dragging) || _inputManager.Zooming)
        {
            _inputManager.SetDragging(false);
            StopCoroutine(dragCoroutine);
            // Debug.Log("Stop Dragging!");
            StartCoroutine(StopDrag());
        }
    }

    private void OnStartDragTilemap()
    {
        Origin = _camera.ScreenToWorldPoint(_inputManager.clickPosition());
    }
    private void OnDragTilemap()
    {
        Difference = Origin - _camera.ScreenToWorldPoint(_inputManager.clickPosition());
        if (Difference != Vector3.zero)
        {
            timeDragging += Time.deltaTime;
            // Debug.Log($"Dragging {Difference} / time {timeDragging} , distance{Vector2.Distance(Difference, currentPos)}!");
            _camera.transform.position = ClampCamera(_camera.transform.position + Difference);
        }
    }
    private IEnumerator Drag()
    {
        yield return new WaitForSeconds(timeDragDelay);
        while (true)
        {
            OnDragTilemap();
            yield return null;
        }
    }

    private IEnumerator StopDrag()
    {
        if (timeDragging > 0)
        {
            yield return new WaitForSeconds(.21f);
        }
        timeDragging = 0;
    }
}
