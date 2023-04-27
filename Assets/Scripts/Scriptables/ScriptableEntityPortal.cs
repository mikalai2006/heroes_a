using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityPortal", menuName = "Game/Entity/Portal")]
public class ScriptableEntityPortal : ScriptableEntityMapObject, IEffected
{
    // [Space(10)]
    // [Header("Options Effect")]
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
