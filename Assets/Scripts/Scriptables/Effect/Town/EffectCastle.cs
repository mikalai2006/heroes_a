using UnityEngine;

[CreateAssetMenu(fileName = "CastleEffect", menuName = "Game/Effect/EffectCastle")]
public class EffectCastle : BaseEffect
{
    [Range(0, 100)] public int koofCreature;
    // public int level;

    public override void RunOne(ref Player player, BaseEntity entity)
    {
        // base.RunOne(ref player, entity);

        ((EntityTown)entity).Data.koofcreature = koofCreature;
        // ((EntityTown)entity).Data.level = level;

        Debug.Log($"Run EffectCastle::: koofCreature={koofCreature}");
    }
}
