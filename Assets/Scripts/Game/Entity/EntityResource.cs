using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Random = UnityEngine.Random;

public class EntityResource : BaseEntity, IDataPlay
{
    public ScriptableEntityMapResource ConfigData => (ScriptableEntityMapResource)ScriptableData;
    public DataResourceMapObject Data;
    public EntityResource(GridTileNode node, List<TypeWorkPerk> listTypeWork)
    {
        List<ScriptableEntityMapResource> list = ResourceSystem.Instance
            .GetEntityByType<ScriptableEntityMapResource>(TypeEntity.MapResource)
            .Where(t => listTypeWork.Contains(t.TypeWorkPerk))
            .ToList();
        ScriptableData = list[UnityEngine.Random.Range(0, list.Count)];
        Data = new DataResourceMapObject();
        SetData();

        base.Init(ScriptableData, node);
    }

    // #region  Release BaseEntityFactory
    // public BaseEntity CreateEntity(TypeEntity typeEntity, GridTileNode node)
    // {
    //     throw new NotImplementedException();
    // }
    // public EntityConfig GetEntityConfig(TypeEntity typeEntity)
    // {
    //     List<ScriptableEntityArtifact> list = ResourceSystem.Instance
    //         .GetEntityByType<ScriptableEntityArtifact>(TypeEntity.Resource)
    //         .ToList();
    //     if (list.Count == 0) return null;
    //     var config = new EntityConfig();
    //     config.ScriptableData = list[Random.Range(0, list.Count)];
    //     return config;
    // }
    // #endregion

    public void SetData()
    {
        ScriptableEntityMapResource scriptData = (ScriptableEntityMapResource)ScriptableData;

        Data.Value = new List<DataResourceValue>();
        Data.TypeWork = scriptData.TypeWorkPerk;//.TypeWorkMapObject;

        GroupResource groupResource = scriptData
            .Resources[Random.Range(0, scriptData.Resources.Count)];
        for (int i = 0; i < groupResource.ListVariant.Count; i++)
        {
            int stepsValue = groupResource.ListVariant[i].maxValue / groupResource.ListVariant[i].step;
            int randomIndexValue = Random.Range(1, stepsValue);
            Data.Value.Add(new DataResourceValue()
            {
                typeResource = groupResource.ListVariant[i].Resource.TypeResource,
                value = groupResource.ListVariant[i].step * randomIndexValue,
                Resource = groupResource.ListVariant[i].Resource
            });

        }

    }

    public void SetPlayer(PlayerData data)
    {
        //Debug.Log($"Town SetPlayer::: id{data.id}-idArea{data.idArea}");

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
public struct DataResourceMapObject
{
    public int idPlayer;
    public List<DataResourceValue> Value;
    public TypeWorkPerk TypeWork;
}

[System.Serializable]
public struct DataResourceValue
{
    public TypeResource typeResource;
    public int value;

    public ScriptableEntityResource Resource;

}
