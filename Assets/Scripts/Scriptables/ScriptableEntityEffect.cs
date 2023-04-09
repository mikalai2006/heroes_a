using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Localization;

public class ScriptableEntityEffect : ScriptableEntity
{
    [Header("Options attributes")]
    public TypeWorkAttribute TypeWorkAttribute;
    // public List<ItemProbabiliti<GroupResource>> Resources;
    // public List<ItemProbabiliti<GroupPrimarySkill>> PrimarySkills;
    // public List<ItemProbabiliti<GroupSecondarySkill>> SecondarySkills;
    // public List<ItemProbabiliti<GroupArtifact>> Artifacts;

    public List<ItemProbabiliti<GroupAttributes>> Attributes;
}


[System.Serializable]
public struct GroupAttributes
{
    [SerializeField] public LocalizedString description;
    public List<ItemResource> Resources;
    public List<ItemSkill> PrimarySkills;
    public List<ItemTwoSkill> SecondarySkills;
    public ItemArtifact Artifacts;
}

[System.Serializable]
public enum TypeWorkAttribute
{
    All = 0,
    One = 1,
}

[System.Serializable]
public struct ItemArtifact
{
    public List<ScriptableAttributeArtifact> Artifact;
    public bool isOne;
}

[System.Serializable]
public struct GroupPrimarySkill
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
    public ScriptableAttributeResource Resource;
    public int value;
    public int minValue;
    public int maxValue;
    public int step;

}

[System.Serializable]
public struct GroupSecondarySkill
{
    public string id;
    public List<ItemTwoSkill> ListVariant;
}

[System.Serializable]
public struct ItemTwoSkill
{
    public ScriptableAttributeSecondarySkill SecondarySkill;
    public int value;
}