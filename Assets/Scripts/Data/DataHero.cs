
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
    public SerializableDictionary<TypeSecondarySkill, SkillDataItem> SSkills;
    // [NonSerialized] public SerializableDictionary<ScriptableAttributeSecondarySkill, int> SecondarySkills;
    public Vector3Int nextPosition;
    public float speed;
    public float mp;
    public float mana;
    // public float manaTotal;
    public bool isBook;
    public List<string> spells;
    [NonSerialized] public EntityBook SpellBook;
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

[Serializable]
public struct SkillDataItem
{
    public int level;
    public int value;
}
