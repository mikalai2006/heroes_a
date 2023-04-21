using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loader;
using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using Random = UnityEngine.Random;
using UnityEngine.Localization;

public class CreateMinesOperation : ILoadingOperation
{
    private readonly MapManager _root;

    public CreateMinesOperation(MapManager generator)
    {
        _root = generator;
    }

    public async UniTask Load(Action<float> onProgress, Action<string> onSetNotify)
    {
        var t = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "createdgameobject").GetLocalizedString();
        onSetNotify(t + " mines ...");

        var factory = new EntityMapObjectFactory();
        List<GridTileNode> allNodes = _root.gridTileHelper.GetAllGridNodes();

        for (int keyArea = 0; keyArea < LevelManager.Instance.Level.listArea.Count; keyArea++)
        {
            Area area = LevelManager.Instance.Level.listArea[keyArea];

            List<GridTileNode> nodes = new List<GridTileNode>();
            for (int i = 0; i < allNodes.Count; i++)
            {
                var node = allNodes[i];
                if (node.StateNode.HasFlag(StateNode.Empty)
                && !node.StateNode.HasFlag(StateNode.Road)
                && !node.StateNode.HasFlag(StateNode.Protected)
                && node.KeyArea == area.id
                && _root.gridTileHelper.GetDisableNeighbours(node).bottom.Count == 0
                && _root.gridTileHelper.GetDisableNeighbours(node).top.Count >= 2
                && _root.gridTileHelper.GetDistanceBetweeenPoints(node.position, area.startPosition) > 10
                && _root.gridTileHelper.GetNeighbourList(node).Count >= 4)
                {
                    nodes.Add(node);
                }
            }
            // var nodes = .Where(node =>
            //     node.StateNode.HasFlag(StateNode.Empty)
            //     && !node.StateNode.HasFlag(StateNode.Road)
            //     && !node.StateNode.HasFlag(StateNode.Protected)
            //     // node.Empty
            //     // && node.Enable
            //     // && !node.Road
            //     // && !node.Protected
            //     && node.KeyArea == area.id
            //     && _root.gridTileHelper.GetDisableNeighbours(node).bottom.Count == 0
            //     && _root.gridTileHelper.GetDisableNeighbours(node).top.Count >= 2
            //     && _root.gridTileHelper.GetDistanceBetweeenPoints(node.position, area.startPosition) > 10
            //     && _root.gridTileHelper.GetNeighbourList(node).Count >= 4
            // ).OrderBy(t => Random.value).ToList();

            if (nodes.Count > 0)
            {
                int countLandscape = Mathf.CeilToInt(
                    LevelManager.Instance.Level.GameModeData.koofMines * .1f * area.countNode);
                area.Stat.countMineN = countLandscape;
                int countCreated = 0;

                while (countCreated < countLandscape && nodes.Count > 0)
                {
                    GridTileNode currentNode = nodes[Random.Range(0, nodes.Count)];

                    GridTileNode nodeWarrior = _root.GetNodeWarrior(currentNode);

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
                    ScriptableEntityMapObject configData = list[UnityEngine.Random.Range(0, list.Count)];

                    if (
                        currentNode != null
                        && nodeWarrior != null
                        && _root.gridTileHelper.GetAllowInsertObjectToNode(currentNode, configData)
                        // currentNode.StateNode.HasFlag(StateNode.Empty)
                        // && !currentNode.StateNode.HasFlag(StateNode.Road)
                        // && !currentNode.StateNode.HasFlag(StateNode.Protected)
                        // // && currentNode.Empty
                        // // && currentNode.Enable
                        // // && !currentNode.Road
                        // // && !currentNode.Protected
                        // && _root.gridTileHelper.CalculateNeighbours(currentNode) >= 5
                        )
                    {
                        EntityMine entity = (EntityMine)factory.CreateMapObject(
                            TypeMapObject.Mine,
                            configData
                        );
                        UnitManager.SpawnEntityMapObjectToNode(currentNode, entity);

                        BaseEntity warrior = UnitManager.SpawnEntityCreature(nodeWarrior);

                        // currentNode.SetProtectedNode(warrior);
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
