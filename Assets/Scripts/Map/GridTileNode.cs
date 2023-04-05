
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public enum TypeStateNode
{
    Disabled = 0,
    Enable = 1,
    // Empty = 2,
    //Protected = 3,
}

[Serializable]
public class GridTileNode : IHeapItem<GridTileNode>
{
    [NonSerialized] private readonly GridTile<GridTileNode> _grid;
    [NonSerialized] private readonly GridTileHelper _gridHelper;
    public int X;
    public int Y;
    public int KeyArea = 0;
    public TypeGround TypeGround = TypeGround.None;
    [SerializeField] private TypeStateNode _state;
    public TypeStateNode State
    {
        get { return _state; }
        set { _state = value; }
    }

    //[SerializeField] public bool _isWalkable;
    public bool Empty => _ocuppiedUnit == null; // _isWalkable && OccupiedUnit == null; // _noPath == false && 
    public bool Disable => State == TypeStateNode.Disabled;
    public bool Enable => State == TypeStateNode.Enable;

    [SerializeField] public bool _isRoad = false;
    public bool Road => _isRoad;

    [NonSerialized] public int level;
    [NonSerialized] public Vector3Int position;

    [NonSerialized] private BaseMapEntity _ocuppiedUnit = null;
    public BaseMapEntity OccupiedUnit => _ocuppiedUnit;
    [NonSerialized] private BaseMapEntity _protectedUnit = null;
    public BaseMapEntity ProtectedUnit => _protectedUnit;
    public bool Protected => _protectedUnit != null;

    private int heapIndex;
    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    [NonSerialized] public int countRelatedNeighbors = 0;
    [NonSerialized] public GridTileNode cameFromNode;
    [NonSerialized] public int gCost;
    [NonSerialized] public int hCost;
    [NonSerialized] public float fCost;
    [NonSerialized] public int koofPath = 10;
    [NonSerialized] public int levelPath = 0;
    [NonSerialized] public bool isCreated = false;
    [NonSerialized] public bool isEdge = false;
    //[NonSerialized] public bool isNature = false;


    public GridTileNode(GridTile<GridTileNode> grid, GridTileHelper gridHelper, int x, int y)
    {
        position = new Vector3Int(x, y, 0);
        _gridHelper = gridHelper;
        _grid = grid;
        this.X = x;
        this.Y = y;
        //_isWalkable = tileData.isWalkable;
        //typeGround = tileData.typeGround;
    }

    public int CompareTo(GridTileNode nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
            compare = hCost.CompareTo(nodeToCompare.hCost);
        return -compare;
    }

    public void SetState(TypeStateNode state)
    {
        _state = state;
        koofPath = 20;

    }

    public void SetProtectedUnit(BaseMapEntity unit)
    {
        _protectedUnit = unit;


        //if (!Disable)
        //{
        //    if (unit == null)
        //    {
        //        _state = _ocuppiedUnit != null ? TypeStateNode.Busy : TypeStateNode.Empty;
        //    } else
        //    {
        //        _state = TypeStateNode.Protected;
        //    }
        //}
        GameManager.Instance.MapManager.SetColorForTile(
            position,
            _protectedUnit == null ? Color.yellow : Color.red
        );
    }

    public void SetOcuppiedUnit(BaseMapEntity unit)
    {
        _ocuppiedUnit = unit;
    }

    public void SetProtectedNeigbours(BaseWarriors warriorUnit, GridTileNode protectedNode = null)
    {
        // if (warriorUnit == null) return;

        List<GridTileNode> nodes = _gridHelper.GetNeighbourList(this, true);

        if (nodes != null || nodes.Count > 0)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (
                    (nodes[i].Enable || !nodes[i].Empty)
                    && (!nodes[i].Protected || nodes[i].ProtectedUnit == _protectedUnit)
                    )
                {
                    nodes[i].SetProtectedUnit(warriorUnit); // ._protectedUnit = warriorUnit;
                    //Debug.LogWarning($"" +
                    //$"position=[{nodes[i]._position}]\n" +
                    //$"ocupiedunit=[{nodes[i].OccupiedUnit?.name}]\n" +
                    //$"warriorunit=[{nodes[i].ProtectedUnit?.name}]\n"
                    //);
                }
            }
        }

        SetProtectedUnit(warriorUnit);
        if (warriorUnit != null)
        {
            warriorUnit.ProtectedNode = protectedNode;
            warriorUnit.OnChangeQuantityWarrior();
        }
    }


    public void CalculateFCost()
    {
        fCost = gCost + hCost; // + (int)dataNode.speed;

    }
    public override string ToString()
    {
        return "GridTileNode:::" +
            "[x" + position.x + ",y" + position.y + "] \n" +
            "Empty=" + Empty + ",\n" +
            "Disable=" + Disable + ",\n" +
            "Enable=" + Enable + ",\n" +
            "Protected=" + Protected + ",\n" +
            "Road=" + Road + ",\n" +
            "keyArea=" + KeyArea + ",\n" +
            "typeGround=" + TypeGround + ",\n" +
            "OccupiedUnit=" + OccupiedUnit?.ToString() + ",\n" +
            "ProtectedUnit=" + ProtectedUnit?.ToString() + ",\n" +
            "countNeighbours=" + countRelatedNeighbors + ",\n" +
            "(gCost=" + gCost + ") (hCost=" + hCost + ") (fCost=" + fCost + ")";
    }

    public void SetAsRoad()
    {
        koofPath = 2;
        _isRoad = true;
    }

}