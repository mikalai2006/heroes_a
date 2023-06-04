using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;

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
    // AttackShootTown = 3,
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
    public Dictionary<ScriptableAttribute, int> AttackShootModificators = new();
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
    public int totalRecoveryHP;
    public int HP { get; internal set; }
    public Dictionary<ScriptableAttribute, int> HPModificators = new();
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
    private GridArenaNode nodeBridge;
    // private EntityHero _hero;
    // public EntityHero Hero => _hero;
    private GridArenaNode lastClickedNode;

    public override void Init(ArenaManager arenaManager, ArenaHeroEntity hero)
    {
        base.Init(arenaManager, hero);
    }

    public async void SetEntity(BaseEntity entity, GridArenaNode node)
    {
        Hero.Data.ArenaCreatures.Add(entity, this);
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

    public async UniTask OpenBridge(GridArenaNode nodeForCheckBridge)
    {
        if (arenaManager.ArenaTown == null) return;
        // if (_arenaManager.town.ArenaEntityTownMB.isOpenBridge) return;

        if (nodeForCheckBridge.StateArenaNode.HasFlag(StateArenaNode.Bridge))
        {
            nodeBridge = nodeForCheckBridge;
            nodeForCheckBridge.StateArenaNode |= StateArenaNode.OpenBridge;
        }

        if (nodeForCheckBridge.RightNode != null
            && nodeForCheckBridge.RightNode.StateArenaNode.HasFlag(StateArenaNode.Bridge)
            && TypeArenaPlayer == TypeArenaPlayer.Right
            )
        {
            nodeBridge = nodeForCheckBridge.RightNode;
            nodeForCheckBridge.RightNode.StateArenaNode |= StateArenaNode.OpenBridge;
        }

        if (nodeForCheckBridge.RightNode.RightNode != null
            && nodeForCheckBridge.RightNode.RightNode.StateArenaNode.HasFlag(StateArenaNode.Bridge)
            && TypeArenaPlayer == TypeArenaPlayer.Right
            )
        {
            nodeBridge = nodeForCheckBridge.RightNode.RightNode;
            nodeForCheckBridge.RightNode.RightNode.StateArenaNode |= StateArenaNode.OpenBridge;
        }

        if (nodeBridge != null)
        {
            await arenaManager.ArenaTown.OpenBridge();
        }

        await UniTask.Yield();
    }

    public async void CloseBridge(GridArenaNode nodeEnd)
    {
        if (nodeBridge == null) return;

        // Check Bridge.
        Debug.Log($"nodeBridge={nodeBridge}");
        if (
            nodeBridge != nodeEnd
            && !nodeBridge.StateArenaNode.HasFlag(StateArenaNode.Occupied)
            && !nodeBridge.StateArenaNode.HasFlag(StateArenaNode.Deathed)
            && !nodeBridge.LeftNode.StateArenaNode.HasFlag(StateArenaNode.Deathed)
            && !nodeBridge.LeftNode.StateArenaNode.HasFlag(StateArenaNode.Occupied)
            )
        {
            await UniTask.Delay(400);
            nodeBridge.StateArenaNode &= ~StateArenaNode.OpenBridge;
            nodeBridge = null;
            arenaManager.ArenaTown.CloseBridge();
        }
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
            }
            else if (difference > 0)
            {
                SetDirection(TypeDirection.Right);
            }
        }

        return Direction;
    }

    public void ChangePosition(GridArenaNode node)
    {
        SetPosition(node);
    }

    public override async UniTask ClickCreature(Vector3Int clickPosition)
    {
        await base.ClickCreature(clickPosition);

        // GridArenaNode nodeByClickPosition = arenaManager.GridArenaHelper.GridTile.GetGridObject(clickPosition);

        if (!arenaManager.FightingOccupiedNodes.Contains(arenaManager.clickedNode)) return;

        // Choose spell.
        var activeSpell = arenaManager.ArenaQueue.ActiveHero != null
            && arenaManager.ArenaQueue.ActiveHero.Entity != null
            && arenaManager.ArenaQueue.ActiveHero.Entity.SpellBook != null
                ? arenaManager.ArenaQueue.ActiveHero.Entity.SpellBook.ChoosedSpell
                : null;
        if (activeSpell != null)
        {
            Debug.Log($"Choose creature for spell!");
            arenaManager.CreateButtonSpell(OccupiedNode);
            // return;
        }
        else
        {
            Debug.Log($"{GetType()}::: Choose creature node for attack {OccupiedNode}!");
            if (arenaManager.AttackedCreature == this && arenaManager.clickedNode == lastClickedNode)
            {
                arenaManager.ChooseNextPositionForAttack();
            }
            else
            {
                // if (arenaManager.AttackedCreature != this && arenaManager.AttackedCreature != null)
                // {
                arenaManager.ClearAttackNode();
                // }
                // arenaManager.clickedNode = nodeByClickPosition; //OccupiedNode;
                arenaManager.AttackedCreature = this;
                lastClickedNode = arenaManager.clickedNode;
                arenaManager.ArenaQueue.activeEntity.arenaEntity.CreateButtonAttackNode(arenaManager.clickedNode);
                Debug.Log($"{GetType()}::: _arenaManager.clickedNode= {arenaManager.clickedNode}!");
            }
        }

        await UniTask.Yield();
    }

    public override async UniTask ClickButtonAction()
    {
        Debug.Log($"{GetType()}::: ClickButtonAction");
        arenaManager.isRunningAction = true;

        if (!arenaManager.ArenaQueue.ActiveHero.Data.autoRun) await AudioManager.Instance.Click();

        if (
            arenaManager.activeCursor == arenaManager.CursorRule.NotAllow
            && !arenaManager.ArenaQueue.ActiveHero.Data.autoRun
        )
        {
            arenaManager.isRunningAction = false;
            return;
        };

        if (
            ((arenaManager.AttackedCreature != null && arenaManager.ArenaQueue.activeEntity.arenaEntity.Data.typeAttack == TypeAttack.Attack)
            || arenaManager.AttackedCreature == null)
            && arenaManager.ArenaQueue.activeEntity.arenaEntity.Data.speed > 0
            )
        {
            // Move creature.
            await MoveCreature();
        }

        // Attack, if exist KeyNodeFromAttack
        if (arenaManager.AttackedCreature != null)
        {
            var nodes = arenaManager.NodesForAttackActiveCreature[arenaManager.KeyNodeFromAttack];
            if (nodes.nodeFromAttack.OccupiedUnit != null)
            {
                await nodes.nodeFromAttack.OccupiedUnit.GoAttack(nodes.nodeFromAttack, nodes.nodeToAttack);
            }
        }

        // // Clear clicked node.
        // arenaManager.clickedNode = null;
        // arenaManager.ClearAttackNode();

        // // Next creature.
        // // await GoEntity();
        arenaManager.isRunningAction = false;
        await arenaManager.NextCreature(false, false);

        // DrawCursor
        // if (_arenaManager.clickedNode != null) DrawButtonAction();

    }

    public override async UniTask MoveCreature()
    {
        await base.MoveCreature();
        var name = Helpers.GetNameByValue(((EntityCreature)Entity).ConfigAttribute.Text.title, Data.quantity);

        var dataSmart = new Dictionary<string, object> {
            { "name",  name},
            { "value", Data.quantity}
            };
        var arguments = new[] { dataSmart };
        var textSmart = Helpers.GetLocalizedPluralString(
            new LocalizedString(Constants.LanguageTable.LANG_STAT, "move_creature"),
            arguments,
            dataSmart
            );
        arenaManager.ArenaStat.AddItem(textSmart);

        await ((ArenaCreature)arenaManager.ArenaQueue.activeEntity.arenaEntity).ArenaMonoBehavior.MoveCreature();
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
        base.SetDamage(damage);

        if (Death)
        {
            ArenaMonoBehavior.RunDeath();
        }
    }

    public override async UniTask GoAttack(GridArenaNode nodeFromAttack, GridArenaNode nodeToAttack)
    {
        var entityForAttack = nodeToAttack.OccupiedUnit;
        var typeAttack = arenaManager.ArenaQueue.activeEntity.arenaEntity.Data.typeAttack;
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
            arenaManager.tileMapArenaUnits.transform
            );
        await asset.Task;
        ArenaMonoBehavior = asset.Result.GetComponent<ArenaCreatureMB>();
        // Debug.Log($"Spawn Entity::: {r_asset.name}");
        ArenaMonoBehavior.Init(this);
    }
    #endregion

    public async override UniTask<List<GridArenaNode>> GetFightingNodes()
    {
        // _tileMapAllowAttack.ClearAllTiles();
        await arenaManager.CreateMoveArea();

        var arenaEntity = arenaManager.ArenaQueue.activeEntity.arenaEntity;

        var creatureData = ((ScriptableAttributeCreature)arenaEntity.Entity.ScriptableDataAttribute);

        var occupiedNodes = arenaManager.GridArenaHelper
            .GetAllGridNodes()
            .Where(t =>
                t.OccupiedUnit != null
                && !t.StateArenaNode.HasFlag(StateArenaNode.Excluded)
                && t.StateArenaNode.HasFlag(StateArenaNode.Occupied)
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
                    && !t.StateArenaNode.HasFlag(StateArenaNode.Excluded)
                    && t.StateArenaNode.HasFlag(StateArenaNode.Occupied)
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
                if (arenaManager.AllowPathNodes.Intersect(neighbours).Count() > 0)
                {
                    arenaManager.FightingOccupiedNodes.Add(node);
                    arenaManager.SetColorAllowFightNode(node, LevelManager.Instance.ConfigGameSettings.colorAllowAttackCreature);
                }
            }
            else
            {
                arenaEntity.SetTypeAttack(TypeAttack.AttackShoot);
                arenaManager.FightingOccupiedNodes.Add(node);
                arenaManager.SetColorAllowFightNode(node, LevelManager.Instance.ConfigGameSettings.colorAllowAttackCreature);
            }
        }

        await UniTask.Yield();
        return arenaManager.FightingOccupiedNodes;
    }
}
