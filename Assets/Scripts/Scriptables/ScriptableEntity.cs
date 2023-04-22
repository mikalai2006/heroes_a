using System;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;

public abstract class ScriptableEntity : ScriptableObject
{
    public string idObject;
    public TypeEntity TypeEntity;
    public Sprite MenuSprite;
    public LocalizedString title;

    [Header("Map Options")]
    [SerializeField] public AssetReferenceGameObject MapPrefab;
    public TypeGround TypeGround;
}

[System.Serializable]
public enum TypeFaction
{
    Castle = 0,
    Stronghold = 1,
    Conflux = 20,
    Fortress = 30,
    Dungeon = 40,
    Inferno = 50,
    Necropolis = 60,
    Rampart = 70,
    Tower = 80,
    Neutral = 100,
}

[Serializable]
public enum TypeEntity
{
    Creature = 10,
    // Artifact = 20,
    // Portal = 30,
    // Resource = 50,
    // Building = 70,
    Town = 80,
    Hero = 90,
    // Explore = 31,
    // Mine = 32,
    // SkillSchool = 33,
    MapObject = 34,
    Building = 35,
}

[System.Serializable]
public struct LangEntity
{
    public LocalizedString title;
    public LocalizedString description;
}


[System.Serializable]
public enum TypeNoPath
{
    Left2Top2 = 1,
    LeftTop2 = 2,
    Top2 = 3,
    RightTop2 = 4,
    Right2Top2 = 5,
    Left2Top = 6,
    LeftTop = 7,
    Top = 8,
    RightTop = 9,
    Right2Top = 10,
    Left2 = 11,
    Left = 12,
    Right = 14,
    Right2 = 15,
    Left2Bottom = 16,
    LeftBottom = 17,
    Bottom = 18,
    RightBottom = 19,
    Right2Bottom = 20,
    Left2Bottom2 = 21,
    LeftBottom2 = 22,
    Bottom2 = 23,
    RightBottom2 = 24,
    Right2Bottom2 = 25,
}