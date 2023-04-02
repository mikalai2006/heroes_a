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
    Tavern = 1 << 1,
    Town_1 = 1 << 2,
    Town_2 = 1 << 3,
    Town_3 = 1 << 4,
    Town_4 = 1 << 5,
    Castle_1 = 1 << 6,
    Castle_2 = 1 << 7,
    Castle_3 = 1 << 8,
    Blacksmith = 1 << 9,
    MarketPlace = 1 << 10,
    ResourceSilo = 1 << 11,
    MageGuild_1 = 1 << 12,
    MageGuild_2 = 1 << 13,
    MageGuild_3 = 1 << 14,
    MageGuild_4 = 1 << 15,
    MageGuild_5 = 1 << 16,
    Specifier_1 = 1 << 17,
    Specifier_2 = 1 << 18,
    Specifier_3 = 1 << 19,
    Specifier_4 = 1 << 20,
    Specifier_5 = 1 << 21,
    Specifier_6 = 1 << 22,

}

[System.Serializable]
public struct Build
{
    public Sprite MenuSprite;
    [SerializeField] public TypeBuild TypeBuild;
    [SerializeField] public TypeBuild RequiredBuilds;
    [SerializeField] public TypeBuildArmy RequiredBuildsArmy;
    public BuildBase UpdatePrefab;
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
