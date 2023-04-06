using System.Collections.Generic;
using System.Linq;

using UnityEngine;

[System.Serializable]
public struct DataMonolith
{
    public int keyArea;
    [SerializeField] public List<Vector3Int> portalPoints;
}

[System.Serializable]
public class EntityMonolith : BaseEntity, ISaveDataPlay
{
    [SerializeField] public DataMonolith Data = new DataMonolith();
    public ScriptableEntityPortal ConfigData => (ScriptableEntityPortal)ScriptableData;
    public EntityMonolith(GridTileNode node, SaveDataUnit<DataMonolith> saveData = null)
    {
        if (saveData == null)
        {
            List<ScriptableEntityPortal> list = ResourceSystem.Instance
                .GetEntityByType<ScriptableEntityPortal>(TypeEntity.Portal)
                .ToList();
            ScriptableData = list[UnityEngine.Random.Range(0, list.Count)];
            Data.portalPoints = new List<Vector3Int>();
        }
        else
        {
            ScriptableData = ResourceSystem.Instance
                .GetEntityByType<ScriptableEntityPortal>(TypeEntity.Portal)
                .Where(t => t.idObject == saveData.idObject)
                .First();
            Data = saveData.data;
            idUnit = saveData.idUnit;
        }

        base.Init(ScriptableData, node);
    }

    public override void SetPlayer(Player player)
    {
        Debug.Log($"Teleport to position::: {Data.portalPoints[0]}");
    }

    #region SaveLoadData
    // public void LoadDataPlay(DataPlay data)
    // {
    //     throw new System.NotImplementedException();
    // }

    public void SaveDataPlay(ref DataPlay data)
    {
        var sdata = SaveUnit(Data);
        data.entity.monoliths.Add(sdata);
    }
    #endregion
}
