using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "NewMapObject", menuName = "Game/Units/New MapObject")]
public class ScriptableMapObject : ScriptableUnitBase
{
    public MapObjectType TypeMapObject;
    public TypeWork TypeWork;
    public List<GroupResource> Resources;
    public List<GroupSkill> Skills;
    public List<GroupTwoSkill> TwoSkills;
    public List<GroupArtifact> Artifacts;
}

[System.Serializable]
public enum MapObjectType
{
    Enemy = 0,
    Artifact = 1,

    Explore = 20,

    SkillSchool = 30,
    Resource = 40,
}


[System.Serializable]
public enum TypeWork
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
    public ScriptableSkill Skill;
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
    public ScriptableResource Resource;
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

[System.Serializable]
public struct GroupArtifact
{
    public string id;
    public List<ItemArtifact> ListVariant;
}

[System.Serializable]
public struct ItemArtifact
{
    public ScriptableArtifact Artifact;
}