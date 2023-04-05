using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityMapObject", menuName = "Game/Entity/New MapObject")]
public class ScriptableEntityMapObject : ScriptableEntityPerk
{
    // [Header("Options MapObject")]
    // public TypeMapObject TypeMapObject;
    // public MapObjectType TypeMapObject;
    // public TypeWorkMapObject TypeWork;
    // public List<GroupResource> Resources;
    // public List<GroupSkill> Skills;
    // public List<GroupTwoSkill> TwoSkills;
    // public List<GroupArtifact> Artifacts;
    // public MapObjectType TypeMapObject;
    // public TypeWorkPerk TypeWorkMapObject;
}

[System.Serializable]
public enum TypeMapObject
{
    // Enemy = 0,
    Artifact = 1,
    Monolith = 4,
    Hero = 5,
    Town = 6,
    Explore = 20,
    Mine = 21,
    Creature = 22,

    SkillSchool = 30,
    Resource = 40,
}