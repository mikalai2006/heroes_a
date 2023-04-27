using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityMine", menuName = "Game/Entity/Mine")]
public class ScriptableEntityMine : ScriptableEntityMapObject, IEffected
{
    [Header("Options Mine")]
    public TypeMine TypeMine;
    // [Space(10)]
    // [Header("Options Perk")]
    // public List<EntityMinePerk> Resources;

    // public override UniTask RunHero(Player player, BaseEntity entity)
    // {
    //     base.RunHero(player, entity);
    // }
    // // public MapObjectType TypeMapObject;
    // // public TypeWorkMapObject TypeWork;
    // // public List<GroupResource> Resources;
    // // public List<GroupSkill> Skills;
    // // public List<GroupTwoSkill> TwoSkills;
    // // public List<GroupArtifact> Artifacts;
    // // public MapObjectType TypeMapObject;
    // // public TypeWorkPerk TypeWorkMapObject;
}

[System.Serializable]
public struct EntityMinePerk
{
    public string id;
    public ScriptableAttributeResource Resource;
    public int value;
}

[System.Serializable]
public enum TypeMine
{
    Free = 0,
    Town = 1,
}