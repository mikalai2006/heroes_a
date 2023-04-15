using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "HallEffect", menuName = "Game/Effect/EffectHall")]
public class EffectHall : BaseEffect, IEffected
{
    public int goldin;

    public override void OnDoHero(ref Player player, BaseEntity entity)
    {
        base.OnDoHero(ref player, entity);
        // EntityMonolith monolith = (EntityMonolith)entity;
        // Vector3Int pointToTeleport
        //     = monolith.Data.portalPoints[Random.Range(0, monolith.Data.portalPoints.Count)];
        // player.ActiveHero.SetPositionHero(pointToTeleport);
        Debug.Log("EffectHall run!");
    }

    public override void OnAddEffect(ref Player player, BaseEntity entity)
    {
        base.OnAddEffect(ref player, entity);

        ((EntityTown)entity).Data.goldin = goldin;
        Debug.Log($"Run EffectHall::: {((EntityTown)entity).Data.goldin}");
    }
}
