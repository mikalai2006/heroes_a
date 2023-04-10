using System.Collections.Generic;
using System.Linq;

using UnityEngine;

[System.Serializable]
public class EntityArtifact : BaseEntity, ISaveDataPlay
{
    private ScriptableEntityArtifact ConfigData => (ScriptableEntityArtifact)ScriptableData;
    [SerializeField] public DataArtifact Data = new DataArtifact();

    public EntityArtifact(
        GridTileNode node,
        ScriptableEntityArtifact configData,
        SaveDataUnit<DataArtifact> saveData = null)
    {
        if (saveData == null)
        {
            ScriptableData = configData;
        }
        else
        {
            ScriptableData = ResourceSystem.Instance
                .GetEntityByType<ScriptableEntityArtifact>(TypeEntity.MapObject)
                .Where(t => t.idObject == saveData.idObject && t.TypeMapObject == TypeMapObject.Artifact)
                .First();
            Data = saveData.data;
            idUnit = saveData.idUnit;
        }
        base.Init(ScriptableData, node);
    }

    public override void SetPlayer(Player player)
    {
        ScriptableEntityArtifact configData = (ScriptableEntityArtifact)ScriptableData;
        configData.OnDoHero(ref player, this);
    }

    #region InitData
    public void InitData(SaveDataUnit<DataArtifact> data)
    {
        Data = data.data;
    }
    #endregion

    #region SaveLoadData
    // public void LoadDataPlay(DataPlay data)
    // {
    //     throw new System.NotImplementedException();
    // }

    public void SaveDataPlay(ref DataPlay data)
    {
        var sdata = SaveUnit(Data);
        data.entity.artifacts.Add(sdata);
    }
    #endregion
}

[System.Serializable]
public struct DataArtifact
{
    public string ida;
}