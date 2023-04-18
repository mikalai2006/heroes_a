using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using Loader;

using UnityEngine;
using UnityEngine.EventSystems;

public class MapEntityTown : BaseMapEntity
{
    public override void InitUnit(BaseEntity mapObject)
    {

        base.InitUnit(mapObject);

        if (mapObject.Player != null)
        {
            SetPlayer(mapObject.Player);
        }
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

    public async override void OnGoHero(Player player)
    {
        base.OnGoHero(player);

        var loadingOperations = new Queue<ILoadingOperation>();

        // GameManager.Instance.AssetProvider.UnloadAdditiveScene(_scene);
        var town = (EntityTown)GetMapObjectClass;
        player.SetActiveTown(town);
        // player.ActiveTown.SetGuest(player.ActiveHero);
        loadingOperations.Enqueue(new TownLoadOperation(town));
        await GameManager.Instance.LoadingScreenProvider.LoadAndDestroy(loadingOperations);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (
            eventData.clickCount >= 2
            && GetMapObjectClass.Player == LevelManager.Instance.ActivePlayer
            )
        {
            OnGoHero(GetMapObjectClass.Player);
        }
        base.OnPointerClick(eventData);
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