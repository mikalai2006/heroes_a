using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "HallEffect", menuName = "Game/Effect/EffectHall")]
public class EffectHall : BaseEffect
{
    public int goldin;
    public override void RunOne(ref Player player, BaseEntity entity)
    {
        // base.RunOne(ref player, entity);

        var res = ((EntityTown)entity).Data.Resources.GetValueOrDefault(TypeResource.Gold);
        res = goldin;
        Debug.Log($"Run EffectHall::: value={goldin}");
    }
}
