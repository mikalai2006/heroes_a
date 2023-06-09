using System;
using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[System.Serializable]
public struct DataMapObject
{
    public string idEntity;
    public Vector3Int position;
    public string idProtMapObj;
}

[Serializable]
public class MapObject
{
    [SerializeField]
    public DataMapObject Data = new DataMapObject();
    public ScriptableEntity _configData;
    public ScriptableEntity ConfigData => _configData;
    [NonSerialized] public GridTileNode OccupiedNode = null;
    [NonSerialized] public GridTileNode ProtectedNode = null;
    public Vector3Int Position => Data.position;
    [NonSerialized] public BaseMapEntity MapObjectGameObject;
    public BaseEntity Entity { get; private set; }
    protected string _idMapObject;
    public string IdMapObject => _idMapObject;

    public MapObject(
        // ScriptableEntityMapObject configData,
        SaveDataMapObject<DataMapObject> saveData = null
        )
    {
        // base.Init();

        if (saveData == null)
        {
            _idMapObject = System.Guid.NewGuid().ToString("N");
            // ScriptableData = configData;
            // _idObject = ScriptableData.idObject;
            // SetData();
            // ConfigData.SetData(this);
        }
        else
        {
            // ScriptableData = ResourceSystem.Instance
            //     .GetEntityByType<ScriptableEntityMapObject>(TypeEntity.MapObject)
            //     .Where(t => t.idObject == saveData.idObject)
            //     .First();

            Data = saveData.data;
            _idMapObject = saveData.idMapObject;
            // _idObject = saveData.idObject;
            // Effects = saveData.DataEffects;
            // Data.position = saveData.position;
        }
    }

    // public void SetData()
    // {
    //     ConfigData.SetData(this);
    // }
    public void SetEntity(BaseEntity entity, GridTileNode node)
    {
        Entity = entity;
        OccupiedNode = node;
        _configData = Entity.ScriptableData;
        Data.idEntity = entity.Id;
    }

    public void DoHero(Player player)
    {
        ScriptableEntityMapObject configData = (ScriptableEntityMapObject)ConfigData;

        // if (configData.Effects != null)
        // {
        //     if (result.keyVariant == -1)
        //     {
        //         await configData.RunHero(player, Entity);
        //     }
        //     else
        //     {
        //         var effect = configData.Effects.ElementAt(Entity.Effects.index);
        //         if (configData.Effects.Count > 0 && effect.Item.items.Count > 0)
        //         {
        //             await effect.Item.items[result.keyVariant].RunHero(player, Entity);
        //         }
        //     }
        // }

        // if (configData.TypeWorkObject == TypeWorkObject.One)
        // {
        //     Entity.DestroyEntity();
        //     // Destroy(gameObject);
        // }

        // // Destroy protected node if creature.
        // if (configData.TypeEntity == TypeEntity.Creature)
        // {
        //     ProtectedNode.DisableProtectedNeigbours(this);
        // }


        if (configData.TypeWorkObject == TypeWorkObject.One || Entity.ScriptableData.TypeEntity == TypeEntity.Creature)
        {
            List<GridTileNode> nodes
                = GameManager.Instance.MapManager.gridTileHelper.GetNodeListAsNoPath(OccupiedNode, configData.RulesInput);
            foreach (var node in nodes)
            {
                node.RemoveStateNode(StateNode.Input);
            }
            // DestroyEntity();

            // OccupiedNode.SetOcuppiedUnit(null);
            // // MapObjectGameObject.DestroyMapObject();
            // UnitManager.MapObjects.Remove(IdMapObject);
            DestroyMapObject();
            Entity.DestroyEntity();
        }
        else
        {
            OccupiedNode.ChangeStatusVisit(true);
        }
    }

    public void DestroyMapObject()
    {
        OccupiedNode.SetOcuppiedUnit(null);
        MapObjectGameObject.DestroyGameObject();
        // MapObjectGameObject.DestroyMapObject();
        UnitManager.MapObjects.Remove(IdMapObject);
    }
    public void CreateMapGameObject(GridTileNode node)
    {
        // Debug.LogWarning($"CreateMapGameObject::: {Entity.ScriptableData.name}");
        Data.position = node.position;
        OccupiedNode = node;
        Entity.SetMapObject(this);
        LoadGameObject();
    }

    public void SetPositionCamera(Vector3 pos)
    {
        var flag = (NoskyMask)(1 << LevelManager.Instance.ActiveUserPlayer.DataPlayer.id);
        var posInt = new Vector3Int((int)pos.x, (int)pos.y);
        if (
            LevelManager.Instance.ConfigGameSettings.showDoBot
            ||
            (
                Entity.Player != null
                &&
                Entity.Player.DataPlayer.team == LevelManager.Instance.ActiveUserPlayer.DataPlayer.team
            )
            ||
            (
                LevelManager.Instance.Level.nosky.ContainsKey(posInt)
                &&
                LevelManager.Instance.Level.nosky[posInt].HasFlag(flag)
            )
        )
        {
            Camera.main.transform.position = pos + new Vector3(0, 0, -10);
        }
    }

    public void SetProtectedNode(GridTileNode protectedNode)
    {
        ProtectedNode = protectedNode;
        Data.idProtMapObj = protectedNode.OccupiedUnit.IdMapObject;
        // ((EntityCreature)Entity).Data.protectedNode = protectedNode.position;
    }

    public void SetPosition(Vector3Int newPosition)
    {
        Data.position = newPosition;
    }

    #region LoadAsset
    private void LoadGameObject()
    {
        AssetReferenceGameObject gameObj = null;
        if (ConfigData.MapPrefab.RuntimeKeyIsValid())
        {
            gameObj = ConfigData.MapPrefab;
        }
        if (Entity.ScriptableDataAttribute != null && Entity.ScriptableDataAttribute.MapPrefab.RuntimeKeyIsValid())
        {
            gameObj = Entity.ScriptableDataAttribute.MapPrefab;
        }

        if (gameObj == null)
        {
            Debug.LogWarning($"Not found mapPrefab {ConfigData.name}!");
            return;
        }

        Addressables.InstantiateAsync(
            gameObj,
            Position,
            Quaternion.identity,
            GameManager.Instance.MapManager.BlokUnits.transform
            ).Completed += LoadedAsset;
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
    #endregion

    #region SaveLoadData
    public void SaveEntity(ref DataPlay data)
    {
        var sdata = new SaveDataMapObject<DataMapObject>();
        sdata.data = Data;
        sdata.idMapObject = IdMapObject;

        data.entity.mapObjects.Add(sdata);
    }
    #endregion
}
