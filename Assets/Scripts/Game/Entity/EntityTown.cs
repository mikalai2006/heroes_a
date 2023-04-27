using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

[Serializable]
public class EntityTown : BaseEntity
{
    [SerializeField] public DataTown Data = new DataTown();
    public ScriptableEntityTown ConfigData => (ScriptableEntityTown)ScriptableData;
    public EntityTown(
        TypeGround typeGround,
        ScriptableEntityTown configData = null,
        SaveDataUnit<DataTown> saveData = null
    )
    {
        base.Init();

        if (saveData == null)
        {

            if (configData == null)
            {
                List<ScriptableEntityTown> list = ResourceSystem.Instance
                    .GetEntityByType<ScriptableEntityTown>(TypeEntity.Town)
                    .Where(t => t.TypeGround == typeGround)
                    .ToList();
                ScriptableData = list[UnityEngine.Random.Range(0, list.Count)];
            }
            else
            {
                ScriptableData = configData;
            }

            Data.idPlayer = -1;
            Data.level = -1;
            Data.name = ConfigData.TownNames[UnityEngine.Random.Range(0, ConfigData.TownNames.Count)];
            _idObject = ScriptableData.idObject;

            Data.Generals = new SerializableDictionary<string, BuildGeneral>();
            Data.Armys = new SerializableDictionary<string, BuildArmy>();
            Data.Resources = new SerializableDictionary<TypeResource, int>();

            // Data.Creatures = new SerializableDictionary<int, EntityCreature>();
            // for (int i = 0; i < 7; i++)
            // {
            //     Data.Creatures.Add(i, null);
            // }
            ResetCreatures();
            Data.countBuild = 0;
        }
        else
        {
            ScriptableData = ResourceSystem.Instance
                .GetEntityByType<ScriptableEntityTown>(TypeEntity.Town)
                .Where(t => t.idObject == saveData.idObject)
                .First();
            Data = saveData.data;
            Data.Generals = new SerializableDictionary<string, BuildGeneral>();
            Data.Armys = new SerializableDictionary<string, BuildArmy>();
            foreach (var item in saveData.data.Generals)
            {
                var configDataBuild = ResourceSystem.Instance
                    .GetAllBuildsForTown()
                    .Where(t => t.idObject == item.Key)
                    .First();
                var newBuild = CreateBuild(configDataBuild, item.Value.level, _player);
            }
            foreach (var item in saveData.data.Armys)
            {
                var configDataBuild = ResourceSystem.Instance
                    .GetAllBuildsForTown()
                    .Where(t => t.idObject == item.Key)
                    .First();
                var newBuild = CreateBuild(configDataBuild, item.Value.level, _player);
            }

            var creatures = saveData.data.Creatures;
            ResetCreatures();
            // Data.Creatures = new SerializableDictionary<int, EntityCreature>();
            // for (int i = 0; i < 7; i++)
            // {
            //     Data.Creatures.Add(i, null);
            // }
            for (int i = 0; i < creatures.Count; i++)
            {
                var creature = creatures[i];
                EntityCreature newCreature = null;
                if (creature.Data.idObject != "")
                {
                    newCreature = new EntityCreature(null, new SaveDataUnit<DataCreature>()
                    {
                        data = creature.Data,
                        idObject = creature.Data.idObject,
                    });
                }
                Data.Creatures[i] = newCreature;
            }

            _idEntity = saveData.idEntity;
            _idObject = saveData.idObject;
            Data.countBuild = saveData.data.countBuild;

            // Data.HeroinTown = new EntityHero(TypeFaction.Neutral, new SaveDataUnit<DataHero>(){
            //     data = saveData.data.HeroinTown.Data,
            //     idObject = saveData.data.HeroinTown
            // });
        }

    }

    private void InitBuilding()
    {
        if (LevelManager.Instance.Level.Settings.compexity < 100 && _player.DataPlayer.playerType == PlayerType.Bot)
        {
            return;
        }

        foreach (var item in ConfigData.StartProgressBuilds)
        {
            var newBuild = CreateBuild(item.Build, item.level, _player);
            newBuild.OnRunOneEffect();
            // ((ScriptableBuilding)newBuild.ConfigData).BuildLevels[newBuild.level].RunOne(ref _player, this);
        }
    }

    public void ChangeLevel(int level)
    {
        Data.level = level;
        if (MapObject.MapObjectGameObject != null)
        {
            ((MapEntityTown)MapObject.MapObjectGameObject).RefreshLevelBuild();
        }
        else
        {
            Debug.Log("Not map object");
        }
    }

    public void ResetCreatures()
    {
        Data.Creatures = new SerializableDictionary<int, EntityCreature>();
        for (int i = 0; i < 7; i++)
        {
            Data.Creatures.Add(i, null);
        }
    }

    public void SetTownAsActive()
    {
        MapObject.SetPositionCamera(MapObject.Position);
        Player.SetActiveTown(this);
    }
    // public void SetPlayer(PlayerData data)
    // {
    //     //Debug.Log($"Town SetPlayer::: id{data.id}-idArea{data.idArea}");
    //     Data.idPlayer = data.id;

    //     Player player = LevelManager.Instance.GetPlayer(Data.idPlayer);

    //     MapEntityTown TownGameObject = (MapEntityTown)MapObjectGameObject;
    //     // TownGameObject.SetPlayer(player);
    // }

    public override void SetPlayer(Player player)
    {
        base.SetPlayer(player);
        var first = false;
        if (Data.idPlayer == -1)
        {
            first = true;
        }

        Data.idPlayer = player.DataPlayer.id;
        player.AddTown(this);

        if (first)
        {
            var resourceOfComplexityGame
                = LevelManager.Instance.ConfigGameSettings.Complexities
                .Find(t => t.value == LevelManager.Instance.Level.Settings.compexity);
            Data.Resources.Add(TypeResource.Gold, player.DataPlayer.playerType == PlayerType.Bot
                ? resourceOfComplexityGame.Computer.goldin
                : resourceOfComplexityGame.Player.goldin);

            InitBuilding();
        }
    }

    #region SaveLoadData
    // public void LoadDataPlay(DataPlay data)
    // {
    //     throw new System.NotImplementedException();
    // }
    public override void SaveEntity(ref DataPlay data)
    {
        var sdata = SaveUnit(Data);
        data.entity.towns.Add(sdata);
    }

    public BaseBuild CreateBuild(ScriptableBuilding buildConfig, int level, Player player)
    {
        if (buildConfig.TypeBuild == TypeBuild.Army)
        {
            BuildArmy build;
            if (Data.Armys.TryGetValue(buildConfig.idObject, out build))
            {
                Data.Armys[buildConfig.idObject].level += 1;
                // Data.Armys[buildConfig.idObject].OnRunEffects();
            }
            else
            {
                build = new BuildArmy(level, (ScriptableBuildingArmy)buildConfig, this, player);
                Data.Armys.Add(buildConfig.idObject, build);
            }

            Data.countBuild += 1;
            return build;
        }
        else
        {
            BuildGeneral build;
            if (Data.Generals.TryGetValue(buildConfig.idObject, out build))
            {
                Data.Generals[buildConfig.idObject].level += 1;
                Data.Generals[buildConfig.idObject].OnRunOneEffect();
                // Data.Generals[buildConfig.idObject].ConfigData.BuildLevels[level].RunOne(ref _player, this);
            }
            else
            {
                build = new BuildGeneral(level, (ScriptableBuildingGeneral)buildConfig, this, player);
                // new BuildGeneral(level, (ScriptableBuildingGeneral)buildConfig, this, player);
                build.OnRunOneEffect();
                Data.Generals.Add(buildConfig.idObject, build);
            }

            // Special effect.
            switch (buildConfig.TypeBuild)
            {
                case TypeBuild.Castle:
                    ChangeLevel(level);
                    break;
            }

            Data.countBuild += 1;
            return build;
        }
    }

    public List<Build> GetListNeedNoBuilds(List<BuildLevelItem> listRequire)
    {
        var result = new List<Build>();
        foreach (var item in listRequire)
        {
            if (item.Build.TypeBuild == TypeBuild.Army)
            {
                BuildArmy isArmy;
                Data.Armys.TryGetValue(item.Build.idObject, out isArmy);
                if (isArmy == null || (isArmy != null && isArmy.level < item.level))
                {
                    result.Add(((ScriptableBuildingArmy)item.Build).BuildLevels[item.level]);
                }
            }
            else
            {
                BuildGeneral isGen;
                Data.Generals.TryGetValue(item.Build.idObject, out isGen);
                if (isGen == null || (isGen != null && isGen.level < item.level))
                {
                    result.Add(((ScriptableBuildingGeneral)item.Build).BuildLevels[item.level]);
                }
            }
        }
        return result;
    }

    public Dictionary<ScriptableBuilding, int> GetLisNextLevelBuilds(ScriptableEntityTown configBuildTown)
    {
        var result = new Dictionary<ScriptableBuilding, int>();
        foreach (var parentBuild in configBuildTown.Builds)
        {

            var indexLevelBuild = GetLevelBuild(parentBuild);
            // Build currentBuild = parentBuild.BuildLevels[0];

            // if (indexLevelBuild != -1)
            // {
            //     if (indexLevelBuild < parentBuild.BuildLevels.Count - 1)
            //     {
            //         indexLevelBuild++;
            //     }
            // }
            // else
            // {
            //     indexLevelBuild = 0;
            // }

            result.Add(parentBuild, indexLevelBuild);
        }
        return result;
    }

    public int GetLevelBuild(ScriptableBuilding configBuildData)
    {
        var result = -1;
        if (configBuildData.TypeBuild == TypeBuild.Army)
        {
            BuildArmy isArmy;
            Data.Armys.TryGetValue(configBuildData.idObject, out isArmy);
            if (isArmy != null)
            {
                result = isArmy.level;
            }
        }
        else
        {
            BuildGeneral isGen;
            Data.Generals.TryGetValue(configBuildData.idObject, out isGen);
            if (isGen != null)
            {
                result = isGen.level;
            }
        }
        return result;
    }

    public override void OnAfterStateChanged(GameState newState)
    {
        base.OnAfterStateChanged(newState);
        // if (LevelManager.Instance.ActivePlayer == _player)
        // {
        switch (newState)
        {
            case GameState.NextWeek:
                NextWeek();
                break;
            case GameState.NextDay:
                NextDay();
                break;
        }
        // }
    }

    private void NextWeek()
    {
        if (Player == LevelManager.Instance.ActivePlayer)
        {
        };
    }
    private void NextDay()
    {
        if (Player == LevelManager.Instance.ActivePlayer)
        {
            // refresh count build.
            Data.countBuild = 0;

            // set resources.
            foreach (var res in Data.Resources)
            {
                _player.ChangeResource(TypeResource.Gold, res.Value);
            }
        };
    }
    // private void OnRunGeneralBuilds()
    // {
    //     if (Player == LevelManager.Instance.ActivePlayer)
    //     {
    //         foreach (var build in Data.Generals)
    //         {
    //             Debug.Log($"OnRunGeneralBuilds::: Next day - {build.Value.ConfigData.name}");
    //         }
    //     };
    // }

    #endregion
}