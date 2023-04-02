using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[CreateAssetMenu(fileName = "NewBuildArmy", menuName = "Game/Build/New Build Army")]
public class ScriptableBuildArmy : ScriptableObject
{
    public string idObject;
    public TypeFaction TypeFaction;
    // public int level;
    public BuildBase Prefab;
    [SerializeField] public List<BuildArmy> BuildArmys;
    [SerializeField] public BitArray ar;
}

[System.Flags]
public enum TypeBuildArmy
{
    None = 1 << 0,
    // Army
    Army_1_1 = 1 << 1,
    Army_1_2 = 1 << 2,
    Army_2_1 = 1 << 3,
    Army_2_2 = 1 << 4,
    Army_3_1 = 1 << 5,
    Army_3_2 = 1 << 6,
    Army_4_1 = 1 << 7,
    Army_4_2 = 1 << 8,
    Army_5_1 = 1 << 9,
    Army_5_2 = 1 << 10,
    Army_6_1 = 1 << 11,
    Army_6_2 = 1 << 12,
    Army_7_1 = 1 << 13,
    Army_7_2 = 1 << 14,
}

[System.Serializable]
public struct BuildArmy
{
    public Sprite MenuSprite;
    [SerializeField] public TypeBuildArmy TypeBuildArmy;
    [SerializeField] public TypeBuild RequiredBuilds;
    [SerializeField] public TypeBuildArmy RequiredBuildsArmy;
    public BuildBase UpdatePrefab;
    [SerializeField] public List<LangItem> Locale;
    [SerializeField] public List<BuildCostResource> CostResource;
}
