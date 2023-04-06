using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityMine", menuName = "Game/Entity/MapObject/New Mine")]
public class ScriptableEntityMine : ScriptableEntityPerk
{
    [Header("Options Mine")]
    public TypeMine TypeMine;
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
public enum TypeMine
{
    Free = 0,
    Town = 1,
}