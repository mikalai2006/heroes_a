using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine;

[CreateAssetMenu(fileName = "DwellingEffect", menuName = "Game/Effect/EffectDwelling")]
public class EffectDwelling : BaseEffect, IEffected
{
    // [Space(10)]
    // [Header("Options Effect")]
    // public TypeWorkObject TypeWorkObject;

    public override void RunHero(Player player, BaseEntity entity)
    {
        // base.RunHero(player, entity);

        var _enity = (EntityDwelling)entity;
        var configData = (ScriptableEntityDwelling)_enity.ConfigData;

        Debug.Log($"EffectDwelling run {_enity.Data.level}-{configData.name}!");
    }
}
