using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityExplore", menuName = "Game/Entity/Explore")]
public class ScriptableEntityExplore : ScriptableEntity, IEffected
{
    [Space(10)]
    [Header("Options Perk")]
    public List<BaseEffect> Perks;

    public void OnDoHero(ref Player player, BaseEntity entity)
    {
        foreach (var perk in Perks)
        {
            perk.OnDoHero(ref player, entity);
        }
    }
}
