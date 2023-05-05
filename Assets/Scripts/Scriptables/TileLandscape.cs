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
    public RuleTile tileRule;
    // public DataNode dataNode;
    public bool isWalkable;
    public bool isFraction;
    public TypeGround typeGround;
    // public TypeFaction typeFaction;
    public int Penalties;
}

[System.Serializable]
public class CornerTiles : Object
{
    public Tile tile;
    public List<TypeNoPath> listTypeNoPath;

}

[System.Flags]
public enum TypeGround
{
    None = 1 << 0,
    Dirt = 1 << 1,
    Grass = 1 << 2,
    Highlands = 1 << 3,
    Rough = 1 << 4,
    Lava = 1 << 5,
    Rock = 1 << 6,
    Sand = 1 << 7,
    Snow = 1 << 8,
    Swamp = 1 << 9,
    Subterranean = 1 << 10,
    Water = 1 << 11,
}
