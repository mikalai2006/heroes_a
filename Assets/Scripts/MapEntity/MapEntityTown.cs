using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using Loader;

using UnityEngine;
// using UnityEngine.EventSystems;
// using UnityEngine.InputSystem;

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
        Debug.Log("On hero in town");

        if (MapObject.Entity.Player == null
            ||
            (MapObject.Entity.Player != null && player.DataPlayer.team != MapObject.Entity.Player.DataPlayer.team)
        )
        {
            MapObject.Entity.SetPlayer(player);
            SetPlayer(player);
        }

        if (LevelManager.Instance.ActivePlayer.DataPlayer.playerType != PlayerType.Bot)
        {
            var loadingOperations = new Queue<ILoadingOperation>();

            // GameManager.Instance.AssetProvider.UnloadAdditiveScene(_scene);
            var town = (EntityTown)MapObject.Entity;
            player.SetActiveTown(town);
            // player.ActiveHero.SetGuestForNode(town.MapObject.OccupiedNode);
            loadingOperations.Enqueue(new TownLoadOperation(town));
            await GameManager.Instance.LoadingScreenProvider.LoadAndDestroy(loadingOperations);
        }
    }

    // public async override void OnClick(InputAction.CallbackContext context)
    // {
    //     // if (context.performed)
    //     // {
    //     //     var rayHit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(_inputManager.clickPosition()));
    //     //     if (!rayHit.collider) return;
    //     //     if (rayHit.collider.gameObject == gameObject) // _model.gameObject
    //     //     {
    //     //         if (context.interaction is PressInteraction || context.interaction is TapInteraction)
    //     //         {
    //     //         }
    //     //         }
    //     // }
    //     // Debug.Log("Click Town!");
    //     await UniTask.Delay(1);
    // }
    // public async override void OnClick(PointerEventData eventData)
    // {
    //     if (
    //         eventData.clickCount >= 2
    //         && MapObject.Entity.Player == LevelManager.Instance.ActivePlayer
    //         )
    //     {
    //         await OnGoHero(MapObject.Entity.Player);
    //     }
    //     base.OnPointerDown(eventData);
    // }

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