using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

[Serializable]
public struct DataExplore
{

}
[Serializable]
public class EntityExpore : BaseEntity, ISaveDataPlay
{
    [SerializeField] public DataExplore Data = new DataExplore();
    public ScriptableEntityExplore ConfigData => (ScriptableEntityExplore)ScriptableData;
    public EntityExpore(GridTileNode node, SaveDataUnit<DataExplore> saveData = null)
    {
        if (saveData == null)
        {
            List<ScriptableEntityExplore> list = ResourceSystem.Instance
                .GetEntityByType<ScriptableEntityExplore>(TypeEntity.Explore)
                .ToList();
            ScriptableData = list[UnityEngine.Random.Range(0, list.Count)];
        }
        else
        {
            ScriptableData = ResourceSystem.Instance
                .GetEntityByType<ScriptableEntityExplore>(TypeEntity.Explore)
                .Where(t => t.idObject == saveData.idObject)
                .First();
            Data = saveData.data;
            idUnit = saveData.idUnit;
        }
        base.Init(ScriptableData, node);
    }

    #region SaveLoadData
    // public void LoadDataPlay(DataPlay data)
    // {
    //     throw new System.NotImplementedException();
    // }

    public void SaveDataPlay(ref DataPlay data)
    {
        var sdata = SaveUnit(Data);
        data.entity.explorers.Add(sdata);
    }
    #endregion
}
