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

    public EntityTown(TypeGround typeGround, SaveDataUnit<DataTown> saveData = null)
    {
        base.Init();

        if (saveData == null)
        {
            List<ScriptableEntityTown> list = ResourceSystem.Instance
                .GetEntityByType<ScriptableEntityTown>(TypeEntity.Town)
                .Where(t => t.TypeGround == typeGround)
                .ToList();
            ScriptableData = list[UnityEngine.Random.Range(0, list.Count)];

            Data.idPlayer = -1;
            Data.name = ConfigData.name;
            idObject = ScriptableData.idObject;
            // Data.ProgressBuilds = ConfigData.BuildTown.StartProgressBuilds.ToList(); // TypeBuild.None | TypeBuild.Tavern_1;
            // Data.LevelsBuilds = new SerializableDictionary<TypeBuild, BuildItem>();
            Data.Generals = new SerializableDictionary<string, BuildGeneral>();
            Data.Armys = new SerializableDictionary<string, BuildArmy>();
            // Data.ProgressBuilds = new List<TypeBuild>();
            foreach (var item in ConfigData.BuildTown.StartProgressBuilds)
            {
                // var configData = ResourceSystem.Instance
                //             .GetAllBuildsForTown()
                //             .Where(t =>
                //                 t.t
                //                 && t.TypeFaction == ConfigData.TypeFaction
                //             // {
                //             //     var result = false;
                //             //     if (t.BuildLevels.Where(
                //             //         b => b.TypeBuild == item
                //             //         ).Count() > 0)
                //             //     {
                //             //         result = true;
                //             //     }
                //             //     return result;
                //             // }
                //             )
                //             .First();
                var newBuild = CreateBuild(item.Build, item.level);
                // Data.ProgressBuilds.Add(newBuild.ConfigData.BuildLevels[item.level].TypeBuild);
            }
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
                var configData = ResourceSystem.Instance
                    .GetAllBuildsForTown()
                    .Where(t => t.idObject == item.Key)
                    .First();
                var newBuild = CreateBuild(configData, item.Value.level);
            }
            foreach (var item in saveData.data.Armys)
            {
                var configData = ResourceSystem.Instance
                    .GetAllBuildsForTown()
                    .Where(t => t.idObject == item.Key)
                    .First();
                var newBuild = CreateBuild(configData, item.Value.level);
            }
            idUnit = saveData.idUnit;
            idObject = saveData.idObject;

            // Data.HeroinTown = new EntityHero(TypeFaction.Neutral, new SaveDataUnit<DataHero>(){
            //     data = saveData.data.HeroinTown.Data,
            //     idObject = saveData.data.HeroinTown
            // });
        }

    }

    public void SetTownAsActive()
    {
        SetPositionCamera(this.Position);
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

        Data.idPlayer = player.DataPlayer.id;
        player.AddTown(this);
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

    public BaseBuild CreateBuild(ScriptableBuilding buildConfig, int level)
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
                build = new BuildArmy(level, (ScriptableBuildingArmy)buildConfig, this);
                Data.Armys.Add(buildConfig.idObject, build);
            }
            return build;
        }
        else
        {
            BuildGeneral build;
            if (Data.Generals.TryGetValue(buildConfig.idObject, out build))
            {
                Data.Generals[buildConfig.idObject].level += 1;
                Data.Generals[buildConfig.idObject].OnRunEffects();
            }
            else
            {
                build = new BuildGeneral(level, (ScriptableBuildingGeneral)buildConfig, this);
                Data.Generals.Add(buildConfig.idObject, build);
            }
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
        switch (newState)
        {
            case GameState.StepNextPlayer:
                // OnRunGeneralBuilds();
                OnRunArmyBuilds();
                break;
        }
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

    private void OnRunArmyBuilds()
    {
        if (Player == LevelManager.Instance.ActivePlayer)
        {
            foreach (var build in Data.Armys)
            {
                Debug.Log($"OnRunArmyBuilds::: Next day - {build.Value.ConfigData.name}");
            }
        };
    }
    #endregion
}