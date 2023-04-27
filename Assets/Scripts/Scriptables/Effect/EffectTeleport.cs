using Cysharp.Threading.Tasks;

using UnityEngine;

[CreateAssetMenu(fileName = "TeleportEffect", menuName = "Game/Effect/EffectTeleport")]
public class EffectTeleport : BaseEffect, IEffected
{
    public override void RunHero(Player player, BaseEntity entity)
    {
        // base.RunHero(player, entity);

        EntityMonolith monolith = (EntityMonolith)entity;
        Vector3Int pointToTeleport
            = monolith.Data.portalPoints[Random.Range(0, monolith.Data.portalPoints.Count)];
        player.ActiveHero.SetPositionHero(pointToTeleport);
        Debug.Log("EffectTeleport::: Run!");
    }
}
