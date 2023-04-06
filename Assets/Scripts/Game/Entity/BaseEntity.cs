using System;
using System.Collections;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public abstract class BaseEntity
{
    [NonSerialized] public GridTileNode OccupiedNode = null;
    [NonSerialized] public GridTileNode ProtectedNode = null;
    [NonSerialized] public ScriptableEntity ScriptableData;
    [NonSerialized] public Vector3Int Position;
    [NonSerialized] public BaseMapEntity MapObjectGameObject;
    protected Player _player;
    public Player Player => _player;
    protected string idUnit;
    public string IdEntity => idUnit;
    protected string idObject;

    public void Init(ScriptableEntity data, GridTileNode node)
    {

        // ScriptableData = data;
        // typeEntity = data.TypeEntity;
        // typeUnit = data.TypeUnit;
        // typeInput = data.typeInput;
        Position = node.position;
        idUnit = System.Guid.NewGuid().ToString("N");
        idObject = data.idObject;
        CreateEntityAsync(data, node);
        //transform.position = pos + new Vector3(.5f,.5f);

    }
    protected void LoadUnit<T>(SaveDataUnit<T> Data)
    {
        idUnit = Data.idUnit;
        Position = Data.position;
        // typeMapObject = Data.typeMapObject;
        // typeEntity = Data.typeEntity;
        idObject = Data.idObject;
    }

    private void CreateEntityAsync(ScriptableEntity entity, GridTileNode node)
    {
        if (entity.MapPrefab.RuntimeKeyIsValid())
        {
            Addressables.InstantiateAsync(
                entity.MapPrefab,
                node.position,
                Quaternion.identity,
                GameManager.Instance.MapManager.UnitManager._tileMapUnits.transform
                ).Completed += LoadedAsset;
        }
    }

    public virtual void LoadedAsset(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            var r_asset = handle.Result;
            MapObjectGameObject = r_asset.GetComponent<BaseMapEntity>();
            MapObjectGameObject.InitUnit(this);
            // Debug.Log($"Spawn Entity::: {entity.name}");
        }
        else
        {
            Debug.LogError($"Error Load prefab::: {handle.Status}");
        }
    }

    // public void SetPlayer(PlayerData data)
    // {
    //     //Debug.Log($"Town SetPlayer::: id{data.id}-idArea{data.idArea}");
    //     Data.idPlayer = data.id;

    //     Player player = LevelManager.Instance.GetPlayer(Data.idPlayer);
    //     Transform flag = transform.Find("Flag");
    //     flag.GetComponent<SpriteRenderer>().color = player.DataPlayer.color;
    // }
    protected SaveDataUnit<T> SaveUnit<T>(T Data)
    {
        var SaveData = new SaveDataUnit<T>();

        SaveData.idUnit = idUnit;
        SaveData.position = Position;
        // SaveData.typeEntity = typeEntity;
        // SaveData.typeMapObject = typeMapObject;
        SaveData.idObject = idObject;
        SaveData.data = Data;

        return SaveData;
    }

    public virtual void SetPlayer(Player player)
    {
        _player = player;
    }
}
