using System;

using UnityEngine;

public class GridArena<T>
{
    private readonly int _width;
    private readonly int _height;
    private readonly float _cellSize;
    private readonly T[,] _gridArray;

    private const float HEX_VERTICAL_OFFSET = 0.75f;

    public int SizeGrid { get { return _width * _height; } private set { } }

    public GridArena(int width, int height, float cellSize, GridArenaHelper gridArenaHelper, Func<GridArena<T>, GridArenaHelper, int, int, T> createValue)
    {
        _width = width;
        _height = height;
        _cellSize = cellSize;

        _gridArray = new T[width, height];

        for (int x = 0; x < _gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < _gridArray.GetLength(1); y++)
            {
                _gridArray[x, y] = createValue(this, gridArenaHelper, x, y);
            }
        }

    }

    public void SetValue(int x, int y, T value)
    {
        _gridArray[x, y] = value;
    }

    public T[,] GetGrid()
    {
        return _gridArray;
    }
    public T GetGridObject(Vector3Int pos)
    {
        //Debug.Log($"GetGrid {x},{z}: {GetWorldPosition(x, z)}");
        return pos.x >= 0 && pos.y >= 0 && pos.x < _width && pos.y < _height ? _gridArray[pos.x, pos.y] : default;
    }
    public T GetGridObject(int x, int y)
    {
        Vector3Int pos = new Vector3Int(x, y);
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

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, 0, 0) * _cellSize
        + new Vector3(0, y, 0) * _cellSize * HEX_VERTICAL_OFFSET
        + ((y % 2 == 1) ? new Vector3(1, 0, 0) * 0.5f : Vector3.zero);
    }

}
