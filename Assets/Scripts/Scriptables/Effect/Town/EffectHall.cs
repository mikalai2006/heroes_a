using UnityEngine;

[CreateAssetMenu(fileName = "HallEffect", menuName = "Game/Effect/EffectHall")]
public class EffectHall : BaseEffect
{
    public int goldin;
    public override void RunOne(ref Player player, BaseEntity entity)
    {
        // base.RunOne(ref player, entity);

        ((EntityTown)entity).Data.goldin = goldin;
        Debug.Log($"Run EffectHall::: {((EntityTown)entity).Data.goldin}");
    }
}
