using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "CastleEffect", menuName = "Game/Effect/EffectCastle")]
public class EffectCastle : BaseEffect, IEffected
{
    [Range(0, 100)] public int koofCreature;

    public override void OnDoHero(ref Player player, BaseEntity entity)
    {
        base.OnDoHero(ref player, entity);
        // EntityMonolith monolith = (EntityMonolith)entity;
        // Vector3Int pointToTeleport
        //     = monolith.Data.portalPoints[Random.Range(0, monolith.Data.portalPoints.Count)];
        // player.ActiveHero.SetPositionHero(pointToTeleport);
        Debug.Log("EffectCastle run!");
    }

    public override void OnAddEffect(ref Player player, BaseEntity entity)
    {
        base.OnAddEffect(ref player, entity);

        ((EntityTown)entity).Data.koofcreature = koofCreature;
        Debug.Log($"Run EffectCastle::: {((EntityTown)entity).Data.goldin}");
    }
}
