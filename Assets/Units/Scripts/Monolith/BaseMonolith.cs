using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DataMonolith
{
    public int keyArea;
    [SerializeField] public List<Vector3Int> portalPoints;
}

public abstract class BaseMonolith : UnitBase, IDataPlay
{
    [SerializeField] public DataMonolith Data;

    public virtual void Init(DataMonolith data)
    {
        
    }

    public override void InitUnit(ScriptableUnitBase data, Vector3Int pos)
    {
        base.InitUnit(data, pos);
        Data = new DataMonolith();
        Data.portalPoints = new List<Vector3Int>();
    }

    //public override void OnSaveUnit()
    //{
    //    SaveUnit(Data);
    //}

    public void LoadDataPlay(DataPlay data)
    {
        //throw new System.NotImplementedException();
    }

    public void SaveDataPlay(ref DataPlay data)
    {
        var sdata = SaveUnit(Data);
        data.Units.monoliths.Add(sdata);
    }
}