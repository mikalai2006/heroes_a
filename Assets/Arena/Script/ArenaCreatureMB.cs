using System;
using System.Threading;

using Cysharp.Threading.Tasks;

using TMPro;

using UnityEngine;
// using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class ArenaCreatureMB : MonoBehaviour // , IPointerDownHandler
{
    // public UITown UITown;
    [SerializeField] protected ArenaCreature _arenaEntity;
    public ArenaCreature ArenaEntity => _arenaEntity;
    [NonSerialized] private Animator _animator;
    [NonSerialized] private Transform _model;
    [NonSerialized] private Transform _quantity;
    private TextMeshProUGUI _quantityText;
    private Camera _camera;
    private Collider2D _collider;
    [SerializeField] public GameObject ShootPrefab;
    private GameObject _shoot;

    private string _nameCreature;

    private CancellationTokenSource cancelTokenSource;
    private Vector2 _pos;
    private InputManager _inputManager;
    private float _speedAnimation;

    #region Unity methods
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
    public void Awake()
    {
        ArenaCreature.OnChangeParamsCreature += RefreshQuantity;
        ArenaQueue.OnNextRound += NextRound;

        if (ShootPrefab != null)
        {
            _shoot = GameObject.Instantiate(ShootPrefab, gameObject.transform.position, Quaternion.identity, transform);
            _shoot.SetActive(false);
        }
        // _camera = GameObject.FindGameObjectWithTag("ArenaCamera")?.GetComponent<Camera>();
        _animator = GetComponentInChildren<Animator>();
        _collider = GetComponentInChildren<Collider2D>();
        _model = transform.Find("Model");
        _quantity = transform.Find("Quantity");
        _quantityText = _quantity.gameObject.transform.GetComponentInChildren<TMPro.TextMeshProUGUI>();
    }

    protected void OnDestroy()
    {
        ArenaCreature.OnChangeParamsCreature -= RefreshQuantity;
        ArenaQueue.OnNextRound -= NextRound;
        if (ArenaEntity.Entity != null)
        {
            // Debug.Log($"ArenaEntity.Entity{ArenaEntity.Entity.ScriptableDataAttribute.name}");
            ((EntityCreature)ArenaEntity.Entity).ConfigAttribute.ArenaModel.ReleaseInstance(gameObject);
            if (_shoot != null) Destroy(_shoot);
        }
    }

    private void NextRound()
    {
        if (!_arenaEntity.Death)
        {
            _arenaEntity.SetRoundData();
        }
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

    public async void ShowDialogInfo()
    {
        _inputManager.Disable();
        var dialogWindow = new UIInfoCreatureArenaOperation(_arenaEntity);
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

    public void Init(ArenaCreature arenaEntity)
    {
        _speedAnimation = LevelManager.Instance.ConfigGameSettings.speedArenaAnimation;
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
        var splitName = ArenaEntity.Entity.ScriptableDataAttribute.name.Split('_');
        _nameCreature = splitName.Length > 1 ? splitName[1] : splitName[0];

        RefreshQuantity();
    }

    private void RefreshQuantity()
    {
        int value = _arenaEntity.Data.quantity;
        _quantityText.text = value.ToString();
        if (value <= 0)
        {
            _quantity.gameObject.SetActive(false);
        }
        else
        {
            _quantity.gameObject.SetActive(true);
        }
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
        GridArenaNode nodeDoor = null;

        var nodeStart = ArenaEntity.Path[0];
        var nodeEnd = ArenaEntity.Path[ArenaEntity.Path.Count - 1];
        _animator.Play(string.Format("{0}{1}", _nameCreature, "Moving"), 0, 0f);

        var difPos = entityData.CreatureParams.Size == 2
            ? new Vector3(ArenaEntity.TypeArenaPlayer == TypeArenaPlayer.Left ? -0.5f : 0.5f, 0, 0)
            : Vector3.zero;

        if (entityData.CreatureParams.Movement == MovementType.Flying)
        {
            var time = _speedAnimation * ArenaEntity.Path.Count;

            // Open Door.
            if (
                nodeEnd.StateArenaNode.HasFlag(StateArenaNode.Door)
                || (
                    nodeEnd.RightNode != null
                    && nodeEnd.RightNode.StateArenaNode.HasFlag(StateArenaNode.Door)
                    && ArenaEntity.TypeArenaPlayer == TypeArenaPlayer.Right
                    )
                )
            {
                await ArenaEntity.OpenDoor();
                if (nodeEnd.StateArenaNode.HasFlag(StateArenaNode.Door))
                {
                    nodeDoor = nodeEnd;
                    nodeEnd.StateArenaNode &= ~StateArenaNode.Door;
                }
                else if (nodeEnd.RightNode.StateArenaNode.HasFlag(StateArenaNode.Door))
                {
                    nodeDoor = nodeEnd.RightNode;
                    nodeEnd.RightNode.StateArenaNode &= ~StateArenaNode.Door;
                }
            }

            UpdateDirection(ArenaEntity.PositionPrefab, nodeEnd.position, nodeEnd);
            await SmoothLerp(transform.position, nodeEnd.center + difPos, time);
            ArenaEntity.Path.Clear();
        }
        else
        {
            var time = _speedAnimation;

            while (ArenaEntity.Path.Count > 0 && !cancellationToken.IsCancellationRequested)
            {
                var nodeTo = ArenaEntity.Path[0];
                nodeEnd = nodeTo;

                // Open Door.
                if (
                    nodeTo.StateArenaNode.HasFlag(StateArenaNode.Door)
                    || (
                        nodeTo.RightNode != null
                        && nodeTo.RightNode.StateArenaNode.HasFlag(StateArenaNode.Door)
                        && ArenaEntity.TypeArenaPlayer == TypeArenaPlayer.Right
                        )
                    )
                {
                    await ArenaEntity.OpenDoor();
                    if (nodeEnd.StateArenaNode.HasFlag(StateArenaNode.Door))
                    {
                        nodeDoor = nodeTo;
                        nodeTo.StateArenaNode &= ~StateArenaNode.Door;
                    }
                    else if (nodeTo.RightNode.StateArenaNode.HasFlag(StateArenaNode.Door))
                    {
                        nodeDoor = nodeTo.RightNode;
                        nodeTo.RightNode.StateArenaNode &= ~StateArenaNode.Door;
                    }
                }

                // ScriptableEntityMapObject configNodeData
                //     = (ScriptableEntityMapObject)nodeTo.OccupiedUnit?.ConfigData;

                //Rotate(nodeTo);

                UpdateDirection(ArenaEntity.PositionPrefab, nodeTo.position, nodeTo);

                await SmoothLerp(transform.position, nodeTo.center + difPos, time);
                await UniTask.Yield();

                // if (
                //     !nodeTo.StateArenaNode.HasFlag(StateArenaNode.Door)
                //     && ArenaEntity.man
                //     // && (nodeTo.RightNode != null && nodeTo.RightNode.StateArenaNode.HasFlag(StateArenaNode.Door))
                //     )
                // {
                //     ArenaEntity.CloseDoor();
                // }

                ArenaEntity.Path.RemoveAt(0);

                if (nodeTo.SpellsState.Count > 0 && nodeTo != nodeStart)
                {
                    foreach (var spell in nodeTo.SpellsState)
                    {
                        await spell.Key.RunEffect(ArenaEntity.OccupiedNode, ArenaEntity.Hero, nodeTo);
                    }
                }
            }
        }

        if (ArenaEntity.Path.Count == 0)
        {
            ArenaEntity.ChangePosition(nodeEnd);
        }

        // Check Door.
        if (
            nodeDoor != null
            && !nodeEnd.StateArenaNode.HasFlag(StateArenaNode.Door)
            && nodeEnd.RightNode != null
            && !nodeEnd.RightNode.StateArenaNode.HasFlag(StateArenaNode.Door)
            )
        {
            ArenaEntity.CloseDoor();
            nodeDoor.StateArenaNode |= StateArenaNode.Door;
        }

        await UniTask.Yield();
        _animator.Play(string.Format("{0}{1}", _nameCreature, "StopMoving"), 0, 0f);

        await UniTask.Delay(200);

        _animator.Play(string.Format("{0}{1}", _nameCreature, "Idle"), 0, 0f);
    }

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
        transform.position = endPosition;
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

    internal async UniTask RunAttack(GridArenaNode nodeFromAttack, GridArenaNode nodeToAttack)
    {
        // Rotate
        Vector3 difOccupied = _arenaEntity.OccupiedNode.center - nodeToAttack.center;
        Vector3 difRelated = nodeToAttack.OccupiedUnit.RelatedNode != null
            ? _arenaEntity.OccupiedNode.center - nodeToAttack.OccupiedUnit.RelatedNode.center
            : Vector3.zero;

        // Debug.Log($"Center::: occupied{_arenaEntity.OccupiedNode.center} ::: nodeToAttack={nodeToAttack.center} | related={nodeToAttack.OccupiedUnit.RelatedNode.center}");
        // Debug.Log($"Dif:::  {difOccupied} | {difRelated}");
        if (
            difOccupied.x > 0f
            && difRelated.x >= 0f
            && _arenaEntity.TypeArenaPlayer == TypeArenaPlayer.Left
            )
        {
            _model.localScale = new Vector3(-1, 1, 1);
        }
        else if (
            difOccupied.x < 0f
            && difRelated.x <= 0f
            && _arenaEntity.TypeArenaPlayer == TypeArenaPlayer.Right
        )
        {
            _model.localScale = new Vector3(1, 1, 1);
        }

        // Animate
        string nameAnimAttack = "AttackStraight";
        if (difOccupied.x > 0 && difOccupied.y == 0)
        {
            nameAnimAttack = "AttackStraight";
            // _model.localScale = new Vector3(-1, 1, 1);
        }
        else if (difOccupied.x < 0 && difOccupied.y == 0)
        {
            nameAnimAttack = "AttackStraight";
            // _model.localScale = new Vector3(1, 1, 1);
        }
        else if (difOccupied.x != 0 && difOccupied.y > 0)
        {
            nameAnimAttack = "AttackDown";
        }
        else if (difOccupied.x != 0 && difOccupied.y < 0)
        {
            nameAnimAttack = "AttackUp";
        }

        string nameAnimationAttack = string.Format("{0}{1}", _nameCreature, nameAnimAttack);

        _animator.Play(nameAnimationAttack, 0, 0f);
        await UniTask.Delay(200);

        await nodeToAttack.OccupiedUnit.RunGettingHit(_arenaEntity.OccupiedNode);

        NormalizeDirection();
        _animator.Play(string.Format("{0}{1}", _nameCreature, "Idle"), 0, 0f);
    }

    internal async UniTask RunGettingHit(GridArenaNode nodeFromAttack)
    {
        // Rotate.
        Vector3 difOccupied = _arenaEntity.OccupiedNode.center - nodeFromAttack.center;
        Vector3 difRelated = _arenaEntity.RelatedNode != null
            ? _arenaEntity.RelatedNode.center - nodeFromAttack.center
            : Vector3.zero;

        // Debug.Log($"Dif:::  {difOccupied} | {difRelated}");
        if (
            difOccupied.x > 0f
            && difRelated.x >= 0f
            && _arenaEntity.TypeArenaPlayer == TypeArenaPlayer.Left
        )
        {
            _model.localScale = new Vector3(-1, 1, 1);
        }
        else if (
            difOccupied.x < 0f
            && difRelated.x <= 0f
            && _arenaEntity.TypeArenaPlayer == TypeArenaPlayer.Right
        )
        {
            _model.localScale = new Vector3(1, 1, 1);
        }

        // Animate.
        string nameAnimDefend = "GettingHit";
        if (_arenaEntity.Data.isDefense)
        {
            nameAnimDefend = "Defend";
        }

        string nameAnimationAttack = string.Format("{0}{1}", _nameCreature, nameAnimDefend);

        _animator.Play(nameAnimationAttack, 0, 0f);

        await UniTask.Delay(200);

        NormalizeDirection();
        _animator.Play(string.Format("{0}{1}", _nameCreature, "Idle"), 0, 0f);
    }


    internal async UniTask RunAttackShoot(GridArenaNode nodeForAttack, Transform positionGameObejct = null)
    {
        string nameAnimAttack = "ShootStraight";
        Vector3 positionToAttack = nodeForAttack != null ? nodeForAttack.center : positionGameObejct.position;
        Vector3 difPos = _arenaEntity.OccupiedNode.center - positionToAttack;

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

        _shoot.SetActive(true);
        await SmoothLerpShoot(transform.position, positionToAttack, _speedAnimation * 2);
        _shoot.SetActive(false);

        await UniTask.Delay(200);

        if (nodeForAttack != null) await nodeForAttack.OccupiedUnit.RunGettingHit(_arenaEntity.OccupiedNode);
        // if (_arenaEntity.Data.isDefense)
        // {
        // }
        // else
        // {
        //     await nodeForAttack.OccupiedUnit.ArenaMonoBehavior.RunGettingHit(_arenaEntity.OccupiedNode);
        // }

        NormalizeDirection();
        _animator.Play(string.Format("{0}{1}", _nameCreature, "Idle"), 0, 0f);
    }

    private async UniTask SmoothLerpShoot(Vector3 startPosition, Vector3 endPosition, float time)
    {

        Vector3 difPos = startPosition - endPosition;

        if (difPos.x > 0)
        {
            _shoot.transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (difPos.x < 0)
        {
            _shoot.transform.localScale = new Vector3(1, 1, 1);
        }

        // float time = LevelManager.Instance.ConfigGameSettings.speedArenaAnimation;
        startPosition = startPosition + new Vector3(0, 1, 0);
        endPosition = endPosition + new Vector3(0, 1, 0);
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            _shoot.transform.position = Vector3.Lerp(startPosition, endPosition, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            // yield return null;
            await UniTask.Yield();
        }
    }

    internal async UniTask RunGettingHitSpell()
    {
        // Animate.
        string nameAnimDefend = "GettingHit";
        // if (_arenaEntity.Data.isDefense)
        // {
        //     nameAnimDefend = "Defend";
        // }

        string nameAnimationAttack = string.Format("{0}{1}", _nameCreature, nameAnimDefend);

        _animator.Play(nameAnimationAttack, 0, 0f);

        await UniTask.Delay(200);

        NormalizeDirection();
        _animator.Play(string.Format("{0}{1}", _nameCreature, "Idle"), 0, 0f);
    }

    internal void RunDeath()
    {
        Debug.Log("RunDeath");
        string nameAnim = "Death";

        string nameAnimation = string.Format("{0}{1}", _nameCreature, nameAnim);

        _animator.Play(nameAnimation, 0, 0f);
        _collider.enabled = false;
        _model.GetComponent<SpriteRenderer>().sortingOrder = -1;
    }

    public async UniTask ColorPulse(Color color, int count)
    {
        var sr = _model.GetComponent<SpriteRenderer>();
        var startColor = sr.color;
        var time = 1000 / count;
        for (int i = 0; i < count; i++)
        {
            sr.color = color;
            await UniTask.Yield();
            await UniTask.Delay(time);
            sr.color = startColor;
            await UniTask.Yield();
            if (i < count - 1) await UniTask.Delay(time);
        }
    }
}
