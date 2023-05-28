using System;
using System.Collections.Generic;
using System.Threading;

using Cysharp.Threading.Tasks;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class ArenaEntityTownMonobehavior : MonoBehaviour
{
    [SerializeField] protected ArenaEntityTown _arenaEntityTown;
    public ArenaEntityTown ArenaEntityTown => _arenaEntityTown;
    [NonSerialized] private Animator _animator;
    [NonSerialized] private Transform _model;
    [NonSerialized] private GameObject _shoot1;
    [NonSerialized] private GameObject _shoot2;
    [NonSerialized] private GameObject _shoot3;
    private InputManager _inputManager;
    private Camera _camera;

    [SerializeField] private GameObject moat;
    [SerializeField] private GameObject wall1;
    [SerializeField] private GameObject wall2;
    [SerializeField] private GameObject wall3;
    [SerializeField] private GameObject wall4;
    [SerializeField] private GameObject tower1;
    [SerializeField] private GameObject tower2;
    [SerializeField] private GameObject tower3;
    [SerializeField] private GameObject door;
    public List<Transform> _fortifications;


    #region Unity methods
    private void OnEnable()
    {
        _inputManager = new InputManager();
        _inputManager.ClickEntity += OnClick;
        // _inputManager.Enable();
    }

    private void OnDisable()
    {
        _inputManager.ClickEntity -= OnClick;
        // _inputManager.Disable();
    }
    public void Awake()
    {
        ArenaQueue.OnNextRound += NextRound;

        _fortifications = gameObject.GetChildren(false);

        _animator = GetComponentInChildren<Animator>();
        _model = transform.Find("Model");
    }
    #endregion

    public void SetStatusColliders(bool status)
    {
        foreach (var go in _fortifications)
        {
            var col = go.GetComponent<Collider2D>();
            if (col != null)
            {
                col.enabled = status;
            }
        }
    }

    public void Init(ArenaEntityTown entityTown)
    {
        _camera = GameObject.FindGameObjectWithTag("ArenaCamera")?.GetComponent<Camera>();

        _arenaEntityTown = entityTown;
    }

    protected void OnDestroy()
    {
        ArenaQueue.OnNextRound -= NextRound;

        ArenaEntityTown.Town.ConfigData.ArenaPrefab.ReleaseInstance(gameObject);
    }

    private void NextRound()
    {
        ArenaEntityTown.SetRoundData();
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

    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            var rayHit = Physics2D.GetRayIntersection(_camera.ScreenPointToRay(_inputManager.clickPosition()));
            if (!rayHit.collider) return;
            // Debug.Log($"OnClick::: {context.interaction} - {context.phase} - {rayHit.collider.gameObject}");
            // if (rayHit.collider.gameObject == gameObject) // _model.gameObject
            // {
            if (
                context.interaction is PressInteraction || context.interaction is TapInteraction
                &&
                _fortifications.Contains(rayHit.collider.gameObject.transform)
                )
            {
                ArenaEntityTown.ClickFortification(rayHit.collider.gameObject);
                // if (rayHit.collider.gameObject == moat)
                // {
                //     ArenaEntityTown.ClickFortification(moat);
                // }
                // else if (rayHit.collider.gameObject == wall1)
                // {
                //     ArenaEntityTown.ClickFortification(wall1);
                // }
                // else if (rayHit.collider.gameObject == wall2)
                // {

                // }
                // else if (rayHit.collider.gameObject == wall3)
                // {

                // }
                // else if (rayHit.collider.gameObject == wall4)
                // {

                // }
                // else if (rayHit.collider.gameObject == tower1)
                // {

                // }
                // else if (rayHit.collider.gameObject == tower2)
                // {

                // }
                // else if (rayHit.collider.gameObject == tower3)
                // {

                // }
                // else if (rayHit.collider.gameObject == door)
                // {

                // }
            }
            else if (context.interaction is HoldInteraction)
            {
                Debug.Log($"Hold::: {gameObject.name}");
            }
            // }
        }
    }
}