using System.Collections.Generic;
using Loader;
using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using Random = UnityEngine.Random;
using UnityEngine.Localization;

public class CreateTownOperation : ILoadingOperation
{
    private readonly MapManager _root;

    public CreateTownOperation(MapManager generator)
    {
        _root = generator;
    }

    public async UniTask Load(Action<float> onProgress, Action<string> onSetNotify)
    {
        var t = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "createdgameobject").GetLocalizedString();
        onSetNotify(t + "towns ...");

        for (int x = 0; x < LevelManager.Instance.Level.listArea.Count; x++)
        {
            Area area = LevelManager.Instance.Level.listArea[x];

            float minCountNodeAreaForTown = ((float)area.countNode / (float)_root.gridTileHelper.GridTile.SizeGrid);

            // For small area cancel spawn town.
            if (minCountNodeAreaForTown < LevelManager.Instance.Level.GameModeData.koofMinTown || !area.isFraction)
            {
                continue;
            }

            //Debug.LogWarning($"Spawn town for {area.typeGround}-{area.isFraction}");

            List<GridTileNode> listGridNode = _root.gridTileHelper.GetAllGridNodes().Where(t =>
                t.StateNode.HasFlag(StateNode.Empty)
                // t.Empty
                // && !t.Protected
                // && t.Enable
                && t.KeyArea == area.id
                && _root.gridTileHelper.CalculateNeighbours(t) == 8
            // && gridTileHelper.GetDistanceBetweeenPoints(t._position, LevelManager.Instance.GetArea(t.keyArea).startPosition) < 10 
            //&& t.level < 10
            ).OrderBy(t => Random.value).ToList();

            if (listGridNode.Count() > 0)
            {
                GridTileNode node = listGridNode[listGridNode.Count - 1];
                Player player = LevelManager.Instance.GetPlayer(area.idPlayer);

                //Create town.
                ScriptableEntityTown configData
                    = ResourceSystem.Instance
                    .GetEntityByType<ScriptableEntityTown>(TypeEntity.Town)
                    // .Where(t => t.ty)
                    .First();
                var entityTown = new EntityTown(
                    node.TypeGround,
                    player != null ? player.StartSetting.town : null
                    );
                UnitManager.SpawnEntityMapObjectToNode(node, entityTown);

                node.AddStateNode(StateNode.Town);
                area.town = entityTown;
                // area.startPosition = entityTown.Position;
                if (player != null)
                {
                    entityTown.SetPlayer(player);
                }

                // Spawn hero.
                if (player != null)
                {
                    var hero = UnitManager.CreateHero(
                        entityTown.ConfigData.TypeFaction,
                        node,
                        player.StartSetting.hero
                    );
                    hero.SetPlayer(player);
                    hero.SetClearSky(node);
                }
                // TODO Spawn random hero
                // int randomCountHero = Helpers.GenerateValueByRangeAndStep(1, 3, 1);
                // for (int i = 0; i < randomCountHero; i++)
                // {
                // }

                area.startPosition = node.position;

                // Spawn mines.
                for (int i = 0; i < entityTown.ConfigData.mines.Count; i++)
                {
                    var listNodes = _root.gridTileHelper.GetAllGridNodes().Where(t =>
                    t.KeyArea == area.id
                    && t.StateNode.HasFlag(StateNode.Empty)
                    // && t.Empty
                    // && t.Enable
                    && _root.gridTileHelper.GetDistanceBetweeenPoints(t.position, node.position) >= 4
                    && _root.gridTileHelper.GetDistanceBetweeenPoints(t.position, node.position) <= 10
                    && _root.gridTileHelper.CalculateNeighboursByArea(t) == 8
                   ).ToList();
                    if (listNodes.Count > 0)
                    {
                        // var factory = new EntityMapObjectFactory();
                        GridTileNode nodeForSpawn = listNodes.OrderBy(t => Random.value).First();

                        if (nodeForSpawn != null)
                        {
                            EntityMine newmine = new EntityMine(entityTown.ConfigData.mines[i]);
                            UnitManager.SpawnEntityMapObjectToNode(nodeForSpawn, newmine);
                        }
                    }
                }

            }
        }


        await UniTask.Delay(1);
    }
}
