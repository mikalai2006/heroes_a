using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

[Serializable]
public class EntityTown : BaseEntity, ISaveDataPlay
{
    [SerializeField] public DataTown Data = new DataTown();
    public ScriptableEntityTown ConfigData => (ScriptableEntityTown)ScriptableData;

    public EntityTown(GridTileNode node, TypeGround typeGround, SaveDataUnit<DataTown> saveData = null)
    {
        if (saveData == null)
        {
            List<ScriptableEntityTown> list = ResourceSystem.Instance
                .GetEntityByType<ScriptableEntityTown>(TypeEntity.Town)
                .Where(t => t.TypeGround == typeGround)
                .ToList();
            ScriptableData = list[UnityEngine.Random.Range(0, list.Count)];

            Data.idPlayer = -1;
            Data.name = ConfigData.name;
            Data.ProgressBuilds = ConfigData.StartProgressBuilds.ToList(); // TypeBuild.None | TypeBuild.Tavern_1;
            Data.LevelsBuilds = new SerializableDictionary<TypeBuild, int>();
        }
        else
        {
            ScriptableData = ResourceSystem.Instance
                .GetEntityByType<ScriptableEntityTown>(TypeEntity.Town)
                .Where(t => t.idObject == saveData.idObject)
                .First();
            Data = saveData.data;
            idUnit = saveData.idUnit;
        }

        base.Init(ScriptableData, node);
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

    public void SaveDataPlay(ref DataPlay data)
    {
        var sdata = SaveUnit(Data);
        data.entity.towns.Add(sdata);
    }
    #endregion
}

[System.Serializable]
public struct DataTown
{
    public int idPlayer;
    public string name;
    public List<TypeBuild> ProgressBuilds;
    // public TypeBuildArmy ProgressBuildsArmy;
    public bool isBuild;
    public SerializableDictionary<TypeBuild, int> LevelsBuilds;
}