using System;
using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Tilemaps;

[Serializable]
public class ArenaHeroEntity
{
    private Tilemap _tileMapArenaUnits;

    [SerializeField]
    public ScriptableEntity _configData;
    public ScriptableEntity ConfigData => _configData;
    private Vector3 _position;
    public Vector3 Position => _position;
    [NonSerialized] public ArenaHeroMonoBehavior ArenaHeroMonoBehavior;
    public BaseEntity Entity { get; private set; }

    public ArenaHeroEntity(Tilemap tileMapArenaUnits)
    {
        _tileMapArenaUnits = tileMapArenaUnits;
    }

    public void SetEntity(BaseEntity entity)
    {
        Entity = entity;
        _configData = Entity.ScriptableData;
    }

    public void SetPosition(Vector3 pos)
    {
        _position = pos;
    }

    public void DestroyMapObject()
    {

    }

    public void CreateMapGameObject()
    {
        LoadGameObject();
    }

    #region LoadAsset
    private void LoadGameObject()
    {
        AssetReferenceGameObject gameObj = null;
        // if (ConfigData.MapPrefab.RuntimeKeyIsValid())
        // {
        //     gameObj = ((ScriptableAttributeCreature)confAtt).ArenaModel;
        // }
        var configHero = ((ScriptableEntityHero)Entity.ScriptableData);
        if (configHero != null && configHero.ClassHero.ArenaPrefab.RuntimeKeyIsValid())
        {
            gameObj = configHero.ClassHero.ArenaPrefab;
        }

        if (gameObj == null)
        {
            Debug.LogWarning($"Not found hero ArenaPrefab {ConfigData.name}!");
            return;
        }

        Addressables.InstantiateAsync(
            gameObj,
            Position, // + new Vector3(0, -.25f, 0),
            Quaternion.identity,
            _tileMapArenaUnits.transform
            ).Completed += LoadedAsset;
    }

    public virtual void LoadedAsset(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            var r_asset = handle.Result;
            ArenaHeroMonoBehavior = r_asset.GetComponent<ArenaHeroMonoBehavior>();
            // Debug.Log($"Spawn Entity::: {r_asset.name}");
            ArenaHeroMonoBehavior.Init(this);
        }
        else
        {
            Debug.LogError($"Error Load prefab::: {handle.Status}");
        }
    }
    #endregion
}
