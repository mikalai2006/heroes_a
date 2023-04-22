using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "NewEntityMapObject", menuName = "Game/Entity/MapObject")]
public class ScriptableEntityMapObject : ScriptableEntityEffect, IEffected
{
    public DialogText DialogText;

    [Space(10)]
    [Header("Options Map Object")]
    public List<TypeNoPath> listTypeNoPath;
    public List<TypeNoPath> RulesDraw => listTypeNoPath;
    public List<TypeNoPath> listRuleInput;
    public List<TypeNoPath> RulesInput => listRuleInput;
    public TypeWorkObject TypeWorkObject;
    public TypeMapObject TypeMapObject;
    public List<ItemProbabiliti<BaseEffect>> Effects;
    public List<BaseEffect> Effects2;

    public virtual void RunHero(ref Player player, BaseEntity entity)
    {
        foreach (var effect in Effects2)
        {
            effect.RunHero(ref player, entity);
        }
    }
}

[System.Serializable]
public struct DialogText
{
    public LocalizedString VisitOk;
    public LocalizedString VisitNo;
    public LocalizedString VisitNoResource;
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
