using System;
using System.Threading;

using Cysharp.Threading.Tasks;

using UnityEngine;
// using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class ArenaMonoBehavior : MonoBehaviour // , IPointerDownHandler
{
    // public UITown UITown;
    [SerializeField] protected ArenaEntity _arenaEntity;
    public ArenaEntity ArenaEntity => _arenaEntity;
    [NonSerialized] private Animator _animator;
    [NonSerialized] private Transform _model;
    [NonSerialized] private Transform _quantity;
    private Camera _camera;
    private Collider2D _collider;

    private CancellationTokenSource cancelTokenSource;
    private Vector2 _pos;
    private InputManager _inputManager;

    private void OnEnable()
    {
        _inputManager = new InputManager();
        _inputManager.ClickEntity += OnClick;
        _inputManager.Enable();
    }

    private void OnDisable()
    {
        _inputManager.ClickEntity -= OnClick;
        _inputManager.Disable();
    }
    #region Unity methods
    public void Awake()
    {
        // _camera = GameObject.FindGameObjectWithTag("ArenaCamera")?.GetComponent<Camera>();
        _animator = GetComponentInChildren<Animator>();
        _collider = GetComponentInChildren<Collider2D>();
        _model = transform.Find("Model");
        _quantity = transform.Find("Quantity");
    }
    protected void OnDestroy()
    {
        ((ScriptableAttributeCreature)ArenaEntity.Entity.ScriptableDataAttribute).ArenaModel.ReleaseInstance(gameObject);
    }

    // public void OnPointerDown(PointerEventData eventData)
    // {
    //     Debug.Log($"Click entity::: {ArenaEntity.Entity.ScriptableDataAttribute.name}");
    // }
    // // public async void OnPointerClick(PointerEventData eventData)
    // // {
    // //     Debug.Log($"Click entity {ArenaEntity.ConfigData.name}");
    // //     await UniTask.Delay(1);
    // // }

    // public void GetPosition(InputAction.CallbackContext context)
    // {
    //     _pos = context.ReadValue<Vector2>();
    // }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            var rayHit = Physics2D.GetRayIntersection(_camera.ScreenPointToRay(_inputManager.clickPosition()));
            if (!rayHit.collider) return;
            // Debug.Log($"OnClick::: {context.interaction} - {context.phase} - {rayHit.collider.gameObject}");
            if (rayHit.collider.gameObject == _model.gameObject)
            {
                if (context.interaction is PressInteraction || context.interaction is TapInteraction)
                {
                    Debug.Log($"Press::: {gameObject.name}");
                    OnDo(context);
                }
                else if (context.interaction is HoldInteraction)
                {
                    Debug.Log($"Hold::: {gameObject.name}");
                    ShowDialogInfo();
                }
            }
        }
    }

    // public void OnInteraction(InputAction.CallbackContext context)
    // {
    //     if (context.performed)
    //     {
    //         if (context.interaction is PressInteraction || context.interaction is TapInteraction)
    //         {
    //             Debug.Log($"OnInteraction::: {context.interaction} - {context.phase} - {context.ReadValue<Vector2>()}");
    //             OnDo(context);
    //         }

    //         else if (context.interaction is HoldInteraction)
    //         {
    //             Debug.Log($"OnInteraction::: {context.interaction} - {context.phase} - {context.ReadValue<Vector2>()}");
    //             OnDo(context);
    //         }
    //         // This exception should never get thrown, but in case it does then at least we know where it came from
    //         else throw new System.Exception("OnEscape received unrecognized button interaction '" + context.interaction + "'");
    //     }
    //     // switch (context.phase)
    //     // {
    //     //     case InputActionPhase.Started:
    //     //         if (context.interaction is HoldInteraction || context.interaction is SlowTapInteraction)
    //     //         {
    //     //             Debug.Log($"HoldStart::: {context.interaction} - {context.phase}");
    //     //         }
    //     //         else
    //     //         {
    //     //             Debug.Log($"Press(Tap)::: {context.interaction} - {context.phase} - {context.ReadValue<Vector2>()}");
    //     //             OnDo(context);
    //     //         }
    //     //         break;

    //     //     case InputActionPhase.Performed:
    //     //         if (context.interaction is HoldInteraction || context.interaction is SlowTapInteraction)
    //     //         {
    //     //             Debug.Log($"HoldRelease::: {context.interaction} - {context.phase}- {context.ReadValue<Vector2>()}");
    //     //             OnDo(context);
    //     //         }
    //     //         else
    //     //         {
    //     //             Debug.Log($"Press(Tap)::: {context.interaction} - {context.phase} - {context.ReadValue<Vector2>()}");
    //     //             OnDo(context);
    //     //         }
    //     //         break;

    //     //     case InputActionPhase.Canceled:
    //     //         Debug.Log($"Cancel::: {context.interaction} - {context.phase}");
    //     //         break;
    //     // }

    // }

    private async void ShowDialogInfo()
    {
        _inputManager.Disable();
        var dialogWindow = new UIInfoCreatureOperation((EntityCreature)_arenaEntity.Entity);
        var result = await dialogWindow.ShowAndHide();
        if (result.isOk)
        {

        }
        _inputManager.Enable();
    }

    private void OnDo(InputAction.CallbackContext context)
    {
        // var rayHit = Physics2D.GetRayIntersection(_camera.ScreenPointToRay(context.ReadValue<Vector2>()));
        // // Debug.Log($"{context.phase}::: {context.interaction} - {context.ReadValue<Vector2>()}");
        // if (!rayHit.collider) return;
        // if (rayHit.collider.gameObject != gameObject)
        // {
        //     return;
        // }
        // Debug.Log($"OnDo::: {context.interaction} - {context.phase}");
    }
    #endregion

    public void Init(ArenaEntity arenaEntity)
    {
        _camera = GameObject.FindGameObjectWithTag("ArenaCamera")?.GetComponent<Camera>();

        _arenaEntity = arenaEntity;
        // _collider.layerOverridePriority = 11 - _arenaEntity.OccupiedNode.position.y;
        gameObject.transform.localPosition = new Vector3(
            gameObject.transform.position.x,
            gameObject.transform.position.y,
            -(11 - _arenaEntity.OccupiedNode.position.y));

        if (ArenaEntity.PositionPrefab.x > 10f)
        {
            _model.transform.localScale = new Vector3(-1, 1, 1);
        }
        // for (int y = 0; y < transform.childCount; y++)
        // {
        //     // obj.TypeBuild = build.BuildLevels[i].TypeBuild;

        //     Transform child = transform.GetChild(y);
        //     if (null == child)
        //         continue;
        // }
    }

    public async UniTask StartMove()
    {
        var entityHero = (EntityCreature)_arenaEntity.Entity;
        // if (entityHero.IsExistPath)
        // {
        // if (entityHero.Data.path[0] == _mapObject.OccupiedNode) entityHero.Data.path.RemoveAt(0);
        cancelTokenSource = new CancellationTokenSource();
        await MoveHero(cancelTokenSource.Token);
        // }
    }
    private async UniTask MoveHero(CancellationToken cancellationToken)
    {
        var entityCreature = (EntityCreature)ArenaEntity.Entity;
        var entityData = (ScriptableAttributeCreature)entityCreature.ConfigAttribute;
        GridArenaNode prevNode = ArenaEntity.OccupiedNode;
        while (
            ArenaEntity.Path.Count > 0
            // && _canMove
            // && entityCreature.Data.mp >= 1
            && !cancellationToken.IsCancellationRequested
            )
        {
            var nodeTo = ArenaEntity.Path[0];
            ScriptableEntityMapObject configNodeData
                = (ScriptableEntityMapObject)nodeTo.OccupiedUnit?.ConfigData;

            // Debug.Log(nodeTo.position);
            Rotate(nodeTo);

            // // UpdateAnimate(ArenaEntity.PositionPrefab, nodeTo.position);
            // // _animator.SetBool("isWalking", true);
            // var difPos = entityData.CreatureParams.Size == 2
            //     ? new Vector3(nodeTo.position.x == 14 ? -0.5f : 0.5f, 0, 0)
            //     : Vector3.zero;
            // // var newPos = ArenaEntity.Path.Count == 1
            // // && ArenaEntity.Direction == TypeDirection.Right
            // // && nodeTo.cameFromNode != null
            // // && nodeTo.cameFromNode.position.y == nodeTo.position.y
            // //     ? (Vector3)nodeTo.cameFromNode.center + difPos
            // //     : (Vector3)nodeTo.center + difPos;
            await SmoothLerp(transform.position, (Vector3)nodeTo.center);

            // entityCreature.SetGuestForNode(nodeTo);

            ArenaEntity.Path.RemoveAt(0);
            if (ArenaEntity.Path.Count == 0)
            {
                ArenaEntity.ChangePosition(nodeTo);
            }

            // if (nodeTo.OccupiedUnit != null)
            // {
            //     // await nodeTo.OccupiedUnit.MapObjectGameObject.OnGoHero(MapObjectClass.Player);
            //     // await DoObject(nodeTo, nodeTo.OccupiedUnit.MapObjectGameObject, prevNode);
            //     cancelTokenSource.Cancel();
            //     cancelTokenSource.Dispose();
            //     // break;
            // }

            prevNode = nodeTo;
        }
        // _canMove = false;
        // _animator.SetBool("isWalking", false);
    }

    private void Rotate(GridArenaNode node)
    {
        var dir = ArenaEntity.Rotate(node);
        if (dir == TypeDirection.Right)
        {
            _model.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            _model.localScale = new Vector3(-1, 1, 1);
        }
        // if ()
        // var direction = currentNode, neighbourNode);
    }

    // public async UniTask DoObject(GridTileNode nodeTo, BaseMapEntity mapEntity, GridTileNode nodePrev)
    // {
    //     var entityCreature = (EntityCreature)ArenaEntity.Entity;

    //     _animator.SetBool("isWalking", false);

    //     entityCreature.ChangeHitHero(nodeTo, nodePrev);
    //     entityCreature.SetPathHero(null);
    // }

    private async UniTask SmoothLerp(Vector3 startPosition, Vector3 endPosition)
    {
        float time = LevelManager.Instance.ConfigGameSettings.speedHero;
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            // yield return null;
            await UniTask.Yield();
        }
    }

    public void UpdateAnimate(Vector3 startPosition, Vector3 endPosition)
    {
        if (_animator != null && startPosition != Vector3Int.zero && endPosition != Vector3Int.zero)
        {
            if (startPosition.x > endPosition.x)
            {
                _model.transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                _model.transform.localScale = new Vector3(1, 1, 1);
            }

            Vector3 direction = endPosition - startPosition;
            //Debug.Log($"Animator change::: {direction}");
            _animator.SetFloat("X", (float)direction.x);
            _animator.SetFloat("Y", (float)direction.y);

            _animator.SetBool("isWalking", true);
        }
    }

}
