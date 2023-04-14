using System.Collections.Generic;
using Loader;
using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using Random = UnityEngine.Random;

public class CreateTownOperation : ILoadingOperation
{
    private readonly MapManager _root;

    public CreateTownOperation(MapManager generator)
    {
        _root = generator;
    }

    public async UniTask Load(Action<float> onProgress, Action<string> onSetNotify)
    {
        onSetNotify("Create towns ...");

        for (int x = 0; x < LevelManager.Instance.Level.listArea.Count; x++)
        {
            Area area = LevelManager.Instance.Level.listArea[x];

            float minCountNodeAreaForTown = ((float)area.countNode / (float)_root.gridTileHelper.GridTile.SizeGrid);

            // For small area cancel spawn town.
            if (minCountNodeAreaForTown < LevelManager.Instance.GameModeData.koofMinTown || !area.isFraction)
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
                Player player = LevelManager.Instance.GetPlayer(area.id);

                //Create town.
                var entityTown = new EntityTown(node.TypeGround);
                UnitManager.SpawnEntityMapObjectToNode(node, entityTown);

                node.AddStateNode(StateNode.Town);
                area.town = entityTown;
                // area.startPosition = entityTown.Position;
                if (player != null)
                {
                    entityTown.SetPlayer(player);
                }

                ScriptableEntityTown configTown = (ScriptableEntityTown)entityTown.ScriptableData;

                // Spawn hero.
                if (player != null)
                {
                    int randomCountHero = Helpers.GenerateValueByRangeAndStep(1, 3, 1);
                    for (int i = 0; i < randomCountHero; i++)
                    {
                        // if (node.OccupiedUnit != null)
                        // {
                        //     node = _root.gridTileHelper.GetNode(node.position.x, node.position.y - 1);
                        // }
                        EntityHero newEntity = new EntityHero(configTown.TypeFaction);
                        // UnitManager.SpawnEntityMapObjectToNode(node, newEntity);
                        node.SetAsGuested(newEntity);
                        newEntity.CreateMapGameObject(node);
                        UnitManager.Entities.Add(newEntity.IdEntity, newEntity);
                        // node.SetOcuppiedUnit(newEntity);
                        LevelManager.Instance.GetArea(area.id).hero = newEntity;
                        newEntity.SetPlayer(player);
                    }
                }
                area.startPosition = node.position;

                // Spawn mines.
                for (int i = 0; i < configTown.mines.Count; i++)
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
                        var factory = new EntityMapObjectFactory();
                        GridTileNode nodeForSpawn = listNodes.OrderBy(t => Random.value).First();

                        if (nodeForSpawn != null)
                        {
                            EntityMine newmine = (EntityMine)factory.CreateMapObject(
                                TypeMapObject.Mine,
                                configTown.mines[i]
                                );
                            UnitManager.SpawnEntityMapObjectToNode(nodeForSpawn, newmine);
                        }
                    }
                }

            }
        }


        await UniTask.Delay(1);
    }
}
