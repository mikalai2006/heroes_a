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
                t.Empty
                && !t.Protected
                && t.Enable
                && t.KeyArea == area.id
                && _root.gridTileHelper.CalculateNeighbours(t) == 8
            // && gridTileHelper.GetDistanceBetweeenPoints(t._position, LevelManager.Instance.GetArea(t.keyArea).startPosition) < 10 
            //&& t.level < 10
            ).OrderBy(t => Random.value).ToList();

            if (listGridNode.Count() > 0)
            {
                GridTileNode node = listGridNode[listGridNode.Count - 1];
                //Create town.
                Player player = LevelManager.Instance.GetPlayer(area.id);

                var entityTown = new EntityTown(node, node.TypeGround);
                _root.UnitManager.SpawnEntityToNode(node, entityTown);
                area.town = entityTown;
                // area.startPosition = entityTown.Position;
                if (player != null)
                {
                    entityTown.SetPlayer(player);
                }

                ScriptableEntityTown configTown = (ScriptableEntityTown)entityTown.ScriptableData;
                //_townsList.Add(createdTown);
                //gridNode.OccupiedUnit = createdTown;
                // }

                // Spawn hero.
                if (player != null)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        EntityHero newEntity = new EntityHero(node, configTown.TypeFaction);
                        _root.UnitManager.SpawnEntityToNode(node, newEntity);
                        // gridNode.SetOcuppiedUnit(createdHero);
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
                    && t.Empty
                    && t.Enable
                    && _root.gridTileHelper.GetDistanceBetweeenPoints(t.position, node.position) >= 4
                    && _root.gridTileHelper.GetDistanceBetweeenPoints(t.position, node.position) <= 10
                    && _root.gridTileHelper.CalculateNeighboursByArea(t) == 8
                   ).ToList();
                    if (listNodes.Count > 0)
                    {
                        GridTileNode nodeForSpawn = listNodes.OrderBy(t => Random.value).First();

                        if (nodeForSpawn != null)
                        {
                            EntityMine newmine = new EntityMine(
                                nodeForSpawn,
                                configTown.TypeGround,
                                TypeMine.Town,
                                configTown.mines[i]);
                            _root.UnitManager.SpawnEntityToNode(nodeForSpawn, newmine);
                        }
                    }
                }

            }
        }


        await UniTask.Delay(1);
    }

}
