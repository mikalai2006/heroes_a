using System.Collections.Generic;
using System.Threading.Tasks;

using Cysharp.Threading.Tasks;

using UnityEngine;

[CreateAssetMenu(fileName = "DwellingEffect", menuName = "Game/Effect/EffectDwelling")]
public class EffectDwelling : BaseEffect
{
    // [Space(10)]
    // [Header("Options Effect")]
    // public TypeWorkObject TypeWorkObject;

    public async override UniTask<EffectResult> RunHero(Player player, BaseEntity entity)
    {
        var result = new EffectResult();

        var _enity = (EntityDwelling)entity;
        var configData = (ScriptableEntityDwelling)_enity.ConfigData;

        Debug.Log($"EffectDwelling run {_enity.Data.level}-{configData.name}!");

        await UniTask.Delay(1);
        return result;
    }
}
