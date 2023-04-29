using System;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;

[System.Serializable]
public abstract class BaseMapEntity : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] protected MapObject _mapObject;
    public MapObject MapObject => _mapObject;
    private float timeClickPrev;

    public virtual void InitUnit(MapObject mapObject)
    {
        _mapObject = mapObject;
        timeClickPrev = Time.realtimeSinceStartup;
    }

    #region Unity methods + events
    public virtual void OnNextDay()
    {
        // Debug.Log($"OnGoHero::: player[{player.DataPlayer.id}]");
    }

    protected virtual void Awake()
    {
        // GameManager.OnBeforeStateChanged += OnBeforeStateChanged;
        // GameManager.OnAfterStateChanged += OnAfterStateChanged;
        //Debug.Log($"Awake {name}");
    }

    public void OnDestroy()
    {
        UnitManager.MapObjects.Remove(MapObject.Entity.IdEntity);

        Debug.LogWarning($"Destroy object [{MapObject.ConfigData.name}] - gameObject[{gameObject.name}]");
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

    public async virtual void OnPointerClick(PointerEventData eventData)
    {
        if (Time.realtimeSinceStartup - timeClickPrev < LevelManager.Instance.ConfigGameSettings.deltaDoubleClick)
        {
            GameManager.Instance.ChangeState(GameState.StartMoveHero);
        }
        else
        {
            if (LevelManager.Instance.ActivePlayer.ActiveHero != null)
            {
                // Debug.Log($"" +
                //     $"UnitBase Click \n" +
                //     $"name-{this.name} \n" +
                //     $"pos-[{transform.position}]\n" +
                //     $"ocup-[{MapObjectClass.OccupiedNode.ToString()}]\n"
                //     );
                Vector3 posObject = transform.position;

                if (posObject != null)
                {
                    Vector3Int end = new Vector3Int((int)posObject.x, (int)posObject.y);
                    LevelManager.Instance.ActivePlayer.ActiveHero.FindPathForHero(end, true);
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

            timeClickPrev = Time.realtimeSinceStartup;
        }
    }
}
