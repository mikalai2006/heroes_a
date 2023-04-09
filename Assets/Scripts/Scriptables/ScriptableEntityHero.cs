using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityHero", menuName = "Game/Entity/Hero")]
public class ScriptableEntityHero : ScriptableEntityMapObject
{
    [Header("Options Hero")]
    public TypeFaction TypeFaction;
    public List<ItemPrimarySkill> PrimarySkill;

}


[System.Serializable]
public struct ItemPrimarySkill
{
    public ScriptableAttributePrimarySkill PrimarySkill;
    public int value;
}