using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;

public abstract class ScriptableEntity : ScriptableObject
{
    public string idObject;
    public TypeEntity TypeEntity;
    [SerializeField] public AssetReferenceGameObject MapPrefab;
    public Sprite MenuSprite;
    [SerializeField] public LangEntity Text;
    [SerializeField] public DialogText DialogText;

    [Header("Map Options")]
    public TypeInput typeInput;
    public List<TypeNoPath> listTypeNoPath;
    public List<TypeNoPath> RulesDraw => listTypeNoPath;
}

[System.Serializable]
public enum TypeInput
{
    Down = 0,
    None = 1,
}

[System.Serializable]
public enum TypeFaction
{
    Castle = 0,
    Stronghold = 1,
    Neutral = 100,
}

[Serializable]
public enum TypeEntity
{
    // Faction = 0,
    Creature = 10,
    Artifact = 20,
    MapObject = 30,
    Resource = 50,
    Building = 70,
    Town = 80,
    Hero = 90,
}

[System.Serializable]
public struct LangEntity
{
    public LocalizedString title;
    public LocalizedString description;
}

[System.Serializable]
public struct DialogText
{
    public LocalizedString VisitOk;
    public LocalizedString VisitNo;
    public LocalizedString VisitNoResource;
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