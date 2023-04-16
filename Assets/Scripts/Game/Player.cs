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
    public int idArea;
    public TypeFaction typeFaction = TypeFaction.Neutral;
    public SerializableDictionary<TypeResource, int> Resource;
    public SerializableShortPosition nosky = new SerializableShortPosition();
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

public enum PlayerType
{
    User = 0,
    Enemy = 1,
    Bot = 2,
}

[System.Serializable]
public class Player
{
    [SerializeField] private PlayerData _data;
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

    public Player(PlayerData data)
    {
        _data = new PlayerData();
        _data = data;
        _data.PlayerDataReferences = new PlayerDataReferences();
        _data.Resource = new SerializableDictionary<TypeResource, int>();
        foreach (TypeResource typeResource in (TypeResource[])Enum.GetValues(typeof(TypeResource)))
        {
            _data.Resource.Add(typeResource, typeResource == TypeResource.Gold ? 100000 : 200);
        }
    }

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

    public void GenerateHeroTavern()
    {
        _data.HeroesInTavern = new List<string>();

        var allHero = ResourceSystem.Instance
            .GetEntityByType<ScriptableEntityHero>(TypeEntity.Hero)
            .Where(t => !UnitManager.Entities.ContainsKey(t.idObject))
            .OrderBy(t => UnityEngine.Random.value)
            .ToList();
        var myFactionHero = allHero.Where(t => t.TypeFaction == _data.typeFaction);

        if (allHero.Count == 0) return;

        if (allHero.Count > 0)
        {
            if (myFactionHero.Count() > 0)
            {
                var firstHero = allHero
                    .Where(t => t.TypeFaction == _data.typeFaction)
                    .OrderBy(t => UnityEngine.Random.value)
                    .ToList();
                _data.HeroesInTavern.Add(firstHero[0].idObject);
            }
            else
            {
                _data.HeroesInTavern.Add(allHero[0].idObject);
            }
            if (allHero.Count > 1) _data.HeroesInTavern.Add(allHero[1].idObject);
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
        Debug.Log($"Bot::: Start - {this.DataPlayer.id}");
        var countProbePath = 10;

        foreach (var hero in _data.PlayerDataReferences.ListHero)
        {
            SetActiveHero(hero);
            Debug.Log($"Bot::: Set active hero {hero.ScriptableData.name}");
            var countProbe = 0;
            while (hero.Data.hit > 0 && countProbe < countProbePath)
            {
                if (hero.Data.path.Count == 0)
                {
                    var potentialPoints = GameManager.Instance
                        .MapManager
                        .gridTileHelper
                        .GetNeighboursAtDistance(hero.OccupiedNode, 25, true)
                        .Where(t =>
                            t.StateNode.HasFlag(StateNode.Occupied)
                            && !t.StateNode.HasFlag(StateNode.Guested)
                            // && !t.StateNode.HasFlag(StateNode.Protected)
                            && !t.StateNode.HasFlag(StateNode.Empty)
                            && (t.OccupiedUnit != null && t.OccupiedUnit.Player != this)
                            )
                        .OrderBy(t => GameManager.Instance
                            .MapManager
                            .gridTileHelper
                            .GetDistanceBetweeenPoints(hero.Position, t.position)
                        )
                        .ToList();
                    // .IsExistExit(hero.OccupiedNode, (StateNode.Occupied | ~StateNode.Guested));
                    // .MapManager
                    // .gridTileHelper
                    // .GetNeighboursAtDistance(hero.OccupiedNode, 10)
                    // .Where(t => !t.StateNode.HasFlag(StateNode.Disable))
                    // .ToList();
                    // var path = GameManager.Instance
                    //     .MapManager
                    //     .gridTileHelper
                    //     .FindPath(
                    //         hero.OccupiedNode.position,
                    //         potentialPoints[Random.Range(0, potentialPoints.Count)].position,
                    //         true
                    //         );
                    // hero.SetPathHero(path);
                    Debug.Log($"Bot::: Find path [{hero.OccupiedNode.position}]");
                    var path = hero.FindPathForHero(potentialPoints[0].position, true);
                    // Debug.Log($"Bot::: Move from [{hero.OccupiedNode.ToString()}] to node {path[path.Count - 1].ToString()}");
                    countProbe++;
                }

                await hero.StartMove();
            }
            Debug.Log($"Bot::: End Move hero {hero.ScriptableData.name}[hit={hero.Data.hit}]");
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
                Debug.Log($"Bot::: Builded - {allowNextBuild.Key.name}");
                var newBuild = town.CreateBuild(allowNextBuild.Key, allowNextBuild.Value);

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

        Debug.Log($"Bot::: Finish - {this.DataPlayer.id}");
        GameManager.Instance.ChangeState(GameState.StepNextPlayer);
    }

    public List<GridTileNode> AIPath()
    {
        List<GridTileNode> result = new List<GridTileNode>();



        return result;
    }

}
