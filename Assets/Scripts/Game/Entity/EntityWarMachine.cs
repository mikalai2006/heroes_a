using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

[System.Serializable]
public struct DataEntityWarMachine
{
    public int value;
    public int level;
    public int idPlayer;
    public int growth;
    public int dopGrowth;
}

[System.Serializable]
public class EntityWarMachine : BaseEntity
{
    [SerializeField] public DataEntityWarMachine Data = new DataEntityWarMachine();
    public ScriptableAttributeWarMachine ConfigDataAttribute => (ScriptableAttributeWarMachine)ScriptableDataAttribute;
    public EntityWarMachine(
        ScriptableAttributeWarMachine configData,
        SaveDataUnit<DataEntityWarMachine> saveData = null)
    {
        base.Init();

        if (saveData == null)
        {
            ScriptableDataAttribute = configData;
            _idEntity = ScriptableDataAttribute.idObject;
        }
        else
        {
            ScriptableDataAttribute = ResourceSystem.Instance
                .GetAttributesByType<ScriptableAttributeWarMachine>(TypeAttribute.WarMachine)
                .Where(t => t.idObject == saveData.idEntity)
                .First();

            Data = saveData.data;
            _id = saveData.id;
            _idEntity = saveData.idEntity;
            Effects = saveData.Effects;
        }
    }

    #region SaveData
    public override void SaveEntity(ref DataPlay data)
    {
        var sdata = SaveUnit(Data);
        // data.entity.dwellings.Add(sdata);
    }
    #endregion
}