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

                        BaseEntity warrior = _root.UnitManager.SpawnWarriorAsync(nodeWarrior);

                        List<TypeWorkPerk> typeWork = new List<TypeWorkPerk>() {
                            TypeWorkPerk.EveryDay, TypeWorkPerk.EveryWeek
                        };
                        List<ScriptableEntityMapObject> list = ResourceSystem.Instance
                            .GetEntityByType<ScriptableEntityMapObject>(TypeEntity.GroupResource)
                            // .Where(t => listTypeWork.Contains(t.TypeWorkPerk))
                            .Where(t => (
                                t.TypeGround & currentNode.TypeGround) == currentNode.TypeGround
                                && typeWork.Contains(t.TypeWorkPerk)
                                )
                            .ToList();
                        var configData = list[UnityEngine.Random.Range(0, list.Count)];
                        if (configData != null)
                        {
                            BaseEntity entity = new EntityMapObject(currentNode,
                                configData,
                                TypeEntity.GroupResource
                                // new List<TypeWorkPerk>() { TypeWorkPerk.EveryDay, TypeWorkPerk.EveryWeek }
                                );
                            _root.UnitManager.SpawnEntityToNode(currentNode, entity);
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
