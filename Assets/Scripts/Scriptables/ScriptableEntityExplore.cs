using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityExplore", menuName = "Game/Entity/MapObject/Explore")]
public class ScriptableEntityExplore : ScriptableEntityMapObject, IEffected
{
    [Space(10)]
    [Header("Options Perk")]
    public List<BaseEffect> Perks;

    public override void RunHero(ref Player player, BaseEntity entity)
    {
        base.RunHero(ref player, entity);
        foreach (var perk in Perks)
        {
            perk.RunHero(ref player, entity);
        }
    }
}
