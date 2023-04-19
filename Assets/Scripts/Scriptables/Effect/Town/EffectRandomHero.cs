using System.Collections.Generic;
using System.Linq;

using UnityEngine;

[CreateAssetMenu(fileName = "EffectRandomHero", menuName = "Game/Effect/EffectRandomHero")]
public class EffectRandomHero : BaseEffect
{
    [Range(2, 2)] public int countHero;

    public override void RunOne(ref Player player, BaseEntity entity)
    {
        // base.RunOne(ref player, entity);

        GenerateHeroForTavern(ref player, entity);
    }
    public override void RunEveryWeek(ref Player player, BaseEntity entity)
    {
        // base.RunOne(ref player, entity);

        GenerateHeroForTavern(ref player, entity);
    }

    private void GenerateHeroForTavern(ref Player player, BaseEntity entity)
    {
        if (player == null) return;

        Debug.Log($"Run GenerateHeroForTavern {player.DataPlayer.ToString()}");
        var heroesInTavern = new List<string>();
        var typeFaction = player.DataPlayer.typeFaction;

        var allHero = ResourceSystem.Instance
            .GetEntityByType<ScriptableEntityHero>(TypeEntity.Hero)
            .Where(t =>
                !UnitManager.IdsExistsHeroes.Contains(t.idObject)
            )
            .OrderBy(t => UnityEngine.Random.value)
            .ToList();
        if (allHero.Count == 0) return;

        var isFactionHeroes = allHero
            .Where(t => t.TypeFaction == typeFaction)
            .ToList()
            .GetRange(0, 2);
        var isNotFactionHeroes = allHero
            .Where(t => t.TypeFaction != typeFaction)
            .ToList()
            .GetRange(0, 2);

        if (isFactionHeroes.Count() > 0)
        {
            foreach (var hero in isFactionHeroes)
            {
                heroesInTavern.Add(hero.idObject);
                UnitManager.IdsExistsHeroes.Add(hero.idObject);
            }
        }

        if (isNotFactionHeroes.Count() > 0)
        {
            foreach (var hero in isNotFactionHeroes)
            {
                heroesInTavern.Add(hero.idObject);
                UnitManager.IdsExistsHeroes.Add(hero.idObject);
            }
        }
        heroesInTavern.OrderBy(t => Random.value);
        // else
        // {
        //     _data.HeroesInTavern.Add(allHero[0].idObject);
        // }
        // if (allHero.Count > 1) _data.HeroesInTavern.Add(allHero[1].idObject);

        player.DataPlayer.HeroesInTavern = heroesInTavern;
    }
}
