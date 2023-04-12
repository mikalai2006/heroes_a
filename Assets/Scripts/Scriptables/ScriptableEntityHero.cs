using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityHero", menuName = "Game/Entity/Hero")]
public class ScriptableEntityHero : ScriptableEntityMapObject
{
    [Header("Options Hero")]
    public TypeFaction TypeFaction;
    public List<ItemPrimarySkill> PrimarySkill;
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