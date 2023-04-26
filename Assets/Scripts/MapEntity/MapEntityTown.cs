using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using Loader;

using UnityEngine;
using UnityEngine.EventSystems;

public class MapEntityTown : BaseMapEntity
{
    [SerializeField] private List<GameObject> levels;
    public override void InitUnit(BaseEntity mapObject)
    {

        base.InitUnit(mapObject);

        if (mapObject.Player != null)
        {
            SetPlayer(mapObject.Player);
        }
        RefreshLevelBuild();
    }
    //private void Awake() => GameManager.OnBeforeStateChanged += OnStateChanged;

    //private void OnDestroy() => GameManager.OnBeforeStateChanged -= OnStateChanged;

    public override void OnAfterStateChanged(GameState newState)
    {
        //if (newState == GameState.HeroTurn) _canMove = true;
        base.OnAfterStateChanged(newState);
    }

    private void SetPlayer(Player player)
    {
        Transform flag = transform.Find("Flag");
        flag.GetComponent<SpriteRenderer>().color = player.DataPlayer.color;
    }

    public virtual void ExecuteMove()
    {
        // Override this to do some hero-specific logic, then call this base method to clean up the turn

        //_canMove = false;
    }

    public async override UniTask OnGoHero(Player player)
    {
        await base.OnGoHero(player);

        if (LevelManager.Instance.ActivePlayer.DataPlayer.playerType != PlayerType.Bot)
        {
            var loadingOperations = new Queue<ILoadingOperation>();

            // GameManager.Instance.AssetProvider.UnloadAdditiveScene(_scene);
            var town = (EntityTown)GetMapObjectClass;
            player.SetActiveTown(town);
            // player.ActiveTown.SetGuest(player.ActiveHero);
            loadingOperations.Enqueue(new TownLoadOperation(town));
            await GameManager.Instance.LoadingScreenProvider.LoadAndDestroy(loadingOperations);
        }
    }

    public async override void OnPointerClick(PointerEventData eventData)
    {
        if (
            eventData.clickCount >= 2
            && GetMapObjectClass.Player == LevelManager.Instance.ActivePlayer
            )
        {
            await OnGoHero(GetMapObjectClass.Player);
        }
        base.OnPointerClick(eventData);
    }

    public void RefreshLevelBuild()
    {
        var level = ((EntityTown)MapObjectClass).Data.level;
        var activeLevel = level + 1;
        if (levels.Count == 0) return;

        foreach (var go in levels)
        {
            go.gameObject.SetActive(false);
        }

        if (activeLevel > levels.Count - 1)
        {
            activeLevel = levels.Count - 1;
        }

        var el = levels.ElementAtOrDefault(activeLevel);
        if (el != null)
        {
            el.gameObject.SetActive(true);
        }
    }
    //public override void OnSaveUnit()
    //{
    //    // SaveUnit(Data);
    //}

    // public void LoadDataPlay(DataPlay data)
    // {
    //     //throw new System.NotImplementedException();
    // }
    // public void SaveDataPlay(ref DataPlay data)
    // {
    //     var sdata = SaveUnit(Data);
    //     data.Units.towns.Add(sdata);
    // }
}