using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class EntityMonolith : BaseEntity, IDataPlay
{
    public ScriptableEntityPortal ConfigData => (ScriptableEntityPortal)ScriptableData;
    public EntityMonolith(GridTileNode node)
    {
        List<ScriptableEntityPortal> list = ResourceSystem.Instance
            .GetEntityByType<ScriptableEntityPortal>(TypeEntity.Portal)
            .ToList();
        ScriptableData = list[UnityEngine.Random.Range(0, list.Count)];
        base.Init(ScriptableData, node);
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
