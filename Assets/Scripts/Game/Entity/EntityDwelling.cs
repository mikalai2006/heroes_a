using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

[System.Serializable]
public struct DataEntityDwelling
{
    public int value;
    public int level;
    public int idPlayer;
    public int growth;
    public int dopGrowth;
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
            _idEntity = ScriptableData.idObject;
            Data.idPlayer = -1;
            SetData();
        }
        else
        {
            ScriptableData = ResourceSystem.Instance
                .GetEntityByType<ScriptableEntityMapObject>(TypeEntity.MapObject)
                .Where(t => t.idObject == saveData.idEntity && t.TypeMapObject == TypeMapObject.Dwelling)
                .First();

            Data = saveData.data;
            _id = saveData.id;
            _idEntity = saveData.idEntity;
            Effects = saveData.Effects;
        }
    }

    // public override void OnAfterStateChanged(GameState newState)
    // {
    //     ScriptableEntityDwelling scriptData = (ScriptableEntityDwelling)ScriptableData;
    //     // if (scriptData.Effects.Count > 0)
    //     // {
    //     //     scriptData.RunHero(ref _player, this);
    //     // }
    // }

    public void SetData()
    {
        ScriptableEntityDwelling scriptData = (ScriptableEntityDwelling)ScriptableData;
        Data.level = 0;
        Data.growth = ConfigData.Creature[Data.level].CreatureParams.Growth;
        Data.value = Data.growth;
    }

    public override async void SetPlayer(Player player)
    {
        ScriptableEntityDwelling configData = (ScriptableEntityDwelling)ScriptableData;
        await configData.RunHero(player, this);
        _player = player;
        Data.idPlayer = player.DataPlayer.id;
    }

    #region SaveData
    public override void SaveEntity(ref DataPlay data)
    {
        var sdata = SaveUnit(Data);
        data.entity.dwellings.Add(sdata);
    }

    public EntityCreature BuyCreatures(int level, int count, SerializableDictionary<int, EntityCreature> creatures)
    {
        List<CostEntity> _costEntities = new List<CostEntity>();
        for (int i = 0; i < ConfigData.Creature[level].CreatureParams.Cost.Count; i++)
        {
            var item = ConfigData.Creature[level].CreatureParams.Cost[i];
            _costEntities.Add(new CostEntity()
            {
                Count = count * item.Count,
                Resource = item.Resource
            });
        }

        var newCreature = new EntityCreature(ConfigData.Creature[level]);
        newCreature.Data.value = count;
        newCreature.Data.idObject = ConfigData.Creature[level].idObject;//configData.idObject;

        foreach (var res in _costEntities)
        {
            LevelManager.Instance.ActivePlayer.ChangeResource(res.Resource.TypeResource, -res.Count);
        }

        Data.value -= count;

        return newCreature;
    }

    public int GetIndexCreature(SerializableDictionary<int, EntityCreature> creatures, ScriptableAttributeCreature creature)
    {
        var indexCreature = creatures.Values.ToList().FindIndex(t => t != null && t.Data.idObject == creature.idObject);

        if (indexCreature != -1) return indexCreature;

        for (var i = 0; i < creatures.Count; i++)
        {
            if (creatures.GetValueOrDefault(i) == null)
            {
                indexCreature = i;
                break;
            }
        }
        return indexCreature;
    }

    public int GetGrowth()
    {
        var growth = Data.growth;
        var dopGrowth = Data.dopGrowth;
        return growth + dopGrowth;
    }
    #endregion
}