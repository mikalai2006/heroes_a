using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;

public abstract class ScriptableUnitBase : ScriptableObject
{

    public string idObject;
    public int level;
    public TypeGround typeGround;
    public TypeInput typeInput;
    public TypeUnit TypeUnit;
    //public TypeGround TypeGround;

    public List<TypeNoPath> listTypeNoPath;
    public List<TypeNoPath> RulesDraw => listTypeNoPath;

    // Used in game
    public UnitBase Prefab;
    // Used in menus
    //public string Description;
    public Sprite MenuSprite;

    [SerializeField] public List<LangItem> Locale;
}

[System.Serializable]
public struct LangItem
{
    public Locale Language;
    public LocaleItem Text;
}

[System.Serializable]
public struct LocaleItem
{
    public string title;
    [TextArea] public string description;
    [TextArea] public string visit_ok;
    [TextArea] public string visit_no;
    [TextArea] public string visit_noresource;
}

[Serializable]
public struct RulesDraw
{
    public bool left;
    public bool right;
    public bool top;
    public bool topLeft;
}

[Serializable]
public enum TypeUnit
{
    Hero = 0,
    Town = 1,
    Portal = 2,
    MapObject = 3,
    Monolith = 4,
    Resource = 5,
    Mine = 6,
    Warrior = 7,
}

public enum TypeInput
{
    Down = 0,
    None = 1,
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