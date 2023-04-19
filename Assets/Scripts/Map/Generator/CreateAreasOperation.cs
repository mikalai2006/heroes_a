using Cysharp.Threading.Tasks;

using Loader;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;

using Random = UnityEngine.Random;

public class CreateAreasOperation : ILoadingOperation
{

    private readonly MapManager _root;
    private float _maxSizeOneWorld;
    private Dictionary<Vector3Int, GridTileNode> _transferOpenList;
    private Dictionary<Vector3Int, GridTileNode> _transferClosedList;
    private Dictionary<Vector3Int, GridTileNode> _openList;
    private Dictionary<Vector3Int, GridTileNode> _closedList;

    public CreateAreasOperation(MapManager generator)
    {
        _root = generator;
    }

    public async UniTask Load(Action<float> onProgress, Action<string> onSetNotify)
    {
        var t = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "createdgameobject").GetLocalizedString();
        onSetNotify(t + " areas ...");

        var listPlayers = _root.Level.listPlayer;
        var countPlayer = listPlayers.Count;

        List<TileLandscape> listTileData
            = _root._dataTypeGround.Values
            .Where(t => t.typeGround != TypeGround.None && t.typeGround != TypeGround.Sand).ToList();

        _maxSizeOneWorld = Mathf.CeilToInt(
            (
                (_root.Level.GameModeData.width * _root.Level.GameModeData.height)
                / (_root.Level.Settings.countPlayer + _root.Level.Settings.countBot)
                )
            * _root.Level.GameModeData.koofSizeArea
        );

        _closedList = new Dictionary<Vector3Int, GridTileNode>();

        _openList = _root.gridTileHelper.GetAllGridNodes().ToDictionary(t => t.position, t => t);

        float oneProgressValue = 1f / (float)countPlayer;

        for (int i = 0; i < listPlayers.Count; i++)
        {

            TileLandscape tileData;
            if (listPlayers[i].StartSetting.town != null)
            {
                tileData = listTileData
                    .Where(t => t.typeGround == listPlayers[i].StartSetting.town.TypeGround)
                    .First();
            }
            else
            {
                tileData = listTileData[Random.Range(0, listTileData.Count)];
            }

            GridTileNode randomNode = _openList.ElementAt(Random.Range(0, _openList.Count)).Value;

            List<GridTileNode> listNeighbors = _root.gridTileHelper.GetNeighbourListWithTypeGround(randomNode);

            while (listNeighbors.Count != 4)
            {
                randomNode = _openList.ElementAt(Random.Range(0, _openList.Count)).Value;

                listNeighbors = _root.gridTileHelper.GetNeighbourListWithTypeGround(randomNode);
            }

            LevelManager.Instance.AddArea(i, tileData, listPlayers[i].DataPlayer.id);

            await CreateArea(i, randomNode, tileData);

            onProgress?.Invoke(i * oneProgressValue);

            randomNode.isCreated = true;

        }

        if (_openList.Count > 0)
        {
            int keyEmptyArea = countPlayer;

            listTileData = _root._dataTypeGround.Values.Where(t => t.typeGround != TypeGround.None).ToList();

            while (_openList.Count > 0)
            {
                TileLandscape randomTileData = listTileData[Random.Range(0, listTileData.Count)];

                LevelManager.Instance.AddArea(keyEmptyArea, randomTileData);

                await CreateArea(keyEmptyArea, _openList.ElementAt(0).Value, randomTileData);

                keyEmptyArea++;
            }
        }

        Debug.LogWarning($"AllTileNodes::: noCreated-{_openList.Count}[Created={_closedList.Count}]");

        _closedList.Clear();

        _openList.Clear();

        await UniTask.Delay(1);
    }


    public async UniTask CreateArea(int keyArea, GridTileNode startNode, TileLandscape tileData)
    {
        Area area = LevelManager.Instance.GetArea(keyArea);

        area.startPosition = startNode.position;

        _transferOpenList = new Dictionary<Vector3Int, GridTileNode>
        {
            { startNode.position, startNode }
        };
        _transferClosedList = new Dictionary<Vector3Int, GridTileNode>();

        startNode.level = 0;
        startNode.KeyArea = keyArea;
        float maxCountNode = (
            (_maxSizeOneWorld) -
            (_root.gridTileHelper.GridTile.GetHeight() * _root.gridTileHelper.GridTile.GetWidth() * .03f)
            );

        while (_transferOpenList.Count > 0 && _openList.Count > 0)
        {

            GridTileNode currentNode = _root.gridTileHelper.GetRandomTileNode(_transferOpenList); // _transferOpenList.ElementAt(0).Value;

            Vector3Int currentNodePosition = currentNode.position; // new Vector3Int(currentNode.x, currentNode.y);

            currentNode.isCreated = true;
            currentNode.TypeGround = tileData.typeGround;

            _transferOpenList.Remove(currentNodePosition);
            _openList.Remove(currentNodePosition);

            _transferClosedList.Add(currentNodePosition, currentNode);
            _closedList.Add(currentNodePosition, currentNode);

            if (area.countNode <= maxCountNode)
            {
                List<GridTileNode> listNeighbors = _root.gridTileHelper.GetNeighbourList(currentNode);//.OrderBy(t => Random.value).ToList();
                currentNode.countRelatedNeighbors = listNeighbors.Count;

                for (int x = 0; x < listNeighbors.Count; x++)
                {
                    GridTileNode neighbourNode = listNeighbors[x];

                    Vector3Int neighbourPosition = neighbourNode.position; // new Vector3Int(neighbourNode.x, neighbourNode.y);

                    //List<GridTileNode> listCreatedNeighbours = listNeighbors.Where(t => t.isCreated).ToList();

                    if (
                        _closedList.ContainsKey(neighbourPosition)
                        && _transferClosedList.ContainsKey(neighbourPosition)
                    )
                    {
                        //if (neighbourNode.typeGround == currentNode.typeGround && neighbourNode.keyArea == currentNode.keyArea)
                        //{
                        //    // neighbourNode.countRelatedNeighbors += 1;
                        //    currentNode.countRelatedNeighbors += 1;
                        //}
                        continue;
                    }

                    if (neighbourNode.isCreated)
                    {
                        if (!_closedList.ContainsKey(neighbourPosition)) _closedList.Add(neighbourPosition, neighbourNode);
                        _transferClosedList.Add(neighbourPosition, neighbourNode);
                        continue;
                    }
                    if (!_transferOpenList.ContainsKey(neighbourPosition)) //  && listNeighbors.Count == 4
                    {
                        neighbourNode.level = currentNode.level + 1;
                        neighbourNode.KeyArea = currentNode.KeyArea;
                        _transferOpenList.Add(neighbourPosition, neighbourNode);
                        area.countNode++;
                        //neighbourNode.countRelatedNeighbors += 1;
                        //currentNode.countRelatedNeighbors += 1;
                    }

                }

            }
            else
            {
                List<GridTileNode> listNeighbors = _root.gridTileHelper.GetNeighbourListWithTypeGround(currentNode);//.OrderBy(t => Random.value).ToList();
                currentNode.countRelatedNeighbors = listNeighbors.Count;

            }

        }
        await UniTask.Delay(1);
    }
}
