using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "NewEntityMapObject", menuName = "Game/Entity/MapObject")]
public class ScriptableEntityMapObject : ScriptableEntityEffect, IEffected
{
    // public DialogText DialogText;
    public TypeMapObject TypeMapObject;
    public List<TypeNoPath> listTypeNoPath;
    public List<TypeNoPath> RulesDraw => listTypeNoPath;
    public List<TypeNoPath> listRuleInput;
    public List<TypeNoPath> RulesInput => listRuleInput;
    public TypeWorkObject TypeWorkObject;
    public List<ItemProbabiliti<ItemEffect>> Effects;

    public virtual void RunHero(ref Player player, BaseEntity entity)
    {
        if (Effects.Count == 0) return;

        foreach (var effect in Effects[entity.DataEffects.index].Item.items)
        {
            effect.RunHero(ref player, entity);
        }
    }
    public virtual void SetData(BaseEntity entity)
    {
        if (Effects.Count == 0) return;

        var variant = Helpers.GetProbabilityItem<ItemEffect>(Effects);
        entity.DataEffects.index = variant.index;
        foreach (var effect in variant.Item.items)
        {
            effect.SetData(entity);
        }
    }

}

[System.Serializable]
public struct ItemEffect
{
    public LocalizedString description;
    public List<BaseEffect> items;
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
