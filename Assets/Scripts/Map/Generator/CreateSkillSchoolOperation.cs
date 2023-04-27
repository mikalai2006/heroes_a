using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loader;
using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using Random = UnityEngine.Random;
using UnityEngine.Localization;

public class CreateSkillSchoolOperation : ILoadingOperation
{
    private readonly MapManager _root;

    public CreateSkillSchoolOperation(MapManager generator)
    {
        _root = generator;
    }

    public async UniTask Load(Action<float> onProgress, Action<string> onSetNotify)
    {
        var t = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "createdgameobject").GetLocalizedString();
        onSetNotify(t + " skill schools ...");

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
                && _root.gridTileHelper.GetDistanceBetweeenPoints(t.position, area.startPosition) > 10
            ).OrderBy(t => Random.value).ToList();

            if (nodes.Count > 0)
            {
                int maxCountSchool = Mathf.CeilToInt(
                    LevelManager.Instance.Level.GameModeData.koofSchoolSkills * area.countNode);
                area.Stat.countSkillSchoolN = maxCountSchool;
                int countCreated = 0;

                while (countCreated < maxCountSchool && nodes.Count > 0)
                {
                    GridTileNode currentNode = nodes[Random.Range(0, nodes.Count)];

                    GridTileNode nodeWarrior = _root.GetNodeWarrior(currentNode);

                    List<TypeWorkObject> typeWork = new List<TypeWorkObject>() {
                            TypeWorkObject.One
                        };
                    List<ScriptableEntityMapObject> list = ResourceSystem.Instance
                        .GetEntityByType<ScriptableEntityMapObject>(TypeEntity.MapObject)
                        .Where(t => (
                            t.TypeGround & currentNode.TypeGround) == currentNode.TypeGround
                            && t.TypeMapObject == TypeMapObject.Skills
                            )
                        .ToList();
                    var configData = list[UnityEngine.Random.Range(0, list.Count)];

                    if (
                        nodeWarrior != null
                        && currentNode != null
                        && configData != null
                        && _root.gridTileHelper.GetAllowInsertObjectToNode(currentNode, configData)
                        // && _root.gridTileHelper.CalculateNeighbours(currentNode) == 8
                        )
                    {
                        // var factory = new EntityMapObjectFactory();

                        EntityMapObject entity = new EntityMapObject(configData);

                        UnitManager.SpawnEntityMapObjectToNode(currentNode, entity);
                        // _root.UnitManager
                        //     .SpawnMapObjectAsync(currentNode, TypeMapObject.SkillSchool);

                        MapObject warrior = UnitManager.SpawnEntityCreature(nodeWarrior, currentNode, 1, configData.RMGValue);

                        // nodeWarrior.SetProtectedNeigbours(warrior, currentNode);
                        // currentNode.SetProtectedNode(warrior);

                        nodes.Remove(currentNode);

                        countCreated++;

                        area.Stat.countSkillSchool++;

                        List<GridTileNode> listExistExitNode = _root.gridTileHelper.IsExistExit(currentNode);
                        if (listExistExitNode.Count > 1)
                        {
                            _root.CreatePortal(currentNode, listExistExitNode);
                        }
                    }
                    else
                    {
                        nodes.Remove(currentNode);
                        //break;
                    }
                }

            }
        }
        await UniTask.Delay(1);
    }
}
