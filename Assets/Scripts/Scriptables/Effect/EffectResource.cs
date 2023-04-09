using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "ResourceEffect", menuName = "Game/Effect/EffectResource")]
public class EffectResource : BaseEffect, IEffected
{
    // [Space(10)]
    // [Header("Options Perk")]

    public override void OnDoHero(ref Player player, BaseEntity entity)
    {
        base.OnDoHero(ref player, entity);
        EntityMonolith monolith = (EntityMonolith)entity;
        Vector3Int pointToTeleport
            = monolith.Data.portalPoints[Random.Range(0, monolith.Data.portalPoints.Count)];
        player.ActiveHero.SetPositionHero(pointToTeleport);
        Debug.Log("EffectResource run!");
    }
}
