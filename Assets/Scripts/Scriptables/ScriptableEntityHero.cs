using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityHero", menuName = "Game/Entity/Hero")]
public class ScriptableEntityHero : ScriptableEntity
{
    [Header("Options Hero")]
    public TypeFaction TypeFaction;
    public SOClassHero ClassHero;
    // public List<ItemPrimarySkill> PrimarySkill;
    public List<LevelSecondarySkill> StartSecondarySkill;
    public List<StartCreatureItem> StartCreatures;

}

[System.Serializable]
public struct StartCreatureItem
{
    public ScriptableEntityCreature creature;
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