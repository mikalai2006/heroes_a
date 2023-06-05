using System;
using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;

public class ArenaWarMachine : ArenaEntityBase
{
    [NonSerialized] public ArenaWarMachineMonoBehavior ArenaWarMachineMonoBehavior;

    public override void Init(ArenaManager arenaManager, ArenaHeroEntity hero)
    {
        base.Init(arenaManager, hero);
    }
    public async void SetEntity(BaseEntity entity, GridArenaNode node)
    {
        _entity = entity;
        _configData = Entity.ScriptableData;
        _occupiedNode = node;
        // Direction = TypeArenaPlayer == TypeArenaPlayer.Left ? TypeDirection.Right : TypeDirection.Left;

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

        Data.isRun = true;
    }
    public void SetPosition(GridArenaNode node)
    {
        OccupiedNode.SetOcuppiedUnit(null);
        var creatureData = ((ScriptableAttributeCreature)Entity.ScriptableDataAttribute);
        var size = creatureData.CreatureParams.Size;

        _centerNode = node.center; // new Vector3(size + .5f, node.positionPrefab.y, node.positionPrefab.z);
        _positionPrefab = size == 2
            ? node.center + new Vector3(TypeArenaPlayer == TypeArenaPlayer.Left ? -0.5f : 0.5f, 0, 0)
            : node.center;
        node.SetOcuppiedUnit(this);
        _occupiedNode = node;
    }

    public async UniTask CreateMapGameObject(GridArenaNode node)
    {
        AssetReferenceGameObject gameObj = null;
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
            else if (configCreature.CreatureParams.Size == 2)
            {
                gameObj = LevelManager.Instance.ConfigGameSettings.ArenaPlaceholderSize2Model;
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
        ArenaWarMachineMonoBehavior = asset.Result.GetComponent<ArenaWarMachineMonoBehavior>();
        // Debug.Log($"Spawn Entity::: {r_asset.name}");
        ArenaWarMachineMonoBehavior.Init(this);
    }

    public override void ShowDialogInfo()
    {
        ArenaWarMachineMonoBehavior.ShowDialogInfo();
    }

    public override void SetDamage(int damage)
    {
        base.SetDamage(damage);

        if (Death)
        {
            ArenaWarMachineMonoBehavior.RunDeath();
        }
    }

    public override async UniTask<List<GridArenaNode>> GetFightingNodes()
    {
        // _arenaManager.ClearAttackNode();
        // //_tileMapAllowAttack.ClearAllTiles();
        // _arenaManager.FightingOccupiedNodes.Clear();
        arenaManager.ResetArenaState();

        var arenaEntity = arenaManager.ArenaQueue.activeEntity.arenaEntity;
        var warMachineConfig = ((ScriptableAttributeWarMachine)((EntityCreature)arenaManager.ArenaQueue.activeEntity.arenaEntity.Entity).ConfigAttribute);

        // Check target for catapult.
        if (warMachineConfig.TypeWarMachine == TypeWarMachine.Catapult)
        {
            var countNotDestructeFortification = arenaManager.ArenaTown.FortificationsGameObject
                .Where(t => t.Value != 0)
                .ToList();
            if (countNotDestructeFortification.Count == 0 && warMachineConfig.TypeWarMachine == TypeWarMachine.Catapult)
            {
                await arenaManager.NextCreature(false, false);
            }
        }

        var resultChoosed = await warMachineConfig
                .ChooseTarget(arenaManager, arenaManager.ArenaQueue.ActiveHero);

        switch (resultChoosed.TypeRunEffect)
        {
            case ArenaTypeRunEffect.AutoChoose:
                if (warMachineConfig.TypeWarMachine != TypeWarMachine.Catapult)
                {
                    var choosedFirstNode = resultChoosed.ChoosedNodes
                        .OrderBy(t => -t.position.y)
                        .First();
                    await warMachineConfig.RunEffect(
                        arenaManager,
                        arenaManager.ArenaQueue.activeEntity.arenaEntity.OccupiedNode,
                        choosedFirstNode
                    );
                }
                else
                {
                    await warMachineConfig.RunEffectByGameObject(
                        arenaManager,
                        arenaManager.ArenaQueue.activeEntity.arenaEntity.OccupiedNode,
                        arenaManager.clickedFortification
                    );
                }
                await arenaManager.NextCreature(false, false); // EndRunWarMachine();
                break;
            case ArenaTypeRunEffect.Choosed:
                if (warMachineConfig.TypeWarMachine != TypeWarMachine.Catapult)
                {
                    foreach (var node in resultChoosed.ChoosedNodes)
                    {
                        if (!node.StateArenaNode.HasFlag(StateArenaNode.Related))
                        {
                            arenaManager.FightingOccupiedNodes.Add(node);
                            arenaManager.SetColorAllowFightNode(node, LevelManager.Instance.ConfigGameSettings.colorAllowAttackCreature);
                        }
                    }
                }
                break;
            case ArenaTypeRunEffect.AutoAll:
                List<UniTask> listTasks = new();
                foreach (var nodeForAction in resultChoosed.ChoosedNodes)
                {
                    listTasks.Add(warMachineConfig.RunEffect(arenaManager, arenaManager.ArenaQueue.activeEntity.arenaEntity.OccupiedNode, nodeForAction));
                }
                await UniTask.WhenAll(listTasks);
                await arenaManager.NextCreature(false, false); // EndRunWarMachine();
                break;
        }
        return arenaManager.FightingOccupiedNodes;
    }

    public override async UniTask ClickCreature(Vector3Int clickPosition)
    {
        await base.ClickCreature(clickPosition);

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
            Debug.Log($"Choose warMachine node for attack!");
            if (arenaManager.AttackedCreature == this)
            {
                arenaManager.ChooseNextPositionForAttack();
            }
            else
            {
                if (arenaManager.AttackedCreature != this && arenaManager.AttackedCreature != null)
                {
                    arenaManager.ClearAttackNode();
                }
                arenaManager.clickedNode = OccupiedNode;
                arenaManager.AttackedCreature = this;
                arenaManager.ArenaQueue.activeEntity.arenaEntity.CreateButtonAttackNode(OccupiedNode);
            }
        }
    }

    // public override async UniTask GoAttack(GridArenaNode nodeFromAttack, GridArenaNode nodeToAttack)
    // {
    //     var entityForAttack = nodeToAttack.OccupiedUnit;
    //     var typeAttack = arenaManager.ArenaQueue.activeEntity.arenaEntity.Data.typeAttack;

    //     for (int i = 0; i < Data.countAttack; i++)
    //     {
    //         if (
    //             nodeToAttack.OccupiedUnit != null
    //             && !nodeToAttack.OccupiedUnit.Death
    //             && !Death
    //             )
    //         {
    //             await ArenaWarMachineMonoBehavior.RunAttackShoot(nodeToAttack);
    //             CalculateAttack(nodeFromAttack, nodeToAttack);
    //         }
    //     }
    // }

    public override async UniTask ClickButtonAction()
    {
        // Debug.Log($"{this.GetType()} ClickButtonAction {_arenaManager.clickedNode}");
        var warMachine = ((ScriptableAttributeWarMachine)((EntityCreature)Entity).ConfigAttribute);
        if (arenaManager.clickedFortification == null && warMachine.TypeWarMachine == TypeWarMachine.Catapult)
            return;
        if ((arenaManager.clickedNode == null || arenaManager.clickedNode.OccupiedUnit == null) && warMachine.TypeWarMachine != TypeWarMachine.Catapult)
            return;

        arenaManager.isRunningAction = true;
        if (!arenaManager.ArenaQueue.ActiveHero.Data.autoRun) await AudioManager.Instance.Click();

        arenaManager._buttonWarMachine.SetActive(false);
        // // // await ((ScriptableAttributeWarMachine)ArenaQueue.activeEntity.arenaEntity.Entity.ScriptableDataAttribute)
        // // //     .AddEffect();
        // switch (warMachine.TypeWarMachine)
        // {
        //     case TypeWarMachine.Catapult:
        //         await warMachine.RunEffectByGameObject(arenaManager, OccupiedNode, arenaManager.clickedFortification);
        //         break;
        //     default:
        //         await warMachine.RunEffect(arenaManager, OccupiedNode, arenaManager.clickedNode);
        //         break;
        // }

        if (warMachine.TypeWarMachine == TypeWarMachine.Catapult)
        {
            await warMachine.RunEffectByGameObject(arenaManager, OccupiedNode, arenaManager.clickedFortification);
        }
        else
        {
            await warMachine.RunEffect(arenaManager, OccupiedNode, arenaManager.clickedNode);
        }

        // // if (_arenaManager.town != null)
        // // {
        // //     _arenaManager.town.ArenaEntityTownMB.SetStatusColliders(false);
        // // }
        // EndRunWarMachine();

        arenaManager.isRunningAction = false;
        await arenaManager.NextCreature(false, false);
    }

    // private void EndRunWarMachine()
    // {
    //     // Clear clicked node.
    //     arenaManager.clickedNode = null;
    //     arenaManager.ClearAttackNode();

    //     // Next creature.
    //     // await GoEntity();
    //     arenaManager.isRunningAction = false;
    //     arenaManager.NextCreature(false, false);

    //     // // DrawCursor
    //     // if (arenaManager.clickedNode != null) arenaManager.DrawButtonAction();

    // }

    // public override void CreateButtonAttackNode()
    // {
    //     // clickedNode = null;
    //     _arenaManager.ClearAttackNode();

    //     // AttackedCreature = clickedEntity;
    //     _arenaManager.clickedNode = OccupiedNode;

    //     var positionButton = new Vector3(OccupiedNode.center.x, OccupiedNode.center.y, -14);
    //     var warMachine = ((ScriptableAttributeWarMachine)((EntityCreature)_arenaManager.ArenaQueue.activeEntity.arenaEntity.Entity).ConfigAttribute);

    //     RuleTile cursor = CursorRule.Shoot;
    //     switch (warMachine.TypeWarMachine)
    //     {
    //         case TypeWarMachine.Catapult:
    //             cursor = CursorRule.Catapult;
    //             break;
    //         case TypeWarMachine.FirstAidTent:
    //             cursor = CursorRule.FirstAidTent;
    //             break;
    //     }

    //     if (
    //         FightingOccupiedNodes.Count > 0
    //         // && NodeForSpell != null
    //         // && FightingOccupiedNodes.Contains(NodeForSpell)
    //         )
    //     {
    //         _buttonWarMachine.SetActive(true);
    //         _buttonAction.SetActive(false);
    //         _buttonSpell.SetActive(false);
    //         _buttonWarMachine.GetComponent<SpriteRenderer>().sprite = cursor.m_DefaultSprite;
    //         _buttonWarMachine.transform.position = positionButton;
    //     }
    //     OnChooseCreatureForSpell?.Invoke();
    // }

    public override async UniTask RunGettingHit(GridArenaNode attackNode)
    {
        await ArenaWarMachineMonoBehavior.RunGettingHit(attackNode);
        await RemoveSpellAction();
    }

    public override async UniTask RunGettingHitSpell()
    {
        await ArenaWarMachineMonoBehavior.RunGettingHitSpell();
        await RemoveSpellAction();
    }

    public override void SetActiveColor(Color color)
    {
        base.SetActiveColor(color);

        ArenaWarMachineMonoBehavior.SetColorModel(color);
    }
}
