using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Game/System/Cursor Tile")]
public class CursorTile : Tile
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

    public List<rule> rules;
    public bool staticTile;

    private List<TileBase> tilesToCheck;

    public bool HasIdentialContent<T>(List<T> A, List<T> B)
    {
        return new HashSet<T>(A).SetEquals(B);
    }

    public int GetNeighborCount(Vector3Int location, ITilemap tilemap)
    {
        int count = 0;

        for (int xd = -1; xd <= 1; xd++)
        {
            for (int yd = -1; yd <= 1; yd++)
            {
                if (yd == 0 && xd == 0) continue;

                TileBase tile = tilemap.GetTile(location + new Vector3Int(xd, yd, 0));

                if (tilesToCheck.Contains(tile)) count++;
            }
        }

        return count;
    }

    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        for (int xd = -1; xd <= 1; xd++)
        {
            for (int yd = -1; yd <= 1; yd++)
            {
                if (yd == 0 && xd == 0) continue;

                tilemap.RefreshTile(position + new Vector3Int(xd, yd, 0));
            }
        }
        // base.RefreshTile(position, tilemap);
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
                var t = tilemap.GetTile<CursorTile>(p);

                if (t == null) continue;

                var r = t.FindRule(s);
                if (r.HasValue && r.Value.outgoing_directions.Contains(directionToOpposite[d]))
                {
                    incoming.Add(d);
                }
                else if (s != null)
                {
                    outgoing.Add(d);
                }
            }

            var rule = FindRule(incoming, outgoing);
            Debug.Log($"rule.HasValue:{rule.HasValue}");
            if (rule.HasValue)
            {
                Debug.Log($"Found rule:{rule.Value.sprite}");
                tileData.sprite = rule.Value.sprite;
            }
        }

    }
}