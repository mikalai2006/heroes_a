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

// [Serializable]
// public enum TypeArenaPlayer
// {
//     Left = 0,
//     Right = 1,
// }

// [Serializable]
// public enum TypeDirection
// {
//     Right = 0,
//     Left = 1
// }

// [Serializable]
// public enum TypeAttack
// {
//     Attack = 0,
//     AttackShoot = 1,
//     AttackWarMachine = 2,
//     AttackShootTown = 3,
//     // CatapultShoot = 2,
//     // AttackSpell = 2
// }

// // [Serializable]
// // public struct ModificatorItem {
// //     public int round;
// //     public ScriptableAttribute Spell;
// // }

// [Serializable]
// public class ArenaEntityData
// {
//     public bool isRun;
//     public int waitTick;
//     public TypeAttack typeAttack;
//     public int lucky;
//     public Dictionary<ScriptableAttribute, int> LuckyModificators = new();
//     public int attack;
//     public Dictionary<ScriptableAttribute, int> AttackModificators = new();
//     public bool isDefense;
//     public int defense;
//     public Dictionary<ScriptableAttribute, int> DefenseModificators = new();
//     public int speed;
//     public Dictionary<ScriptableAttribute, int> SpeedModificators = new();
//     public int fireDefense;
//     public Dictionary<ScriptableAttribute, int> FireDefenseModificators = new();
//     public int airDefense;
//     public Dictionary<ScriptableAttribute, int> AirDefenseModificators = new();
//     public int waterDefense;
//     public Dictionary<ScriptableAttribute, int> WaterDefenseModificators = new();
//     public MovementType typeMove;
//     public int shoots;
//     public int quantity;
//     public int counterAttack;
//     public int countAttack;
//     public int damageMin;
//     public int damageMax;
//     public Dictionary<ScriptableAttribute, int> DamageModificators = new();
//     public int maxHP;
//     public int totalHP;
//     public int HP { get; internal set; }
//     // int - quantity round
//     public Dictionary<ScriptableAttributeSpell, int> SpellsState = new();
// }

[Serializable]
public abstract class ArenaEntityBase
{
    public ArenaEntityData Data = new();
    public bool Death => Data.totalHP <= 0;
    public int Speed => Data.speed + Data.SpeedModificators.Values.Sum();

    protected ArenaManager _arenaManager;
    public static event Action OnChangeParamsCreature;

    [SerializeField] protected ScriptableEntity _configData;
    public ScriptableEntity ConfigData => _configData;
    [NonSerialized] public GridArenaNode _occupiedNode = null;
    public GridArenaNode OccupiedNode => _occupiedNode;
    [NonSerialized] protected GridArenaNode _relatedNode = null;
    public GridArenaNode RelatedNode => _relatedNode;
    // [NonSerialized] public GridArenaNode ProtectedNode = null;
    // [NonSerialized] private List<GridArenaNode> _path = null;
    // public List<GridArenaNode> Path => _path;
    protected Vector3 _centerNode;
    public Vector3 CenterNode => _centerNode;
    protected Vector3 _positionPrefab;
    public Vector3 PositionPrefab => _positionPrefab;
    // [NonSerialized] public ArenaMonoBehavior ArenaMonoBehavior;
    public TypeArenaPlayer TypeArenaPlayer;
    protected BaseEntity _entity;
    public BaseEntity Entity => _entity;
    // public TypeDirection Direction;
    protected EntityHero _hero;
    public EntityHero Hero => _hero;

    public virtual void Init(ArenaManager arenaManager, EntityHero hero)
    {
        _arenaManager = arenaManager;
        _hero = hero;
    }

    // public virtual void SetEntity(BaseEntity entity, GridArenaNode node)
    // {

    // }

    public virtual async UniTask SetRoundData()
    {
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

    public virtual void SetTypeAttack(TypeAttack typeAttack)
    {
        Data.typeAttack = typeAttack;
    }

    public virtual void ShowDialogInfo()
    {

    }
    public virtual void SetDamage(int damage)
    {

    }
    public async virtual UniTask GoCounterAttack(GridArenaNode nodeFromAttack, GridArenaNode nodeToAttack)
    {
        await UniTask.Yield();
    }
    public virtual async UniTask RunGettingHit(GridArenaNode attackNode)
    {
        await UniTask.Yield();
    }
    public virtual async UniTask RunGettingHitSpell()
    {
        await UniTask.Yield();
    }
    public virtual async UniTask GoAttack(GridArenaNode nodeFromAttack, GridArenaNode nodeToAttack)
    {
        await UniTask.Yield();
    }

    public void CalculateAttack(GridArenaNode nodeFromAttack, GridArenaNode nodeToAttack)
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

    public virtual async UniTask ClickButtonAction()
    {
        await UniTask.Yield();
    }

    public virtual void UpdateEntity()
    {
        OnChangeParamsCreature?.Invoke();
    }

    protected async UniTask RemoveSpellAction()
    {
        var keys = Data.SpellsState.Keys.ToList();
        var listSpellOfAction = keys.Where(t => t.typeSpellDuration == TypeSpellDuration.RoundOrAction);
        foreach (var spell in listSpellOfAction)
        {
            await spell.RemoveEffect(OccupiedNode, _hero);
            Data.SpellsState.Remove(spell);
        }
    }

    public virtual async UniTask GetFightingNodes()
    {
        await UniTask.Yield();
    }

    // public void SetPosition(GridArenaNode node)
    // {
    //     OccupiedNode.SetOcuppiedUnit(null);
    //     var creatureData = ((ScriptableAttributeCreature)Entity.ScriptableDataAttribute);
    //     var size = creatureData.CreatureParams.Size;

    //     if (size == 2)
    //     {
    //         NormalizeNode(node);
    //     }
    //     _centerNode = node.center; // new Vector3(size + .5f, node.positionPrefab.y, node.positionPrefab.z);
    //     _positionPrefab = size == 2
    //         ? node.center + new Vector3(TypeArenaPlayer == TypeArenaPlayer.Left ? -0.5f : 0.5f, 0, 0)
    //         : node.center;
    //     node.SetOcuppiedUnit(this);
    //     OccupiedNode = node;
    // }

    // private void NormalizeNode(GridArenaNode node)
    // {
    //     if (RelatedNode != null && RelatedNode.OccupiedUnit == this)
    //     {
    //         RelatedNode.SetOcuppiedUnit(null);
    //         RelatedNode.SetRelatedStatus(false);
    //         _relatedNode = null;
    //     }

    //     if (
    //         // Direction == TypeDirection.Right
    //         TypeArenaPlayer == TypeArenaPlayer.Left
    //         && node.LeftNode != null
    //         && node.LeftNode.OccupiedUnit == null
    //         )
    //     {
    //         _relatedNode = node.LeftNode;
    //         // _positionPrefab = node.OccupiedUnit._centerNode + new Vector3(-0.5f, 0, 0);
    //         RelatedNode.SetOcuppiedUnit(this);
    //         RelatedNode.SetRelatedStatus(true);
    //     }
    //     else if (
    //     // Direction == TypeDirection.Left
    //     TypeArenaPlayer == TypeArenaPlayer.Right
    //     && node.RightNode != null
    //     && node.RightNode.OccupiedUnit == null
    //     )
    //     {
    //         _relatedNode = node.RightNode;
    //         // _positionPrefab = node.OccupiedUnit._centerNode + new Vector3(0.5f, 0, 0);
    //         RelatedNode.SetOcuppiedUnit(this);
    //         RelatedNode.SetRelatedStatus(true);
    //     }
    // }

    // public void SetDirection(TypeDirection direction)
    // {
    //     Direction = direction;
    // }

    // public TypeDirection Rotate(GridArenaNode node)
    // {
    //     var prevNode = node.cameFromNode;
    //     if (prevNode != null)
    //     {
    //         var difference = node.center.x - prevNode.center.x;
    //         if (difference < 0)
    //         {
    //             SetDirection(TypeDirection.Left);
    //             // RelatedNode.SetOcuppiedUnit(null);
    //             // _relatedNode = node.RightNode;
    //             // RelatedNode.SetOcuppiedUnit(this);
    //             // Debug.Log($"Rotate Left::: {node.RightNode}");
    //         }
    //         else if (difference > 0)
    //         {
    //             SetDirection(TypeDirection.Right);
    //             // RelatedNode.SetOcuppiedUnit(null);
    //             // _relatedNode = node.LeftNode;
    //             // RelatedNode.SetOcuppiedUnit(this);
    //             // Debug.Log($"Rotate Right::: {node.LeftNode}");
    //         }
    //     }

    //     return Direction;
    // }

    // public void ChangePosition(GridArenaNode node)
    // {
    //     SetPosition(node);
    // }

    public async virtual UniTask ClickCreature()
    {
        // Exit if arena run.
        if (_arenaManager.isRunningAction)
        {
            return;
        }

        // Run sound click.
        await AudioManager.Instance.Click();

        _arenaManager.clickedNode = OccupiedNode;

        if (!_arenaManager.FightingOccupiedNodes.Contains(OccupiedNode))
        {
            Debug.Log($"Creature not maybe to attack!");
            _arenaManager.DrawNotAllowButton();
            return;
        }


        // if (!_arenaManager.isRunningAction)
        // {
        //     Debug.Log("ClickCreature");
        //     await AudioManager.Instance.Click();
        //     if (_arenaManager.FightingOccupiedNodes.Contains(OccupiedNode))
        //     {
        //         var activeSpell = _arenaManager.ArenaQueue.ActiveHero != null && _arenaManager.ArenaQueue.ActiveHero.SpellBook != null
        //         ? _arenaManager.ArenaQueue.ActiveHero.SpellBook.ChoosedSpell
        //         : null;

        //         if (activeSpell == null)
        //         {
        //             var creature = ((EntityCreature)_arenaManager.ArenaQueue.activeEntity.arenaEntity.Entity);
        //             switch (_arenaManager.ArenaQueue.activeEntity.arenaEntity.Data.typeAttack)
        //             {
        //                 case TypeAttack.AttackWarMachine:
        //                     _arenaManager.CreateButtonWarMachine(OccupiedNode);
        //                     break;
        //                 case TypeAttack.AttackShootTown:
        //                     Debug.Log($"Choose creature for shoot town attack!");
        //                     break;
        //                 default:
        //                     _arenaManager.CreateButtonAttackNode(this);
        //                     break;
        //             }
        //         }
        //         else
        //         {
        //             Debug.Log($"Choose creature for spell!");
        //             _arenaManager.CreateButtonSpell(OccupiedNode);
        //         }
        //     }
        //     else
        //     {
        //         Debug.Log($"Creature not maybe to attack!");
        //     }
        // }
        await UniTask.Delay(1);
    }

    public virtual void CreateButtonAttackNode(GridArenaNode clickedNode)
    {
        if (_arenaManager.ChoosedSpell != null) return;

        var allowNodes = _arenaManager.AllowPathNodes.Concat(_arenaManager.AllowMovedNodes).ToList();

        var neighbourNodesEnemyEntity = _arenaManager.ArenaQueue.activeEntity.arenaEntity.OccupiedNode
            .Neighbours()
            // GridArenaHelper
            // .GetNeighbourList(ArenaQueue.activeEntity.arenaEntity.OccupiedNode)
            .Where(t => t.OccupiedUnit != null && t.OccupiedUnit.TypeArenaPlayer != _arenaManager.ArenaQueue.activeEntity.arenaEntity.TypeArenaPlayer);
        // var activeSpell = ArenaQueue.ActiveHero.SpellBook != null
        //     ? ArenaQueue.ActiveHero.SpellBook.ChoosedSpell
        //     : null;
        if (
            // (_arenaManager.ArenaQueue.activeEntity.arenaEntity.Data.shoots == 0
            // || neighbourNodesEnemyEntity.Count() > 0)
            // &&
            _arenaManager.ArenaQueue.activeEntity.arenaEntity.Data.typeAttack == TypeAttack.Attack
            )
        {
            List<GridArenaNode> neighbours = clickedNode
                .Neighbours()
                // GridArenaHelper.GetNeighbourList(clickedEntity.OccupiedNode)
                .Where(t => t.OccupiedUnit == null || t.OccupiedUnit == _arenaManager.ArenaQueue.activeEntity.arenaEntity)
                .ToList();
            var allowNeighbours = neighbours.Intersect(allowNodes).ToList();
            foreach (var node in allowNeighbours)
            {
                _arenaManager.NodesForAttackActiveCreature.Add(new AttackItemNode()
                {
                    nodeFromAttack = node,
                    nodeToAttack = clickedNode
                });
            }

            if (clickedNode.OccupiedUnit.RelatedNode != null)
            {
                List<GridArenaNode> neighboursRelatedNode
                    = clickedNode.OccupiedUnit.RelatedNode.Neighbours()
                    // GridArenaHelper.GetNeighbourList(clickedEntity.RelatedNode)
                    .Where(t =>
                        (t.OccupiedUnit == null
                        || t.OccupiedUnit == _arenaManager.ArenaQueue.activeEntity.arenaEntity)
                        // && !allowNeighbours.Contains(t)
                        )
                    .ToList();
                // neighbours = neighbours.Concat(neighboursRelatedNode).ToList();
                var allowNeighboursRelatedNode = neighboursRelatedNode.Intersect(allowNodes).ToList();
                foreach (var node in allowNeighboursRelatedNode)
                {
                    _arenaManager.NodesForAttackActiveCreature.Add(new AttackItemNode()
                    {
                        nodeFromAttack = node,
                        nodeToAttack = clickedNode
                    });
                }
            }
        }
        else
        {
            _arenaManager.NodesForAttackActiveCreature.Add(new AttackItemNode()
            {
                nodeFromAttack = _arenaManager.ArenaQueue.activeEntity.arenaEntity.OccupiedNode,
                nodeToAttack = clickedNode
            });
        }

        // NodesForAttackActiveCreature = neighbours.Intersect(allowNodes).ToList();


        // GridArenaNode randomFirstNode
        //     = NodesForAttackActiveCreature[UnityEngine.Random.Range(0, NodesForAttackActiveCreature.Count)];
        // clickedNode = NodesForAttackActiveCreature[1];
        _arenaManager.ChooseNextPositionForAttack();
        _arenaManager.DrawAttackNodes();
        // _arenaManager.clickedNode = OccupiedNode;
    }

    public virtual async Task ColorPulse(Color color, int count)
    {
        await UniTask.Yield();
    }

    public virtual void CreateButton(GridArenaNode occupiedNode)
    {

    }


    // public void SetTypeAttack(TypeAttack typeAttack)
    // {
    //     Data.typeAttack = typeAttack;
    // }

    // public void SetPath(List<GridArenaNode> path)
    // {
    //     _path = path == null ? new() : path;
    // }

    // public void SetDamage(int damage)
    // {
    //     Data.totalHP -= damage;
    //     if (Death)
    //     {
    //         Data.totalHP = 0;
    //         Data.quantity = 0;
    //         _arenaManager.ArenaQueue.RemoveEntity(this);

    //         ArenaMonoBehavior.RunDeath();
    //         OccupiedNode.SetDeathedNode(this);
    //         OccupiedNode.SetOcuppiedUnit(null);
    //         if (RelatedNode != null)
    //         {
    //             RelatedNode.SetDeathedNode(this);
    //             RelatedNode.SetOcuppiedUnit(null);
    //         }
    //     }
    //     else
    //     {
    //         Data.quantity = (int)Math.Ceiling((double)Data.totalHP / (double)Data.HP);
    //     }
    //     Debug.Log($"Quantity::: quantity={Data.quantity},{Data.totalHP},{Data.HP}");
    //     OnChangeParamsCreature?.Invoke();
    // }

    // private void CalculateAttack(GridArenaNode nodeFromAttack, GridArenaNode nodeToAttack)
    // {
    //     var randomDamage = new System.Random();
    //     int totalDamage = 0;
    //     int dopDamage = Data.DamageModificators.Values.Sum();
    //     if (Data.quantity <= 10)
    //     {
    //         for (int i = 0; i < Data.quantity; i++)
    //         {
    //             totalDamage += randomDamage.Next(Data.damageMin, Data.damageMax) + dopDamage;
    //         }
    //     }
    //     else
    //     {
    //         int totalRandomDamage = 0;
    //         for (int i = 0; i < 10; i++)
    //         {
    //             totalRandomDamage += randomDamage.Next(Data.damageMin, Data.damageMax) + dopDamage;
    //         }

    //         totalDamage = (int)Math.Ceiling(totalRandomDamage * (Data.quantity / 10.0));
    //     }

    //     Debug.Log($"{Entity.ScriptableDataAttribute.name} run damage {totalDamage}");
    //     var entityForAttack = nodeToAttack.OccupiedUnit;
    //     entityForAttack.SetDamage(totalDamage);
    // }

    // internal async UniTask GoAttack(GridArenaNode nodeFromAttack, GridArenaNode nodeToAttack)
    // {
    //     var entityForAttack = nodeToAttack.OccupiedUnit;
    //     var typeAttack = _arenaManager.ArenaQueue.activeEntity.arenaEntity.Data.typeAttack;
    //     if (typeAttack == TypeAttack.Attack)
    //     {
    //         // Debug.Log($"Attack [{this.Entity.ScriptableDataAttribute.name}] / [{entityForAttack.Entity.ScriptableDataAttribute.name}]");

    //         for (int i = 0; i < Data.countAttack; i++)
    //         {
    //             if (
    //                 nodeToAttack.OccupiedUnit != null
    //                 && !nodeToAttack.OccupiedUnit.Death
    //                 && !Death
    //                 )
    //             {
    //                 await ArenaMonoBehavior.RunAttack(nodeFromAttack, nodeToAttack);
    //                 CalculateAttack(nodeFromAttack, nodeToAttack);
    //                 if (
    //                     nodeToAttack != OccupiedNode
    //                     && nodeToAttack.OccupiedUnit != null
    //                     && nodeToAttack.OccupiedUnit.Data.counterAttack > 0
    //                     && !Death
    //                     && nodeToAttack.OccupiedUnit.Data.attack > 0
    //                     )
    //                 {
    //                     await nodeToAttack.OccupiedUnit.GoCounterAttack(nodeToAttack, nodeFromAttack);
    //                 }
    //             }
    //         }
    //     }
    //     else if (typeAttack == TypeAttack.AttackShoot || typeAttack == TypeAttack.AttackShootTown)
    //     {
    //         // Debug.Log($"ShootAttack [{this.Entity.ScriptableDataAttribute.name}] / [{entityForAttack.Entity.ScriptableDataAttribute.name}]");

    //         for (int i = 0; i < Data.countAttack; i++)
    //         {
    //             if (
    //                 nodeToAttack.OccupiedUnit != null
    //                 && !nodeToAttack.OccupiedUnit.Death
    //                 && !Death
    //                 )
    //             {
    //                 await ArenaMonoBehavior.RunAttackShoot(nodeToAttack);
    //                 CalculateAttack(nodeFromAttack, nodeToAttack);
    //             }
    //         }
    //     }
    // }

    // // internal async UniTask GoRunSpell()
    // // {
    // //     Debug.Log("GoRunSpell");
    // //     var activeSpell = _arenaManager.ArenaQueue.ActiveHero != null
    // //         ? _arenaManager.ArenaQueue.ActiveHero.Data.SpellBook.ChoosedSpell
    // //         : null;

    // //     if (activeSpell.AnimatePrefab.RuntimeKeyIsValid())
    // //     {

    // //         Debug.Log("InstantiateAsync");
    // //         Addressables.InstantiateAsync(
    // //             activeSpell.AnimatePrefab,
    // //             new Vector3(0, 1, 0),
    // //             Quaternion.identity,
    // //             ArenaMonoBehavior.transform
    // //             ).Completed += RunSpellResult;
    // //     }
    // //     else
    // //     {
    // //         await RunSpell();
    // //     }

    // //     await UniTask.Delay(1);
    // // }

    // // public async void RunSpellResult(AsyncOperationHandle<GameObject> handle)
    // // {
    // //     if (handle.Status == AsyncOperationStatus.Succeeded)
    // //     {
    // //         handle.Result.gameObject.transform.localPosition = new Vector3(0, 1, 0);
    // //         await RunSpell();
    // //         await UniTask.Delay(1000);
    // //         Addressables.Release(handle);
    // //     }
    // //     else
    // //     {
    // //         Debug.LogError($"Error Load prefab::: {handle.Status}");
    // //     }
    // // }

    // // internal async UniTask RunSpell()
    // // {
    // //     Debug.Log($"Run spell!");
    // //     var heroRunSpell = _arenaManager.ArenaQueue.ActiveHero;
    // //     var choosedSpell = _arenaManager.ArenaQueue.ActiveHero.Data.SpellBook.ChoosedSpell;
    // //     await choosedSpell.AddEffect(this, heroRunSpell);
    // //     if (choosedSpell.typeSpellDuration != TypeSpellDuration.Instant)
    // //     {
    // //         int countRound = heroRunSpell.Data.PSkills[TypePrimarySkill.Power];
    // //         if (Data.SpellsState.ContainsKey(choosedSpell))
    // //         {
    // //             Data.SpellsState[choosedSpell] = countRound;
    // //         }
    // //         else
    // //         {
    // //             Data.SpellsState.Add(choosedSpell, countRound);
    // //         }
    // //     }
    // //     // _arenaManager.ArenaQueue.ActiveHero.Data.SpellBook.ChooseSpell(null);
    // //     // OnChangeParamsCreature?.Invoke();
    // // }

    // internal async UniTask GoCounterAttack(GridArenaNode nodeFromAttack, GridArenaNode nodeToAttack)
    // {
    //     if (!Death)
    //     {
    //         await ArenaMonoBehavior.RunAttack(nodeFromAttack, nodeToAttack);
    //         CalculateAttack(nodeFromAttack, nodeToAttack);
    //         Data.counterAttack -= 1;
    //     }
    // }

    // internal async UniTask RunGettingHit(GridArenaNode attackNode)
    // {
    //     await ArenaMonoBehavior.RunGettingHit(attackNode);

    //     var keys = Data.SpellsState.Keys.ToList();
    //     var listSpellOfAction = keys.Where(t => t.typeSpellDuration == TypeSpellDuration.RoundOrAction);
    //     foreach (var spell in listSpellOfAction)
    //     {
    //         await spell.RemoveEffect(OccupiedNode, _hero);
    //         Data.SpellsState.Remove(spell);
    //     }

    // }


    #region CreateDestroy
    // public void DestroyMapObject()
    // {
    //     // OccupiedNode.SetOcuppiedUnit(null);
    //     // MapObjectGameObject.DestroyGameObject();
    //     // // MapObjectGameObject.DestroyMapObject();
    //     // UnitManager.MapObjects.Remove(IdMapObject);
    // }
    // public async UniTask CreateMapGameObject(GridArenaNode node)
    // {
    //     // Debug.LogWarning($"CreateMapGameObject::: {Entity.ScriptableData.name}");
    //     // _positionPrefab = node.positionPrefab;
    //     // OccupiedNode = node;
    //     // Entity.SetMapObject(this);
    //     await LoadGameObject();
    // }
    // private async UniTask LoadGameObject()
    // {
    //     AssetReferenceGameObject gameObj = null;
    //     // if (ConfigData.MapPrefab.RuntimeKeyIsValid())
    //     // {
    //     //     gameObj = ((ScriptableAttributeCreature)confAtt).ArenaModel;
    //     // }
    //     var configCreature = ((ScriptableAttributeCreature)Entity.ScriptableDataAttribute);
    //     if (configCreature != null && configCreature.ArenaModel.RuntimeKeyIsValid())
    //     {
    //         gameObj = configCreature.ArenaModel;
    //     }
    //     if (gameObj == null)
    //     {
    //         if (configCreature.CreatureParams.Shoots != 0)
    //         {
    //             gameObj = LevelManager.Instance.ConfigGameSettings.ArenaPlaceholderShootModel;
    //         }
    //         else
    //         {
    //             gameObj = LevelManager.Instance.ConfigGameSettings.ArenaPlaceholderModel;
    //         }
    //     }
    //     if (gameObj == null)
    //     {
    //         Debug.LogWarning($"Not found ArenaPrefab {ConfigData.name}!");
    //         return;
    //     }

    //     var asset = Addressables.InstantiateAsync(
    //         gameObj,
    //         PositionPrefab, // + new Vector3(0, -.25f, 0),
    //         Quaternion.identity,
    //         _arenaManager.tileMapArenaUnits.transform
    //         );
    //     await asset.Task;
    //     ArenaMonoBehavior = asset.Result.GetComponent<ArenaMonoBehavior>();
    //     // Debug.Log($"Spawn Entity::: {r_asset.name}");
    //     ArenaMonoBehavior.Init(this);
    // }

    // // public virtual void LoadedAsset()
    // // {
    // //     if (handle.Status == AsyncOperationStatus.Succeeded)
    // //     {
    // //         var r_asset = handle.Result;
    // //         ArenaMonoBehavior = r_asset.GetComponent<ArenaMonoBehavior>();
    // //         // Debug.Log($"Spawn Entity::: {r_asset.name}");
    // //         ArenaMonoBehavior.Init(this);
    // //     }
    // //     else
    // //     {
    // //         Debug.LogError($"Error Load prefab::: {handle.Status}");
    // //     }
    // // }
    #endregion
}
