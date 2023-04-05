using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityHero", menuName = "Game/Entity/New Hero")]
public class ScriptableEntityHero : ScriptableEntity
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