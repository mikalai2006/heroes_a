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
    AttackCatapult = 2,
    AttackShootTown = 3,
    Aid = 4,
    // CatapultShoot = 2,
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
    public int airDefense;
    public Dictionary<ScriptableAttribute, int> AirDefenseModificators = new();
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
public class ArenaCreature : ArenaEntityBase
{
    // public ArenaEntityData Data = new();
    // public bool Death => Data.totalHP <= 0;
    // public int Speed => Data.speed + Data.SpeedModificators.Values.Sum();


    // private ArenaManager _arenaManager;
    // public static event Action OnChangeParamsCreature;

    // [SerializeField] protected ScriptableEntity _configData;
    // public ScriptableEntity ConfigData => _configData;
    // [NonSerialized] public GridArenaNode OccupiedNode = null;
    // [NonSerialized] private GridArenaNode _relatedNode = null;
    // public GridArenaNode RelatedNode => _relatedNode;
    // [NonSerialized] public GridArenaNode ProtectedNode = null;
    [NonSerialized] private List<GridArenaNode> _path = null;
    public List<GridArenaNode> Path => _path;
    // private Vector3 _centerNode;
    // public Vector3 CenterNode => _centerNode;
    // private Vector3 _positionPrefab;
    // public Vector3 PositionPrefab => _positionPrefab;
    [NonSerialized] public ArenaCreatureMB ArenaMonoBehavior;
    // public TypeArenaPlayer TypeArenaPlayer;
    // public BaseEntity Entity { get; private set; }
    public TypeDirection Direction;
    // private EntityHero _hero;
    // public EntityHero Hero => _hero;

    public override void Init(ArenaManager arenaManager, EntityHero hero)
    {
        base.Init(arenaManager, hero);
    }

    public async void SetEntity(BaseEntity entity, GridArenaNode node)
    {
        _entity = entity;
        _configData = Entity.ScriptableData;
        _occupiedNode = node;
        Direction = TypeArenaPlayer == TypeArenaPlayer.Left ? TypeDirection.Right : TypeDirection.Left;

        var creatureData = ((ScriptableAttributeCreature)Entity.ScriptableDataAttribute);
        Data.shoots = creatureData.CreatureParams.Shoots;
        Data.speed = creatureData.CreatureParams.Speed;
        Data.typeMove = creatureData.CreatureParams.Movement;
        Data.quantity = ((EntityCreature)Entity).Data.value;
        Data.isDefense = false;
        await SetRoundData();

        Data.damageMin = creatureData.CreatureParams.DamageMin;
        Data.damageMax = creatureData.CreatureParams.DamageMax;

        Data.defense = creatureData.CreatureParams.Defense;
        Data.attack = creatureData.CreatureParams.Attack;

        Data.HP = creatureData.CreatureParams.HP;
        Data.totalHP = Data.maxHP = creatureData.CreatureParams.HP * ((EntityCreature)Entity).Data.value;

        Data.typeAttack = creatureData.CreatureParams.Shoots > 0 ? TypeAttack.AttackShoot : TypeAttack.Attack;

        Data.isRun = true;
    }

    public override async UniTask SetRoundData()
    {
        Data.counterAttack = 1;
        Data.countAttack = 1;


        // // TODO effects.
        // var spells = Data.SpellsState.Keys.ToList();
        // foreach (var spell in spells)
        // {
        //     Data.SpellsState[spell] -= 1;
        //     if (Data.SpellsState[spell] <= 0)
        //     {
        //         await spell.RemoveEffect(OccupiedNode, Hero);
        //         Data.SpellsState.Remove(spell);
        //     }
        // }
        await base.SetRoundData();
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
        _occupiedNode = node;
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

    public async UniTask OpenDoor()
    {
        Debug.Log("Wait door");
        await _arenaManager.town.OpenDoor();
        await UniTask.Yield();
    }

    public async void CloseDoor()
    {
        await UniTask.Delay(400);
        Debug.Log("Wait door");
        _arenaManager.town.CloseDoor();
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

    public override async UniTask ClickCreature()
    {
        await base.ClickCreature();
        if (!_arenaManager.FightingOccupiedNodes.Contains(OccupiedNode)) return;

        // var creature = ((EntityCreature)_arenaManager.ArenaQueue.activeEntity.arenaEntity.Entity);
        // switch (_arenaManager.ArenaQueue.activeEntity.arenaEntity.Data.typeAttack)
        // {
        //     case TypeAttack.AttackWarMachine:
        //         _arenaManager.CreateButtonWarMachine(OccupiedNode);
        //         break;
        //     case TypeAttack.AttackShootTown:
        //         _arenaManager.ArenaQueue.activeEntity.arenaEntity.CreateButtonAttackNode();
        //         Debug.Log($"Choose creature for shoot town attack!");
        //         break;
        //     default:

        // Choose spell.
        var activeSpell = _arenaManager.ArenaQueue.ActiveHero != null && _arenaManager.ArenaQueue.ActiveHero.SpellBook != null
        ? _arenaManager.ArenaQueue.ActiveHero.SpellBook.ChoosedSpell
        : null;
        if (activeSpell != null)
        {
            Debug.Log($"Choose creature for spell!");
            _arenaManager.CreateButtonSpell(OccupiedNode);
            // return;
        }
        else
        {
            Debug.Log($"{GetType()}::: Choose creature node for attack {OccupiedNode}!");
            if (_arenaManager.AttackedCreature == this)
            {
                _arenaManager.ChooseNextPositionForAttack();
            }
            else
            {
                if (_arenaManager.AttackedCreature != this && _arenaManager.AttackedCreature != null)
                {
                    _arenaManager.ClearAttackNode();
                }
                _arenaManager.clickedNode = OccupiedNode;
                _arenaManager.AttackedCreature = this;
                _arenaManager.ArenaQueue.activeEntity.arenaEntity.CreateButtonAttackNode(OccupiedNode);
                Debug.Log($"{GetType()}::: _arenaManager.clickedNode= {_arenaManager.clickedNode}!");
            }
        }
        //         break;
        // }
        // if (creature.ConfigAttribute.TypeAttribute == TypeAttribute.Creature)
        // {
        //     Debug.Log($"Choose creature for attack!");
        //     _arenaManager.CreateButtonAttackNode(this);
        // }
        // else
        // {
        //     Debug.Log($"Choose target for war machine!");
        //     _arenaManager.CreateButtonWarMachine(OccupiedNode);
        // }
    }

    // public override void CreateButtonAttackNode(GridArenaNode clickedNode)
    // {
    //     var allowNodes = _arenaManager.AllowPathNodes.Concat(_arenaManager.AllowMovedNodes).ToList();

    //     var neighbourNodesEnemyEntity = _arenaManager.ArenaQueue.activeEntity.arenaEntity.OccupiedNode
    //         .Neighbours()
    //         // GridArenaHelper
    //         // .GetNeighbourList(ArenaQueue.activeEntity.arenaEntity.OccupiedNode)
    //         .Where(t => t.OccupiedUnit != null && t.OccupiedUnit.TypeArenaPlayer != _arenaManager.ArenaQueue.activeEntity.arenaEntity.TypeArenaPlayer);
    //     // var activeSpell = ArenaQueue.ActiveHero.SpellBook != null
    //     //     ? ArenaQueue.ActiveHero.SpellBook.ChoosedSpell
    //     //     : null;
    //     if (
    //         (_arenaManager.ArenaQueue.activeEntity.arenaEntity.Data.shoots == 0
    //         || neighbourNodesEnemyEntity.Count() > 0)
    //         && _arenaManager.ChoosedSpell == null
    //         )
    //     {
    //         List<GridArenaNode> neighbours = clickedNode
    //             .Neighbours()
    //             // GridArenaHelper.GetNeighbourList(clickedEntity.OccupiedNode)
    //             .Where(t => t.OccupiedUnit == null || t.OccupiedUnit == _arenaManager.ArenaQueue.activeEntity.arenaEntity)
    //             .ToList();
    //         var allowNeighbours = neighbours.Intersect(allowNodes).ToList();
    //         foreach (var node in allowNeighbours)
    //         {
    //             _arenaManager.NodesForAttackActiveCreature.Add(new AttackItemNode()
    //             {
    //                 nodeFromAttack = node,
    //                 nodeToAttack = clickedNode
    //             });
    //         }

    //         if (RelatedNode != null)
    //         {
    //             List<GridArenaNode> neighboursRelatedNode
    //                 = RelatedNode.Neighbours()
    //                 // GridArenaHelper.GetNeighbourList(clickedEntity.RelatedNode)
    //                 .Where(t => t.OccupiedUnit == null || t.OccupiedUnit == _arenaManager.ArenaQueue.activeEntity.arenaEntity)
    //                 .ToList();
    //             // neighbours = neighbours.Concat(neighboursRelatedNode).ToList();
    //             var allowNeighboursRelatedNode = neighboursRelatedNode.Intersect(allowNodes).ToList();
    //             foreach (var node in allowNeighboursRelatedNode)
    //             {
    //                 _arenaManager.NodesForAttackActiveCreature.Add(new AttackItemNode()
    //                 {
    //                     nodeFromAttack = node,
    //                     nodeToAttack = RelatedNode
    //                 });
    //             }
    //         }
    //     }
    //     else
    //     {
    //         _arenaManager.NodesForAttackActiveCreature.Add(new AttackItemNode()
    //         {
    //             nodeFromAttack = _arenaManager.ArenaQueue.activeEntity.arenaEntity.OccupiedNode,
    //             nodeToAttack = clickedNode
    //         });
    //     }

    //     // NodesForAttackActiveCreature = neighbours.Intersect(allowNodes).ToList();


    //     // GridArenaNode randomFirstNode
    //     //     = NodesForAttackActiveCreature[UnityEngine.Random.Range(0, NodesForAttackActiveCreature.Count)];
    //     // clickedNode = NodesForAttackActiveCreature[1];
    //     _arenaManager.ChooseNextPositionForAttack();
    //     _arenaManager.DrawAttackNodes();
    //     // _arenaManager.clickedNode = OccupiedNode;
    // }

    public override async UniTask ClickButtonAction()
    {
        Debug.Log($"{GetType()}::: ClickButtonAction");
        _arenaManager.isRunningAction = true;

        await AudioManager.Instance.Click();

        if (_arenaManager.activeCursor == _arenaManager.CursorRule.NotAllow)
        {
            _arenaManager.isRunningAction = false;
            return;
        };

        if (
            ((_arenaManager.AttackedCreature != null && _arenaManager.ArenaQueue.activeEntity.arenaEntity.Data.typeAttack == TypeAttack.Attack)
            || _arenaManager.AttackedCreature == null)
            && _arenaManager.ArenaQueue.activeEntity.arenaEntity.Data.speed > 0
            )
        {
            // Move creature.
            await ((ArenaCreature)_arenaManager.ArenaQueue.activeEntity.arenaEntity).ArenaMonoBehavior.MoveCreature();
        }

        // Attack, if exist KeyNodeFromAttack
        if (_arenaManager.AttackedCreature != null)
        {
            var nodes = _arenaManager.NodesForAttackActiveCreature[_arenaManager.KeyNodeFromAttack];
            if (nodes.nodeFromAttack.OccupiedUnit != null)
            {
                await nodes.nodeFromAttack.OccupiedUnit.GoAttack(nodes.nodeFromAttack, nodes.nodeToAttack);
            }
        }

        // Clear clicked node.
        _arenaManager.clickedNode = null;
        _arenaManager.ClearAttackNode();

        // Next creature.
        // await GoEntity();
        _arenaManager.NextCreature(false, false);

        // DrawCursor
        // if (_arenaManager.clickedNode != null) DrawButtonAction();

        _arenaManager.isRunningAction = false;
    }

    // private void ChooseNextPositionForAttack()
    // {
    //     _arenaManager.ChooseNextPositionForAttack();
    // }

    private void DrawButtonAction()
    {

        // RuleTile ruleCursor = CursorRule.NotAllow;

        // // ScriptableAttributeSpell activeSpell = ArenaQueue.ActiveHero != null
        // //     ? ArenaQueue.ActiveHero.SpellBook.ChoosedSpell
        // //     : null;
        // Vector3 positionButton = new Vector3(clickedNode.center.x, clickedNode.center.y, zCoord);

        // if (ChoosedSpell == null)
        // {
        //     ScriptableAttributeCreature activeCreatureData
        //         = (ScriptableAttributeCreature)ArenaQueue.activeEntity.arenaEntity.Entity.ScriptableDataAttribute;

        //     if (AllowPathNodes.Contains(clickedNode))
        //     {
        //         ruleCursor = activeCreatureData.CreatureParams.Movement == MovementType.Flying
        //             ? CursorRule.GoFlying
        //             : CursorRule.GoGround;
        //     }
        //     if (NodesForAttackActiveCreature.Count > 0)
        //     {
        //         if (KeyNodeFromAttack != -1 && AttackedCreature != null)
        //         {
        //             var nodesForAttack = NodesForAttackActiveCreature[KeyNodeFromAttack];
        //             // var neighboursNodesEnemy = GridArenaHelper
        //             //     .GetNeighbourList(ArenaQueue.activeEntity.OccupiedNode)
        //             //     .Where(t => t.OccupiedUnit != null && t.OccupiedUnit.TypeArenaPlayer != ArenaQueue.activeEntity.TypeArenaPlayer);
        //             if (ChoosedSpell == null)
        //             {
        //                 ruleCursor = CursorRule.FightFromLeft;
        //                 if (ArenaQueue.activeEntity.arenaEntity.Data.shoots > 0 && ArenaQueue.activeEntity.arenaEntity.Data.typeAttack == TypeAttack.AttackShoot)
        //                 {
        //                     // check distance.
        //                     if (nodesForAttack.nodeToAttack.DistanceTo(ArenaQueue.activeEntity.arenaEntity.OccupiedNode) <= ArenaQueue.activeEntity.arenaEntity.Speed)
        //                     {
        //                         ruleCursor = CursorRule.Shoot;
        //                     }
        //                     else
        //                     {
        //                         ruleCursor = CursorRule.ShootHalf;
        //                     }
        //                     positionButton = new Vector3(nodesForAttack.nodeToAttack.center.x, nodesForAttack.nodeToAttack.center.y, zCoord);
        //                 }
        //                 else
        //                 {
        //                     Vector3 difPos = nodesForAttack.nodeFromAttack.center - nodesForAttack.nodeToAttack.center;
        //                     if (difPos.x > 0 && difPos.y > 0)
        //                     {
        //                         ruleCursor = CursorRule.FightFromTopRight;
        //                     }
        //                     else if (difPos.x < 0 && difPos.y > 0)
        //                     {
        //                         ruleCursor = CursorRule.FightFromTopLeft;
        //                     }
        //                     else if (difPos.x > 0 && difPos.y == 0)
        //                     {
        //                         ruleCursor = CursorRule.FightFromRight;
        //                     }
        //                     else if (difPos.x < 0 && difPos.y == 0)
        //                     {
        //                         ruleCursor = CursorRule.FightFromLeft;
        //                     }
        //                     else if (difPos.x < 0 && difPos.y < 0)
        //                     {
        //                         ruleCursor = CursorRule.FightFromBottomLeft;
        //                     }
        //                     else if (difPos.x > 0 && difPos.y < 0)
        //                     {
        //                         ruleCursor = CursorRule.FightFromBottomRight;
        //                     }
        //                     // _buttonAction.transform.position = new Vector3(clickedNode.center.x, clickedNode.center.y, -5);
        //                 }
        //             }
        //             else
        //             {
        //                 // Spell cursor.
        //                 ruleCursor = CursorRule.Spell;
        //                 positionButton = new Vector3(nodesForAttack.nodeToAttack.center.x, nodesForAttack.nodeToAttack.center.y, zCoord);
        //             }
        //         }
        //     }
        // }

        // _buttonAction.SetActive(true);
        // _buttonSpell.SetActive(false);
        // _buttonWarMachine.SetActive(false);
        // _buttonAction.GetComponent<SpriteRenderer>().sprite = ruleCursor.m_DefaultSprite;
        // _buttonAction.transform.position = positionButton;
        // activeCursor = ruleCursor;
        // // _tileMapCursor.SetTile(clickedNode.position, ruleCursor);
    }


    public void DoHero(Player player)
    {

    }
    public override void SetTypeAttack(TypeAttack typeAttack)
    {
        //Data.typeAttack = typeAttack;
        base.SetTypeAttack(typeAttack);
    }

    public void SetPath(List<GridArenaNode> path)
    {
        _path = path == null ? new() : path;
    }

    public override void SetDamage(int damage)
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
        // OnChangeParamsCreature?.Invoke();
        UpdateEntity();
    }

    public override async UniTask GoAttack(GridArenaNode nodeFromAttack, GridArenaNode nodeToAttack)
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
        else if (typeAttack == TypeAttack.AttackShoot || typeAttack == TypeAttack.AttackShootTown)
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

    public override async UniTask GoCounterAttack(GridArenaNode nodeFromAttack, GridArenaNode nodeToAttack)
    {
        if (!Death)
        {
            await ArenaMonoBehavior.RunAttack(nodeFromAttack, nodeToAttack);
            CalculateAttack(nodeFromAttack, nodeToAttack);
            Data.counterAttack -= 1;
        }
    }

    public override async UniTask RunGettingHit(GridArenaNode attackNode)
    {
        await ArenaMonoBehavior.RunGettingHit(attackNode);
        await RemoveSpellAction();
    }

    public override async UniTask RunGettingHitSpell()
    {
        await ArenaMonoBehavior.RunGettingHitSpell();
        await RemoveSpellAction();
    }

    public override async Task ColorPulse(Color color, int count)
    {
        await ArenaMonoBehavior.ColorPulse(color, count);
    }

    public override void ShowDialogInfo()
    {
        ArenaMonoBehavior.ShowDialogInfo();
    }

    #region CreateDestroy
    public void DestroyMapObject()
    {
        // OccupiedNode.SetOcuppiedUnit(null);
        // MapObjectGameObject.DestroyGameObject();
        // // MapObjectGameObject.DestroyMapObject();
        // UnitManager.MapObjects.Remove(IdMapObject);
    }
    public async UniTask CreateMapGameObject(GridArenaNode node)
    {
        // Debug.LogWarning($"CreateMapGameObject::: {Entity.ScriptableData.name}");
        // _positionPrefab = node.positionPrefab;
        // OccupiedNode = node;
        // Entity.SetMapObject(this);
        await LoadGameObject();
    }
    private async UniTask LoadGameObject()
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
            if (configCreature.CreatureParams.Shoots != 0)
            {
                gameObj = LevelManager.Instance.ConfigGameSettings.ArenaPlaceholderShootModel;
            }
            else
            {
                gameObj = LevelManager.Instance.ConfigGameSettings.ArenaPlaceholderModel;
            }
        }
        if (gameObj == null)
        {
            Debug.LogWarning($"Not found ArenaPrefab {ConfigData.name}!");
            return;
        }

        var asset = Addressables.InstantiateAsync(
            gameObj,
            PositionPrefab, // + new Vector3(0, -.25f, 0),
            Quaternion.identity,
            _arenaManager.tileMapArenaUnits.transform
            );
        await asset.Task;
        ArenaMonoBehavior = asset.Result.GetComponent<ArenaCreatureMB>();
        // Debug.Log($"Spawn Entity::: {r_asset.name}");
        ArenaMonoBehavior.Init(this);
    }

    // public virtual void LoadedAsset()
    // {
    //     if (handle.Status == AsyncOperationStatus.Succeeded)
    //     {
    //         var r_asset = handle.Result;
    //         ArenaMonoBehavior = r_asset.GetComponent<ArenaMonoBehavior>();
    //         // Debug.Log($"Spawn Entity::: {r_asset.name}");
    //         ArenaMonoBehavior.Init(this);
    //     }
    //     else
    //     {
    //         Debug.LogError($"Error Load prefab::: {handle.Status}");
    //     }
    // }
    #endregion

    public async override UniTask GetFightingNodes()
    {
        // _tileMapAllowAttack.ClearAllTiles();
        await _arenaManager.CreateMoveArea();

        var arenaEntity = _arenaManager.ArenaQueue.activeEntity.arenaEntity;

        var creatureData = ((ScriptableAttributeCreature)arenaEntity.Entity.ScriptableDataAttribute);

        var occupiedNodes = _arenaManager.GridArenaHelper
            .GetAllGridNodes()
            .Where(t =>
                t.OccupiedUnit != null
                && t.OccupiedUnit.TypeArenaPlayer != arenaEntity.TypeArenaPlayer
                // && !t.StateArenaNode.HasFlag(StateArenaNode.Deathed)
                );

        List<GridArenaNode> neighboursNodesEnemy = new();
        if (creatureData.CreatureParams.Shoots != 0)
        {
            neighboursNodesEnemy = arenaEntity.OccupiedNode
                .Neighbours()
                // GridArenaHelper
                //     .GetNeighbourList(arenaEntity.OccupiedNode)
                .Where(t =>
                    t.OccupiedUnit != null
                    && t.OccupiedUnit.TypeArenaPlayer != arenaEntity.TypeArenaPlayer
                    // && !t.StateArenaNode.HasFlag(StateArenaNode.Deathed)
                    )
                .ToList();
        }

        foreach (var node in occupiedNodes)
        {
            if (
                (arenaEntity.Data.shoots == 0
                ||
                (creatureData.CreatureParams.Shoots != 0 && neighboursNodesEnemy.Count() > 0))
                )
            {
                arenaEntity.SetTypeAttack(TypeAttack.Attack);
                var neighbours = node.Neighbours(); //GridArenaHelper.GetNeighbourList(node);
                if (_arenaManager.AllowPathNodes.Intersect(neighbours).Count() > 0)
                {
                    _arenaManager.FightingOccupiedNodes.Add(node);
                    _arenaManager.SetColorAllowFightNode(node, LevelManager.Instance.ConfigGameSettings.colorAllowAttackCreature);
                }
            }
            else
            {
                arenaEntity.SetTypeAttack(TypeAttack.AttackShoot);
                _arenaManager.FightingOccupiedNodes.Add(node);
                _arenaManager.SetColorAllowFightNode(node, LevelManager.Instance.ConfigGameSettings.colorAllowAttackCreature);
            }
        }

        await UniTask.Yield();
    }
}
