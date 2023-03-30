using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "NewBuild", menuName = "Game/Build/New Build")]
public class ScriptableBuildBase : ScriptableObject
{
    public string idObject;
    public TypeFaction TypeFaction;
    public int level;
    [SerializeField] public List<Build> Builds;
    [SerializeField] public List<ScriptableBuildBase> RequireBuilds;
    [SerializeField] public List<BuildCostResource> RequireResource;
}

[System.Flags]
public enum TypeBuild
{
    Tavern_1 = 1 << 0,
    Tavern_2 = 1 << 1,
    Tower_1 = 1 << 2,
    Tower_2 = 1 << 3,
    Tower_3 = 1 << 4,
    Tower_4 = 1 << 5,
    Castle_1 = 1 << 6,
    Castle_2 = 1 << 7,
    Castle_3 = 1 << 8,
    Forge = 1 << 9,
    ResourceStorage = 1 << 10,
    GuildMagic_1 = 1 << 11,
    GuildMagic_2 = 1 << 12,
    GuildMagic_3 = 1 << 13,
    GuildMagic_4 = 1 << 14,
    GuildMagic_5 = 1 << 15,
    // Army
    Army_1_1 = 1 << 16,
    Army_1_2 = 1 << 17,
    Army_2_1 = 1 << 18,
    Army_2_2 = 1 << 19,
    Army_3_1 = 1 << 20,
    Army_3_2 = 1 << 21,
    Army_4_1 = 1 << 22,
    Army_4_2 = 1 << 23,
    Army_5_1 = 1 << 24,
    Army_5_2 = 1 << 25,
    Army_6_1 = 1 << 26,
    Army_6_2 = 1 << 27,
    Army_7_1 = 1 << 28,
    Army_7_2 = 1 << 29,
}

[System.Serializable]
public enum BuildType
{
    Tavern_1 = 0,
    Tavern_2 = 1,
    Tower_1 = 2
}


[System.Serializable]
public struct Build
{
    public Sprite MenuSprite;
    public BuildBase Prefab;
    [SerializeField] public TypeBuild TypeBuild;
    [SerializeField] public TypeBuild RequiredBuilds;
    [SerializeField] public List<LangItem> Locale;
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
