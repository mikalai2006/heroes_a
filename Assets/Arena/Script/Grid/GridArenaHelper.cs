using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

[Serializable]
public class GridArenaHelper
{
    [SerializeField] private GridArena<GridArenaNode> _gridArena;
    private ArenaManager _arenaManager;
    public GridArena<GridArenaNode> GridTile
    {
        get { return _gridArena; }
        private set { }
    }

    public GridArenaHelper(int width, int height, ArenaManager arenaManager, float cellSize = 1)
    {
        _arenaManager = arenaManager;
        _gridArena = new GridArena<GridArenaNode>(width, height, cellSize, this, (GridArena<GridArenaNode> grid, GridArenaHelper gridArenaHelper, int x, int y) => new GridArenaNode(grid, gridArenaHelper, x, y));
    }


    public List<GridArenaNode> GetAllGridNodes()
    {
        List<GridArenaNode> list = new List<GridArenaNode>(_gridArena.GetWidth() * _gridArena.GetHeight());
        if (_gridArena != null)
        {
            for (int x = 0; x < _gridArena.GetWidth(); x++)
            {
                for (int y = 0; y < _gridArena.GetHeight(); y++)
                {
                    list.Add(_gridArena.GetGridObject(new Vector3Int(x, y)));
                }
            }
        }
        return list;
    }

    public List<GridArenaNode> FindPath(Vector3Int start, Vector3Int end, List<GridArenaNode> allowNodes = null)
    {
        GridArenaNode startNode = _gridArena.GetGridObject(start);
        GridArenaNode endNode = _gridArena.GetGridObject(end);

        var entityData = ((EntityCreature)startNode.OccupiedUnit.Entity).ConfigAttribute;
        allowNodes = allowNodes == null ? GetNeighboursAtDistance(
            startNode,
            entityData.CreatureParams.Speed
            ) : allowNodes;
        // _arenaManager.ResetPathColor();

        BinaryHeap<GridArenaNode> openSet = new BinaryHeap<GridArenaNode>(_gridArena.GetWidth() * _gridArena.GetHeight());
        HashSet<GridArenaNode> closedSet = new HashSet<GridArenaNode>();
        openSet.Add(startNode);

        for (int x = 0; x < _gridArena.GetWidth(); x++)
        {
            for (int z = 0; z < _gridArena.GetHeight(); z++)
            {
                GridArenaNode GridArenaNode = _gridArena.GetGridObject(new Vector3Int(x, z));
                GridArenaNode.gCost = int.MaxValue;
                GridArenaNode.CalculateFCost();
                GridArenaNode.cameFromNode = null;
                //GridArenaNode.typeTile = TypeTile.Zero;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while (openSet.Count > 0)
        {
            GridArenaNode currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            if (currentNode == endNode)
            {
                // final
                List<GridArenaNode> pathList = CalculatePath(endNode);
                return pathList;
            }

            foreach (GridArenaNode neighbourNode in GetNeighbourList(currentNode))
            {
                if (
                    closedSet.Contains(neighbourNode)
                    || (
                        !allowNodes.Contains(neighbourNode)
                        && entityData.CreatureParams.Movement != MovementType.Flying
                    )
                ) continue;

                bool walk = (
                    (
                        neighbourNode.StateArenaNode.HasFlag(StateArenaNode.Empty)
                        &&
                        !neighbourNode.StateArenaNode.HasFlag(StateArenaNode.Disable)
                        &&
                        !neighbourNode.StateArenaNode.HasFlag(StateArenaNode.Occupied)
                    )
                    || (neighbourNode.OccupiedUnit != null && neighbourNode.OccupiedUnit == startNode.OccupiedUnit)
                    || (
                        neighbourNode.OccupiedUnit != endNode.OccupiedUnit
                        &&
                        entityData.CreatureParams.Movement == MovementType.Flying
                    )
                );
                if (!walk)
                {
                    closedSet.Add(neighbourNode);
                    continue;
                }

                var diagonalIntersect = GetIntersectHexagons(neighbourNode, currentNode, startNode.OccupiedUnit).Count;
                if (((
                        entityData.CreatureParams.Size > 1
                        && currentNode.position.y != neighbourNode.position.y
                        && diagonalIntersect <= entityData.CreatureParams.Size - 1
                    )
                    ||
                    (
                        entityData.CreatureParams.Size > 1
                        && currentNode.position.y == neighbourNode.position.y
                        && GetEmptyCellByX(neighbourNode, allowNodes).Count < entityData.CreatureParams.Size - 1
                        && neighbourNode != startNode.LeftNode
                        && neighbourNode != startNode.RightNode
                    // && endNode != startNode.LeftNode
                    // && endNode != startNode.RightNode
                    )
                        && entityData.CreatureParams.Movement != MovementType.Flying)
                    )
                {
                    continue;
                }

                float tentativeGCost = currentNode.gCost + 10; //CalculateDistanceCost(currentNode, neighbourNode);

                if (tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    if (!openSet.Contains(neighbourNode))
                    {
                        if (entityData.CreatureParams.Size == 2)
                        {
                            var direction = GetDirection(currentNode, neighbourNode);
                            if (
                                ((
                                    // direction == -1
                                    // &&
                                    neighbourNode.RightNode != null
                                    && (
                                        neighbourNode.RightNode.OccupiedUnit == null
                                        || neighbourNode.RightNode.OccupiedUnit == startNode.OccupiedUnit
                                        )
                                )
                                ||
                                (
                                    // direction == 1
                                    // &&
                                    neighbourNode.LeftNode != null
                                    && (
                                        neighbourNode.LeftNode.OccupiedUnit == null
                                        || neighbourNode.LeftNode.OccupiedUnit == startNode.OccupiedUnit
                                        )
                                ))
                            // &&
                            // (GetIntersectHexagons(neighbourNode, currentNode).Count < entityData.CreatureParams.Size)
                            // ||
                            // (neighbourNode.LeftNode == null || neighbourNode.RightNode == null)
                            )
                            {
                                openSet.Add(neighbourNode);
                                // _arenaManager.SetColorPathNode(neighbourNode);
                            }
                            // else if (neighbourNode == endNode)
                            // {
                            //     Debug.Log($"CheckEndNode before::: neig {neighbourNode}, end {endNode}");
                            //     endNode = CheckEndNode(neighbourNode.cameFromNode, endNode);
                            //     Debug.Log($"CheckEndNode after::: from {neighbourNode.cameFromNode} to {endNode}");
                            // }
                        }
                        else
                        {
                            openSet.Add(neighbourNode);
                            // _arenaManager.SetColorPathNode(neighbourNode);
                        }
                    }
                }
            }
        }
        Debug.Log($"Not found path");

        return null; // CalculatePath(endNode);
    }

    private GridArenaNode CheckEndNode(GridArenaNode startNode, GridArenaNode endNode)
    {
        // var direction = GetDirection(startNode, endNode);

        // if (direction > 0 && endNode.RightNode != null)
        // {
        //     return endNode.RightNode;
        // }
        // else if (direction < 0 && endNode.LeftNode != null)
        // {
        //     return endNode.LeftNode;
        // }
        // else
        // {
        //     return endNode;
        // }
        if (
            endNode.RightNode != null
            && endNode.RightNode.StateArenaNode.HasFlag(StateArenaNode.Empty)
            )
        {
            Debug.Log("Check RightNode");
            return endNode.RightNode;
        }
        else if (endNode.LeftNode != null && endNode.LeftNode.StateArenaNode.HasFlag(StateArenaNode.Empty))
        {
            Debug.Log("Check LeftNode");
            return endNode.LeftNode;
        }
        else
        {
            Debug.Log("Check Some Node");
            return endNode;
        }
        // if (endNode.RightNode != null && endNode.RightNode.StateArenaNode.HasFlag(StateArenaNode.Empty))
        // {
        //     return endNode.RightNode;
        // }
        // else if (endNode.LeftNode != null && endNode.LeftNode.StateArenaNode.HasFlag(StateArenaNode.Empty))
        // {
        //     return endNode.LeftNode;
        // }
        // else
        // {
        //     return endNode;
        // }
    }

    public int GetDirection(GridArenaNode startNode, GridArenaNode endNode)
    {
        var result = 0;
        var difference = startNode.positionPrefab.x - endNode.positionPrefab.x;
        if (difference > 0)
        {
            result = -1;
        }
        else if (difference < 0)
        {
            result = 1;
        };

        // Debug.Log($"Direction:::{startNode.positionPrefab}:::{endNode.positionPrefab}::: {result}");
        return result;
    }

    public List<GridArenaNode> GetNeighbourList(GridArenaNode currentNode)
    {
        Vector3Int position = currentNode.position;
        List<GridArenaNode> neighbourList = new List<GridArenaNode>();
        int xOffset = 0;
        if (position.y % 2 != 0)
            xOffset = 1;

        var one = GetNode(position.x - 1, position.y);
        if (one != null) neighbourList.Add(one);

        var two = GetNode(position.x + 1, position.y);
        if (two != null) neighbourList.Add(two);

        var three = GetNode(position.x + xOffset - 1, position.y + 1);
        if (three != null) neighbourList.Add(three);

        var four = GetNode(position.x + xOffset, position.y + 1);
        if (four != null) neighbourList.Add(four);

        var five = GetNode(position.x + xOffset - 1, position.y - 1);
        if (five != null) neighbourList.Add(five);

        var six = GetNode(position.x + xOffset, position.y - 1);
        if (six != null) neighbourList.Add(six);

        return neighbourList;
    }
    // public List<GridArenaNode> GetEmptyNeighbours(GridArenaNode currentNode, List<GridArenaNode> listNodes)
    // {
    //     Vector3Int position = currentNode.position;
    //     List<GridArenaNode> neighbourList = new List<GridArenaNode>();
    //     int xOffset = 0;
    //     if (position.y % 2 != 0)
    //         xOffset = 1;

    //     var one = GetNode(position.x - 1, position.y);
    //     if (one != null && one.OccupiedUnit == null && listNodes.Contains(one)) neighbourList.Add(one);

    //     var two = GetNode(position.x + 1, position.y);
    //     if (two != null && two.OccupiedUnit == null && listNodes.Contains(two)) neighbourList.Add(two);

    //     var three = GetNode(position.x + xOffset - 1, position.y + 1);
    //     if (three != null && three.OccupiedUnit == null && listNodes.Contains(three)) neighbourList.Add(three);

    //     var four = GetNode(position.x + xOffset, position.y + 1);
    //     if (four != null && four.OccupiedUnit == null && listNodes.Contains(four)) neighbourList.Add(four);

    //     var five = GetNode(position.x + xOffset - 1, position.y - 1);
    //     if (five != null && five.OccupiedUnit == null && listNodes.Contains(five)) neighbourList.Add(five);

    //     var six = GetNode(position.x + xOffset, position.y - 1);
    //     if (six != null && six.OccupiedUnit == null && listNodes.Contains(six)) neighbourList.Add(six);

    //     return neighbourList;
    // }

    public List<GridArenaNode> GetEmptyCellByX(GridArenaNode currentNode, List<GridArenaNode> listNodes)
    {
        Vector3Int position = currentNode.position;
        List<GridArenaNode> neighbourList = new List<GridArenaNode>();

        var one = GetNode(position.x - 1, position.y);
        if (one != null && one.OccupiedUnit == null && listNodes.Contains(one)) neighbourList.Add(one);

        var two = GetNode(position.x + 1, position.y);
        if (two != null && two.OccupiedUnit == null && listNodes.Contains(two)) neighbourList.Add(two);

        return neighbourList;
    }
    public void CreateWeightCellByX()
    {
        List<GridArenaNode> allNodes = GetAllGridNodes();

        for (int i = 0; i < allNodes.Count; i++)
        {
            var node = allNodes[i];
            node.weight = 1;
            if (node.LeftNode != null && node.LeftNode.StateArenaNode.HasFlag(StateArenaNode.Empty))
            {
                node.weight++;
            }
            if (node.RightNode != null && node.RightNode.StateArenaNode.HasFlag(StateArenaNode.Empty))
            {
                node.weight++;
            }
        }

    }
    public List<GridArenaNode> GetIntersectHexagons(GridArenaNode nodeStart, GridArenaNode nodeEnd, ArenaEntity entity)
    {
        var neighboursNodeStart = GetNeighbourList(nodeStart);
        var neighboursNodeEnd = GetNeighbourList(nodeEnd);

        List<GridArenaNode> intersectNodes = new();

        // var moreNodes = neighboursNodeEnd.Count > neighboursNodeStart.Count
        //     ? neighboursNodeEnd
        //     : neighboursNodeStart;

        for (int i = 0; i < neighboursNodeStart.Count; i++)
        {
            var node = neighboursNodeStart[i];
            if (neighboursNodeEnd.Contains(node) && node != nodeStart && node != nodeEnd)
            {
                intersectNodes.Add(node);
            }
        }

        return intersectNodes
            .Where(t => t.OccupiedUnit == null || t.OccupiedUnit == entity)
            .ToList();
    }


    public List<GridArenaNode> GetNeighboursAtDistance(GridArenaNode startNode, int distance)
    {
        List<GridArenaNode> openListNodes = new List<GridArenaNode>() { startNode };
        List<GridArenaNode> closedListNodes = new List<GridArenaNode>();
        var entity = ((EntityCreature)startNode.OccupiedUnit.Entity).ConfigAttribute;

        while (openListNodes.Count > 0)
        {
            GridArenaNode currentNode = openListNodes[0];

            openListNodes.Remove(currentNode);
            closedListNodes.Add(currentNode);

            foreach (GridArenaNode neighbourNode in GetNeighbourList(currentNode))
            {
                if (closedListNodes.Contains(neighbourNode)) continue;

                var dist = startNode.DistanceTo(neighbourNode);
                if (
                    !openListNodes.Contains(neighbourNode)
                    && dist <= distance
                    // && (
                    //     neighbourNode.StateArenaNode.HasFlag(StateArenaNode.Empty)
                    //     || (
                    //         neighbourNode.OccupiedUnit != null
                    //         && neighbourNode.OccupiedUnit == startNode.OccupiedUnit
                    //     )
                    //     )
                    )
                {
                    // Debug.Log($"Neighbour ({startNode.position})|end({neighbourNode.position})::: dist={dist}");
                    openListNodes.Add(neighbourNode);
                }
            }
        }

        var result = new List<GridArenaNode>();
        foreach (var node in closedListNodes)
        {
            if (node.position != startNode.position
                // && ((GetEmptyCellByX(node, closedListNodes).Count + 1) >= entity.CreatureParams.Size)
                // // ||
                // // GetNeighbourList(node).Select(t => t.OccupiedUnit).Contains(startNode.OccupiedUnit)
                )
            {
                result.Add(node);
            }
        }

        return result;
    }
    public List<GridArenaNode> GetNeighboursAtDistanceAndFindPath(List<GridArenaNode> allowNodes, GridArenaNode startNode)
    {
        var result = new List<GridArenaNode>();
        var entityData = ((ScriptableAttributeCreature)startNode.OccupiedUnit.Entity.ScriptableDataAttribute);
        foreach (var node in allowNodes)
        {
            var path = FindPath(startNode.position, node.position);
            if (
                path != null
                && path.Count - 1 <= entityData.CreatureParams.Speed
                // &&
                //     (
                //         entityData.CreatureParams.Size == 1
                //         ||
                //         (
                //             entityData.CreatureParams.Size > 1
                //             &&
                //         )
                //     )
                )
            {
                result.Add(node);
                node.level = path.Count - 1;
            }
        }

        return result;
    }

    private List<GridArenaNode> CalculatePath(GridArenaNode endNode)
    {
        List<GridArenaNode> path = new List<GridArenaNode>();
        path.Add(endNode);

        GridArenaNode currentNode = endNode;
        while (currentNode.cameFromNode != null)
        {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();
        return path;
    }

    // private Vector3Int[] GetAxialDirectionVectors()
    // {
    //     return new[]{
    //         new Vector3Int(+1, 0),
    //         new Vector3Int(+1, -1),
    //         new Vector3Int(0, -1),
    //         new Vector3Int(-1, 0),
    //         new Vector3Int(-1, +1),
    //         new Vector3Int(0, +1),
    //     };
    // }

    public GridArenaNode GetNode(int x, int y)
    {
        return _gridArena.GetGridObject(new Vector3Int(x, y));
    }
    public GridArenaNode GetNode(Vector3Int pos)
    {
        return _gridArena.GetGridObject(pos);
    }

    private float CalculateDistanceCost(GridArenaNode a, GridArenaNode b)
    {
        // float xDistance = Mathf.Abs(a.position.x - b.position.x);
        // float yDistance = Mathf.Abs(a.position.y - b.position.y);
        // float remaining = Mathf.Abs(xDistance - yDistance);
        // return Mathf.Min(xDistance, yDistance) + remaining;
        return Mathf.RoundToInt(10 * Vector3.Distance(_gridArena.GetWorldPosition(a.position.x, a.position.y), _gridArena.GetWorldPosition(b.position.x, b.position.y)));
    }
}
