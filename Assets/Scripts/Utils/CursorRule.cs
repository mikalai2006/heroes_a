using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System.Collections.Generic;
using System;

[CreateAssetMenu(menuName = "Game/System/Cursor Rule")]
public class CursorRule : RuleTile<CursorRule.Neighbor>
{
    [System.Serializable]
    public enum direction
    {
        up, down, left, right
    }
    Dictionary<direction, Vector3Int> directionToVector = new Dictionary<direction, Vector3Int>(){
        { direction.up, Vector3Int.up },
        { direction.down, Vector3Int.down },
        { direction.left, Vector3Int.left },
        { direction.right, Vector3Int.right },
    };
    Dictionary<direction, direction> directionToOpposite = new Dictionary<direction, direction>(){
        { direction.up, direction.down },
        { direction.down, direction.up },
        { direction.left, direction.right },
        { direction.right, direction.left },
    };

    [System.Serializable]
    public struct rule
    {
        public Sprite sprite;
        public List<direction> incoming_directions;
        public List<direction> outgoing_directions;
    }

    [Header("Advanced Tile")]
    public List<rule> rules;
    public bool staticTile;


    [Header("Advanced Tile")]
    [Tooltip("If enabled, the tile will connect to these tiles too when the mode is set to \"This\"")]
    public bool alwaysConnect;
    [Tooltip("Tiles to connect to")]
    public TileBase[] tilesToConnect;
    [Space]
    [Tooltip("Check itseft when the mode is set to \"any\"")]
    public bool checkSelf = true;


    public class Neighbor : RuleTile.TilingRule.Neighbor
    {
        public const int Input = 3;
        public const int Output = 4;
        public const int Nothing = 5;
    }


    public rule? FindRule(Sprite sprite)
    {
        foreach (rule r in rules)
        {
            if (sprite == r.sprite) return r;
        }
        return null;
    }

    public rule? FindRule(List<direction> incoming, List<direction> outgoing)
    {
        foreach (rule r in rules)
        {
            if (HasIdentialContent<direction>(incoming, r.incoming_directions)
            && HasIdentialContent<direction>(outgoing, r.outgoing_directions))
            {
                return r;
            }
        }
        return null;
    }

    public bool HasIdentialContent<T>(List<T> A, List<T> B)
    {
        return new HashSet<T>(A).SetEquals(B);
    }

    public override bool RuleMatch(int neighbor, TileBase tile)
    {
        if (tile == null) return false;
        // Debug.Log($"Check tile rule {tile?.name}");

        // switch (neighbor)
        // {
        //     case Neighbor.This: return Check_This(tile);
        //     case Neighbor.NotThis: return Check_NotThis(tile);
        //     case Neighbor.Input: return Check_Any(tile);
        //     case Neighbor.Output: return Check_Specified(tile);
        //     case Neighbor.Nothing: return Check_Nothing(tile);
        // }
        return false; //base.RuleMatch(neighbor, tile);
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);
        if (!staticTile)
        {
            List<direction> incoming = new List<direction>();
            List<direction> outgoing = new List<direction>();
            foreach (direction d in Enum.GetValues(typeof(direction)))
            {
                var p = position + directionToVector[d];
                var s = tilemap.GetSprite(p);
                var t = tilemap.GetTile<CursorRule>(p);

                if (t == null) continue;

                var r = t.FindRule(s);
                if (r.HasValue && r.Value.outgoing_directions.Contains(directionToOpposite[d]))
                {
                    Debug.Log($"Incoming");
                    incoming.Add(d);
                }
                else if (s != null)
                {
                    Debug.Log($"Outgoing");
                    outgoing.Add(d);
                }
            }

            var rule = FindRule(incoming, outgoing);
            if (rule.HasValue)
            {
                Debug.Log($"Found rule:{rule.Value.sprite} - {position}");
                tileData.sprite = rule.Value.sprite;
            }
        }

    }

    /// <summary>
    /// Возвращает true, если плитка является этой или если плитка является одной из указанных плиток, если включено всегда подключение.
    /// </summary>
    /// <param name="tile">Соседняя плитка для сравнения</param>
    /// <returns></returns>
    bool Check_This(TileBase tile)
    {
        if (!alwaysConnect) return tile == this;
        else return tilesToConnect.Contains(tile) || tile == this;

        //.Contains requires "using System.Linq;"
    }

    /// <summary>
    /// Возвращает true, если тайл не является этим.
    /// </summary>
    /// <param name="tile">Соседняя плитка для сравнения</param>
    /// <returns></returns>
    bool Check_NotThis(TileBase tile)
    {
        if (!alwaysConnect) return tile != this;
        else return !tilesToConnect.Contains(tile) && tile != this;

        //.Contains requires "using System.Linq;"
    }

    /// <summary>
    /// Верните true, если плитка не пуста, или не это, если опция проверки себя отключена.
    /// </summary>
    /// <param name="tile">Neighboring tile to compare to</param>
    /// <returns></returns>
    bool Check_Any(TileBase tile)
    {
        if (checkSelf) return tile != null;
        else return tile != null && tile != this;
    }

    /// <summary>
    /// Возвращает true, если плитка является одной из указанных плиток.
    /// </summary>
    /// <param name="tile">Neighboring tile to compare to</param>
    /// <returns></returns>
    bool Check_Specified(TileBase tile)
    {
        return tilesToConnect.Contains(tile);

        //.Contains requires "using System.Linq;"
    }

    /// <summary>
    /// Возвращает true, если плитка пуста.
    /// </summary>
    /// <param name="tile">Neighboring tile to compare to</param>
    /// <param name="tile"></param>
    /// <returns></returns>
    bool Check_Nothing(TileBase tile)
    {
        return tile == null;
    }
}