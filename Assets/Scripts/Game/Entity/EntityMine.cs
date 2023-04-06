using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class EntityMine : BaseEntity, IDataPlay
{
    public ScriptableEntityMine ConfigData => (ScriptableEntityMine)ScriptableData;
    public EntityMine(GridTileNode node)
    {
        List<ScriptableEntityMine> list = ResourceSystem.Instance
            .GetEntityByType<ScriptableEntityMine>(TypeEntity.Mine)
            .ToList();
        ScriptableData = list[UnityEngine.Random.Range(0, list.Count)];
        base.Init(ScriptableData, node);
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
