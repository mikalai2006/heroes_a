
using System;
using System.Collections.Generic;

using UnityEngine;

[Flags]
public enum StateNode
{
    Disable = 1 << 0,
    Empty = 1 << 1,
    Protected = 1 << 2,
    Occupied = 1 << 3,
    Road = 1 << 4,
    Teleport = 1 << 5,
    Cave = 1 << 6,
    Town = 1 << 7,
    Input = 1 << 8
}


[Serializable]
public class GridTileNode : IHeapItem<GridTileNode>
{
    [NonSerialized] private readonly GridTile<GridTileNode> _grid;
    [NonSerialized] private readonly GridTileHelper _gridHelper;
    [SerializeField] public StateNode StateNode = StateNode.Empty;
    public int X;
    public int Y;
    public int KeyArea = 0;
    public TypeGround TypeGround = TypeGround.None;

    [NonSerialized] public int level;
    [NonSerialized] public Vector3Int position;
    [NonSerialized] private GridTileNode _inputNode = null;
    public GridTileNode InputNode => _inputNode;

    [NonSerialized] private BaseEntity _ocuppiedUnit = null;
    public BaseEntity OccupiedUnit => _ocuppiedUnit;
    [NonSerialized] private BaseEntity _protectedUnit = null;
    public BaseEntity ProtectedUnit => _protectedUnit;
    public bool Protected => _protectedUnit != null;
    public bool IsAllowSpawn =>
        (StateNode.Empty | ~StateNode.Protected | ~StateNode.Occupied) == (StateNode.Empty | ~StateNode.Protected | ~StateNode.Occupied);

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

    public GridTileNode(GridTile<GridTileNode> grid, GridTileHelper gridHelper, int x, int y)
    {
        position = new Vector3Int(x, y, 0);
        _gridHelper = gridHelper;
        _grid = grid;
        this.X = x;
        this.Y = y;
    }

    public int CompareTo(GridTileNode nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
            compare = hCost.CompareTo(nodeToCompare.hCost);
        return -compare;
    }

    public void SetDisableNode()
    {
        StateNode = StateNode.Disable;
    }

    public void AddStateNode(StateNode state)
    {
        StateNode |= state;
    }

    public void RemoveStateNode(StateNode state)
    {
        StateNode ^= state;
    }

    /// <summary>
    ///  Mark node as protected.
    /// </summary>
    /// <param name="entity">Entity or null</param>
    public void SetProtectedUnit(BaseEntity entity)
    {
        _protectedUnit = entity;
        if (entity == null)
        {
            StateNode &= ~(StateNode.Protected);
        }
        else
        {
            StateNode |= StateNode.Protected;
        }
#if UNITY_EDITOR
        GameManager.Instance.MapManager.SetColorForTile(
            position,
            _protectedUnit == null ? Color.yellow : Color.red
        );
#endif
    }

    /// <summary>
    /// Set node as input point.
    /// </summary>
    /// <param name="entity">Entity or null</param>
    public void SetAsInputPoint(GridTileNode node)
    {
        StateNode |= StateNode.Input;
        _inputNode = node;
    }

    /// <summary>
    /// Set occupied entity for node.
    /// </summary>
    /// <param name="entity">Entity or null</param>
    public void SetOcuppiedUnit(BaseEntity entity)
    {
        _ocuppiedUnit = entity;
        if (entity == null)
        {
            StateNode &= ~(StateNode.Occupied);
            StateNode |= StateNode.Empty;
        }
        else
        {
            StateNode &= ~(StateNode.Empty);
            StateNode |= StateNode.Occupied;
        }
    }

    public void DisableProtectedNeigbours(BaseEntity warriorUnit, GridTileNode protectedNode = null)
    {
        List<GridTileNode> nodes = _gridHelper.GetNeighbourList(this, true);

        if (nodes != null || nodes.Count > 0)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].ProtectedUnit == warriorUnit)
                {
                    nodes[i].SetProtectedUnit(null);
                }
            }
        }

        SetProtectedUnit(null);
    }

    public void SetProtectedNeigbours(BaseEntity warriorUnit, GridTileNode protectedNode = null)
    {
        List<GridTileNode> nodes = _gridHelper.GetNeighbourList(this, true);

        if (nodes != null || nodes.Count > 0)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (
                    !nodes[i].StateNode.HasFlag(StateNode.Disable)
                    && !nodes[i].StateNode.HasFlag(StateNode.Protected)
                    )
                {
                    nodes[i].SetProtectedUnit(warriorUnit);
                }
            }
        }

        SetProtectedUnit(warriorUnit);

        if (warriorUnit != null)
        {
            warriorUnit.ProtectedNode = protectedNode;
            var warrior = (EntityCreature)warriorUnit;
            warrior.OnChangeQuantityWarrior();
        }
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;

    }

    public void SetAsRoad()
    {
        koofPath = 9;
        StateNode |= StateNode.Road;
    }

#if UNITY_EDITOR
    public override string ToString()
    {
        return "GridTileNode:::" +
            "keyArea=" + KeyArea + ",\n" +
            "[x" + position.x + ",y" + position.y + "] \n" +
            "typeGround=" + TypeGround + ",\n" +
            "OccupiedUnit=" + OccupiedUnit?.ToString() + ",\n" +
            "StateNode=" + Convert.ToString((int)StateNode, 2) + ",\n" +
            "ProtectedUnit=" + ProtectedUnit?.ToString() + ",\n" +
            "countNeighbours=" + countRelatedNeighbors + ",\n" +
            "(gCost=" + gCost + ") (hCost=" + hCost + ") (fCost=" + fCost + ")";
    }
#endif

}