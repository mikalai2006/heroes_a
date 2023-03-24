using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class GridTile<TGridObject>
{

    private readonly int _width;
    private readonly int _height;
    private readonly float _cellSize;
    private readonly TGridObject[,] _gridArray;

    public int SizeGrid { get { return _width * _height; } private set { } }

    public GridTile(int width, int height, float cellSize, GridTileHelper gridTileHelper, Func<GridTile<TGridObject>, GridTileHelper, int, int, TGridObject> createValue)
    {
        _width = width;
        _height = height;
        _cellSize = cellSize;

        _gridArray = new TGridObject[width, height];

        for (int x = 0; x < _gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < _gridArray.GetLength(1); y++)
            {
                _gridArray[x, y] = createValue(this, gridTileHelper, x, y);
            }
        }

        // return gridArray;
    }

    // private Vector3 GetWorldPosition(int x, int y)
    // {
    //     return new Vector3(x, 0, y) * _cellSize;
    // }

    public void SetValue(int x, int y, TGridObject value)
    {
        _gridArray[x, y] = value;
    }

    public TGridObject[,] GetGrid()
    {
        return _gridArray;
    }
    public TGridObject GetGridObject(Vector3Int pos)
    {
        //Debug.Log($"GetGrid {x},{z}: {GetWorldPosition(x, z)}");
        return pos.x >= 0 && pos.y >= 0 && pos.x < _width && pos.y < _height ? _gridArray[pos.x, pos.y] : default;
    }

    public int GetHeight()
    {
        return _height;
    }

    public int GetWidth()
    {
        return _width;
    }

}
