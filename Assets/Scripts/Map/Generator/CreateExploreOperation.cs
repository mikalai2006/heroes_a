using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loader;
using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using Random = UnityEngine.Random;

public class CreateExploreOperation : ILoadingOperation
{
    private readonly MapManager _root;

    public CreateExploreOperation(MapManager generator)
    {
        _root = generator;
    }

    public async UniTask Load(Action<float> onProgress, Action<string> onSetNotify)
    {
        onSetNotify("Create explore ...");
        var factory = new EntityMapObjectFactory();

        for (int x = 0; x < LevelManager.Instance.Level.listArea.Count; x++)
        {
            Area area = LevelManager.Instance.Level.listArea[x];

            List<GridTileNode> nodes = _root.gridTileHelper.GetAllGridNodes().Where(t =>
                t.Empty
                && t.Enable
                && !t.Road
                && !t.Protected
                && t.KeyArea == area.id
                && _root.gridTileHelper.CalculateNeighbours(t) == 8
                && _root.gridTileHelper.GetDistanceBetweeenPoints(t.position, area.startPosition) > 10
            ).OrderBy(t => Random.value).ToList();

            if (nodes.Count > 0)
            {
                int maxCountExplore = Mathf.CeilToInt(
                    LevelManager.Instance.GameModeData.koofExplore * area.countNode);
                area.Stat.countExploreN = maxCountExplore;
                int countCreated = 0;

                while (countCreated < maxCountExplore && nodes.Count > 0)
                {
                    GridTileNode currentNode = nodes[Random.Range(0, nodes.Count)];
                    GridTileNode nodeWarrior = _root.GetNodeWarrior(currentNode);

                    if (nodeWarrior != null
                        && nodeWarrior.Empty
                        && currentNode != null
                        && _root.gridTileHelper.CalculateNeighbours(currentNode) == 8
                        )
                    {
                        List<ScriptableEntityMapObject> list = ResourceSystem.Instance
                            .GetEntityByType<ScriptableEntityMapObject>(TypeEntity.MapObject)
                            .Where(t => t.TypeMapObject == TypeMapObject.Explore)
                            .ToList();
                        ScriptableEntityMapObject configData
                            = list[UnityEngine.Random.Range(0, list.Count)];
                        EntityExpore entity
                            = (EntityExpore)factory.CreateMapObject(TypeMapObject.Explore, currentNode, configData);
                        UnitManager.SpawnEntityToNode(currentNode, entity);

                        BaseEntity warrior = UnitManager.SpawnWarrior(nodeWarrior);

                        nodeWarrior.SetProtectedNeigbours(warrior, currentNode);

                        nodes.Remove(currentNode);

                        countCreated++;

                        area.Stat.countExplore++;

                        List<GridTileNode> listExistExitNode = _root.gridTileHelper.IsExistExit(currentNode);
                        if (listExistExitNode.Count > 1)
                        {
                            _root.CreatePortal(currentNode, listExistExitNode);
                        }
                    }
                    else
                    {
                        nodes.Remove(currentNode);
                    }
                }

            }
        }
        await UniTask.Delay(1);
    }
}
