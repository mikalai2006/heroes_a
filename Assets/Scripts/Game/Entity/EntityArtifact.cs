using System.Collections.Generic;
using System.Linq;

using UnityEngine;

[System.Serializable]
public class EntityArtifact : BaseEntity
{
    private ScriptableEntityArtifact ConfigData => (ScriptableEntityArtifact)ScriptableData;
    [SerializeField] public DataArtifact Data = new DataArtifact();

    public EntityArtifact(
        ScriptableEntityArtifact configData,
        SaveDataUnit<DataArtifact> saveData = null)
    {
        base.Init();

        if (saveData == null)
        {
            ScriptableData = configData;
            SetData();
            idObject = ScriptableData.idObject;
        }
        else
        {
            // ResourceSystem.Instance
            //     .GetEntityByType<ScriptableEntityArtifact>(TypeEntity.MapObject)
            //     .Where(t => t.idObject == saveData.idObject && t.TypeMapObject == TypeMapObject.Artifact)
            Data = saveData.data;
            ScriptableData = ResourceSystem.Instance
                .GetEntityByType<ScriptableEntityMapObject>(TypeEntity.MapObject)
                .Where(t => t.TypeMapObject == TypeMapObject.Artifact
                && t.idObject == saveData.idObject).First();
            List<ScriptableAttributeArtifact> listArtifacts
            = ResourceSystem.Instance
            .GetAttributesByType<ScriptableAttributeArtifact>(TypeAttribute.Artifact)
            .Where(t => t.idObject == Data.ida)
            .ToList();
            idUnit = saveData.idUnit;
            idObject = saveData.idObject;
        }
    }

    public void SetData()
    {
        ScriptableEntityArtifact configData = (ScriptableEntityArtifact)ScriptableData;
        Data.ida = configData.Artifact.idObject;
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

    public override void SaveEntity(ref DataPlay data)
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