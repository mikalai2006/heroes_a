using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "NewBuilding", menuName = "Game/Building/New Build")]
public class ScriptableBuildBase : ScriptableObject
{
    public string idObject;
    public TypeFaction TypeFaction;
    public BuildBase Prefab;

    // [EnumTypeBuild(typeof(TestTypeBuild))]
    // public TestTypeBuild[] TypeRequireBuild2;

    // [SerializeField] public TypeBuild2 TypeBuild2;
    [SerializeField] public List<Build> BuildLevels;
}

// [System.Flags]
// public enum TypeBuild2
// {
//     None = 1,
//     Tavern = 2,
//     Town = 3,
//     Castle = 4,
//     Blacksmith = 5,
//     MarketPlace = 6,
//     ResourceSilo = 7,
//     MageGuild = 8,

//     Army_1 = 10,
//     Army_2 = 11,
//     Army_3 = 12,
//     Army_4 = 13,
//     Army_5 = 14,
//     Army_6 = 15,
//     Army_7 = 18,

//     Specifier_1 = 19,
//     Specifier_2 = 20,
//     Specifier_3 = 21,
//     Specifier_4 = 22,
//     Specifier_5 = 23,
//     Specifier_6 = 24,
//     Specifier_99 = 99,
//     Specifier_100 = 100,

// }
// [System.Flags]
// public enum TypeBuild
// {
//     None = 1 << 0,
//     Tavern = 1 << 1,
//     Town_1 = 1 << 2,
//     Town_2 = 1 << 3,
//     Town_3 = 1 << 4,
//     Town_4 = 1 << 5,
//     Castle_1 = 1 << 6,
//     Castle_2 = 1 << 7,
//     Castle_3 = 1 << 8,
//     Blacksmith = 1 << 9,
//     MarketPlace = 1 << 10,
//     ResourceSilo = 1 << 11,
//     MageGuild_1 = 1 << 12,
//     MageGuild_2 = 1 << 13,
//     MageGuild_3 = 1 << 14,
//     MageGuild_4 = 1 << 15,
//     MageGuild_5 = 1 << 16,
//     Specifier_1 = 1 << 17,
//     Specifier_2 = 1 << 18,
//     Specifier_3 = 1 << 19,
//     Specifier_4 = 1 << 20,
//     Specifier_5 = 1 << 21,
//     Specifier_6 = 1 << 22,

// }
public enum TypeBuild
{
    None = 1,
    Tavern = 2,
    Village = 3,
    Town = 4,
    City = 5,
    Capitol = 6,
    Fort = 7,
    Citadel = 8,
    Castle = 9,
    // Castle_4 = 10,
    Blacksmith = 11,
    MarketPlace = 12,
    ResourceSilo = 13,
    MageGuild_1 = 14,
    MageGuild_2 = 15,
    MageGuild_3 = 16,
    MageGuild_4 = 17,
    MageGuild_5 = 18,

    Army_1_1 = 19,
    Army_1_2 = 20,
    Army_2_1 = 21,
    Army_2_2 = 22,
    Army_3_1 = 23,
    Army_3_2 = 24,
    Army_4_1 = 25,
    Army_4_2 = 26,
    Army_5_1 = 27,
    Army_5_2 = 28,
    Army_6_1 = 29,
    Army_6_2 = 30,
    Army_7_1 = 31,
    Army_7_2 = 32,

    Specific_1_1 = 33,
    Specific_1_2 = 34,
    Specific_2_1 = 35,
    Specific_2_2 = 36,
    Specific_3_1 = 37,
    Specific_3_2 = 38,
    Specific_4_1 = 39,
    Specific_4_2 = 40,
    Specific_5_1 = 41,
    Specific_5_2 = 42,
    Specific_6_1 = 43,
    Specific_6_2 = 44,


}

[System.Serializable]
public struct Build
{
    public Sprite MenuSprite;
    [SerializeField] public TypeBuild TypeBuild;
    public BuildBase UpdatePrefab;
    [SerializeField] public TypeBuild[] RequiredBuilds;
    // [SerializeField] public TypeBuildArmy RequiredBuildsArmy;
    // [SerializeField] public List<LangItem> Locale;
    [SerializeField] public LangBuild Text;
    // [SerializeField] public TestTypeBuild[] RequireBuilds2;
    // [SerializeField] public int level;
    [SerializeField] public List<BuildCostResource> CostResource;
}

[System.Serializable]
public struct LangBuild
{
    public LocalizedString title;
    public LocalizedString description;
}

[System.Serializable]
public struct BuildCostResource
{
    public ScriptableAttributeResource Resource;
    public int Count;
}

[System.Serializable]
public struct TypeRequireBuild
{
    public ScriptableBuildBase typeBuild2;
    public int level;
    public bool yes;
}