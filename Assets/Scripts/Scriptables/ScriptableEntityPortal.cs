using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityPortal", menuName = "Game/Entity/Portal")]
public class ScriptableEntityPortal : ScriptableEntity, IEffected
{
    [Space(10)]
    [Header("Options Effect")]
    public List<BaseEffect> Perks;

    public void OnDoHero(ref Player player, BaseEntity entity)
    {
        foreach (var perk in Perks)
        {
            perk.OnDoHero(ref player, entity);
        }
    }
}
