using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

[CreateAssetMenu(fileName = "NewEntityDwelling", menuName = "Game/Entity/Dwelling")]
public class ScriptableEntityDwelling : ScriptableEntityMapObject, IEffected
{
    [Header("Options Dwelling")]
    public TypeFaction TypeFaction;
    // [SerializeField] public List<CostEntity> CostResource;
    [SerializeField] public List<ScriptableAttributeCreature> Creature;
    // public override UniTask RunHero(Player player, BaseEntity entity)
    // {
    //     base.RunHero(player, entity);
    //     // foreach (var perk in Perks)
    //     // {
    //     //     perk.OnDoHero(ref player, entity);
    //     // }
    // }
}
