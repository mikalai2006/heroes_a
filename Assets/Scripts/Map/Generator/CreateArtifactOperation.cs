using Cysharp.Threading.Tasks;

using Loader;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Random = UnityEngine.Random;

public class CreateArtifactOperation : ILoadingOperation
{
    private readonly MapManager _root;

    public CreateArtifactOperation(MapManager generator)
    {
        _root = generator;
    }

    public async UniTask Load(Action<float> onProgress, Action<string> onSetNotify)
    {
        onSetNotify?.Invoke("Create artifacts ...");

        for (int x = 0; x < LevelManager.Instance.Level.listArea.Count; x++)
        {
            Area area = LevelManager.Instance.Level.listArea[x];

            List<GridTileNode> nodes = _root.gridTileHelper.GetAllGridNodes().Where(t =>
                t.Empty
                && t.Enable
                && !t.Protected
                && t.KeyArea == area.id
                && _root.gridTileHelper.CalculateNeighbours(t) < 2
                && _root.gridTileHelper.GetDistanceBetweeenPoints(t.position, area.startPosition) > 10
            ).OrderBy(t => Random.value).ToList();

            if (nodes.Count > 0)
            {
                int maxCount = Mathf.CeilToInt(LevelManager.Instance.GameModeData.koofArtifacts * area.countNode);

                area.Stat.countArtifactN = maxCount;

                int countCreated = 0;

                while (countCreated < maxCount && nodes.Count > 0)
                {
                    GridTileNode currentNode = nodes[Random.Range(0, nodes.Count)];
                    GridTileNode nodeWarrior = _root.GetNodeWarrior(currentNode);

                    if (nodeWarrior != null
                        && nodeWarrior.Empty
                        && currentNode != null
                        //&& gridTileHelper.CalculateNeighbours(currentNode) < 3
                        )
                    {
                        BaseEntity entity = new EntityArtifact(currentNode, null);
                        _root.UnitManager.SpawnEntityToNode(currentNode, entity);
                        //_root.UnitManager.SpawnArtifactAsync(currentNode);

                        BaseEntity warrior = _root.UnitManager.SpawnWarriorAsync(nodeWarrior);

                        nodeWarrior.SetProtectedNeigbours(warrior, currentNode);

                        nodes.Remove(currentNode);

                        countCreated++;

                        area.Stat.countArtifact++;

                        List<GridTileNode> listExistExitNode = _root.gridTileHelper.IsExistExit(currentNode);
                        if (listExistExitNode.Count > 1)
                        {
                            _root.CreatePortal(currentNode, listExistExitNode);
                        }
                        //onProgress?.Invoke((float)countCreated / (float)maxCount);
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
