using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Cysharp.Threading.Tasks;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class ArenaEntityTownMB : MonoBehaviour
{
    [SerializeField] protected ArenaEntityTown _arenaEntityTown;
    public ArenaEntityTown ArenaEntityTown => _arenaEntityTown;
    [NonSerialized] private Animator _animator;
    [NonSerialized] private Transform _model;
    // public GameObject _shoot1;
    // public GameObject _shoot2;
    // public GameObject _shoot3;
    private InputManager _inputManager;
    private Camera _camera;
    // [SerializeField] private List<Transform> listTownObjects = new ();
    public GameObject moat;
    // [SerializeField] private GameObject wall1;
    // [SerializeField] private GameObject wall2;
    // [SerializeField] private GameObject wall3;
    // [SerializeField] private GameObject wall4;
    // [SerializeField] private GameObject tower1;
    // [SerializeField] private GameObject tower2;
    // [SerializeField] private GameObject tower3;
    [SerializeField] private GameObject bridge;
    private Transform bridgeTarget;
    private Animator bridgeAnimator;
    public bool isOpenBridge = false;
    // public List<Transform> _fortifications = new();


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
        _animator = GetComponentInChildren<Animator>();
        _model = transform.Find("Model");
        _camera = GameObject.FindGameObjectWithTag("ArenaCamera")?.GetComponent<Camera>();
        bridgeTarget = bridge.transform.GetChild(1);
        bridgeAnimator = bridgeTarget.GetComponent<Animator>();
        bridgeTarget.gameObject.SetActive(false);

        moat.SetActive(false);
    }
    #endregion

    public void SetStatusColliders(bool status)
    {
        foreach (var go in ArenaEntityTown.FortificationsGameObject)
        {
            var col = go.Key.GetComponent<Collider2D>();
            if (col != null)
            {
                if (go.Value == 0)
                {
                    col.enabled = false;
                }
                else
                {
                    col.enabled = status;
                }
            }
        }
    }

    public void Init(ArenaEntityTown entityTown)
    {
        _arenaEntityTown = entityTown;

        var allChildren = gameObject.GetChildren(false);
        foreach (var child in allChildren)
        {
            if (child.GetComponent<Collider2D>() != null)
            {
                ArenaEntityTown.AddFortification(child);
            }
        }

        SetStatusColliders(false);

        if (_arenaEntityTown.isMoat)
        {
            moat.SetActive(true);
        }
    }

    protected void OnDestroy()
    {
        ArenaQueue.OnNextRound -= NextRound;

        ArenaEntityTown.Town.ConfigData.ArenaPrefab.ReleaseInstance(gameObject);
    }

    private void NextRound()
    {
        // ArenaEntityTown.SetRoundData();
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
                ArenaEntityTown.FortificationsGameObject.ContainsKey(rayHit.collider.gameObject.transform)
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
                // else if (rayHit.collider.gameObject == bridge)
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

    internal async UniTask OpenBridge()
    {
        bridgeTarget.gameObject.SetActive(true);
        bridgeAnimator.Play("BridgeOpen");
        isOpenBridge = true;

        await UniTask.Yield();
    }
    internal void CloseBridge()
    {
        if (!isOpenBridge) return;

        bridgeAnimator.Play("BridgeClose");
        isOpenBridge = false;
        bridgeTarget.gameObject.SetActive(false);
    }

    internal async UniTask ResreshObject(Transform transform, int totalDamage)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        if (totalDamage > 0)
        {
            var level = totalDamage;
            if (transform.childCount <= level)
            {
                level = level - 1;
            }
            transform.GetChild(level).gameObject.SetActive(true);

            // if bridge close - disable transform.
            if (transform.name.IndexOf("Bridge") != -1 && !isOpenBridge)
            {
                transform.GetChild(level).gameObject.SetActive(false);
            }
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(true);
            // transform.gameObject.SetActive(false);
        }
        await UniTask.Delay(1);
    }
}