using System.Collections.Generic;
using UnityEngine;
using Loader;
using Random = UnityEngine.Random;
using Cysharp.Threading.Tasks;
using System;
using System.Linq;

public class CreateResourceOperation : ILoadingOperation
{
    private readonly MapManager _root;

    public CreateResourceOperation(MapManager generator)
    {
        _root = generator;
    }

    public async UniTask Load(Action<float> onProgress, Action<string> onSetNotify)
    {
        onSetNotify("Create resource ...");

        for (int x = 0; x < LevelManager.Instance.Level.listArea.Count; x++)
        {
            Area area = LevelManager.Instance.Level.listArea[x];

            List<GridTileNode> nodes = _root.gridTileHelper.GetAllGridNodes().Where(t =>
                t.Empty
                && t.Enable
                && !t.Protected
                && t.KeyArea == area.id
                && _root.gridTileHelper.CalculateNeighbours(t) > 2
                && _root.gridTileHelper.GetDistanceBetweeenPoints(t.position, area.startPosition) > 5
            ).OrderBy(t => Random.value).ToList();

            if (nodes.Count > 0)
            {
                int maxCountResource = Mathf.CeilToInt(LevelManager.Instance.GameModeData.koofFreeResource * area.countNode);

                area.Stat.countFreeResourceN = maxCountResource;

                int countCreated = 0;

                while (countCreated < maxCountResource && nodes.Count > 0)
                {
                    GridTileNode currentNode = nodes[Random.Range(0, nodes.Count)];

                    BaseMapEntity unit = await _root.UnitManager.SpawnResourceAsync(currentNode, new List<TypeWorkPerk>() { TypeWorkPerk.One });

                    nodes.Remove(currentNode);

                    countCreated++;

                    area.Stat.countFreeResource++;

                    List<GridTileNode> listExistExitNode = _root.gridTileHelper.IsExistExit(currentNode);

                    if (listExistExitNode.Count > 1)
                    {
                        await _root.CreatePortalAsync(currentNode, listExistExitNode);
                    }
                }

            }
        }

        await UniTask.Delay(1);
    }
}
