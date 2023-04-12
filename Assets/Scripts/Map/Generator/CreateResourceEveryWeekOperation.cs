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
        var factory = new EntityMapObjectFactory();

        for (int x = 0; x < LevelManager.Instance.Level.listArea.Count; x++)
        {
            Area area = LevelManager.Instance.Level.listArea[x];

            List<GridTileNode> nodes = _root.gridTileHelper.GetAllGridNodes().Where(t =>
                    t.StateNode.HasFlag(StateNode.Empty)
                    && !t.StateNode.HasFlag(StateNode.Road)
                    && !t.StateNode.HasFlag(StateNode.Protected)
                    // t.Empty
                    // && t.Enable
                    // && !t.Road
                    // && !t.Protected
                    && t.KeyArea == area.id
                    && _root.gridTileHelper.CalculateNeighbours(t) == 8
                    && _root.gridTileHelper.GetDistanceBetweeenPoints(t.position, area.startPosition) > 5
                ).OrderBy(t => Random.value).ToList();

            if (nodes.Count > 0)
            {
                int maxCountResource = Mathf.CeilToInt(
                    LevelManager.Instance.GameModeData.koofResource * area.countNode);
                area.Stat.countEveryResourceN = maxCountResource;
                int countCreated = 0;

                while (countCreated < maxCountResource && nodes.Count > 0)
                {
                    GridTileNode currentNode = nodes[Random.Range(0, nodes.Count)];

                    GridTileNode nodeWarrior = _root.GetNodeWarrior(currentNode);

                    if (nodeWarrior != null && currentNode != null && _root.gridTileHelper.CalculateNeighbours(currentNode) == 8)
                    {

                        List<TypeWorkObject> typeWork = new List<TypeWorkObject>() {
                            TypeWorkObject.EveryDay, TypeWorkObject.EveryWeek
                        };
                        List<ScriptableEntityMapObject> list = ResourceSystem.Instance
                            .GetEntityByType<ScriptableEntityMapObject>(TypeEntity.MapObject)
                            .Where(t =>
                                t.TypeMapObject == TypeMapObject.Resources
                                && (t.TypeGround & currentNode.TypeGround) == currentNode.TypeGround
                                && typeWork.Contains(t.TypeWorkObject)
                                )
                            .ToList();
                        var configData = list[UnityEngine.Random.Range(0, list.Count)];
                        if (configData != null)
                        {
                            BaseEntity entity = factory.CreateMapObject(
                                TypeMapObject.Resources,
                                currentNode,
                                configData
                                );

                            BaseEntity warrior = new EntityCreature(nodeWarrior); // UnitManager.SpawnWarrior(nodeWarrior);

                            UnitManager.SpawnEntityToNode(currentNode, entity);
                            // _root.UnitManager.SpawnMapObjectAsync(
                            //     currentNode,
                            //     TypeMapObject.Resource,
                            //     new List<TypeWorkPerk>() { TypeWorkPerk.EveryDay, TypeWorkPerk.EveryWeek }
                            // );
                            if (entity.ScriptableData.name == "WaterWheel")
                            {
                                GameManager.Instance.MapManager.CreateCreeks(currentNode);
                            }

                            area.Stat.countEveryResource++;

                            nodeWarrior.SetProtectedNeigbours(warrior, currentNode);
                            // currentNode.SetProtectedNode(warrior);

                            nodes.Remove(currentNode);

                            countCreated++;

                            List<GridTileNode> listExistExitNode = _root.gridTileHelper.IsExistExit(currentNode);

                            if (listExistExitNode.Count > 1)
                            {
                                _root.CreatePortal(currentNode, listExistExitNode);
                            }
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
