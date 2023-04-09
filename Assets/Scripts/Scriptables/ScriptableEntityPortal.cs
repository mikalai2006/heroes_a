using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityPortal", menuName = "Game/Entity/MapObject/Portal")]
public class ScriptableEntityPortal : ScriptableEntityMapObject, IEffected
{
    [Space(10)]
    [Header("Options Effect")]
    public List<BaseEffect> Perks;

    public override void OnDoHero(ref Player player, BaseEntity entity)
    {
        base.OnDoHero(ref player, entity);
        foreach (var perk in Perks)
        {
            perk.OnDoHero(ref player, entity);
        }
    }
}
