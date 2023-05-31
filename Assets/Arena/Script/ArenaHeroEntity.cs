using System;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Tilemaps;

public enum TypeArenaHero
{
    Visible = 0,
    Hidden = 1
}

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

    public bool autoRun;
    public TypeArenaHero typeArenaHero;
    public PlayerType playerType;
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

    public AIArena AI;
    private ArenaManager _arenaManager;
    // private EntityHero EntityHero;

    public ArenaHeroEntity(Tilemap tileMapArenaUnits)
    {
        _tileMapArenaUnits = tileMapArenaUnits;
    }

    public async UniTask SetEntity(ArenaManager arenaManager, BaseEntity entity, TypeArenaHero typeArenaHero, Vector3 pos)
    {
        _arenaManager = arenaManager;
        _position = pos;

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

        await CreateAI(typeArenaHero);

        if (entity != null && Entity.SpellBook != null)
        {
            _configData = Entity.ConfigData;

            Entity.SpellBook.SetCountSpellPerRound();
        }

        if (entity != null && entity.Player != null)
        {
            Data.playerType = Entity.Player.DataPlayer.playerType;
        }
        else
        {
            Data.playerType = PlayerType.Bot;
        }

    }

    public async UniTask CreateAI(TypeArenaHero typeArenaHero)
    {
        Data.typeArenaHero = typeArenaHero;

        AI = new AIArena();
        AI.Init(_arenaManager, this);

        if (typeArenaHero == TypeArenaHero.Hidden)
        // if not hero (only creatures), set autoRun.
        {
            await SetStatusAutoRun(true);
        }
        else
        // else create hero visual.
        {
            CreateMapGameObject();
            await SetStatusAutoRun(false);
        }

        // if bot - set autoRun.
        if (
            Entity == null
            || (Entity != null && Entity.Player != null && Entity.Player.DataPlayer.playerType == PlayerType.Bot))
        {
            await SetStatusAutoRun(true);
        }

    }

    public async UniTask SetStatusAutoRun(bool status)
    {
        Data.autoRun = status;
        if (status)
        {
            await AI.Run();
        }
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
