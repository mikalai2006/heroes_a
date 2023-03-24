using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataMap
{
    [SerializeField] public List<GridTileNode> mapNode;
    [SerializeField] public List<GridTileNatureNode> natureNode;
    public DataGameMode GameModeData;
    public bool isWater;
    public int countArea;

    public DataMap()
    {

        mapNode = new List<GridTileNode>();
        natureNode = new List<GridTileNatureNode>();
        GameModeData = new DataGameMode();
    }

}

