
using System;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public struct DataHero
{
    public int idPlayer;
    public string ide;
    public string name;
    public string idObject;
    public int level;
    public int nextLevel;
    public SerializableDictionary<TypePrimarySkill, int> PSkills;
    public SerializableDictionary<TypeSecondarySkill, int> SSkills;
    public Vector3Int nextPosition;
    public float speed;
    public float hit;
    public float mana;


    public List<string> artifacts;
    [NonSerialized] public List<EntityArtifact> Artifacts;
    public SerializableDictionary<int, EntityCreature> Creatures;
    public List<GridTileNode> path; // [NonSerialized]
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