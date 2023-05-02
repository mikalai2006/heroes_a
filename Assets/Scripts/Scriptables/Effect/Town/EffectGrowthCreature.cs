using System.Linq;

using UnityEngine;

[CreateAssetMenu(fileName = "GrowthCreatureEffect", menuName = "Game/Effect/EffectGrowthCreature")]
public class EffectGrowthCreature : BaseEffect
{
    [Range(1, 7)] public int levelArmy;
    public int growth;
    public override void RunOne(ref Player player, BaseEntity entity)
    {
        // base.RunOne(ref player, entity);

        var ArmyBuild = ((EntityTown)entity).Data.Armys
            .Where(t =>
                t.Value != null
                && ((ScriptableEntityDwelling)t.Value.Dwelling.ScriptableData).Creature[t.Value.level].CreatureParams.Level == levelArmy
                )
            .FirstOrDefault()
            .Value;
        if (ArmyBuild != null)
        {
            ArmyBuild.Dwelling.Data.dopGrowth = growth;
        }
    }
}
