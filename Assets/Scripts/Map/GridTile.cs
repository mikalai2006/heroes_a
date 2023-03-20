using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTile<TGridObject>
{

    private int width;
    private int height;
    private float _cellSize;
    private TGridObject[,] gridArray;

    public int SizeGrid { get { return width * height; } private set { } }

    public GridTile(int width, int height, float cellSize, GridTileHelper gridTileHelper, Func<GridTile<TGridObject>, GridTileHelper, int, int, TGridObject> createValue)
    {
        this.width = width;
        this.height = height;
        this._cellSize = cellSize;

        gridArray = new TGridObject[width,height];

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                gridArray[x, y] = createValue(this, gridTileHelper, x, y);
            }
        }

        // return gridArray;
    }

    private Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, 0, y) * _cellSize;
    }

    public void SetValue(int x, int y, TGridObject Value)
    {
        gridArray[x, y] = Value;
    }

    public TGridObject[,] GetGrid()
    {
        return gridArray;
    }
    public TGridObject getGridObject(Vector3Int pos)
    {
        //Debug.Log($"GetGrid {x},{z}: {GetWorldPosition(x, z)}");
        if (pos.x >= 0 && pos.y >= 0 && pos.x < width && pos.y < height)
        {
            return gridArray[pos.x, pos.y];
        } else
        {
            return default(TGridObject);
        }
    }

    public int GetHeight()
    {
        return height;
    }

    public int GetWidth()
    {
        return width;
    }

}
