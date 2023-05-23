using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Tilemaps;

[Serializable]
public enum TypeArenaPlayer
{
    Left = 0,
    Right = 1,
}

[Serializable]
public enum TypeDirection
{
    Right = 0,
    Left = 1
}

[Serializable]
public enum TypeAttack
{
    Attack = 0,
    AttackShoot = 1,
    // AttackSpell = 2
}

// [Serializable]
// public struct ModificatorItem {
//     public int round;
//     public ScriptableAttribute Spell;
// }

[Serializable]
public class ArenaEntityData
{
    public bool isRun;
    public int waitTick;
    public TypeAttack typeAttack;
    public int lucky;
    public Dictionary<ScriptableAttribute, int> LuckyModificators = new();
    public int attack;
    public Dictionary<ScriptableAttribute, int> AttackModificators = new();
    public bool isDefense;
    public int defense;
    public Dictionary<ScriptableAttribute, int> DefenseModificators = new();
    public int speed;
    public Dictionary<ScriptableAttribute, int> SpeedModificators = new();
    public int fireDefense;
    public Dictionary<ScriptableAttribute, int> FireDefenseModificators = new();
    public int waterDefense;
    public Dictionary<ScriptableAttribute, int> WaterDefenseModificators = new();
    public MovementType typeMove;
    public int shoots;
    public int quantity;
    public int counterAttack;
    public int countAttack;
    public int damageMin;
    public int damageMax;
    public Dictionary<ScriptableAttribute, int> DamageModificators = new();
    public int maxHP;
    public int totalHP;
    public int HP { get; internal set; }
    // int - quantity round
    public Dictionary<ScriptableAttributeSpell, int> SpellsState = new();
}

[Serializable]
public class ArenaEntity
{
    public ArenaEntityData Data = new();
    public bool Death => Data.totalHP <= 0;
    public int Speed => Data.speed + Data.SpeedModificators.Values.Sum();


    private ArenaManager _arenaManager;
    public static event Action OnChangeParamsCreature;

    [SerializeField] public ScriptableEntity _configData;
    public ScriptableEntity ConfigData => _configData;
    [NonSerialized] public GridArenaNode OccupiedNode = null;
    [NonSerialized] private GridArenaNode _relatedNode = null;
    public GridArenaNode RelatedNode => _relatedNode;
    [NonSerialized] public GridArenaNode ProtectedNode = null;
    [NonSerialized] private List<GridArenaNode> _path = null;
    public List<GridArenaNode> Path => _path;
    private Vector3 _centerNode;
    public Vector3 CenterNode => _centerNode;
    private Vector3 _positionPrefab;
    public Vector3 PositionPrefab => _positionPrefab;
    [NonSerialized] public ArenaMonoBehavior ArenaMonoBehavior;
    public TypeArenaPlayer TypeArenaPlayer;
    public BaseEntity Entity { get; private set; }
    public TypeDirection Direction;
    private EntityHero _hero;
    public EntityHero Hero => _hero;

    public ArenaEntity(ArenaManager arenaManager, EntityHero hero)
    {
        _arenaManager = arenaManager;
        _hero = hero;
    }

    public void SetEntity(BaseEntity entity, GridArenaNode node)
    {
        Entity = entity;
        _configData = Entity.ScriptableData;
        OccupiedNode = node;
        Direction = TypeArenaPlayer == TypeArenaPlayer.Left ? TypeDirection.Right : TypeDirection.Left;

        var creatureData = ((ScriptableAttributeCreature)Entity.ScriptableDataAttribute);
        Data.shoots = creatureData.CreatureParams.Shoots;
        Data.speed = creatureData.CreatureParams.Speed;
        Data.typeMove = creatureData.CreatureParams.Movement;
        Data.quantity = ((EntityCreature)Entity).Data.value;
        Data.isDefense = false;
        SetRoundData();

        Data.damageMin = creatureData.CreatureParams.DamageMin;
        Data.damageMax = creatureData.CreatureParams.DamageMax;

        Data.defense = creatureData.CreatureParams.Defense;
        Data.attack = creatureData.CreatureParams.Attack;

        Data.HP = creatureData.CreatureParams.HP;
        Data.totalHP = Data.maxHP = creatureData.CreatureParams.HP * ((EntityCreature)Entity).Data.value;

        Data.isRun = true;
    }

    public async void SetRoundData()
    {
        Data.counterAttack = 1;
        Data.countAttack = 1;

        // TODO effects.
        var spells = Data.SpellsState.Keys.ToList();
        foreach (var spell in spells)
        {
            Data.SpellsState[spell] -= 1;
            if (Data.SpellsState[spell] <= 0)
            {
                await spell.RemoveEffect(OccupiedNode, Hero);
                Data.SpellsState.Remove(spell);
            }
        }
    }

    public void SetPosition(GridArenaNode node)
    {
        OccupiedNode.SetOcuppiedUnit(null);
        var creatureData = ((ScriptableAttributeCreature)Entity.ScriptableDataAttribute);
        var size = creatureData.CreatureParams.Size;

        if (size == 2)
        {
            NormalizeNode(node);
        }
        _centerNode = node.center; // new Vector3(size + .5f, node.positionPrefab.y, node.positionPrefab.z);
        _positionPrefab = size == 2
            ? node.center + new Vector3(TypeArenaPlayer == TypeArenaPlayer.Left ? -0.5f : 0.5f, 0, 0)
            : node.center;
        node.SetOcuppiedUnit(this);
        OccupiedNode = node;
    }

    private void NormalizeNode(GridArenaNode node)
    {
        if (RelatedNode != null && RelatedNode.OccupiedUnit == this)
        {
            RelatedNode.SetOcuppiedUnit(null);
            RelatedNode.SetRelatedStatus(false);
            _relatedNode = null;
        }

        if (
            // Direction == TypeDirection.Right
            TypeArenaPlayer == TypeArenaPlayer.Left
            && node.LeftNode != null
            && node.LeftNode.OccupiedUnit == null
            )
        {
            _relatedNode = node.LeftNode;
            // _positionPrefab = node.OccupiedUnit._centerNode + new Vector3(-0.5f, 0, 0);
            RelatedNode.SetOcuppiedUnit(this);
            RelatedNode.SetRelatedStatus(true);
        }
        else if (
        // Direction == TypeDirection.Left
        TypeArenaPlayer == TypeArenaPlayer.Right
        && node.RightNode != null
        && node.RightNode.OccupiedUnit == null
        )
        {
            _relatedNode = node.RightNode;
            // _positionPrefab = node.OccupiedUnit._centerNode + new Vector3(0.5f, 0, 0);
            RelatedNode.SetOcuppiedUnit(this);
            RelatedNode.SetRelatedStatus(true);
        }
    }

    public void SetDirection(TypeDirection direction)
    {
        Direction = direction;
    }

    public TypeDirection Rotate(GridArenaNode node)
    {
        var prevNode = node.cameFromNode;
        if (prevNode != null)
        {
            var difference = node.center.x - prevNode.center.x;
            if (difference < 0)
            {
                SetDirection(TypeDirection.Left);
                // RelatedNode.SetOcuppiedUnit(null);
                // _relatedNode = node.RightNode;
                // RelatedNode.SetOcuppiedUnit(this);
                // Debug.Log($"Rotate Left::: {node.RightNode}");
            }
            else if (difference > 0)
            {
                SetDirection(TypeDirection.Right);
                // RelatedNode.SetOcuppiedUnit(null);
                // _relatedNode = node.LeftNode;
                // RelatedNode.SetOcuppiedUnit(this);
                // Debug.Log($"Rotate Right::: {node.LeftNode}");
            }
        }

        return Direction;
    }

    public void ChangePosition(GridArenaNode node)
    {
        SetPosition(node);
    }

    public async void ClickCreature()
    {
        if (!_arenaManager.isRunningAction)
        {
            Debug.Log("ClickCreature");
            await AudioManager.Instance.Click();
            if (_arenaManager.FightingOccupiedNodes.Contains(OccupiedNode))
            {
                var activeSpell = _arenaManager.ArenaQueue.ActiveHero != null
                ? _arenaManager.ArenaQueue.ActiveHero.Data.SpellBook.ChoosedSpell
                : null;

                if (activeSpell == null)
                {
                    Debug.Log($"Choose creature for attack!");
                    _arenaManager.CreateAttackNode(this);
                }
                else
                {
                    Debug.Log($"Choose creature for spell!");
                    _arenaManager.CreateButtonSpell(OccupiedNode);
                }
            }
            else
            {
                Debug.Log($"Creature not maybe to attack!");
            }
        }
        await UniTask.Delay(1);
    }


    public void DoHero(Player player)
    {

    }
    public void SetTypeAttack(TypeAttack typeAttack)
    {
        Data.typeAttack = typeAttack;
    }

    public void SetPath(List<GridArenaNode> path)
    {
        _path = path;
    }

    public void SetDamage(int damage)
    {
        Data.totalHP -= damage;
        if (Death)
        {
            Data.totalHP = 0;
            Data.quantity = 0;
            _arenaManager.ArenaQueue.RemoveEntity(this);

            ArenaMonoBehavior.RunDeath();
            OccupiedNode.SetDeathedNode(this);
            OccupiedNode.SetOcuppiedUnit(null);
            if (RelatedNode != null)
            {
                RelatedNode.SetDeathedNode(this);
                RelatedNode.SetOcuppiedUnit(null);
            }
        }
        else
        {
            Data.quantity = (int)Math.Ceiling((double)Data.totalHP / (double)Data.HP);
        }
        Debug.Log($"Quantity::: quantity={Data.quantity},{Data.totalHP},{Data.HP}");
        OnChangeParamsCreature?.Invoke();
    }

    private void CalculateAttack(GridArenaNode nodeFromAttack, GridArenaNode nodeToAttack)
    {
        var randomDamage = new System.Random();
        int totalDamage = 0;
        int dopDamage = Data.DamageModificators.Values.Sum();
        if (Data.quantity <= 10)
        {
            for (int i = 0; i < Data.quantity; i++)
            {
                totalDamage += randomDamage.Next(Data.damageMin, Data.damageMax) + dopDamage;
            }
        }
        else
        {
            int totalRandomDamage = 0;
            for (int i = 0; i < 10; i++)
            {
                totalRandomDamage += randomDamage.Next(Data.damageMin, Data.damageMax) + dopDamage;
            }

            totalDamage = (int)Math.Ceiling(totalRandomDamage * (Data.quantity / 10.0));
        }

        Debug.Log($"{Entity.ScriptableDataAttribute.name} run damage {totalDamage}");
        var entityForAttack = nodeToAttack.OccupiedUnit;
        entityForAttack.SetDamage(totalDamage);
    }

    internal async UniTask GoAttack(GridArenaNode nodeFromAttack, GridArenaNode nodeToAttack)
    {
        var entityForAttack = nodeToAttack.OccupiedUnit;
        var typeAttack = _arenaManager.ArenaQueue.activeEntity.arenaEntity.Data.typeAttack;
        if (typeAttack == TypeAttack.Attack)
        {
            // Debug.Log($"Attack [{this.Entity.ScriptableDataAttribute.name}] / [{entityForAttack.Entity.ScriptableDataAttribute.name}]");

            for (int i = 0; i < Data.countAttack; i++)
            {
                if (
                    nodeToAttack.OccupiedUnit != null
                    && !nodeToAttack.OccupiedUnit.Death
                    && !Death
                    )
                {
                    await ArenaMonoBehavior.RunAttack(nodeFromAttack, nodeToAttack);
                    CalculateAttack(nodeFromAttack, nodeToAttack);
                    if (
                        nodeToAttack != OccupiedNode
                        && nodeToAttack.OccupiedUnit != null
                        && nodeToAttack.OccupiedUnit.Data.counterAttack > 0
                        && !Death
                        && nodeToAttack.OccupiedUnit.Data.attack > 0
                        )
                    {
                        await nodeToAttack.OccupiedUnit.GoCounterAttack(nodeToAttack, nodeFromAttack);
                    }
                }
            }
        }
        else if (typeAttack == TypeAttack.AttackShoot)
        {
            // Debug.Log($"ShootAttack [{this.Entity.ScriptableDataAttribute.name}] / [{entityForAttack.Entity.ScriptableDataAttribute.name}]");

            for (int i = 0; i < Data.countAttack; i++)
            {
                if (
                    nodeToAttack.OccupiedUnit != null
                    && !nodeToAttack.OccupiedUnit.Death
                    && !Death
                    )
                {
                    await ArenaMonoBehavior.RunAttackShoot(nodeToAttack);
                    CalculateAttack(nodeFromAttack, nodeToAttack);
                }
            }
        }
    }

    // internal async UniTask GoRunSpell()
    // {
    //     Debug.Log("GoRunSpell");
    //     var activeSpell = _arenaManager.ArenaQueue.ActiveHero != null
    //         ? _arenaManager.ArenaQueue.ActiveHero.Data.SpellBook.ChoosedSpell
    //         : null;

    //     if (activeSpell.AnimatePrefab.RuntimeKeyIsValid())
    //     {

    //         Debug.Log("InstantiateAsync");
    //         Addressables.InstantiateAsync(
    //             activeSpell.AnimatePrefab,
    //             new Vector3(0, 1, 0),
    //             Quaternion.identity,
    //             ArenaMonoBehavior.transform
    //             ).Completed += RunSpellResult;
    //     }
    //     else
    //     {
    //         await RunSpell();
    //     }

    //     await UniTask.Delay(1);
    // }

    // public async void RunSpellResult(AsyncOperationHandle<GameObject> handle)
    // {
    //     if (handle.Status == AsyncOperationStatus.Succeeded)
    //     {
    //         handle.Result.gameObject.transform.localPosition = new Vector3(0, 1, 0);
    //         await RunSpell();
    //         await UniTask.Delay(1000);
    //         Addressables.Release(handle);
    //     }
    //     else
    //     {
    //         Debug.LogError($"Error Load prefab::: {handle.Status}");
    //     }
    // }

    // internal async UniTask RunSpell()
    // {
    //     Debug.Log($"Run spell!");
    //     var heroRunSpell = _arenaManager.ArenaQueue.ActiveHero;
    //     var choosedSpell = _arenaManager.ArenaQueue.ActiveHero.Data.SpellBook.ChoosedSpell;
    //     await choosedSpell.AddEffect(this, heroRunSpell);
    //     if (choosedSpell.typeSpellDuration != TypeSpellDuration.Instant)
    //     {
    //         int countRound = heroRunSpell.Data.PSkills[TypePrimarySkill.Power];
    //         if (Data.SpellsState.ContainsKey(choosedSpell))
    //         {
    //             Data.SpellsState[choosedSpell] = countRound;
    //         }
    //         else
    //         {
    //             Data.SpellsState.Add(choosedSpell, countRound);
    //         }
    //     }
    //     // _arenaManager.ArenaQueue.ActiveHero.Data.SpellBook.ChooseSpell(null);
    //     // OnChangeParamsCreature?.Invoke();
    // }

    internal async UniTask GoCounterAttack(GridArenaNode nodeFromAttack, GridArenaNode nodeToAttack)
    {
        if (!Death)
        {
            await ArenaMonoBehavior.RunAttack(nodeFromAttack, nodeToAttack);
            CalculateAttack(nodeFromAttack, nodeToAttack);
            Data.counterAttack -= 1;
        }
    }

    internal async UniTask RunGettingHit(GridArenaNode attackNode)
    {
        await ArenaMonoBehavior.RunGettingHit(attackNode);

        var keys = Data.SpellsState.Keys.ToList();
        var listSpellOfAction = keys.Where(t => t.typeSpellDuration == TypeSpellDuration.RoundOrAction);
        foreach (var spell in listSpellOfAction)
        {
            await spell.RemoveEffect(OccupiedNode, _hero);
            Data.SpellsState.Remove(spell);
        }

    }


    #region CreateDestroy
    public void DestroyMapObject()
    {
        // OccupiedNode.SetOcuppiedUnit(null);
        // MapObjectGameObject.DestroyGameObject();
        // // MapObjectGameObject.DestroyMapObject();
        // UnitManager.MapObjects.Remove(IdMapObject);
    }
    public void CreateMapGameObject(GridArenaNode node)
    {
        // Debug.LogWarning($"CreateMapGameObject::: {Entity.ScriptableData.name}");
        // _positionPrefab = node.positionPrefab;
        // OccupiedNode = node;
        // Entity.SetMapObject(this);
        LoadGameObject();
    }
    private void LoadGameObject()
    {
        AssetReferenceGameObject gameObj = null;
        // if (ConfigData.MapPrefab.RuntimeKeyIsValid())
        // {
        //     gameObj = ((ScriptableAttributeCreature)confAtt).ArenaModel;
        // }
        var configCreature = ((ScriptableAttributeCreature)Entity.ScriptableDataAttribute);
        if (configCreature != null && configCreature.ArenaModel.RuntimeKeyIsValid())
        {
            gameObj = configCreature.ArenaModel;
        }

        if (gameObj == null)
        {
            Debug.LogWarning($"Not found ArenaPrefab {ConfigData.name}!");
            return;
        }

        Addressables.InstantiateAsync(
            gameObj,
            PositionPrefab, // + new Vector3(0, -.25f, 0),
            Quaternion.identity,
            _arenaManager.tileMapArenaUnits.transform
            ).Completed += LoadedAsset;
    }

    public virtual void LoadedAsset(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            var r_asset = handle.Result;
            ArenaMonoBehavior = r_asset.GetComponent<ArenaMonoBehavior>();
            // Debug.Log($"Spawn Entity::: {r_asset.name}");
            ArenaMonoBehavior.Init(this);
        }
        else
        {
            Debug.LogError($"Error Load prefab::: {handle.Status}");
        }
    }
    #endregion
}
