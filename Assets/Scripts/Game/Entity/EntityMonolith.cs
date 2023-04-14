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
public class EntityMonolith : BaseEntity
{
    [SerializeField] public DataMonolith Data = new DataMonolith();
    public ScriptableEntityPortal ConfigData => (ScriptableEntityPortal)ScriptableData;
    public EntityMonolith(ScriptableEntityPortal configData, SaveDataUnit<DataMonolith> saveData = null)
    {
        base.Init();

        if (saveData == null)
        {
            // List<ScriptableEntityPortal> list = ResourceSystem.Instance
            //     .GetEntityByType<ScriptableEntityPortal>(TypeEntity.Portal)
            //     .ToList();
            // ScriptableData = list[UnityEngine.Random.Range(0, list.Count)];
            ScriptableData = configData;
            idObject = ScriptableData.idObject;
            Data.portalPoints = new List<Vector3Int>();
        }
        else
        {
            ScriptableData = ResourceSystem.Instance
                .GetEntityByType<ScriptableEntityMapObject>(TypeEntity.MapObject)
                .Where(t => t.idObject == saveData.idObject && t.TypeMapObject == TypeMapObject.Portal)
                .First();
            Data = saveData.data;
            idUnit = saveData.idUnit;
            idObject = saveData.idObject;
        }
    }

    public override void SetPlayer(Player player)
    {
        ScriptableEntityPortal configData = (ScriptableEntityPortal)ScriptableData;
        configData.OnDoHero(ref player, this);
        // Debug.Log($"Teleport to position::: {Data.portalPoints[0]}");
    }

    #region SaveLoadData
    // public void LoadDataPlay(DataPlay data)
    // {
    //     throw new System.NotImplementedException();
    // }

    public override void SaveEntity(ref DataPlay data)
    {
        var sdata = SaveUnit(Data);
        data.entity.monoliths.Add(sdata);
    }
    #endregion
}
