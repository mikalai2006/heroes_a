using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[System.Serializable]
public class DataEntityEffectsBase
{
    public string ide;
    public int value;
    public string ido;
    [System.NonSerialized] public BaseEffect Effect;
}

[System.Serializable]
public struct DataEntityEffects
{
    public List<DataEntityEffectsBase> Effects;
    public int index;
}

public abstract class BaseEntity
{
    [NonSerialized] public ScriptableEntity ScriptableData;
    [NonSerialized] public ScriptableAttribute ScriptableDataAttribute;
    public DataEntityEffects Effects = new DataEntityEffects()
    {
        Effects = new List<DataEntityEffectsBase>()
    };
    protected Player _player;
    public Player Player => _player;
    protected string _id;
    public string Id => _id;
    protected string _idEntity;
    public string IdEntity => _idEntity;
    public MapObject MapObject { get; protected set; }


    public void Init()
    {
        _id = System.Guid.NewGuid().ToString("N");
        AddEvents();
    }
    public void DestroyEntity()
    {
        RemoveEvents();
        MapObject.DestroyMapGameObject();
        UnitManager.Entities.Remove(Id);
        UnitManager.MapObjects.Remove(MapObject.IdMapObject);
        MapObject = null;
    }

    public void SetMapObject(MapObject mapObject)
    {
        MapObject = mapObject;
    }

    #region Events GameState
    public void AddEvents()
    {
        GameManager.OnBeforeStateChanged += OnBeforeStateChanged;
        GameManager.OnAfterStateChanged += OnAfterStateChanged;
    }
    public void RemoveEvents()
    {
        GameManager.OnBeforeStateChanged -= OnBeforeStateChanged;
        GameManager.OnAfterStateChanged -= OnAfterStateChanged;
    }

    public virtual void OnBeforeStateChanged(GameState newState)
    {
        // switch (newState)
        // {
        //     case GameState.SaveGame:
        //         // OnSaveUnit();
        //         break;
        // }
    }

    public virtual void OnAfterStateChanged(GameState newState)
    {
    }
    #endregion


    #region SaveLoadData
    public virtual void OnSaveUnit()
    {
        // SaveUnit(new object());
    }
    protected SaveDataUnit<T> SaveUnit<T>(T Data)
    {
        var SaveData = new SaveDataUnit<T>();

        SaveData.id = _id;
        SaveData.idEntity = _idEntity;
        SaveData.data = Data;
        SaveData.Effects = Effects;

        return SaveData;
    }
    protected void LoadUnit<T>(SaveDataUnit<T> Data)
    {
        _id = Data.id;
        _idEntity = Data.idEntity;
    }

    public virtual void SaveEntity(ref DataPlay data)
    {

    }
    #endregion

    public virtual void SetPlayer(Player player)
    {
        _player = player;
    }

}
