using System.Threading.Tasks;

using Cysharp.Threading.Tasks;

using UnityEngine;

[CreateAssetMenu(fileName = "ShowSkyEffect", menuName = "Game/Effect/EffectShowSky")]
public class EffectShowSky : BaseEffect
{
    [Space(10)]
    [Header("Options Effect")]
    public int countShowCell;

    public async override UniTask<EffectResult> RunHero(Player player, BaseEntity entity)
    {
        var result = new EffectResult();
        // base.RunHero(player, entity);
        // EntityMonolith monolith = (EntityMonolith)entity;
        // Vector3Int pointToTeleport
        //     = monolith.Data.portalPoints[Random.Range(0, monolith.Data.portalPoints.Count)];
        // player.ActiveHero.SetPositionHero(pointToTeleport);
        Debug.Log("EffectShowSky::: Run!");

        await UniTask.Delay(1);
        return result;
    }
}
