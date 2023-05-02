using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "HallEffect", menuName = "Game/Effect/EffectHall")]
public class EffectHall : BaseEffect
{
    public int goldin;
    public override void RunOne(ref Player player, BaseEntity entity)
    {
        // base.RunOne(ref player, entity);

        var resources = ((EntityTown)entity).Data.Resources;
        if (resources.ContainsKey(TypeResource.Gold))
        {
            resources[TypeResource.Gold] = goldin;
        }
        else
        {
            resources.Add(TypeResource.Gold, goldin);
        }
    }
}
