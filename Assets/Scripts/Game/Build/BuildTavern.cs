using UnityEngine;

public class BuildTavern : BuildGeneralBase
{
    public DataBuildGeneral Data = new DataBuildGeneral();

    public BuildTavern(
        int level,
        ScriptableBuilding configData,
        EntityTown town,
        Player player,
        SaveDataBuild<DataBuildGeneral> saveData = null
        )
    {
        base.Init(level, town, player);

        if (saveData == null)
        {
            ConfigData = configData;
            // OnRunEffects();
        }
        else
        {
            ConfigData = configData;
            Data = saveData.data;
        }
    }

    // private void BuyHero()
    // {
    //     _player.GenerateHeroForTavern();
    // }

    // private void GenerateHeroForTavern()
    // {
    //     var player = _player;
    //     if (player == null) return;
    //     if (LevelManager.Instance.ActivePlayer == player)
    //     {
    //         Debug.Log($"GenerateHeroForTavern {player.DataPlayer.id}[{player.DataPlayer.HeroesInTavern.Count}]");

    //         var heroesInTavern = new List<string>();
    //         if (player.DataPlayer.HeroesInTavern.Count == 1)
    //         {
    //             heroesInTavern.Concat(player.DataPlayer.HeroesInTavern);
    //         }

    //         var typeFaction = player.DataPlayer.typeFaction;
    //         var allHero = ResourceSystem.Instance
    //             .GetEntityByType<ScriptableEntityHero>(TypeEntity.Hero)
    //             .Where(t =>
    //                 !UnitManager.IdsExistsHeroes.Contains(t.idObject)
    //             )
    //             .OrderBy(t => UnityEngine.Random.value)
    //             .ToList();
    //         if (allHero.Count == 0) return;

    //         // Generate hero native faction.
    //         var isFactionHeroes = allHero
    //             .Where(t => t.TypeFaction == typeFaction)
    //             .ToList();
    //         if (isFactionHeroes.Count > 0)
    //         {
    //             for (int i = 0; i < 2; i++)
    //             {
    //                 heroesInTavern.Add(isFactionHeroes[i].idObject);
    //                 UnitManager.IdsExistsHeroes.Add(isFactionHeroes[i].idObject);
    //             }
    //         }
    //         if (heroesInTavern.Count == 2)
    //         {
    //             player.DataPlayer.HeroesInTavern = heroesInTavern;
    //             return;
    //         }
    //         else
    //         {
    //             var isNotFactionHeroes = allHero
    //             .Where(t => t.TypeFaction != typeFaction)
    //             .ToList();
    //             for (int i = heroesInTavern.Count; i < 2; i++)
    //             {
    //                 heroesInTavern.Add(isNotFactionHeroes[i].idObject);
    //                 UnitManager.IdsExistsHeroes.Add(isNotFactionHeroes[i].idObject);
    //             }
    //         }

    //         player.DataPlayer.HeroesInTavern = heroesInTavern;
    //     }
    // }

    // #region Event GameManager
    // public override void OnAfterStateChanged(GameState newState)
    // {
    //     base.OnAfterStateChanged(newState);
    //     if (Player == LevelManager.Instance.ActivePlayer)
    //     {
    //         switch (newState)
    //         {
    //             case GameState.NextDay:
    //                 OnNextDay();
    //                 break;
    //             case GameState.NextWeek:
    //                 OnNextWeek();
    //                 break;
    //         }
    //     }
    // }
    // private void OnNextWeek()
    // {
    //     Debug.Log($"Tavern::: Next week - {ConfigData.name}");
    // }
    // private void OnNextDay()
    // {

    //     Debug.Log($"Tavern::: Next day - {ConfigData.name}");

    // }
    // #endregion
}