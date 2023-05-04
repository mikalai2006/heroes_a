using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityHero", menuName = "Game/Entity/Hero")]
public class ScriptableEntityHero : ScriptableEntityMapObject, IEffected
{
    [Header("Options Hero")]
    public TypeFaction TypeFaction;
    public TypeGender TypeGender;
    public SOClassHero ClassHero;
    // public List<ItemPrimarySkill> PrimarySkill;
    public List<LevelSecondarySkill> StartSecondarySkill;
    public List<StartCreatureItem> StartCreatures;
    public List<ScriptableAttributeSpell> StartSpells;

    // public override UniTask RunHero(Player player, BaseEntity entity)
    // {
    //     // base.RunHero(player, entity);
    //     // foreach (var primarySkill in Skills)
    //     // {
    //     //     Debug.Log($"Increment hero skills {primarySkill.PrimarySkill.name}[{primarySkill.value}]");
    //     // }

    // }
}

[System.Serializable]
public enum TypeGender
{
    Male = 0,
    Female = 1,
}

[System.Serializable]
public struct StartCreatureItem
{
    public ScriptableAttributeCreature creature;
    public int min;
    public int max;
}

[System.Serializable]
public struct ItemPrimarySkill
{
    public ScriptableAttributePrimarySkill PrimarySkill;
    public int value;
}

[System.Serializable]
public struct LevelSecondarySkill
{
    public ScriptableAttributeSecondarySkill SecondarySkill;
    public int value;
}
