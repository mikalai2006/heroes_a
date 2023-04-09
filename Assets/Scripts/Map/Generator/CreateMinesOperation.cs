using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loader;
using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using Random = UnityEngine.Random;

public class CreateMinesOperation : ILoadingOperation
{
    private readonly MapManager _root;

    public CreateMinesOperation(MapManager generator)
    {
        _root = generator;
    }

    public async UniTask Load(Action<float> onProgress, Action<string> onSetNotify)
    {
        onSetNotify("Create mines ...");
        var factory = new EntityMapObjectFactory();

        for (int keyArea = 0; keyArea < LevelManager.Instance.Level.listArea.Count; keyArea++)
        {
            Area area = LevelManager.Instance.Level.listArea[keyArea];

            List<GridTileNode> nodes = _root.gridTileHelper.GetAllGridNodes().Where(node =>
                node.Empty
                && node.Enable
                && !node.Road
                && !node.Protected
                && node.KeyArea == area.id
                && _root.gridTileHelper.GetDistanceBetweeenPoints(node.position, area.startPosition) > 10
                && _root.gridTileHelper.GetNeighbourList(node).Count >= 4
            ).OrderBy(t => Random.value).ToList();

            if (nodes.Count > 0)
            {
                int countLandscape = Mathf.CeilToInt(
                    LevelManager.Instance.GameModeData.koofMines * .1f * area.countNode);
                area.Stat.countMineN = countLandscape;
                int countCreated = 0;

                while (countCreated < countLandscape && nodes.Count > 0)
                {
                    GridTileNode currentNode = nodes[Random.Range(0, nodes.Count)];

                    NeighboursNature disableNeighbours
                        = _root.gridTileHelper.GetDisableNeighbours(currentNode);

                    GridTileNode nodeWarrior = _root.GetNodeWarrior(currentNode);

                    if (
                        currentNode != null
                        && nodeWarrior != null
                        && disableNeighbours.bottom.Count == 0
                        && disableNeighbours.top.Count >= 2
                        && _root.gridTileHelper.CalculateNeighbours(currentNode) >= 5
                        )
                    {

                        List<ScriptableEntityMapObject> list = ResourceSystem.Instance
                            .GetEntityByType<ScriptableEntityMapObject>(TypeEntity.MapObject)
                            .Where(t =>
                                t.TypeMapObject == TypeMapObject.Mine
                                && ((ScriptableEntityMine)t).TypeMine == TypeMine.Free
                                && (
                                    (t.TypeGround & currentNode.TypeGround) == currentNode.TypeGround
                                    ||
                                    (t.TypeGround & TypeGround.None) == TypeGround.None
                                    )
                                )
                            .ToList();
                        ScriptableEntityMapObject configData
                            = list[UnityEngine.Random.Range(0, list.Count)];
                        EntityMine entity = (EntityMine)factory.CreateMapObject(
                            TypeMapObject.Mine,
                            currentNode,
                            configData
                        );
                        UnitManager.SpawnEntityToNode(currentNode, entity);

                        BaseEntity warrior = UnitManager.SpawnWarrior(nodeWarrior);

                        nodeWarrior.SetProtectedNeigbours(warrior, currentNode);

                        nodes.Remove(currentNode);

                        countCreated++;

                        area.Stat.countMine++;

                        List<GridTileNode> listExistExitNode
                            = _root.gridTileHelper.IsExistExit(currentNode);

                        if (listExistExitNode.Count > 1)
                        {
                            _root.CreatePortal(currentNode, listExistExitNode);
                        }

                    }
                    else
                    {
                        nodes.Remove(currentNode);
                        continue;
                    }
                }

            }
        }

        await UniTask.Delay(1);
    }
}
