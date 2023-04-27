using System.Collections.Generic;
using UnityEngine;
using Loader;
using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using Random = UnityEngine.Random;
using UnityEngine.Localization;

public class CreateDwellingOperation : ILoadingOperation
{
    private readonly MapManager _root;

    public CreateDwellingOperation(MapManager generator)
    {
        _root = generator;
    }

    public async UniTask Load(Action<float> onProgress, Action<string> onSetNotify)
    {
        var t = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "createdgameobject").GetLocalizedString();
        onSetNotify(t + " dwellings ...");

        for (int keyArea = 0; keyArea < LevelManager.Instance.Level.listArea.Count; keyArea++)
        {
            Area area = LevelManager.Instance.Level.listArea[keyArea];

            if (area.town != null)
            {
                EntityTown townArea = (EntityTown)area.town;

                List<GridTileNode> nodes = _root.gridTileHelper.GetAllGridNodes().Where(t =>
                    t.StateNode.HasFlag(StateNode.Empty)
                    && !t.StateNode.HasFlag(StateNode.Road)
                    && !t.StateNode.HasFlag(StateNode.Protected)
                    // node.Empty
                    // && node.Enable
                    // && !node.Road
                    // && !node.Protected
                    && t.KeyArea == area.id
                    && _root.gridTileHelper.GetDistanceBetweeenPoints(t.position, area.startPosition) > 10
                    && _root.gridTileHelper.GetNeighbourList(t).Count >= 4
                ).OrderBy(t => Random.value).ToList();

                if (nodes.Count > 0)
                {
                    int count = Mathf.CeilToInt(
                        LevelManager.Instance.Level.GameModeData.koofDwelling * .1f * area.countNode);
                    area.Stat.countDwellingN = count;
                    int countCreated = 0;
                    List<ScriptableEntityMapObject> list = ResourceSystem.Instance
                        .GetEntityByType<ScriptableEntityMapObject>(TypeEntity.MapObject)
                        .Where(t =>
                            t.TypeMapObject == TypeMapObject.Dwelling
                            && ((ScriptableEntityDwelling)t).TypeFaction == townArea.ConfigData.TypeFaction
                            // && (
                            //     (t.TypeGround & currentNode.TypeGround) == currentNode.TypeGround
                            //     ||
                            //     (t.TypeGround & TypeGround.None) == TypeGround.None
                            //     )
                            )
                        .ToList();

                    if (list.Count != 0)
                    {

                        while (countCreated < count && nodes.Count > 0)
                        {
                            GridTileNode currentNode = nodes[Random.Range(0, nodes.Count)];

                            NeighboursNature disableNeighbours
                                = _root.gridTileHelper.GetDisableNeighbours(currentNode);

                            GridTileNode nodeWarrior = _root.GetNodeWarrior(currentNode);

                            ScriptableEntityDwelling configData
                                = (ScriptableEntityDwelling)list[UnityEngine.Random.Range(0, list.Count)];

                            if (
                                currentNode != null
                                && nodeWarrior != null
                                && disableNeighbours.bottom.Count == 0
                                // && disableNeighbours.top.Count >= 2
                                // && _root.gridTileHelper.CalculateNeighbours(currentNode) >= 5
                                && configData != null
                                && _root.gridTileHelper.GetAllowInsertObjectToNode(currentNode, configData)
                                )
                            {

                                EntityDwelling entity = new EntityDwelling(configData);
                                UnitManager.SpawnEntityMapObjectToNode(currentNode, entity);

                                MapObject warrior = UnitManager.SpawnEntityCreature(nodeWarrior, currentNode, 1, configData.RMGValue);

                                // nodeWarrior.SetProtectedNeigbours(warrior, currentNode);
                                // currentNode.SetProtectedNode(warrior);

                                nodes.Remove(currentNode);

                                countCreated++;

                                area.Stat.countDwelling++;

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
            }
        }

        await UniTask.Delay(1);
    }
}
