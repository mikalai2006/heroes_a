using UnityEngine;

[CreateAssetMenu(fileName = "MageGuildEffect", menuName = "Game/Effect/Town/EffectMageGuild")]
public class EffectMageGuild : BaseEffect
{
    public override void RunEveryDay(Player player, BaseEntity entity)
    {
        EntityTown town = (EntityTown)entity;
        EntityHero heroInTown = town.HeroInTown;
        EntityHero heroGuest = town.HeroGuest;
        if (heroInTown != null) heroInTown.ChangeManaHero(heroInTown.GetMana());
        if (heroGuest != null) heroGuest.ChangeManaHero(heroGuest.GetMana());
        Debug.Log("EffectMageGuild run!");
    }

}
