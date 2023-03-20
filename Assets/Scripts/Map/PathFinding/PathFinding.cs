//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System;

//public class PathFinding : MonoBehaviour
//{

//	private PathRequestManager pathRequestManager;
//	private Grid grid;

//	void Awake()
//	{
//		grid = GetComponent<Grid>();
//		pathRequestManager = GetComponent<PathRequestManager>();
//	}

//	public void StartFindPath(Vector3 startPos, Vector2 targetPos, int subPathIndex)
//	{
//		StartCoroutine(FindPath(startPos, targetPos, subPathIndex));
//	}

//	IEnumerator FindPath(Vector3 startPos, Vector3 targetPos, int subPathIndex)
//	{
//		Vector3[] waypoints = new Vector3[0];
//		bool pathSuccess = false;

//		GridNode startNode = grid.NodeFromWorldPoint(startPos);
//		GridNode targetNode = grid.NodeFromWorldPoint(targetPos);

//		if (startNode.walkable && targetNode.walkable)
//		{
//			BinaryHeap<GridNode> openSet = new BinaryHeap<GridNode>(grid.MaxSize);
//			HashSet<GridNode> closedSet = new HashSet<GridNode>();
//			openSet.Add(startNode);

//			while (openSet.Count > 0)
//			{
//				GridNode currentNode = openSet.RemoveFirst();
//				closedSet.Add(currentNode);

//				if (currentNode == targetNode)
//				{
//					pathSuccess = true;
//					break;
//				}

//				foreach (GridNode neighbor in grid.GetNeighbors(currentNode))
//				{
//					if (!neighbor.walkable || closedSet.Contains(neighbor))
//						continue;

//					int subPathModifier = subPathIndex > -1 ? 0 : 1;
//					int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor) + (neighbor.movementPenalty * subPathModifier);
//					if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
//					{
//						neighbor.gCost = newMovementCostToNeighbor;
//						neighbor.hCost = GetDistance(neighbor, targetNode);
//						neighbor.parent = currentNode;

//						if (!openSet.Contains(neighbor))
//							openSet.Add(neighbor);
//						else
//							openSet.UpdateItem(neighbor);
//					}
//				}
//			}
//		}
//		yield return null;

//		if (pathSuccess)
//		{
//			waypoints = BuildPath(startNode, targetNode);
//			pathSuccess = waypoints.Length > 0;
//		}
//		pathRequestManager.FinishedProcessingPath(waypoints, pathSuccess, subPathIndex);
//	}

//	int GetDistance(GridNode nodeA, GridNode nodeB)
//	{
//		int dstX = Mathf.Abs(nodeA.gridRow - nodeB.gridRow);
//		int dstY = Mathf.Abs(nodeA.gridCol - nodeB.gridCol);

//		if (dstX > dstY)
//			return 14 * dstY + 10 * (dstX - dstY);
//		return 14 * dstX + 10 * (dstY - dstX);
//	}

//	Vector3[] BuildPath(GridNode startNode, GridNode endNode)
//	{
//		List<GridNode> path = new List<GridNode>();
//		GridNode currentNode = endNode;
//		while (currentNode != startNode)
//		{
//			path.Add(currentNode);
//			currentNode = currentNode.parent;
//		}
//		path.Add(currentNode);
//		Vector3[] waypoints = SimplifyPath(path);
//		Array.Reverse(waypoints);
//		return waypoints;
//	}

//	public Vector3[] SimplifyPath(List<GridNode> path)
//	{
//		List<Vector3> waypoints = new List<Vector3>();
//		Vector2 directionOld = Vector2.zero;

//		waypoints.Add(path[0].worldPosition);
//		for (int i = 1; i < path.Count; i++)
//		{
//			Vector2 directionNew = new Vector2(path[i - 1].gridRow - path[i].gridRow, path[i - 1].gridCol - path[i].gridCol);
//			if (directionNew != directionOld)
//			{
//				waypoints.Add(path[i].worldPosition);
//			}
//			directionOld = directionNew;
//		}
//		return waypoints.ToArray();
//	}

//	private Vector3[] SimplifyPath(List<Vector3> path)
//	{
//		List<Vector3> waypoints = new List<Vector3>();
//		Vector2 directionOld = Vector2.zero;

//		for (int i = 1; i < path.Count; i++)
//		{
//			Vector2 directionNew = new Vector2(path[i - 1].x - path[i].x, path[i - 1].y - path[i].y);
//			if (directionNew != directionOld)
//			{
//				waypoints.Add(path[i]);
//			}
//			directionOld = directionNew;
//		}
//		return waypoints.ToArray();
//	}

//	public Vector3[] MergeSubPaths(Dictionary<int, Vector3[]> subPaths)
//	{
//		List<Vector3> complexPath = new List<Vector3>();
//		for (int subPathIndex = 0; subPathIndex < subPaths.Count; subPathIndex++)
//		{
//			Vector3[] subPath = null;
//			subPaths.TryGetValue(subPathIndex, out subPath);
//			if (subPath != null)
//			{
//				foreach (Vector3 point in subPath)
//					complexPath.Add(point);
//			}
//		}
//		return SimplifyPath(complexPath);
//	}
//}