using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loader;
using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using Random = UnityEngine.Random;

public class CreateSkillSchoolOperation : ILoadingOperation
{
    private readonly MapManager _root;

    public CreateSkillSchoolOperation(MapManager generator)
    {
        _root = generator;
    }

    public async UniTask Load(Action<float> onProgress, Action<string> onSetNotify)
    {
        onSetNotify("Create skill schools ...");

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
                int maxCountSchool = Mathf.CeilToInt(LevelManager.Instance.GameModeData.koofSchoolSkills * area.countNode);
                area.Stat.countSkillSchoolN = maxCountSchool;
                int countCreated = 0;

                while (countCreated < maxCountSchool && nodes.Count > 0)
                {
                    GridTileNode currentNode = nodes[Random.Range(0, nodes.Count)];

                    GridTileNode nodeWarrior = _root.GetNodeWarrior(currentNode);

                    if (nodeWarrior != null && currentNode != null && _root.gridTileHelper.CalculateNeighbours(currentNode) == 8)
                    {
                        BaseMapEntity unit = await _root.UnitManager
                            .SpawnMapObjectAsync(currentNode, TypeMapObject.SkillSchool);

                        MapEntityCreature warrior = (MapEntityCreature)await _root.UnitManager.SpawnWarriorAsync(nodeWarrior);

                        nodeWarrior.SetProtectedNeigbours(warrior, currentNode);

                        nodes.Remove(currentNode);

                        countCreated++;

                        area.Stat.countSkillSchool++;

                        List<GridTileNode> listExistExitNode = _root.gridTileHelper.IsExistExit(currentNode);
                        if (listExistExitNode.Count > 1)
                        {
                            await _root.CreatePortalAsync(currentNode, listExistExitNode);
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
