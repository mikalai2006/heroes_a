using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "ShowSkyEffect", menuName = "Game/Effect/EffectShowSky")]
public class EffectShowSky : BaseEffect, IEffected
{
    [Space(10)]
    [Header("Options Effect")]
    public int countShowCell;

    public override void OnDoHero(ref Player player, BaseEntity entity)
    {
        base.OnDoHero(ref player, entity);
        EntityMonolith monolith = (EntityMonolith)entity;
        Vector3Int pointToTeleport
            = monolith.Data.portalPoints[Random.Range(0, monolith.Data.portalPoints.Count)];
        player.ActiveHero.SetPositionHero(pointToTeleport);
        Debug.Log("EffectShowSky run!");
    }
}
