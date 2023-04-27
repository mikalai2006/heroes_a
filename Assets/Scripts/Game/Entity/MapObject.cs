using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[System.Serializable]
public struct DataMapObject
{
    public string idEntity;
    public Vector3Int position;
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
    protected string _idEntity;
    public string IdEntity => _idEntity;

    public MapObject(
        // ScriptableEntityMapObject configData,
        SaveDataMapObject<DataMapObject> saveData = null
        )
    {
        // base.Init();

        if (saveData == null)
        {
            _idEntity = System.Guid.NewGuid().ToString("N");
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
            _idEntity = saveData.idEntity;
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
        Data.idEntity = entity.IdEntity;
    }

    public void SetPlayer(Player player)
    {
        ScriptableEntityMapObject configData = (ScriptableEntityMapObject)ConfigData;
        // configData.RunHero(ref player, this);
        // ScriptableResource dataScriptable = ResourceSystem.Instance.GetUnit<ScriptableResource>(idObject);

        // ItemResource dataResource = dataScriptable.ListResource[Random.Range(0, dataScriptable.ListResource.Count)];
        // int value = dataResource.listValue[Random.Range(0, dataResource.listValue.Length)];
        // player.ChangeResource(dataResource.TypeResource, value);
        // for (int i = 0; i < Data.Value.Count; i++)
        // {
        //     player.ChangeResource(Data.Value[i].typeResource, Data.Value[i].value);
        // }
        if (configData.TypeWorkObject == TypeWorkObject.One)
        {
            List<GridTileNode> nodes
                = GameManager.Instance.MapManager.gridTileHelper.GetNodeListAsNoPath(OccupiedNode, configData.RulesInput);
            foreach (var node in nodes)
            {
                node.RemoveStateNode(StateNode.Input);
            }
            // DestroyEntity();
        }
        else
        {
            OccupiedNode.ChangeStatusVisit(true);
        }
    }

    public void SetPositionCamera(Vector3 pos)
    {
        Camera.main.transform.position = pos + new Vector3(0, 0, -10);
    }

    public void SetProtectedNode(GridTileNode protectedNode)
    {
        ProtectedNode = protectedNode;
        ((EntityCreature)Entity).Data.protectedNode = protectedNode.position;
    }

    public void DestroyMapGameObject()
    {
        OccupiedNode.SetOcuppiedUnit(null);
        // OccupiedNode = null;
    }

    public void CreateMapGameObject(GridTileNode node)
    {
        Data.position = node.position;
        OccupiedNode = node;
        Entity.SetMapObject(this);
        LoadGameObject();
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
        sdata.idEntity = IdEntity;

        data.entity.mapObjects.Add(sdata);
    }
    #endregion
}
