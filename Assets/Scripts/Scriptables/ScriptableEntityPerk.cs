using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.AddressableAssets;

public class ScriptableEntityPerk : ScriptableEntity
{
    [Header("Options perks")]
    // public TypeFaction TypeFaction;
    // [SerializeField] public TypeBuild TypeBuild;
    // public TypeWorkMapObject TypeWork;
    // // public AssetReferenceGameObject UpdatePrefab;
    // [SerializeField] public TypeBuild[] RequiredBuilds;
    // // [SerializeField] public LangBuild Text;
    // [SerializeField] public List<BuildCostResource> CostResource;


    // public TypeMapObject TypeMapObject;
    public TypeWorkPerk TypeWorkPerk;
    public List<GroupResource> Resources;
    public List<GroupSkill> PrimarySkills;
    public List<GroupTwoSkill> SecondarySkills;
    // public List<GroupTwoSkill> TwoSkills;
    public List<ScriptableEntityArtifact> Artifacts;
}

[System.Serializable]
public enum TypeWorkPerk
{
    One = 1,
    EveryDay = 2,
    EveryWeek = 3,
}

[System.Serializable]
public struct GroupSkill
{
    public string id;
    public List<ItemSkill> ListVariant;
}

[System.Serializable]
public struct ItemSkill
{
    // public TypeSkill TypeSkill;
    public ScriptableAttributePrimarySkill Skill;
    public int Value;
}


[System.Serializable]
public struct GroupResource
{
    public string id;
    public List<ItemResource> ListVariant;
}

[System.Serializable]
public struct ItemResource
{
    // public TypeResource TypeResource;
    public ScriptableEntityResource Resource;
    public int maxValue;
    public int step;
}

[System.Serializable]
public struct GroupTwoSkill
{
    public string id;
    public List<ItemTwoSkill> ListVariant;
}

[System.Serializable]
public struct ItemTwoSkill
{
    public ScriptableTwoSkill TwoSkill;
    public int maxValue;
    public int step;
}