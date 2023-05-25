using System;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Localization;

[System.Serializable]
public abstract class BaseMapEntity : MonoBehaviour//, IPointerDownHandler
{
    [SerializeField] protected MapObject _mapObject;
    public MapObject MapObject => _mapObject;
    private float timeClickPrev;
    private GlobalMapInputManager _inputManager;
    private Camera _camera;

    public virtual void InitUnit(MapObject mapObject)
    {
        _mapObject = mapObject;
        timeClickPrev = Time.realtimeSinceStartup;
    }

    #region Unity methods + events
    private void OnEnable()
    {
        _inputManager = new GlobalMapInputManager();
        _inputManager.Click += OnClick;
    }

    private void OnDisable()
    {
        _inputManager.Click -= OnClick;
    }
    protected virtual void Awake()
    {
        _camera = Camera.main;
        // GameManager.OnBeforeStateChanged += OnBeforeStateChanged;
        // GameManager.OnAfterStateChanged += OnAfterStateChanged;
        //Debug.Log($"Awake {name}");
    }
    public virtual void OnNextDay()
    {
        // Debug.Log($"OnGoHero::: player[{player.DataPlayer.id}]");
    }


    public void DestroyGameObject()
    {
        Destroy(gameObject);
    }
    public void OnDestroy()
    {
        UnitManager.MapObjects.Remove(MapObject.Entity.IdEntity);
        // Debug.LogWarning($"Destroy object [{MapObject.ConfigData.name}] - gameObject[{gameObject.name}]");
        _mapObject.ConfigData.MapPrefab.ReleaseInstance(gameObject);

        _mapObject = null;
        // GameManager.OnBeforeStateChanged -= OnBeforeStateChanged;
        // GameManager.OnAfterStateChanged -= OnAfterStateChanged;
    }

    // public virtual void OnBeforeStateChanged(GameState newState)
    // {
    //     switch (newState)
    //     {
    //         case GameState.SaveGame:
    //             // OnSaveUnit();
    //             break;
    //     }
    // }

    // public virtual void OnAfterStateChanged(GameState newState)
    // {
    // }
    #endregion

    public async virtual UniTask OnGoHero(Player player)
    {
        await UniTask.Delay(1);
    }

    public async virtual void OnClick(InputAction.CallbackContext context)
    {
        // Debug.Log($"Click mapObject /{context.interaction}");
        if (
            context.performed
            && !_inputManager.ClickedOnUi()
            )
        {
            var rayHit = Physics2D.GetRayIntersection(_camera.ScreenPointToRay(_inputManager.clickPosition()));
            if (!rayHit.collider) return;
            if (rayHit.collider.gameObject == gameObject) // _model.gameObject
            {
                if (context.interaction is PressInteraction || context.interaction is TapInteraction)
                {
                    var activeHero = LevelManager.Instance.ActivePlayer.ActiveHero;
                    if (activeHero != null)
                    {

                        Vector3 posObject = transform.position;
                        if (activeHero.Data.path.Count > 0 && activeHero.Data.path[activeHero.Data.path.Count - 1].position == posObject)
                        {
                            GameManager.Instance.ChangeState(GameState.StartMoveHero);
                        }
                        else
                        {
                            if (posObject != null)
                            {
                                Vector3Int end = new Vector3Int((int)posObject.x, (int)posObject.y);
                                activeHero.FindPathForHero(end, true);
                            }
                        }
                    }
                    else
                    {
                        var dialogData = new DataDialogHelp()
                        {
                            Header = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "Help").GetLocalizedString(),
                            Description = new LocalizedString(Constants.LanguageTable.LANG_TABLE_ADVENTURE, "needchoosehero").GetLocalizedString(),
                        };

                        var dialogWindow = new DialogHelpProvider(dialogData);
                        await dialogWindow.ShowAndHide();
                    }
                }
                else if (context.interaction is HoldInteraction)
                {
                    Debug.Log($"Show info object::: {gameObject.name}");
                    _inputManager.Disable();
                    var dialogData = new DataDialogHelp()
                    {
                        Header = MapObject.ConfigData.Text.title.GetLocalizedString(),
                        Description = MapObject.ConfigData.Text.description.GetLocalizedString(),
                    };

                    var dialogWindow = new DialogHelpProvider(dialogData);
                    await dialogWindow.ShowAndHide();
                    _inputManager.Enable();
                }
            }
        }
    }
}
