using System;
using System.Threading;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class ArenaHeroMonoBehavior : MonoBehaviour
{
    [SerializeField] protected ArenaHeroEntity _arenaHeroEntity;
    public ArenaHeroEntity ArenaEntityHero => _arenaHeroEntity;
    [NonSerialized] private Animator _animator;
    [NonSerialized] private Transform _model;
    private Camera _camera;
    private InputManager _inputManager;
    private CancellationTokenSource cancelTokenSource;

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
    protected void Awake()
    {
        ArenaQueue.OnNextRound += NextRound;
        _animator = GetComponentInChildren<Animator>();
        _model = transform.Find("Model");
        _camera = GameObject.FindGameObjectWithTag("ArenaCamera")?.GetComponent<Camera>();
    }
    protected void OnDestroy()
    {
        ArenaQueue.OnNextRound -= NextRound;
    }

    private void NextRound()
    {
        ((EntityHero)ArenaEntityHero.Entity).Data.SpellBook.SetCountSpellPerRound();
    }

    public async void OnClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            var rayHit = Physics2D.GetRayIntersection(_camera.ScreenPointToRay(_inputManager.clickPosition()));
            if (!rayHit.collider) return;
            if (rayHit.collider.gameObject == gameObject)
            {
                if (context.interaction is PressInteraction || context.interaction is TapInteraction)
                {
                    await AudioManager.Instance.Click();
                    _inputManager.Disable();
                    var dialogHeroInfo = new UIInfoArenaHeroOperation((EntityHero)ArenaEntityHero.Entity);
                    var result = await dialogHeroInfo.ShowAndHide();
                    if (result.isOk)
                    {

                    }
                    _inputManager.Enable();
                }
                else if (context.interaction is HoldInteraction)
                {
                }
            }
        }
    }
    #endregion

    public void Init(ArenaHeroEntity entityHero)
    {
        _arenaHeroEntity = entityHero;

        if (gameObject.transform.position.x > 10f)
        {
            gameObject.transform.localScale = new Vector3(-1, 1, 1);
        }
    }
    public async UniTask RunSpellAnimation()
    {
        if (_animator != null)
        {
            _animator.SetBool("spell", true);

            await UniTask.Delay(300);

            _animator.SetBool("spell", false);

            await UniTask.Yield();
        }
    }

}
