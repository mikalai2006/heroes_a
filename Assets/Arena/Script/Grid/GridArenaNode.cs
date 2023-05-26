
using System;
using System.Collections.Generic;

using UnityEngine;

[Flags]
public enum StateArenaNode
{
    Disable = 1 << 0,
    Empty = 1 << 1,
    Occupied = 1 << 2,
    Related = 1 << 3,
    Moved = 1 << 4,
    Deathed = 1 << 5,
    Spellsed = 1 << 6,
    Obstacles = 1 << 7
}

[Serializable]
public class GridArenaNode : IHeapItem<GridArenaNode>
{
    [NonSerialized] private readonly GridArena<GridArenaNode> _grid;
    [NonSerialized] private readonly GridArenaHelper _gridArenaHelper;
    [SerializeField] public StateArenaNode StateArenaNode = StateArenaNode.Empty;
    private int X;
    private int Y;
    [NonSerialized] public Vector3Int position;
    [NonSerialized] public Vector3 center;
    [NonSerialized] private ArenaEntity _ocuppiedUnit = null;
    public ArenaEntity OccupiedUnit => _ocuppiedUnit;
    [NonSerialized] private List<ArenaEntity> _deathedUnits = null;
    public List<ArenaEntity> DeathedUnits => _deathedUnits;

    private int xOffset => position.y % 2 != 0 ? 1 : 0;
    public GridArenaNode LeftNode => _grid.GetGridObject(new Vector3Int(X - 1, Y));
    public GridArenaNode RightNode => _grid.GetGridObject(new Vector3Int(X + 1, Y));
    public GridArenaNode LeftTopNode => _grid.GetGridObject(new Vector3Int(X + xOffset - 1, Y + 1));
    public GridArenaNode LeftBottomNode => _grid.GetGridObject(new Vector3Int(X + xOffset - 1, Y - 1));
    public int level = 0;
    public int weight = 0;
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
    [NonSerialized] public GridArenaNode cameFromNode;
    [NonSerialized] public float gCost;
    [NonSerialized] public float hCost;
    [NonSerialized] public float fCost;
    [NonSerialized] private ArenaEntitySpell _spellsUnit = null;
    public ArenaEntitySpell SpellUnit => _spellsUnit;
    // int - quantity round
    public Dictionary<ScriptableAttributeSpell, int> SpellsState = new();


    public GridArenaNode(GridArena<GridArenaNode> grid, GridArenaHelper gridArenaHelper, int x, int y)
    {
        _grid = grid;
        _gridArenaHelper = gridArenaHelper;

        position = new Vector3Int(x, y, 0);
        X = x;
        Y = y;
    }

    public int CompareTo(GridArenaNode nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
            compare = hCost.CompareTo(nodeToCompare.hCost);
        return -compare;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public void SetDeathedNode(ArenaEntity entity)
    {
        if (_deathedUnits == null)
        {
            _deathedUnits = new();
        }

        if (entity != null)
        {
            StateArenaNode |= StateArenaNode.Deathed;
            _deathedUnits.Add(entity);
        }
        else
        {
            StateArenaNode ^= StateArenaNode.Deathed;
            _deathedUnits.Remove(entity);
        }

    }
    public void SetRelatedStatus(bool status)
    {
        if (status)
        {
            StateArenaNode |= StateArenaNode.Related;
        }
        else
        {
            StateArenaNode ^= StateArenaNode.Related;
        }

    }
    public void SetOcuppiedUnit(ArenaEntity entity)
    {
        _ocuppiedUnit = entity;
        if (entity == null)
        {
            StateArenaNode ^= StateArenaNode.Occupied;
            StateArenaNode |= StateArenaNode.Empty;
        }
        else
        {
            StateArenaNode |= StateArenaNode.Occupied;
            StateArenaNode ^= StateArenaNode.Empty;
        }

    }
    public void SetSpellsUnit(ArenaEntitySpell entity)
    {
        _spellsUnit = entity;
    }
    public void SetSpellsStatus(bool status)
    {
        if (!status)
        {
            StateArenaNode ^= StateArenaNode.Spellsed;
            // StateArenaNode |= StateArenaNode.Empty;
        }
        else
        {
            StateArenaNode |= StateArenaNode.Spellsed;
            // StateArenaNode ^= StateArenaNode.Empty;
        }

    }
    public float DistanceTo(GridArenaNode other)
    {
        int dx = X - other.X;     // signed deltas
        int dy = Y - other.Y;
        int x = Mathf.Abs(dx);  // absolute deltas
        int y = Mathf.Abs(dy);
        // special case if we start on an odd row or if we move into negative x direction
        if ((dx < 0) ^ ((other.Y & 1) == 1))
            x = Mathf.Max(0, x - (y + 1) / 2);
        else
            x = Mathf.Max(0, x - (y) / 2);
        return x + y;
    }

    public void SetCenter(Vector3 center)
    {
        this.center = center;
    }

    /// <summary>
    /// Get neighbours for node from grid.
    /// </summary>
    /// <param name="currentNode"></param>
    /// <returns></returns>
    public List<GridArenaNode> Neighbours()
    {
        Vector3Int position = this.position;
        List<GridArenaNode> neighbourList = new List<GridArenaNode>();

        var one = _grid.GetGridObject(position.x - 1, position.y);
        if (one != null) neighbourList.Add(one);

        var three = _grid.GetGridObject(position.x + xOffset - 1, position.y + 1);
        if (three != null) neighbourList.Add(three);

        var four = _grid.GetGridObject(position.x + xOffset, position.y + 1);
        if (four != null) neighbourList.Add(four);

        var two = _grid.GetGridObject(position.x + 1, position.y);
        if (two != null) neighbourList.Add(two);

        var six = _grid.GetGridObject(position.x + xOffset, position.y - 1);
        if (six != null) neighbourList.Add(six);

        var five = _grid.GetGridObject(position.x + xOffset - 1, position.y - 1);
        if (five != null) neighbourList.Add(five);

        return neighbourList;
    }

#if UNITY_EDITOR
    public override string ToString()
    {
        // string listN = "";
        // foreach (var neigh in NeighbourPositions)
        // {
        //     listN += "/" + neigh.ToString();
        // }

        return "GridArenaNode:::" +
            "[_x=" + X + "),y=" + Y + "] \n" +
            "center [x" + center.x + ",y" + center.y + "] \n" +
            "weight [" + weight + "] \n" +
            "WorldPos [" + _grid.GetWorldPosition(X, Y) + "] \n" +
            "position [x" + position.x + ",y" + position.y + "] \n" +
            (OccupiedUnit != null ? "OccupiedUnit=" + OccupiedUnit.Entity.ScriptableDataAttribute?.ToString() + ",\n" : "") +
            "StateNode=" + Convert.ToString((int)StateArenaNode, 2) + ",\n" +
            "(gCost=" + gCost + ") (hCost=" + hCost + ") (fCost=" + fCost + ")";
    }

#endif

}