using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loader;
using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using Random = UnityEngine.Random;

public class CreateNatureOperation : ILoadingOperation
{
    private readonly MapManager _root;

    public CreateNatureOperation(MapManager generator)
    {
        _root = generator;
    }
    public async UniTask Load(Action<float> onProgress, Action<string> onSetNotify)
    {
        onSetNotify("Create nature ...");

        for (int x = 0; x < LevelManager.Instance.Level.listArea.Count; x++)
        {
            Area area = LevelManager.Instance.Level.listArea[x];

            List<GridTileNode> nodes = _root.gridTileHelper.GetAllGridNodes().Where(t =>
                t.Empty
                && t.Enable
                && !t.Road
                && !t.Protected
                && t.KeyArea == area.id
                && _root.gridTileHelper.CalculateNeighbours(t) > 6
            ).OrderBy(t => Random.value).ToList();
            if (nodes.Count > 0)
            {
                int countLandscape = Mathf.CeilToInt(LevelManager.Instance.GameModeData.koofNature * nodes.Count * .1f);
                int countCreated = 0;
                while (countCreated < countLandscape && nodes.Count > 0)
                {
                    GridTileNode currentNode = nodes[0];
                    if (currentNode != null && _root.gridTileHelper.CalculateNeighbours(currentNode) > 6)
                    {
                        TileLandscape tileData = _root._dataTypeGround[currentNode.TypeGround];
                        List<TileNature> listNature = ResourceSystem.Instance.GetNature().Where(t =>
                            t.typeGround == tileData.typeGround
                            && !t.isCorner
                        ).ToList().ToList();
                        TileNature tileForDraw = listNature[Random.Range(0, listNature.Count)]; // _tileData.natureTiles[Random.Range(0, _tileData.natureTiles.Count)];

                        _root._tileMapNature.SetTile(currentNode.position, tileForDraw);

                        _root._listNatureNode.Add(new GridTileNatureNode(currentNode, tileForDraw.idObject, false, tileForDraw.name));

                        _root.gridTileHelper.SetDisableNode(currentNode, tileForDraw.listTypeNoPath, Color.green);

                        nodes.Remove(currentNode);

                        countCreated++;
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
