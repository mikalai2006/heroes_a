using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityExplore", menuName = "Game/Entity/Explore")]
public class ScriptableEntityExplore : ScriptableEntityMapObject, IEffected
{
    // [Space(10)]
    // [Header("Options Perk")]
    // public List<BaseEffect> Perks;

    // public override void RunHero(Player player, BaseEntity entity)
    // {
    //     base.RunHero(player, entity);
    //     foreach (var perk in Perks)
    //     {
    //         perk.RunHero(player, entity);
    //     }
    // }
}
