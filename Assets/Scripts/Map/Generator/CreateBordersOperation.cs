using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loader;
using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using Random = UnityEngine.Random;

public class CreateBordersOperation : ILoadingOperation
{
    public string Description => "Create Landscape ...";

    private readonly MapManager _root;

    public CreateBordersOperation(MapManager generator)
    {
        _root = generator;
    }

    public async UniTask Load(Action<float> onProgress)
    {
        var task1 = CreateEdgeMountain();
        var task2 = CreateBorderMountain();
        var task3 = CreateRandomMountain();

        await UniTask.WhenAll(task1, task2, task3);

        //await UniTask.Delay(1);
    }

    /// <summary>
    /// Create mountain for border areas
    /// </summary>
    /// <returns>UniTask</returns>
    private async UniTask CreateBorderMountain()
    {
        List<GridTileNode> nodes = _root.gridTileHelper.GetAllGridNodes().Where(t =>
            _root.gridTileHelper.CalculateNeighboursByArea(t) < 4
            && t.Empty
            && t.Enable
            //&& !t.isEdge
            && (
                t.X > 1 &&
                t.X < _root.gameModeData.width - 2 &&
                t.Y > 1 &&
                t.Y < _root.gameModeData.height - 2
            )
        ).ToList();

        foreach (GridTileNode tileNode in nodes)
        {
            if (tileNode.Empty && tileNode.Enable)
            {

                TileLandscape tileData = _root._dataTypeGround[tileNode.TypeGround];
                List<TileNature> listNature = ResourceSystem.Instance.GetNature().Where(t =>
                            t.typeGround == tileData.typeGround
                            && t.isCorner
                        ).ToList();
                TileNature cornerTiles = listNature[Random.Range(0, listNature.Count)];

                _root._tileMapNature.SetTile(tileNode.position, cornerTiles);

                _root._listNatureNode.Add(new GridTileNatureNode(tileNode, cornerTiles.idObject, false, cornerTiles.name));

                _root.gridTileHelper.SetDisableNode(tileNode, cornerTiles.listTypeNoPath, Color.blue);

            }
        }

        await UniTask.Delay(1);
    }

    /// <summary>
    /// Create mountain for edge map
    /// </summary>
    /// <returns>UniTask</returns>
    private async UniTask CreateEdgeMountain()
    {

        foreach (GridTileNode tileNode in _root.gridTileHelper.GetAllGridNodes().Where(t =>
            t.isEdge
            && _root.gridTileHelper.CalculateNeighbours(t) >= 5
            && t.Empty
            && t.Enable
        ))
        {
            TileLandscape tileData = _root._dataTypeGround[tileNode.TypeGround];

            List<TileNature> listNature = ResourceSystem.Instance.GetNature().Where(t =>
                        t.typeGround == tileData.typeGround
                        && t.isCorner
                    ).ToList();

            TileNature cornerTiles = listNature[Random.Range(0, listNature.Count)];

            _root._tileMapNature.SetTile(tileNode.position, cornerTiles);

            _root._listNatureNode.Add(new GridTileNatureNode(tileNode, cornerTiles.idObject, false, cornerTiles.name));

            _root.gridTileHelper.SetDisableNode(tileNode, cornerTiles.listTypeNoPath, Color.blue);

        }

        await UniTask.Delay(1);
    }

    private async UniTask CreateRandomMountain()
    {
        if (LevelManager.Instance.GameModeData.noiseScaleMontain == 0 || LevelManager.Instance.GameModeData.koofMountains == 0)
        {
            return;
        }
        // Random value for noise.
        var xOffSet = Random.Range(-10000f, 10000f);
        var zOffSet = Random.Range(-10000f, 10000f);

        for (int x = 0; x < _root.gameModeData.width; x++)
        {
            for (int y = 0; y < _root.gameModeData.height; y++)
            {
                GridTileNode currentNode = _root.gridTileHelper.GetNode(x, y);

                float noiseValue = Mathf.PerlinNoise(x * LevelManager.Instance.GameModeData.noiseScaleMontain + xOffSet, y * LevelManager.Instance.GameModeData.noiseScaleMontain + zOffSet);

                bool isMountain = noiseValue < LevelManager.Instance.GameModeData.koofMountains;

                //Area area = LevelManager.Instance.GetArea(currentNode.keyArea);

                //float minCountNoMountain = area.countNode * LevelManager.Instance.koofMountains;

                // Create Mountain.
                if (isMountain && currentNode.Empty && currentNode.Enable && _root.gridTileHelper.GetNeighbourListWithTypeGround(currentNode).Count > 3)
                {
                    TileLandscape tileData = _root._dataTypeGround[currentNode.TypeGround];

                    List<TileNature> listTileForDraw = ResourceSystem.Instance.GetNature().Where(t =>
                        t.typeGround == tileData.typeGround
                        && !t.isWalkable
                    ).ToList(); //  _tileData.cornerTiles.Concat(_tileData.natureTiles).ToList();

                    TileNature tileForDraw = listTileForDraw[Random.Range(0, listTileForDraw.Count)];

                    if (currentNode.Empty && currentNode.Enable)
                    {
                        _root._tileMapNature.SetTile(currentNode.position, tileForDraw);

                        _root._listNatureNode.Add(new GridTileNatureNode(currentNode, tileForDraw.idObject, false, tileForDraw.name));

                        _root.gridTileHelper.SetDisableNode(currentNode, tileForDraw.listTypeNoPath, Color.black);

                    }

                    //area.countMountain++;
                }
            }
        }

        await UniTask.Delay(1);
    }

}
