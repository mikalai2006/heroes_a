
using System;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public struct DataHero
{
    public int idPlayer;
    public Vector3Int nextPosition;
    public float speed;
    public float hit;
    public float mana;
    public string name;

    [NonSerialized] public List<EntityArtifact> Artifacts;
    [NonSerialized] public List<EntityCreature> Creatures;
    [NonSerialized] public List<GridTileNode> path;
}