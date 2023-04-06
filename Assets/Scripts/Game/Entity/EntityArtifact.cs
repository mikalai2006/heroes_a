using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class EntityArtifact : BaseEntity, IDataPlay
{
    private ScriptableEntityArtifact ConfigData => (ScriptableEntityArtifact)ScriptableData;
    public DataArtifact Data;

    public EntityArtifact(GridTileNode node)
    {
        List<ScriptableEntityArtifact> list = ResourceSystem.Instance
             .GetEntityByType<ScriptableEntityArtifact>(TypeEntity.Artifact)
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

[System.Serializable]
public struct DataArtifact
{

}