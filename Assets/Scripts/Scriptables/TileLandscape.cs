using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public struct DataNode
{
    public float speed;

}

[CreateAssetMenu(fileName = "NewLandscape", menuName = "Game/Nature/New Landscape")]
public class TileLandscape : ScriptableObject
{
    [SerializeField] public string idObject;

    public string objectName;

    public RuleTile tileRule;

    public DataNode dataNode;

    public float speed;

    public bool isWalkable;

    public bool isFraction;

    //public RuleTile noiseRule;

    //public List<Tile> tiles;

    //public List<CornerTiles> cornerTiles;

    public TypeGround typeGround;

    //public List<TileNature> natureTiles;
}

[System.Serializable]
public class CornerTiles : Object
{
    public Tile tile;
    public List<TypeNoPath> listTypeNoPath;

}

public enum TypeGround
{
    Water = 10,
    Grass = 20,
    Rough = 30,
    Sand = 40,
    None = 1000,
}