using System;

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


    public void Init(ScriptableEntity data, GridTileNode node)
    {
        AddEvents();
        // ScriptableData = data;
        // typeEntity = data.TypeEntity;
        // typeUnit = data.TypeUnit;
        // typeInput = data.typeInput;
        Position = node.position;
        idUnit = System.Guid.NewGuid().ToString("N");
        idObject = data.idObject;

        // Init load gameObject.
        CreateEntityAsync(data, node);

    }

    public void DestroyEntity()
    {
        // Debug.Log($"Destroy entity::: {ScriptableData.name}");
        OccupiedNode.SetOcuppiedUnit(null);
        RemoveEvents();
    }

    protected void SetPositionCamera(Vector3 pos)
    {
        Camera.main.transform.position = pos + new Vector3(0, 0, -10);
    }

    // #region InitData
    // public virtual void InitData<T>(SaveDataUnit<T> data)
    // {

    // }
    // #endregion


    #region SaveLoadData
    public virtual void OnSaveUnit()
    {
        // SaveUnit(new object());
    }
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
    protected void LoadUnit<T>(SaveDataUnit<T> Data)
    {
        idUnit = Data.idUnit;
        Position = Data.position;
        // typeMapObject = Data.typeMapObject;
        // typeEntity = Data.typeEntity;
        idObject = Data.idObject;
    }
    #endregion

    private void CreateEntityAsync(ScriptableEntity entity, GridTileNode node)
    {
        if (entity.MapPrefab.RuntimeKeyIsValid())
        {
            Addressables.InstantiateAsync(
                entity.MapPrefab,
                node.position,
                Quaternion.identity,
                GameManager.Instance.MapManager.BlokUnits.transform
                ).Completed += LoadedAsset;
        }
    }

    public virtual void LoadedAsset(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            var r_asset = handle.Result;
            MapObjectGameObject = r_asset.GetComponent<BaseMapEntity>();
            // Debug.Log($"Spawn Entity::: {r_asset.name}");
            MapObjectGameObject.InitUnit(this);
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

    public virtual void SetPlayer(Player player)
    {
        _player = player;
    }
}
