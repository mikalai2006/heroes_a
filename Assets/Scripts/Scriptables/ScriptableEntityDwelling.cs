using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewEntityDwelling", menuName = "Game/Entity/MapObject/Dwelling")]
public class ScriptableEntityDwelling : ScriptableEntityMapObject, IEffected
{
    [Header("Options Dwelling")]
    public TypeFaction TypeFaction;
    // [SerializeField] public List<CostEntity> CostResource;
    [SerializeField] public List<ScriptableEntityCreature> Creature;
    public override void RunHero(ref Player player, BaseEntity entity)
    {
        base.RunHero(ref player, entity);
        // foreach (var perk in Perks)
        // {
        //     perk.OnDoHero(ref player, entity);
        // }
    }
}
