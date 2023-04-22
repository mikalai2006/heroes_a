using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

using UnityEngine;
using Cysharp.Threading.Tasks;

[System.Serializable]
public class PlayerData
{
    public int id;
    public Color color;
    public PlayerType playerType;
    // public TypePlayerItem playerType2;
    // public string name;
    // public int idArea;
    public TypeFaction typeFaction = TypeFaction.Neutral;
    public SerializableDictionary<TypeResource, int> Resource;
    public SerializableShortPosition nosky = new SerializableShortPosition();
    public PlayerDataReferences PlayerDataReferences; // [System.NonSerialized] 
    public List<string> HeroesInTavern;

    // public Hero ActiveHero => PlayerDataReferences.ActiveHero;

    public PlayerData()
    {
        PlayerDataReferences = new PlayerDataReferences();
        HeroesInTavern = new List<string>();
    }
}

[System.Serializable]
public class PlayerDataReferences
{
    [SerializeField] public List<EntityHero> ListHero;
    [SerializeField] public List<EntityTown> ListTown;
    [SerializeField] public List<EntityMine> ListMines;
    [SerializeField] public EntityHero ActiveHero;
    [SerializeField] public EntityTown ActiveTown;

    public PlayerDataReferences()
    {
        ListTown = new List<EntityTown>();
        ListHero = new List<EntityHero>();
        ListMines = new List<EntityMine>();
    }
}

[System.Serializable]
public class StartSetting
{
    [SerializeField] public ScriptableEntityHero hero;
    [SerializeField] public ScriptableEntityTown town;
    public CurrentPlayerType TypePlayerItem;
    public TypeStartBonus bonus;

    public StartSetting()
    {
    }
}

[System.Serializable]
public class CurrentPlayerType
{
    public string title;
    public PlayerType TypePlayer;
}

[System.Serializable]
public enum TypeStartBonus
{
    None = 0,
    Gold = 1,
    Artifact = 2,
}

[System.Serializable]
public enum PlayerType
{
    User = 0,
    Enemy = 1,
    Bot = 2,
}

[System.Serializable]
public class Player
{
    [SerializeField] private PlayerData _data = new PlayerData();
    public StartSetting StartSetting;
    public PlayerData DataPlayer
    {
        get { return _data; }
        set
        {
            _data = value;

            // Event change data.

        }
    }
    public EntityHero ActiveHero => _data.PlayerDataReferences.ActiveHero;
    public EntityTown ActiveTown => _data.PlayerDataReferences.ActiveTown;
    public bool IsMaxCountHero => _data.PlayerDataReferences.ListHero
            .Where(t => t.Data.State.HasFlag(StateHero.OnMap))
            .Count() == LevelManager.Instance.ConfigGameSettings.maxCountHero;

    public Player(PlayerData data)
    {
        _data = new PlayerData();
        StartSetting = new StartSetting();
        _data = data;
        _data.PlayerDataReferences = new PlayerDataReferences();
        _data.Resource = new SerializableDictionary<TypeResource, int>();
        foreach (TypeResource typeResource in (TypeResource[])Enum.GetValues(typeof(TypeResource)))
        {
            _data.Resource.Add(typeResource, typeResource == TypeResource.Gold ? 100000 : 200);
        }

        AddEvents();
    }

    #region Events GameState
    public void AddEvents()
    {
        GameManager.OnBeforeStateChanged += OnBeforeStateChanged;
        GameManager.OnAfterStateChanged += OnAfterStateChanged;
    }
    public void RemoveEvents()
    {
        GameManager.OnBeforeStateChanged -= OnBeforeStateChanged;
        GameManager.OnAfterStateChanged -= OnAfterStateChanged;
    }

    public virtual void OnBeforeStateChanged(GameState newState)
    {
    }

    public virtual void OnAfterStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.NextDay:
                GenerateHeroForTavern();
                break;
        }
    }
    #endregion

    public void SetActiveHero(EntityHero entityHero)
    {
        // GameManager.Instance.ChangeState(GameState.ChooseHero);

        _data.PlayerDataReferences.ActiveHero = entityHero;
    }

    public void SetActiveTown(EntityTown entityTown)
    {
        // GameManager.Instance.ChangeState(GameState.ChooseHero);

        _data.PlayerDataReferences.ActiveTown = entityTown;

    }

    public void AddHero(EntityHero hero)
    {
        // hero.SetPlayer(DataPlayer);
        DataPlayer.PlayerDataReferences.ListHero.Add(hero);
    }

    public void SetNosky(List<GridTileNode> listNode)
    {
        for (int i = 0; i < listNode.Count; i++)
        {
            if (!_data.nosky.ContainsKey(listNode[i].position))
            {
                _data.nosky.Add(listNode[i].position, true);
            }
        }
    }

    public void AddMines(EntityMine mine)
    {
        // mine.SetPlayer(this);
        DataPlayer.PlayerDataReferences.ListMines.Add(mine);
    }
    public void ChangeResource(TypeResource typeResource, int value = 0)
    {
        _data.Resource[typeResource] += value;

        GameManager.Instance.ChangeState(GameState.ChangeResources);
    }

    public bool IsExistsResource(List<CostEntity> resources)
    {
        bool result = true;
        foreach (var res in resources)
        {
            if (_data.Resource[res.Resource.TypeResource] < res.Count)
            {
                result = false;
                break;
            }
        }
        return result;
    }

    public EntityHero GetHero(int id)
    {
        return DataPlayer.PlayerDataReferences.ListHero[id];
    }

    public void BuyHero(ScriptableEntityHero configData)
    {
        var newHero = UnitManager.CreateHero(
            TypeFaction.Neutral,
            ActiveTown.OccupiedNode,
            configData);
        newHero.SetPlayer(this);
        _data.HeroesInTavern.Remove(configData.idObject);
        foreach (var res in LevelManager.Instance.ConfigGameSettings.CostHero)
        {
            ChangeResource(res.Resource.TypeResource, -res.Count);
        }

        GenerateHeroForTavern();

    }

    private void GenerateHeroForTavern()
    {
        var player = this;
        if (player == null) return;
        if (LevelManager.Instance.ActivePlayer == player)
        {
            // Debug.Log($"GenerateHeroForTavern {player.DataPlayer.id}[{player.DataPlayer.HeroesInTavern.Count}]");

            var heroesInTavern = new List<string>();
            if (player.DataPlayer.HeroesInTavern.Count == 1)
            {
                heroesInTavern.Concat(player.DataPlayer.HeroesInTavern);
            }

            var typeFaction = player.DataPlayer.typeFaction;
            var allHero = ResourceSystem.Instance
                .GetEntityByType<ScriptableEntityHero>(TypeEntity.Hero)
                .Where(t =>
                    !UnitManager.IdsExistsHeroes.Contains(t.idObject)
                )
                .OrderBy(t => UnityEngine.Random.value)
                .ToList();
            if (allHero.Count == 0) return;

            // Generate hero native faction.
            var isFactionHeroes = allHero
                .Where(t => t.TypeFaction == typeFaction)
                .ToList();
            if (isFactionHeroes.Count > 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (isFactionHeroes.ElementAtOrDefault(i) != null)
                    {
                        heroesInTavern.Add(isFactionHeroes[i].idObject);
                        UnitManager.IdsExistsHeroes.Add(isFactionHeroes[i].idObject);
                    }
                }
            }
            if (heroesInTavern.Count == 2)
            {
                player.DataPlayer.HeroesInTavern = heroesInTavern;
                return;
            }
            else
            {
                var isNotFactionHeroes = allHero
                .Where(t => t.TypeFaction != typeFaction)
                .ToList();
                for (int i = heroesInTavern.Count; i < 2; i++)
                {
                    if (isNotFactionHeroes.ElementAtOrDefault(i) != null)
                    {
                        heroesInTavern.Add(isNotFactionHeroes[i].idObject);
                        UnitManager.IdsExistsHeroes.Add(isNotFactionHeroes[i].idObject);
                    }
                }
            }

            player.DataPlayer.HeroesInTavern = heroesInTavern;
        }
    }

    public void AddTown(BaseEntity town)
    {
        // EntityTown _town = (EntityTown)town;
        // _town.SetPlayer(DataPlayer);
        if (_data.typeFaction == TypeFaction.Neutral)
        {
            ScriptableEntityTown townConfig = (ScriptableEntityTown)town.ScriptableData;
            _data.typeFaction = townConfig.TypeFaction;
        }
        DataPlayer.PlayerDataReferences.ListTown.Add((EntityTown)town);

    }

    public EntityTown GetTown(int id)
    {
        return DataPlayer.PlayerDataReferences.ListTown[id];
    }

    public async UniTask RunBot()
    {
        // Debug.Log($"Bot::: Start - {this.DataPlayer.id}");

        foreach (var hero in _data.PlayerDataReferences.ListHero)
        {
            SetActiveHero(hero);
            await hero.AIFind();
            // Debug.Log($"Bot::: Set active hero {hero.ScriptableData.name}");
            // Debug.Log($"Bot::: End Move hero {hero.ScriptableData.name}[hit={hero.Data.hit}]");
            // GameManager.Instance.ChangeState(GameState.StartMoveHero);
        }

        // AI Make building.
        foreach (var town in _data.PlayerDataReferences.ListTown)
        {
            ScriptableEntityTown entityTown = (ScriptableEntityTown)town.ScriptableData;
            ScriptableBuildTown configBuildTown = (ScriptableBuildTown)entityTown.BuildTown;
            var allCurrentLevelsBuilding = town.GetLisNextLevelBuilds(configBuildTown);
            Dictionary<ScriptableBuilding, int> allowBulding = new Dictionary<ScriptableBuilding, int>();
            foreach (var parentBuild in allCurrentLevelsBuilding)
            {
                var indexNextBuild = 0;

                if (parentBuild.Value == parentBuild.Key.BuildLevels.Count - 1)
                {
                    continue;
                }
                else
                {
                    indexNextBuild = parentBuild.Value + 1;
                }
                allowBulding.Add(parentBuild.Key, indexNextBuild);
            }
            var potentialAllowNextBuild = allowBulding
                .Where(t => town.GetListNeedNoBuilds(t.Key.BuildLevels[t.Value].RequireBuilds).Count == 0
                    && IsExistsResource(t.Key.BuildLevels[t.Value].CostResource)
                );
            if (potentialAllowNextBuild.Count() > 0)
            {
                var allowNextBuild = potentialAllowNextBuild
                    .OrderBy(t => Random.value)
                    .First();
                // Debug.Log($"Bot::: Builded - {allowNextBuild.Key.name}");
                var newBuild = town.CreateBuild(allowNextBuild.Key, allowNextBuild.Value, this);

                var build = allowNextBuild.Key.BuildLevels[allowNextBuild.Value];
                for (int i = 0; i < build.CostResource.Count; i++)
                {
                    ChangeResource(
                        build.CostResource[i].Resource.TypeResource,
                        -build.CostResource[i].Count
                        );
                }
            }
        }

        // Debug.Log($"Bot::: Finish - {this.DataPlayer.id}");
        GameManager.Instance.ChangeState(GameState.NextDay);
    }

    public List<GridTileNode> AIPath()
    {
        List<GridTileNode> result = new List<GridTileNode>();



        return result;
    }

}
