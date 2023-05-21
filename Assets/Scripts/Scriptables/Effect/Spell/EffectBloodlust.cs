using System.Collections;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine;

[CreateAssetMenu(fileName = "EffectBless", menuName = "Game/EffectSpell/Bloodlust")]
public class EffectBloodlust : BaseEffectSpell
{
    public override UniTask AddEffect(ArenaEntity entity, Player player = null)
    {
        entity.Data.damageMin = entity.Data.damageMax;
        Debug.Log($"Set damage for {entity.Entity.ScriptableDataAttribute.name} to {entity.Data.damageMin}");
        return base.AddEffect(entity, player);
    }
}
