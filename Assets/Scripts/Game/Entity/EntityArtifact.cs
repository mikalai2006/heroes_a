using System.Collections.Generic;
using System.Linq;

using UnityEngine;

[System.Serializable]
public class EntityArtifact : BaseEntity, ISaveDataPlay
{
    private ScriptableEntityArtifact ConfigData => (ScriptableEntityArtifact)ScriptableData;
    [SerializeField] public DataArtifact Data = new DataArtifact();

    public EntityArtifact(GridTileNode node, SaveDataUnit<DataArtifact> saveData = null)
    {
        if (saveData == null)
        {
            List<ScriptableEntityArtifact> list = ResourceSystem.Instance
                .GetEntityByType<ScriptableEntityArtifact>(TypeEntity.Artifact)
                .ToList();
            ScriptableData = list[UnityEngine.Random.Range(0, list.Count)];
        }
        else
        {
            ScriptableData = ResourceSystem.Instance
                .GetEntityByType<ScriptableEntityArtifact>(TypeEntity.Artifact)
                .Where(t => t.idObject == saveData.idObject)
                .First();
            Data = saveData.data;
            idUnit = saveData.idUnit;
        }
        base.Init(ScriptableData, node);
    }

    public override void SetPlayer(Player player)
    {
        Debug.Log($"Hero take artifact::: {ScriptableData.name}!");
    }

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

}