using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "DwellingEffect", menuName = "Game/Effect/EffectDwelling")]
public class EffectDwelling : BaseEffect, IEffected
{
    // [Space(10)]
    // [Header("Options Effect")]
    // public TypeWorkObject TypeWorkObject;

    public override void RunHero(ref Player player, BaseEntity entity)
    {
        base.RunHero(ref player, entity);

        var _enity = (EntityDwelling)entity;
        var configData = (ScriptableEntityDwelling)entity.ScriptableData;

        Debug.Log($"EffectDwelling run {_enity.Data.level}-{configData.name}!");
    }
}
