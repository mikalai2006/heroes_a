using System.Threading.Tasks;

using Cysharp.Threading.Tasks;

using UnityEngine;

[CreateAssetMenu(fileName = "WarMachineEffect", menuName = "Game/Effect/EffectWarMachine")]
public class EffectWarMachine : BaseEffect
{
    public TypeWarMachine TypeWarMachine;
    public int count;

    public async override UniTask<EffectResult> RunHero(Player player, BaseEntity entity)
    {
        var _processCompletionSource = new TaskCompletionSource<EffectResult>();
        // base.RunHero(player, entity);
        // EntityMonolith monolith = (EntityMonolith)entity;
        // Vector3Int pointToTeleport
        //     = monolith.Data.portalPoints[Random.Range(0, monolith.Data.portalPoints.Count)];
        // player.ActiveHero.SetPositionHero(pointToTeleport);
        Debug.Log("EffectWarMachine run!");

        return await _processCompletionSource.Task;
    }

    // public override void RunOne(ref Player player, BaseEntity entity)
    // {
    //     base.RunOne(ref player, entity);

    //     // ((EntityTown)entity).Data.koofcreature = koofCreature;
    //     Debug.Log($"Run EffectWarMachine::: {((EntityTown)entity).Data.goldin}");
    // }
}

