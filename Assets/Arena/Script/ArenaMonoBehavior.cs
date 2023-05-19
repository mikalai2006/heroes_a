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

    private string _nameCreature;

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
            if (rayHit.collider.gameObject == gameObject) // _model.gameObject
            {
                if (context.interaction is PressInteraction || context.interaction is TapInteraction)
                {
                    // Debug.Log($"Press::: {gameObject.name}");
                    ClickCreature(context);
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

    private void ClickCreature(InputAction.CallbackContext context)
    {
        ArenaEntity.ClickCreature();
        // Debug.Log($"Click {name}");
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

        if (ArenaEntity.CenterNode.x > 10f)
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
        _nameCreature = ArenaEntity.Entity.ScriptableDataAttribute.name.Split('_')?[1];
    }

    public async UniTask MoveCreature()
    {
        // Rotate(ArenaEntity.Path[0]);

        // _animator.SetBool("move", true);
        _animator.Play(string.Format("{0}{1}", _nameCreature, "StartMoving"), 0, 0f);
        if (ArenaEntity.Path[ArenaEntity.Path.Count - 1] != ArenaEntity.Path[0])
        {
            await UniTask.Delay(200);
            var entityHero = (EntityCreature)_arenaEntity.Entity;
            // if (entityHero.IsExistPath)
            // {
            // if (entityHero.Data.path[0] == _mapObject.OccupiedNode) entityHero.Data.path.RemoveAt(0);
            cancelTokenSource = new CancellationTokenSource();
            await MoveEntity(cancelTokenSource.Token);
        }
        NormalizeDirection();
        // }
    }
    private async UniTask MoveEntity(CancellationToken cancellationToken)
    {
        var entityCreature = (EntityCreature)ArenaEntity.Entity;
        var entityData = (ScriptableAttributeCreature)entityCreature.ConfigAttribute;

        var nodeEnd = ArenaEntity.Path[ArenaEntity.Path.Count - 1];
        _animator.Play(string.Format("{0}{1}", _nameCreature, "Moving"), 0, 0f);

        var difPos = entityData.CreatureParams.Size == 2
            ? new Vector3(ArenaEntity.TypeArenaPlayer == TypeArenaPlayer.Left ? -0.5f : 0.5f, 0, 0)
            : Vector3.zero;

        if (entityData.CreatureParams.Movement == MovementType.Flying)
        {
            UpdateDirection(ArenaEntity.PositionPrefab, nodeEnd.position, nodeEnd);
            await SmoothLerp(transform.position, nodeEnd.center + difPos, LevelManager.Instance.ConfigGameSettings.speedArenaAnimation * 2);
            ArenaEntity.Path.Clear();
        }
        else
        {
            while (ArenaEntity.Path.Count > 0 && !cancellationToken.IsCancellationRequested)
            {
                var nodeTo = ArenaEntity.Path[0];
                // ScriptableEntityMapObject configNodeData
                //     = (ScriptableEntityMapObject)nodeTo.OccupiedUnit?.ConfigData;

                //Rotate(nodeTo);

                UpdateDirection(ArenaEntity.PositionPrefab, nodeTo.position, nodeTo);

                await SmoothLerp(transform.position, nodeTo.center + difPos, LevelManager.Instance.ConfigGameSettings.speedArenaAnimation);

                ArenaEntity.Path.RemoveAt(0);
            }
        }

        if (ArenaEntity.Path.Count == 0)
        {
            ArenaEntity.ChangePosition(nodeEnd);
        }
        _animator.Play(string.Format("{0}{1}", _nameCreature, "StopMoving"), 0, 0f);

        await UniTask.Delay(200);

        _animator.Play(string.Format("{0}{1}", _nameCreature, "Idle"), 0, 0f);

    }

    // private void Rotate(GridArenaNode node)
    // {
    //     var dir = ArenaEntity.Rotate(node);
    //     if (dir == TypeDirection.Right)
    //     {
    //         _model.localScale = new Vector3(1, 1, 1);
    //     }
    //     else
    //     {
    //         _model.localScale = new Vector3(-1, 1, 1);
    //     }

    //     // _animator.SetBool("isRotate", true);
    //     // if ()
    //     // var direction = currentNode, neighbourNode);
    // }

    // public async UniTask DoObject(GridTileNode nodeTo, BaseMapEntity mapEntity, GridTileNode nodePrev)
    // {
    //     var entityCreature = (EntityCreature)ArenaEntity.Entity;

    //     _animator.SetBool("isWalking", false);

    //     entityCreature.ChangeHitHero(nodeTo, nodePrev);
    //     entityCreature.SetPathHero(null);
    // }

    private async UniTask SmoothLerp(Vector3 startPosition, Vector3 endPosition, float time)
    {
        // float time = LevelManager.Instance.ConfigGameSettings.speedArenaAnimation;
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            // yield return null;
            await UniTask.Yield();
        }
    }

    public void UpdateDirection(Vector3 startPosition, Vector3 endPosition, GridArenaNode node)
    {
        if (_animator != null && startPosition != Vector3Int.zero && endPosition != Vector3Int.zero)
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

            Vector3 direction = endPosition - startPosition;
            //Debug.Log($"Animator change::: {direction}");
            // _animator.SetFloat("X", (float)direction.x);
            // _animator.SetFloat("Y", direction.y);

            // _animator.SetBool("walk", true);
        }
    }

    private void NormalizeDirection()
    {
        if (ArenaEntity.TypeArenaPlayer == TypeArenaPlayer.Left)
        {
            _model.localScale = new Vector3(1, 1, 1);
            ArenaEntity.SetDirection(TypeDirection.Right);
        }
        else
        {
            _model.localScale = new Vector3(-1, 1, 1);
            ArenaEntity.SetDirection(TypeDirection.Left);
        }
    }

    internal async UniTask RunAttack(GridArenaNode nodeForAttack)
    {
        string nameAnimAttack = "AttackStraight";
        Vector3 difPos = _arenaEntity.OccupiedNode.center - nodeForAttack.center;

        if (difPos.x > 0 && difPos.y == 0)
        {
            nameAnimAttack = "AttackStraight";
            _model.localScale = new Vector3(-1, 1, 1);
        }
        else if (difPos.x < 0 && difPos.y == 0)
        {
            nameAnimAttack = "AttackStraight";
            _model.localScale = new Vector3(1, 1, 1);
        }
        else if (difPos.x != 0 && difPos.y > 0)
        {
            nameAnimAttack = "AttackDown";
        }
        else if (difPos.x != 0 && difPos.y < 0)
        {
            nameAnimAttack = "AttackUp";
        }

        Debug.Log($"Attack {_nameCreature}/{nameAnimAttack}");

        string nameAnimationAttack = string.Format("{0}{1}", _nameCreature, nameAnimAttack);

        _animator.Play(nameAnimationAttack, 0, 0f);
        await UniTask.Delay(100);
        await nodeForAttack.OccupiedUnit.ArenaMonoBehavior.RunDefense(_arenaEntity.OccupiedNode);

        // await UniTask.Delay(400);

        // _animator.Play(nameAnimationAttack, 0, 0f);

        // await UniTask.Delay(400);
        NormalizeDirection();
        _animator.Play(string.Format("{0}{1}", _nameCreature, "Idle"), 0, 0f);
        await UniTask.Delay(1);
    }


    internal async UniTask RunDefense(GridArenaNode nodeFromAttack)
    {
        Vector3 difPos = _arenaEntity.OccupiedNode.center - nodeFromAttack.center;

        if (difPos.x > 0)
        {
            _model.localScale = new Vector3(-1, 1, 1);
        }
        else if (difPos.x < 0)
        {
            _model.localScale = new Vector3(1, 1, 1);
        }

        string nameAnimDefend = "Defend";

        string nameAnimationAttack = string.Format("{0}{1}", _nameCreature, nameAnimDefend);

        _animator.Play(nameAnimationAttack, 0, 0f);

        await UniTask.Delay(400);

        NormalizeDirection();
        _animator.Play(string.Format("{0}{1}", _nameCreature, "Idle"), 0, 0f);
        await UniTask.Delay(1);
    }

    internal async UniTask RunAttackShoot(GridArenaNode nodeForAttack)
    {
        string nameAnimAttack = "ShootStraight";
        Vector3 difPos = _arenaEntity.OccupiedNode.center - nodeForAttack.center;

        if (difPos.x > 0 && difPos.y == 0)
        {
            nameAnimAttack = "ShootStraight";
            _model.localScale = new Vector3(-1, 1, 1);
        }
        else if (difPos.x < 0 && difPos.y == 0)
        {
            nameAnimAttack = "ShootStraight";
            _model.localScale = new Vector3(1, 1, 1);
        }
        else if (difPos.x != 0 && difPos.y > 0)
        {
            nameAnimAttack = "ShootDown";
        }
        else if (difPos.x != 0 && difPos.y < 0)
        {
            nameAnimAttack = "ShootUp";
        }

        string nameAnimationAttack = string.Format("{0}{1}", _nameCreature, nameAnimAttack);
        Debug.Log($"Shoot {nameAnimationAttack}");

        _animator.Play(nameAnimationAttack, 0, 0f);
        await UniTask.Delay(100);
        await nodeForAttack.OccupiedUnit.ArenaMonoBehavior.RunGettingHit(_arenaEntity.OccupiedNode);

        NormalizeDirection();
        _animator.Play(string.Format("{0}{1}", _nameCreature, "Idle"), 0, 0f);
        await UniTask.Delay(1);
    }
    internal async UniTask RunGettingHit(GridArenaNode nodeFromAttack)
    {
        // Vector3 difPos = _arenaEntity.OccupiedNode.center - nodeFromAttack.center;

        // if (difPos.x > 0)
        // {
        //     _model.localScale = new Vector3(-1, 1, 1);
        // }
        // else if (difPos.x < 0)
        // {
        //     _model.localScale = new Vector3(1, 1, 1);
        // }

        string nameAnimDefend = "GettingHit";

        string nameAnimationAttack = string.Format("{0}{1}", _nameCreature, nameAnimDefend);

        Debug.Log($"Shoot Defense {nameAnimationAttack}");
        _animator.Play(nameAnimationAttack, 0, 0f);

        await UniTask.Delay(400);

        // NormalizeDirection();
        _animator.Play(string.Format("{0}{1}", _nameCreature, "Idle"), 0, 0f);
        await UniTask.Delay(1);
    }
}
