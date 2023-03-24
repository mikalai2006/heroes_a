using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[CreateAssetMenu(fileName ="New Nature",menuName ="Units/Create Nature")]
public class TileNature : RuleTile<TileNature.Neighbor>
{
    public string idObject;

    public TypeGround typeGround;

    public AnimatedTile tile;

    public DataNode dataNode;

    public bool isWalkable;

    public bool isCorner;

    public List<TypeNoPath> listTypeNoPath;

    public class Neighbor : RuleTile.TilingRule.Neighbor
    {
        public const int Null = 3;
        public const int NotNull = 4;
    }

    public override bool RuleMatch(int neighbor, TileBase tile)
    {
        switch (neighbor)
        {
            case Neighbor.Null: return tile == null;
            case Neighbor.NotNull: return tile != null;
        }
        return base.RuleMatch(neighbor, tile);
    }
}
