using UnityEngine;

[CreateAssetMenu(fileName = "WarMachineEffect", menuName = "Game/Effect/EffectWarMachine")]
public class EffectWarMachine : BaseEffect, IEffected
{
    public TypeWarMachine TypeWarMachine;
    public int count;

    public override void OnDoHero(ref Player player, BaseEntity entity)
    {
        base.OnDoHero(ref player, entity);
        // EntityMonolith monolith = (EntityMonolith)entity;
        // Vector3Int pointToTeleport
        //     = monolith.Data.portalPoints[Random.Range(0, monolith.Data.portalPoints.Count)];
        // player.ActiveHero.SetPositionHero(pointToTeleport);
        Debug.Log("EffectWarMachine run!");
    }

    public override void OnAddEffect(ref Player player, BaseEntity entity)
    {
        base.OnAddEffect(ref player, entity);

        // ((EntityTown)entity).Data.koofcreature = koofCreature;
        Debug.Log($"Run EffectWarMachine::: {((EntityTown)entity).Data.goldin}");
    }
}

[System.Serializable]
public enum TypeWarMachine
{
    AmmoCart = 1,
    Ballista = 2,
    FirstAidTent = 3,
    Catapult = 4,
}