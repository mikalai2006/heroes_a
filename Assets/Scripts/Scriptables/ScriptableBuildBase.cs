using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewBuild", menuName = "Game/Build/New Build")]
public class ScriptableBuildBase : ScriptableObject
{
    public string idObject;
    public TypeFaction TypeFaction;
    // public int level;
    public BuildBase Prefab;
    [SerializeField] public List<Build> BuildLevels;
}

[System.Flags]
public enum TypeBuild
{
    None = 1 << 0,
    Tavern_1 = 1 << 1,
    Tavern_2 = 1 << 2,
    Town_1 = 1 << 3,
    Town_2 = 1 << 4,
    Town_3 = 1 << 5,
    Town_4 = 1 << 6,
    Castle_1 = 1 << 7,
    Castle_2 = 1 << 8,
    Castle_3 = 1 << 9,
    Blacksmith = 1 << 10,
    MarketPlace = 1 << 11,
    ResourceSilo = 1 << 12,
    MageGuild_1 = 1 << 13,
    MageGuild_2 = 1 << 14,
    MageGuild_3 = 1 << 15,
    MageGuild_4 = 1 << 16,
    MageGuild_5 = 1 << 17,
    // Army
    Army_1_1 = 1 << 18,
    Army_1_2 = 1 << 19,
    Army_2_1 = 1 << 20,
    Army_2_2 = 1 << 21,
    Army_3_1 = 1 << 22,
    Army_3_2 = 1 << 23,
    Army_4_1 = 1 << 24,
    Army_4_2 = 1 << 25,
    Army_5_1 = 1 << 26,
    Army_5_2 = 1 << 27,
    Army_6_1 = 1 << 26,
    Army_6_2 = 1 << 27,
    Army_7_1 = 1 << 28,
    Army_7_2 = 1 << 29,
}

[System.Serializable]
public struct Build
{
    public Sprite MenuSprite;
    [SerializeField] public TypeBuild TypeBuild;
    [SerializeField] public TypeBuild RequiredBuilds;
    [SerializeField] public List<LangItem> Locale;
    [SerializeField] public List<BuildCostResource> CostResource;
}

[System.Serializable]
public struct BuildCostResource
{
    public ScriptableResource Resource;
    public int Count;
}

[System.Serializable]
public enum TypeFaction
{
    Castle = 0,
    Stronghold = 1,
}
