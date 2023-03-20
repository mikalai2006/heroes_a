using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class GridTileHelper
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    [SerializeField] public GridTile<GridTileNode> gridTile;
    //private List<GridTileNode> _transferOpenList;
    //private List<GridTileNode> _transferClosedList;
    //private float _maxSizeOneWorld;
    private List<GridTileNode> openList = new List<GridTileNode>();
    private List<GridTileNode> closedList = new List<GridTileNode>();
    public GridTile<GridTileNode> GridTile
    {
        get { return gridTile; }
        private set { }
    }

    public GridTileHelper(int width, int height, float cellSize = 1)
    {
        gridTile = new GridTile<GridTileNode>(width, height, cellSize, this, (GridTile<GridTileNode> grid, GridTileHelper gridHelper, int x, int y) => new GridTileNode(grid, gridHelper, x, y));
    }


    public List<GridTileNode> GetAllGridNodes()
    {
        List<GridTileNode> list = new List<GridTileNode>(gridTile.GetWidth() * gridTile.GetHeight());
        if (gridTile != null)
        {
            for (int x = 0; x < gridTile.GetWidth(); x++)
            {
                for (int y = 0; y < gridTile.GetHeight(); y++)
                {
                    list.Add(gridTile.getGridObject(new Vector3Int(x, y)));
                }
            }
        }
        return list;
    }

    /// <summary>
    /// Get random node.
    /// </summary>
    /// <returns></returns>
    public GridTileNode GetRandomTileNode(List<GridTileNode> listNodes)
    {
        int maxIndex = UnityEngine.Random.Range(0, listNodes.Count);
        int indexRandomNode = UnityEngine.Random.Range(0, maxIndex);
        //Debug.Log($"AllCount {openList.Count}[close={closedList.Count}] | Count {listNodes.Count} rand index {indexRandomNode} ::: {listNodes[indexRandomNode].ToString()}");
        return listNodes[indexRandomNode];
    }
    public GridTileNode GetRandomTileNode(Dictionary<Vector3Int, GridTileNode> listNodes)
    {
        //List<GridTileNode> listNodesx = listNodes.Values.Where(x => x.countRelatedNeighbors > 2).ToList();
        //Debug.Log($"count list oc= {listNodesx.Count}");
        int maxIndex = UnityEngine.Random.Range(0, listNodes.Count);
        int indexRandomNode = UnityEngine.Random.Range(0, maxIndex);
        //Debug.Log($"AllCount {openList.Count}[close={closedList.Count}] | Count {listNodes.Count} rand index {indexRandomNode} ::: {listNodes[indexRandomNode].ToString()}");
        return listNodes.ElementAt(indexRandomNode).Value;
    }
    //public void CreateAreas(int countArea, float maxSizeOneWorld, List<TileLandscape> listTileData, TileLandscape emptyTile)
    //{
    //    _maxSizeOneWorld = maxSizeOneWorld;

    //    openList = GetAllGridNodes();

    //    for (int i = 0; i < countArea; i++)
    //    {

    //        TileLandscape randomTileData = listTileData[UnityEngine.Random.Range(0, listTileData.Count)];
    //        //int randomCoordX = Random.Range(2, gridTile.GetWidth() - 2);
    //        //int randomCoordY = Random.Range(2, gridTile.GetHeight() - 2);
    //        //int indexRandomNode = Random.Range(5, openList.Count);

    //        GridTileNode randomNode = openList[UnityEngine.Random.Range(0, openList.Count)];

    //        List<GridTileNode> listNeighbors = GetNeighbourListWithTypeGround(randomNode, randomNode.typeGround);
    //        while(
    //            listNeighbors.Count != 4
    //            )
    //        {
    //            randomNode = openList[UnityEngine.Random.Range(0, openList.Count)];

    //            listNeighbors = GetNeighbourListWithTypeGround(randomNode, randomNode.typeGround);
    //        }

    //        //Debug.Log($"Area {i} start position = {randomNode._position}");

    //        LevelManager.Instance.AddArea(i);
    //        CreateArea(i, randomNode, randomTileData);

    //        randomNode.isCreated = true;
    //    }

    //    if (openList.Count > 0)
    //    {
    //        int keyEmprtyArea = countArea;
    //        while (openList.Count > 0)
    //        {
    //            // CreateEmptyArea(emptyTile);
    //            LevelManager.Instance.AddArea(keyEmprtyArea);
    //            CreateArea(keyEmprtyArea, openList[0], emptyTile);
    //            keyEmprtyArea++;
    //        }
    //    }
    //    Debug.Log($"AllTileNodes::: noCreated-{openList.Count}[Created={closedList.Count}]");
    //}

    ////private void CreateEmptyArea(TileData tileData)
    ////{
    ////    for (int i = 0; i < openList.Count; i++)
    ////    {
    ////        openList[i].SetWalkable(false);
    ////        openList[i].typeGround = tileData.typeGround;
    ////        //openList[i].isCreated = true;
    ////        openList[i].keyArea = 1000;
    ////        openList[i].level = 1000;
    ////        openList[i].countRelatedNeighbors = 0;
    ////    }
    ////}

    //public void CreateArea(int keyArea, GridTileNode startNode, TileLandscape tileData)
    //{
    //    LevelManager.Instance.GetArea(keyArea).startPosition = startNode._position;
    //    _transferOpenList = new List<GridTileNode>() { startNode };
    //    _transferClosedList = new List<GridTileNode>();

    //    startNode.level = 0;
    //    startNode.keyArea = keyArea;

    //    while (_transferOpenList.Count > 0 && openList.Count > 0)
    //    {

    //        LevelManager.Instance.GetArea(keyArea).countNode ++;
    //        //Debug.Log($"_maxSizeOneWorld={_maxSizeOneWorld}, created = { ((float)_transferClosedList.Count / ((float)gridTile.GetHeight() * (float)gridTile.GetWidth()))}");
    //        if (_maxSizeOneWorld < ((float)_transferClosedList.Count / ((float)gridTile.GetHeight() * (float)gridTile.GetWidth())))
    //        {
    //            break;
    //        }

    //        GridTileNode currentNode = GetRandomTileNode(_transferOpenList);

    //        _transferOpenList.Remove(currentNode);
    //        openList.Remove(currentNode);
    //        _transferClosedList.Add(currentNode);
    //        closedList.Add(currentNode);

    //        currentNode.isCreated = true;
    //        currentNode.typeGround = tileData.typeGround;

    //        List<GridTileNode> listNeighbors = GetNeighbourList(currentNode);

    //        foreach (GridTileNode neighbourNode in listNeighbors)
    //        {
    //            if (closedList.Contains(neighbourNode) && _transferClosedList.Contains(neighbourNode))
    //            {
    //                if (neighbourNode.typeGround == currentNode.typeGround && neighbourNode.keyArea == currentNode.keyArea)
    //                {
    //                    neighbourNode.countRelatedNeighbors += 1;
    //                    currentNode.countRelatedNeighbors += 1;
    //                }
    //                continue;
    //            }


    //            if (neighbourNode.isCreated)
    //            {
    //                closedList.Add(neighbourNode);
    //                _transferClosedList.Add(neighbourNode);
    //                continue;
    //            }
    //            if (!_transferOpenList.Contains(neighbourNode) && listNeighbors.Count == 4)
    //            {
    //                neighbourNode.level = currentNode.level + 1;
    //                neighbourNode.keyArea = currentNode.keyArea;
    //                _transferOpenList.Add(neighbourNode);
    //            }

    //        }

    //    }
    //}

    public List<GridTileNode> FindPath(Vector3Int start, Vector3Int end, bool isTrigger = false, bool force = false, bool ignoreNotWalkable = false)
    {
        //GameManager.Instance.mapManager.ClearTestColor();

        GridTileNode startNode = gridTile.getGridObject(start);
        GridTileNode startNodeTrigger = startNode;
        GridTileNode endNode;
        if (isTrigger)
        {
            endNode = gridTile.getGridObject(end - new Vector3Int(0, 1, 0));
        }
        else
        {
            endNode = gridTile.getGridObject(end);
        }

        if (!startNodeTrigger.Empty)
        {
            startNode = gridTile.getGridObject(start - new Vector3Int(0, 1, 0));
        }

        BinaryHeap<GridTileNode> openSet = new BinaryHeap<GridTileNode>(gridTile.GetWidth() * gridTile.GetHeight());
        HashSet<GridTileNode> closedSet = new HashSet<GridTileNode>();
        openSet.Add(startNode);

        //openList = new List<GridTileNode> { startNode };
        //closedList = new List<GridTileNode>();

        for (int x = 0; x < gridTile.GetWidth(); x++)
        {
            for (int z = 0; z < gridTile.GetHeight(); z++)
            {
                GridTileNode GridTileNode = gridTile.getGridObject(new Vector3Int(x, z));
                GridTileNode.gCost = int.MaxValue;
                GridTileNode.CalculateFCost();
                GridTileNode.cameFromNode = null;
                //GridTileNode.typeTile = TypeTile.Zero;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while (openSet.Count > 0)
        {
            GridTileNode currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            //GridTileNode currentNode = GetLowestCostNode(openList);
            //GameManager.Instance.mapManager.SetTestColor(currentNode._position, Color.blue);

            if (currentNode == endNode)
            {
                // final
                List<GridTileNode> pathList = CalculatePath(endNode);
                if (isTrigger)
                {
                    pathList.Add(gridTile.getGridObject(end));
                }
                if (!startNodeTrigger.Empty)
                {
                    pathList.Reverse();
                    pathList.Add(startNodeTrigger);
                    pathList.Reverse();
                }

                return pathList;
            }

            //openList.Remove(currentNode);
            //closedList.Add(currentNode);

            foreach (GridTileNode neighbourNode in GetNeighbourList(currentNode, force))
            {
                if (closedSet.Contains(neighbourNode)) continue;

                //GridTileNode nodeTile = gridTile.getGridObject(neighbourNode._position);
                //bool mayBeWalked = neighbourNode.Walkable || (!neighbourNode.Walkable
                //        && neighbourNode.OccupiedUnit == endNode.ProtectedUnit
                //        && neighbourNode.OccupiedUnit != null
                //        );
                bool walk = (
                    neighbourNode.Empty
                    && neighbourNode.Enable
                    && (
                        !neighbourNode.Protected
                        || (
                            neighbourNode.Protected
                            && neighbourNode.ProtectedUnit == endNode.ProtectedUnit
                            )
                        )
                    )
                    || (
                    !endNode.Empty &&
                    // endNode.OccupiedUnit.typeInput == TypeInput.Down &&
                    endNode.ProtectedUnit == neighbourNode.ProtectedUnit
                    && endNode.OccupiedUnit == neighbourNode.OccupiedUnit // TODO
                    )
                    //|| (
                    //endNode.Protected &&
                    //endNode.ProtectedUnit == neighbourNode.ProtectedUnit
                    //)
                    || ignoreNotWalkable;
                if ((!walk || neighbourNode.Disable ) && !ignoreNotWalkable)
                {
                    closedSet.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);

                if (tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    if (!openSet.Contains(neighbourNode))
                    {
                        neighbourNode.levelPath += currentNode.levelPath;
                        openSet.Add(neighbourNode);
                        //Grid2DManager.Instance.SetTextMeshNode(neighbourNode, string.Format(
                        //    "gCost {0} hCost {1} fCost {2} koof {3}",
                        //    neighbourNode.gCost,
                        //    neighbourNode.hCost,
                        //    neighbourNode.fCost,
                        //    neighbourNode.koofPath
                        //    ));
                    }
                }
            }

        }

        return CalculatePath(endNode);
    }
    public List<GridTileNode> IsExistExit(GridTileNode node)
    {
        BinaryHeap<GridTileNode> openSet = new BinaryHeap<GridTileNode>(gridTile.GetWidth() * gridTile.GetHeight());
        HashSet<GridTileNode> closedSet = new HashSet<GridTileNode>();
        HashSet<GridTileNode> removedSet = new HashSet<GridTileNode>();
        openSet.Add(node);

        List<TypeUnit> exitTriggers = new List<TypeUnit> { TypeUnit.Monolith, TypeUnit.Town, TypeUnit.Hero };

        while (openSet.Count > 0)
        {
            GridTileNode currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            foreach (GridTileNode neighbourNode in GetNeighbourList(currentNode, true))
            {
                if (neighbourNode.Disable)
                {
                    removedSet.Add(neighbourNode);
                    continue;
                }

                if (removedSet.Contains(neighbourNode)) continue;

                if (neighbourNode.keyArea != node.keyArea)
                {
                    removedSet.Add(neighbourNode);
                    continue;
                }

                if (neighbourNode.OccupiedUnit != null && neighbourNode != node)
                {
                    if (exitTriggers.Contains(neighbourNode.OccupiedUnit.typeUnit))
                    {
                        //Debug.Log($"Exit OccupiedUnit::: {neighbourNode.OccupiedUnit.typeUnit}");
                        return new List<GridTileNode> { neighbourNode };
                    }
                }

                if (closedSet.Contains(neighbourNode)) continue;

                if (!openSet.Contains(neighbourNode))
                {
                    openSet.Add(neighbourNode);
                }
                //else
                //{
                //    openSet.UpdateItem(neighbourNode);
                //}


            }

        }

        return closedSet.ToList();
    }

    public List<GridTileNode> IsExistExit2(GridTileNode node)
    {


        List<TypeUnit> exitTriggers = new List<TypeUnit> { TypeUnit.Monolith, TypeUnit.Town, TypeUnit.Hero };
        List<GridTileNode> openListNodes = new List<GridTileNode>() { node };
        List<GridTileNode> closedListNodes = new List<GridTileNode>();
        List<GridTileNode> removedListNodes = new List<GridTileNode>();
        //Color col = Color.green;
        //col.a = (float)node.keyArea * UnityEngine.Random.value;

        while (openListNodes.Count > 0)
        {
            GridTileNode currentNode = openListNodes[0];
            //Grid2DManager.Instance.SetColorForTile(currentNode._position, col);
            openListNodes.Remove(currentNode);
            closedListNodes.Add(currentNode);
            //if (currentNode.OccupiedUnit != null)
            //{
            //    // exit.
            //    if (exitTriggers.Contains(currentNode.OccupiedUnit.typeUnit))
            //    {
            //        //Debug.Log($"Exit OccupiedUnit::: {currentNode.OccupiedUnit.typeUnit}");
            //        return new List<GridTileNode> { currentNode  };
            //    }

            //}
            foreach (GridTileNode neighbourNode in GetNeighbourList(currentNode, true))
            {
                if (neighbourNode.Disable)
                {
                    //closedListNodes.Add(neighbourNode);
                    removedListNodes.Add(neighbourNode);
                    continue;
                }

                if (removedListNodes.Contains(neighbourNode)) continue;

                if (neighbourNode.keyArea != node.keyArea)
                {
                    removedListNodes.Add(neighbourNode);
                    continue;
                }

                if (neighbourNode.OccupiedUnit != null && neighbourNode != node)
                {
                    if (exitTriggers.Contains(neighbourNode.OccupiedUnit.typeUnit))
                    {
                        //Debug.Log($"Exit OccupiedUnit::: {neighbourNode.OccupiedUnit.typeUnit}");
                        return new List<GridTileNode> { neighbourNode };
                    }
                }

                if (closedListNodes.Contains(neighbourNode)) continue;

                if (!openListNodes.Contains(neighbourNode))
                {
                    openListNodes.Add(neighbourNode);
                }
            }

        }

        return closedListNodes;
    }

    public List<GridTileNode> GetNeighboursAtDistance(GridTileNode startNode, int distance, bool force = true)
    {
        List<GridTileNode> openListNodes = new List<GridTileNode>() { startNode };
        List<GridTileNode> closedListNodes = new List<GridTileNode>();
        //List<GridTileNode> removedListNodes = new List<GridTileNode>();

        while (openListNodes.Count > 0)
        {
            GridTileNode currentNode = openListNodes[0];

            openListNodes.Remove(currentNode);
            closedListNodes.Add(currentNode);

            foreach (GridTileNode neighbourNode in GetNeighbourList(currentNode, true))
            {
                //if (neighbourNode.Disable && !force)
                //{
                //    removedListNodes.Add(neighbourNode);
                //    continue;
                //}

                //if (removedListNodes.Contains(neighbourNode)) continue;
                if (closedListNodes.Contains(neighbourNode)) continue;

                //if (neighbourNode.keyArea != node.keyArea )
                //{
                //    removedListNodes.Add(neighbourNode);
                //    continue;
                //}

                //if (neighbourNode.OccupiedUnit != null && neighbourNode != node)
                //{
                //    if (exitTriggers.Contains(neighbourNode.OccupiedUnit.typeUnit))
                //    {
                //        //Debug.Log($"Exit OccupiedUnit::: {neighbourNode.OccupiedUnit.typeUnit}");
                //        return new List<GridTileNode> { neighbourNode };
                //    }
                //}


                if (
                    !openListNodes.Contains(neighbourNode)
                    && GetDistanceBetweeenPoints(startNode._position, neighbourNode._position) <= distance
                    )
                {
                    openListNodes.Add(neighbourNode);
                }
            }

        }

        return closedListNodes;
    }

    public List<GridTileNode> GetNeighbourList(GridTileNode currentNode, bool isDiagonal = false)
    {
        List<GridTileNode> neighbourList = new List<GridTileNode>();

        if (currentNode.x - 1 >= 0)
        {
            //left
            neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y));
            if (isDiagonal)
            {
                // left down
                if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y - 1));
                // left up
                if (currentNode.y + 1 < gridTile.GetHeight()) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y + 1));
            }
        }

        if (currentNode.x + 1 < gridTile.GetWidth())
        {
            // right
            neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y));
            if (isDiagonal)
            {
                // right down
                if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y - 1));
                //right up
                if (currentNode.y + 1 < gridTile.GetHeight()) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y + 1));
            }
        }

        // down
        if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x, currentNode.y - 1));
        // up
        if (currentNode.y + 1 < gridTile.GetHeight()) neighbourList.Add(GetNode(currentNode.x, currentNode.y + 1));

        return neighbourList;
    }

    public List<GridTileNode> GetNeighbourListWithTypeGroundOrNone(GridTileNode currentNode)
    {
        List<GridTileNode> neighbourList = new List<GridTileNode>();

        if (currentNode.x - 1 >= 0)
        {
            //left
            GridTileNode left = GetNode(currentNode.x - 1, currentNode.y);
            if (currentNode.typeGround == left.typeGround || left.typeGround == TypeGround.None) neighbourList.Add(left);
            //// left down
            //if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y - 1));
            //// left up
            //if (currentNode.y + 1 < gridTile.GetHeight()) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y + 1));
        }

        if (currentNode.x + 1 < gridTile.GetWidth())
        {
            // right
            GridTileNode right = GetNode(currentNode.x + 1, currentNode.y);
            if (currentNode.typeGround == right.typeGround || right.typeGround == TypeGround.None) neighbourList.Add(right);
            //// right down
            //GridTileNode rightDown = GetNode(currentNode.x + 1, currentNode.y);
            //if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y - 1));
            ////right up
            //if (currentNode.y + 1 < gridTile.GetHeight()) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y + 1));
        }

        // down
        if (currentNode.y - 1 >= 0)
        {
            GridTileNode down = GetNode(currentNode.x, currentNode.y - 1);
            if (currentNode.typeGround == down.typeGround || down.typeGround == TypeGround.None) neighbourList.Add(down);
        }
        // up
        if (currentNode.y + 1 < gridTile.GetHeight())
        {

            GridTileNode up = GetNode(currentNode.x, currentNode.y + 1);
            if (currentNode.typeGround == up.typeGround || up.typeGround == TypeGround.None) neighbourList.Add(up);
        }

        return neighbourList;
    }

    public List<GridTileNode> GetNeighbourListWithTypeGround(GridTileNode currentNode)
    {
        List<GridTileNode> neighbourList = new List<GridTileNode>();

        if (currentNode.x - 1 >= 0)
        {
            //left
            GridTileNode left = GetNode(currentNode.x - 1, currentNode.y);
            if (currentNode.typeGround == left.typeGround) neighbourList.Add(left);
            //// left down
            //if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y - 1));
            //// left up
            //if (currentNode.y + 1 < gridTile.GetHeight()) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y + 1));
        }

        if (currentNode.x + 1 < gridTile.GetWidth())
        {
            // right
            GridTileNode right = GetNode(currentNode.x + 1, currentNode.y);
            if (currentNode.typeGround == right.typeGround) neighbourList.Add(right);
            //// right down
            //GridTileNode rightDown = GetNode(currentNode.x + 1, currentNode.y);
            //if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y - 1));
            ////right up
            //if (currentNode.y + 1 < gridTile.GetHeight()) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y + 1));
        }

        // down
        if (currentNode.y - 1 >= 0)
        {
            GridTileNode down = GetNode(currentNode.x, currentNode.y - 1);
            if (currentNode.typeGround == down.typeGround) neighbourList.Add(down);
        }
        // up
        if (currentNode.y + 1 < gridTile.GetHeight())
        {

            GridTileNode up = GetNode(currentNode.x, currentNode.y + 1);
            if (currentNode.typeGround == up.typeGround) neighbourList.Add(up);
        }
        return neighbourList;
    }

    public NeighboursNature GetDisableNeighbours(GridTileNode currentNode)
    {
        NeighboursNature neighbours = new NeighboursNature();
        neighbours.left = new List<GridTileNode>();
        neighbours.right = new List<GridTileNode>();
        neighbours.top = new List<GridTileNode>();
        neighbours.bottom = new List<GridTileNode>();

        if (currentNode.x - 1 >= 0)
        {
            //left
            GridTileNode left = GetNode(currentNode.x - 1, currentNode.y);
            if (left.Disable && left.Empty) neighbours.left.Add(left);
            // left down
            if (currentNode.y - 1 >= 0)
            {
                GridTileNode leftdown = GetNode(currentNode.x - 1, currentNode.y - 1);
                if (leftdown.Disable && leftdown.Empty)
                {
                    neighbours.left.Add(leftdown);
                    neighbours.bottom.Add(leftdown);
                }
            }
            // left up
            if (currentNode.y + 1 < gridTile.GetHeight())
            {
                GridTileNode leftup = GetNode(currentNode.x - 1, currentNode.y + 1);
                if (leftup.Disable && leftup.Empty)
                {
                    neighbours.top.Add(leftup);
                    neighbours.left.Add(leftup);
                }
            }
        }

        if (currentNode.x + 1 < gridTile.GetWidth())
        {
            // right
            GridTileNode right = GetNode(currentNode.x + 1, currentNode.y);
            if (right.Disable && right.Empty) neighbours.right.Add(right);
            // right down
            if (currentNode.y - 1 >= 0)
            {
                GridTileNode rightDown = GetNode(currentNode.x + 1, currentNode.y - 1);
                if (rightDown.Disable && rightDown.Empty)
                {
                    neighbours.right.Add(rightDown);
                    neighbours.bottom.Add(rightDown);
                }
            }
            //right up
            if (currentNode.y + 1 < gridTile.GetHeight())
            {
                GridTileNode rightUp = GetNode(currentNode.x + 1, currentNode.y + 1);
                if (rightUp.Disable && rightUp.Empty)
                {
                    neighbours.right.Add(rightUp);
                    neighbours.top.Add(rightUp);
                }
            }
        }

        // down
        if (currentNode.y - 1 >= 0)
        {
            GridTileNode down = GetNode(currentNode.x, currentNode.y - 1);
            if (down.Disable && down.Empty) neighbours.bottom.Add(down);
        }
        // up
        if (currentNode.y + 1 < gridTile.GetHeight())
        {
            GridTileNode up = GetNode(currentNode.x, currentNode.y + 1);
            if (up.Disable && up.Empty) neighbours.top.Add(up);
        }
        return neighbours;
    }

    public GridTileNode GetNode(int x, int y)
    {
        return gridTile.getGridObject(new Vector3Int(x, y));
    }
    public GridTileNode GetNode(Vector3Int pos)
    {
        return gridTile.getGridObject(pos);
    }

    private List<GridTileNode> CalculatePath(GridTileNode endNode)
    {
        List<GridTileNode> path = new List<GridTileNode>();
        path.Add(endNode);

        GridTileNode currentNode = endNode;
        while (currentNode.cameFromNode != null)
        {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();
        return path;
    }

    private int CalculateDistanceCost(GridTileNode a, GridTileNode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * a.koofPath * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * b.koofPath * remaining;
    }

    private GridTileNode GetLowestCostNode(List<GridTileNode> GridTileNodeList)
    {
        GridTileNode lowestFCostNode = GridTileNodeList[0];
        for (int i = 1; i < GridTileNodeList.Count; i++)
        {
            if (GridTileNodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = GridTileNodeList[i];
            }
        }
        return lowestFCostNode;
    }

    public float GetDistanceBetweeenPoints(Vector3Int a, Vector3Int b)
    {
        float xCoord = b.x - a.x;
        float yCoord = b.y - a.y;
        float remaining = Mathf.Pow(xCoord, 2) + Mathf.Pow(yCoord, 2);
        return Mathf.Sqrt(remaining);
    }

    public int CalculateNeighbours(GridTileNode node)
    {
        List<GridTileNode> neighbours = GetNeighbourList(node, true);
        int countWalkableNeighbours = 0;
        for (int i = 0; i < neighbours.Count; i++)
        {
            if (neighbours[i].Empty && neighbours[i].Enable && !neighbours[i].Protected)
            {
                countWalkableNeighbours++;
            }
        }

        return countWalkableNeighbours;
    }
    public int CalculateNeighboursByArea(GridTileNode node, bool diagonal = true)
    {
        List<GridTileNode> neighbours = GetNeighbourList(node, diagonal);
        int countWalkableNeighbours = 0;
        for (int i = 0; i < neighbours.Count; i++)
        {
            if (
                neighbours[i].Empty
                && neighbours[i].Enable
                // && neighbours[i].typeGround == node.typeGround
                && neighbours[i].keyArea == node.keyArea
                && !neighbours[i].Protected
                )
            {
                countWalkableNeighbours++;
            }
        }

        return countWalkableNeighbours;
    }
}

public struct NeighboursNature
{
    public List<GridTileNode> left;
    public List<GridTileNode> right;

    public List<GridTileNode> top;
    public List<GridTileNode> bottom;
}