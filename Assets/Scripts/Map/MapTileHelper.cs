using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DataMapTileHelper
{
    public int _width;
    public int _height;
    public bool _isWater;
    public int _countArea;
}
public class MapTileHelper
{
    //[SerializeField] DataMapTileHelper Data;

    private GridTileHelper gridTileHelper;

    private List<GridTileNatureNode> _listNatureNode = new List<GridTileNatureNode>();
    public List<GridTileNode> MapNodes => gridTileHelper?.GetAllGridNodes();
    public List<GridTileNatureNode> NatureNodes => _listNatureNode;

    public MapTileHelper(int width, int height)
    {
        // Create grid tile nodes.
        gridTileHelper = new GridTileHelper(width, height);
    }


}
