using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loader;
using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using Random = UnityEngine.Random;

public class CreateResourceEveryWeekOperation : ILoadingOperation
{
    private readonly MapManager _root;

    public CreateResourceEveryWeekOperation(MapManager generator)
    {
        _root = generator;
    }

    public async UniTask Load(Action<float> onProgress, Action<string> onSetNotify)
    {
        onSetNotify("Create every week resource ...");

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
                    && _root.gridTileHelper.GetDistanceBetweeenPoints(t.position, area.startPosition) > 5
                ).OrderBy(t => Random.value).ToList();

            if (nodes.Count > 0)
            {
                int maxCountResource = Mathf.CeilToInt(LevelManager.Instance.GameModeData.koofResource * area.countNode);
                area.Stat.countEveryResourceN = maxCountResource;
                int countCreated = 0;

                while (countCreated < maxCountResource && nodes.Count > 0)
                {
                    GridTileNode currentNode = nodes[Random.Range(0, nodes.Count)];

                    GridTileNode nodeWarrior = _root.GetNodeWarrior(currentNode);

                    if (nodeWarrior != null && currentNode != null && _root.gridTileHelper.CalculateNeighbours(currentNode) == 8)
                    {

                        BaseWarriors warrior = _root.UnitManager.SpawnWarrior(nodeWarrior);

                        UnitBase unit = _root.UnitManager.SpawnMapObjectToPosition(
                            currentNode,
                            MapObjectType.Resource
                        );

                        area.Stat.countEveryResource++;

                        nodeWarrior.SetProtectedNeigbours(warrior, currentNode);

                        nodes.Remove(currentNode);

                        countCreated++;

                        List<GridTileNode> listExistExitNode = _root.gridTileHelper.IsExistExit(currentNode);

                        if (listExistExitNode.Count > 1)
                        {
                            _root.CreatePortal(currentNode, listExistExitNode);
                        }
                        //else
                        //{
                        //    Debug.Log($"NoExit::: keyArea{currentNode.keyArea}[{currentNode._position}]- Null");
                        //}

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
