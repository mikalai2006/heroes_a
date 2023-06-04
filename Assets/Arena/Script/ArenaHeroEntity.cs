using System;
using System.Collections.Generic;

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
public enum TypearenaHeroStatus
{
    None = 0,
    Victorious = 1,
    Defendied = 2,
    PayOffed = 3,
    Runned = 4,
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
    public Dictionary<BaseEntity, ArenaCreature> ArenaCreatures;
}

[Serializable]
public class ArenaHeroEntity
{
    public ArenaHeroEntityData Data;
    private Tilemap _tileMapArenaUnits;
    public TypearenaHeroStatus typearenaHeroStatus;
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
        Data.ArenaCreatures = new();

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
            // await SetStatusAutoRun(true);
            Data.autoRun = true;
        }
        else
        // else create hero visual.
        {
            await CreateMapGameObject();
            // await SetStatusAutoRun(false);
            Data.autoRun = false;
        }

        // if bot - set autoRun.
        if (
            Entity == null
            || (Entity != null && Entity.Player != null && Entity.Player.DataPlayer.playerType == PlayerType.Bot))
        {
            Data.autoRun = true;
            // await SetStatusAutoRun(true);
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

    #region LoadAsset
    private async UniTask CreateMapGameObject()
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

        var asset = Addressables.InstantiateAsync(
            gameObj,
            Position, // + new Vector3(0, -.25f, 0),
            Quaternion.identity,
            _tileMapArenaUnits.transform
            );
        await asset.Task;
        ArenaHeroMonoBehavior = asset.Result.GetComponent<ArenaHeroMonoBehavior>();
        // Debug.Log($"Spawn Entity::: {r_asset.name}");
        ArenaHeroMonoBehavior.Init(this);

    }
    #endregion
}
