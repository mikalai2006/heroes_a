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
    public int command;
    // public TypePlayerItem playerType2;
    // public string name;
    // public int idArea;
    public TypeFaction typeFaction = TypeFaction.Neutral;
    public SerializableDictionary<TypeResource, int> Resource;
    // public SerializableShortPosition nosky = new SerializableShortPosition();
    [System.NonSerialized] public PlayerDataReferences PlayerDataReferences;
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
        // TypePlayerItem = new CurrentPlayerType();
    }
}

[System.Serializable]
public class CurrentPlayerType
{
    public string title;
    public PlayerType TypePlayer;
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

    public Player()
    {
        _data = new PlayerData();
        StartSetting = new StartSetting();
        _data.PlayerDataReferences = new PlayerDataReferences();
    }

    public void New(PlayerData data)
    {
        Load(data);
        // Set resources.
        _data.Resource = new SerializableDictionary<TypeResource, int>();
        var gameSetting = LevelManager.Instance.ConfigGameSettings;
        var resourceOfComplexityGame
            = gameSetting.Complexities
            .Find(t => t.value == LevelManager.Instance.Level.Settings.compexity);
        // (TypeResource[])Enum.GetValues(typeof(TypeResource))
        var resources = resourceOfComplexityGame.Player.Resources;
        if (_data.playerType == PlayerType.Bot)
        {
            resources = resourceOfComplexityGame.Computer.Resources;
        }
        foreach (var res in resources)
        {
            _data.Resource.Add(res.Resource.TypeResource, res.value);
        }

        // Add events.
        AddEvents();
    }

    public void Load(PlayerData data)
    {
        _data = data;
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
            case GameState.NextWeek:
                GenerateHeroForTavern();
                break;
                // case GameState.StartMoveHero:
                //     await ActiveHero.StartMove();
                //     break;
        }
    }
    #endregion

    public void SetActiveHero(EntityHero entityHero)
    {
        _data.PlayerDataReferences.ActiveHero = entityHero;
    }

    public void SetActiveTown(EntityTown entityTown)
    {
        _data.PlayerDataReferences.ActiveTown = entityTown;
    }

    public void AddHero(EntityHero hero)
    {
        // hero.SetPlayer(DataPlayer);
        DataPlayer.PlayerDataReferences.ListHero.Add(hero);
    }

    public void SetNosky(List<GridTileNode> listNode)
    {
        var val = ((NoskyMask)(1 << LevelManager.Instance.ActivePlayer.DataPlayer.id));
        for (int i = 0; i < listNode.Count; i++)
        {
            var nosky = LevelManager.Instance.Level.nosky;
            if (!nosky.ContainsKey(listNode[i].position))
            {
                nosky.Add(listNode[i].position, val);
            }
            else
            {
                nosky[listNode[i].position] |= val;
            }
        }
        // for (int i = 0; i < listNode.Count; i++)
        // {
        //     if (!_data.nosky.ContainsKey(listNode[i].position))
        //     {
        //         _data.nosky.Add(listNode[i].position, true);
        //     }
        // }
    }

    public bool GetOpenSkyByNode(Vector3Int position)
    {
        var node = GameManager.Instance.MapManager.GridTileHelper().GetNode(position);
        var flag = ((NoskyMask)(1 << DataPlayer.id));
        var nosky = LevelManager.Instance.Level.nosky;

        return nosky.ContainsKey(node.position) && nosky[node.position].HasFlag(flag);
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
            ActiveTown.MapObject.OccupiedNode,
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
            // ScriptableEntityTown townConfig = (ScriptableEntityTown)((EntityTown)town).ConfigData;
            _data.typeFaction = ((EntityTown)town).ConfigData.TypeFaction;
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
            hero.SetHeroAsActive();
            await hero.AIFind();
        }

        // AI Make building.
        foreach (var town in _data.PlayerDataReferences.ListTown)
        {
            var allCurrentLevelsBuilding = town.GetLisNextLevelBuilds(town.ConfigData);
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
        GameManager.Instance.ChangeState(GameState.NextPlayer);
    }

    public List<GridTileNode> AIPath()
    {
        List<GridTileNode> result = new List<GridTileNode>();



        return result;
    }
}
