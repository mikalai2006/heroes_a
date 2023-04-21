using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityMapObject", menuName = "Game/Entity/MapObject")]
public class ScriptableEntityMapObject : ScriptableEntityEffect, IEffected
{
    [Header("Options Map Object")]
    public List<TypeNoPath> listTypeNoPath;
    public List<TypeNoPath> RulesDraw => listTypeNoPath;
    public TypeWorkObject TypeWorkObject;
    public TypeMapObject TypeMapObject;
    public List<TypeNoPath> listRuleInput;
    public List<TypeNoPath> RulesInput => listRuleInput;
    public List<BaseEffect> Effects;

    public virtual void RunHero(ref Player player, BaseEntity entity)
    {
        foreach (var perk in Effects)
        {
            perk.RunHero(ref player, entity);
        }
    }
}
[System.Serializable]
public enum TypeMapObject
{
    Explore = 1,
    Resources = 2,
    Portal = 3,
    Skills = 4,
    Mine = 5,
    Artifact = 6,
    Dwelling = 7,
}
[System.Serializable]
public enum TypeWorkObject
{
    One = 1,
    EveryDay = 2,
    EveryWeek = 3,
    EveryMonth = 4,
    FirstVisit = 5,
}
