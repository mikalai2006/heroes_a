using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using Loader;

using UnityEngine;
using UnityEngine.EventSystems;

public class MapEntityTown : BaseMapEntity
{
    [SerializeField] private List<GameObject> levels;
    public override void InitUnit(MapObject mapObject)
    {

        base.InitUnit(mapObject);

        if (mapObject.Entity.Player != null)
        {
            SetPlayer(mapObject.Entity.Player);
        }
        RefreshLevelBuild();
    }

    private void SetPlayer(Player player)
    {
        Transform flag = transform.Find("Flag");
        flag.GetComponent<SpriteRenderer>().color = player.DataPlayer.color;
    }

    public async override UniTask OnGoHero(Player player)
    {
        await base.OnGoHero(player);

        if (LevelManager.Instance.ActivePlayer.DataPlayer.playerType != PlayerType.Bot)
        {
            var loadingOperations = new Queue<ILoadingOperation>();

            // GameManager.Instance.AssetProvider.UnloadAdditiveScene(_scene);
            var town = (EntityTown)MapObject.Entity;
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
            && MapObject.Entity.Player == LevelManager.Instance.ActivePlayer
            )
        {
            await OnGoHero(MapObject.Entity.Player);
        }
        base.OnPointerClick(eventData);
    }

    public void RefreshLevelBuild()
    {
        var level = ((EntityTown)_mapObject.Entity).Data.level;
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
}