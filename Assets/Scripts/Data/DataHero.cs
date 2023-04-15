
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
    public SerializableDictionary<int, EntityCreature> Creatures;
    [NonSerialized] public List<GridTileNode> path;
    public StateHero State;
}

[Serializable]
[Flags]
public enum StateHero
{
    OnMap = 1 << 0,
    InTown = 1 << 1,
    InGuest = 1 << 2
}