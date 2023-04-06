using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class EntityTown : BaseEntity, IDataPlay
{
    public ScriptableEntityTown ConfigData => (ScriptableEntityTown)ScriptableData;
    [SerializeField] public DataTown Data = new DataTown();

    public EntityTown(GridTileNode node)
    {
        List<ScriptableEntityTown> list = ResourceSystem.Instance
            .GetEntityByType<ScriptableEntityTown>(TypeEntity.Town)
            .ToList();
        ScriptableData = list[UnityEngine.Random.Range(0, list.Count)];
        Data.idPlayer = -1;
        Data.name = ConfigData.name;
        Data.ProgressBuilds = ConfigData.StartProgressBuilds.ToList(); // TypeBuild.None | TypeBuild.Tavern_1;
        Data.LevelsBuilds = new SerializableDictionary<TypeBuild, int>();
        base.Init(ScriptableData, node);
    }

    public void SetPlayer(PlayerData data)
    {
        //Debug.Log($"Town SetPlayer::: id{data.id}-idArea{data.idArea}");
        Data.idPlayer = data.id;

        Player player = LevelManager.Instance.GetPlayer(Data.idPlayer);

        MapEntityTown TownGameObject = (MapEntityTown)MapObjectGameObject;
        // TownGameObject.SetPlayer(player);
    }

    public void LoadDataPlay(DataPlay data)
    {
        //throw new System.NotImplementedException();
    }

    public void SaveDataPlay(ref DataPlay data)
    {
        // var sdata = SaveUnit(Data);
        // data.Units.warriors.Add(sdata);
    }
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