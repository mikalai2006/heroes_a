using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "CastleEffect", menuName = "Game/Effect/EffectCastle")]
public class EffectCastle : BaseEffect
{
    [Range(0, 100)] public int koofCreature;

    public override void RunOne(ref Player player, BaseEntity entity)
    {
        // base.RunOne(ref player, entity);

        ((EntityTown)entity).Data.koofcreature = koofCreature;
        Debug.Log($"Run EffectCastle::: {((EntityTown)entity).Data.goldin}");
    }
}
