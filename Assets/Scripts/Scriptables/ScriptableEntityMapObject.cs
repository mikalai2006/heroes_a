using System;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "NewEntityMapObject", menuName = "Game/Entity/MapObject")]
public class ScriptableEntityMapObject : ScriptableEntity, IEffected
{
    // public DialogText DialogText;
    public TypeMapObject TypeMapObject;
    public int RMGValue;
    public List<TypeNoPath> listTypeNoPath;
    public List<TypeNoPath> RulesDraw => listTypeNoPath;
    public List<TypeNoPath> listRuleInput;
    public List<TypeNoPath> RulesInput => listRuleInput;
    public TypeWorkObject TypeWorkObject;
    public TypeWorkAttribute TypeWorkEffect;
    public List<ItemProbabiliti<ItemEffect>> Effects;

    public async virtual UniTask RunHero(Player player, BaseEntity entity)
    {
        if (Effects.Count == 0) return;

        foreach (var effect in Effects[entity.Effects.index].Item.items)
        {
            await effect.RunHero(player, entity);
        }

        await UniTask.Delay(1);
    }
    public virtual void SetData(BaseEntity entity)
    {
        if (Effects.Count == 0) return;

        var variant = Helpers.GetProbabilityItem<ItemEffect>(Effects);
        entity.Effects.index = variant.index;
        foreach (var effect in variant.Item.items)
        {
            effect.SetData(entity);
        }
    }

}

[System.Serializable]
public enum TypeWorkAttribute
{
    All = 0,
    One = 1,
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
    Hero = 8,
    CreatureBanks = 9,
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
