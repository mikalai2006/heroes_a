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
public struct ArenaHeroEntityData
{
    public int ballisticShoot;
    public int ballisticChanceHitKeep;
    public int ballisticChanceHitTower;
    public int ballisticChanceHitBridge;
    public int ballisticChanceHitIntendedWall;
    public int ballisticChanceNoDamage;
    public int ballisticChance1Damage;
    public int ballisticChance2Damage;
}

[Serializable]
public class ArenaHeroEntity
{
    public ArenaHeroEntityData Data;
    private Tilemap _tileMapArenaUnits;

    [SerializeField]
    public ScriptableEntityHero _configData;
    public ScriptableEntityHero ConfigData => _configData;
    private Vector3 _position;
    public Vector3 Position => _position;
    [NonSerialized] public ArenaHeroMonoBehavior ArenaHeroMonoBehavior;
    public EntityHero Entity { get; private set; }

    public ArenaHeroEntity(Tilemap tileMapArenaUnits)
    {
        _tileMapArenaUnits = tileMapArenaUnits;
    }

    public void SetEntity(BaseEntity entity)
    {
        Data = new()
        {
            ballisticShoot = 1,
            ballisticChanceHitKeep = 5,
            ballisticChanceHitBridge = 25,
            ballisticChanceHitTower = 10,
            ballisticChanceHitIntendedWall = 50,
            ballisticChanceNoDamage = 10,
            ballisticChance1Damage = 60,
            ballisticChance2Damage = 30
        };

        Entity = ((EntityHero)entity);
        _configData = Entity.ConfigData;
        if (entity != null && Entity.SpellBook != null)
        {
            Entity.SpellBook.SetCountSpellPerRound();
        }
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
