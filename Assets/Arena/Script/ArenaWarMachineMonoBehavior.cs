using System;
using System.Threading;

using Cysharp.Threading.Tasks;


using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class ArenaWarMachineMonoBehavior : MonoBehaviour
{
    [SerializeField] protected ArenaWarMachine _arenaEntity;
    public ArenaWarMachine ArenaEntity => _arenaEntity;
    [NonSerialized] private Animator _animator;
    [NonSerialized] private Transform _model;
    private Camera _camera;
    private Collider2D _collider;
    [SerializeField] public GameObject ShootPrefab;
    private GameObject _shoot;

    private string _nameCreature;

    private CancellationTokenSource cancelTokenSource;
    private Vector2 _pos;
    private InputManager _inputManager;

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
        // ArenaWarMachine.OnChangeParamsCreature += RefreshQuantity;
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
    }

    protected void OnDestroy()
    {
        // ArenaWarMachine.OnChangeParamsCreature -= RefreshQuantity;
        ArenaQueue.OnNextRound -= NextRound;

        ((ScriptableAttributeCreature)ArenaEntity.Entity.ScriptableDataAttribute).ArenaModel.ReleaseInstance(gameObject);
        if (_shoot != null) Destroy(_shoot);
    }

    private async void NextRound()
    {
        if (!_arenaEntity.Death)
        {
            await _arenaEntity.SetRoundData();
        }
    }

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
                    // Debug.Log($"Hold::: {gameObject.name}");
                    ShowDialogInfo();
                }
            }
        }
    }

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

    private async void ClickCreature(InputAction.CallbackContext context)
    {
        var pos = _inputManager.clickPosition();
        Vector2 posMouse = _camera.ScreenToWorldPoint(pos);
        Vector3Int tilePos = ArenaEntity.arenaManager.tileMapArenaGrid.WorldToCell(posMouse);
        await ArenaEntity.ClickCreature(tilePos);
        // Debug.Log($"Click {name}");
    }
    #endregion

    public void Init(ArenaWarMachine arenaEntity)
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
        var splitName = ArenaEntity.Entity.ScriptableDataAttribute.name.Split('_');
        _nameCreature = splitName.Length > 1 ? splitName[1] : splitName[0];
    }

    internal async UniTask RunGettingHit(GridArenaNode nodeFromAttack)
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
        // Debug.Log($"Shoot {nameAnimationAttack}");

        _animator.Play(nameAnimationAttack, 0, 0f);

        _shoot.SetActive(true);
        await SmoothLerpShoot(transform.position, positionToAttack, LevelManager.Instance.ConfigGameSettings.speedArenaAnimation * 2);
        _shoot.SetActive(false);

        await UniTask.Delay(200);

        if (nodeForAttack != null) await nodeForAttack.OccupiedUnit.RunGettingHit(_arenaEntity.OccupiedNode);

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
        string nameAnimate = "GettingHit";

        string fullNameAnimation = string.Format("{0}{1}", _nameCreature, nameAnimate);

        _animator.Play(fullNameAnimation, 0, 0f);

        await UniTask.Delay(200);

        _animator.Play(string.Format("{0}{1}", _nameCreature, "Idle"), 0, 0f);
    }

    internal void RunDeath()
    {
        // Debug.Log("RunDeath");
        string nameAnim = "Death";

        string nameAnimation = string.Format("{0}{1}", _nameCreature, nameAnim);

        _animator.Play(nameAnimation, 0, 0f);
        _collider.enabled = false;
        _model.GetComponent<SpriteRenderer>().sortingOrder = -1;
    }
}
