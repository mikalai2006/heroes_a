using Cysharp.Threading.Tasks;

using UnityEngine;

[CreateAssetMenu(fileName = "TeleportEffect", menuName = "Game/Effect/EffectTeleport")]
public class EffectTeleport : BaseEffect
{
    public async override UniTask<EffectResult> RunHero(Player player, BaseEntity entity)
    {
        var result = new EffectResult();

        EntityMonolith monolith = (EntityMonolith)entity;
        Vector3Int pointToTeleport
            = monolith.Data.portalPoints[Random.Range(0, monolith.Data.portalPoints.Count)];
        player.ActiveHero.FastMoveHero(pointToTeleport);
        Debug.Log("EffectTeleport::: Run!");

        await UniTask.Delay(1);
        return result;
    }
}
