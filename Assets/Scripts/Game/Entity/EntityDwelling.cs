using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

[System.Serializable]
public struct DataEntityDwelling
{
    public int value;
    public int level;
}

[System.Serializable]
public class EntityDwelling : BaseEntity
{
    [SerializeField] public DataEntityDwelling Data = new DataEntityDwelling();
    public ScriptableEntityDwelling ConfigData => (ScriptableEntityDwelling)ScriptableData;
    public EntityDwelling(
        ScriptableEntityDwelling configData,
        SaveDataUnit<DataEntityDwelling> saveData = null)
    {
        base.Init();

        if (saveData == null)
        {
            ScriptableData = configData;
            idObject = ScriptableData.idObject;
            SetData();
        }
        else
        {
            ScriptableData = ResourceSystem.Instance
                .GetEntityByType<ScriptableEntityMapObject>(TypeEntity.MapObject)
                .Where(t => t.idObject == saveData.idObject && t.TypeMapObject == TypeMapObject.Dwelling)
                .First();
            Data = saveData.data;
            idUnit = saveData.idUnit;
            idObject = saveData.idObject;
        }

    }

    public void SetData()
    {
        ScriptableEntityDwelling scriptData = (ScriptableEntityDwelling)ScriptableData;
        Data.value = 75;
        Data.level = 0;
    }

    public override void SetPlayer(Player player)
    {
        ScriptableEntityDwelling configData = (ScriptableEntityDwelling)ScriptableData;
        configData.RunHero(ref player, this);
        _player = player;
    }

    #region SaveData
    public override void SaveEntity(ref DataPlay data)
    {
        var sdata = SaveUnit(Data);
        data.entity.dwellings.Add(sdata);
    }

    public EntityCreature BuyCreatures(int level, int count)
    {
        ScriptableEntityDwelling configData = (ScriptableEntityDwelling)ScriptableData;

        List<CostEntity> _costEntities = new List<CostEntity>();
        for (int i = 0; i < configData.Creature[level].CreatureParams.Cost.Count; i++)
        {
            var item = configData.Creature[level].CreatureParams.Cost[i];
            _costEntities.Add(new CostEntity()
            {
                Count = count * item.Count,
                Resource = item.Resource
            });
        }

        var newCreature = new EntityCreature(configData.Creature[level]);
        newCreature.Data.value = count;
        newCreature.Data.idObject = configData.idObject;

        foreach (var res in _costEntities)
        {
            LevelManager.Instance.ActivePlayer.ChangeResource(res.Resource.TypeResource, -res.Count);
        }

        Data.value -= count;

        return newCreature;
    }
    #endregion
}