using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;

public class ArenaWarMachine : ArenaEntityBase
{
    [NonSerialized] public ArenaWarMachineMonoBehavior ArenaWarMachineMonoBehavior;

    public override void Init(ArenaManager arenaManager, EntityHero hero)
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
        ArenaWarMachineMonoBehavior = asset.Result.GetComponent<ArenaWarMachineMonoBehavior>();
        // Debug.Log($"Spawn Entity::: {r_asset.name}");
        ArenaWarMachineMonoBehavior.Init(this);
    }

    public override async UniTask GetFightingNodes()
    {
        // _arenaManager.ClearAttackNode();
        // //_tileMapAllowAttack.ClearAllTiles();
        // _arenaManager.FightingOccupiedNodes.Clear();
        _arenaManager.ResetArenaState();

        var arenaEntity = _arenaManager.ArenaQueue.activeEntity.arenaEntity;
        var warMachineConfig = ((ScriptableAttributeWarMachine)((EntityCreature)_arenaManager.ArenaQueue.activeEntity.arenaEntity.Entity).ConfigAttribute);
        var resultChoosed = await warMachineConfig
                .ChooseTarget(_arenaManager, _arenaManager.ArenaQueue.ActiveHero);

        switch (resultChoosed.TypeRunEffect)
        {
            case ArenaTypeRunEffect.AutoChoose:
                if (warMachineConfig.TypeWarMachine != TypeWarMachine.Catapult)
                {
                    var choosedFirstNode = resultChoosed.ChoosedNodes
                        .OrderBy(t => -t.position.y)
                        .First();
                    await warMachineConfig.RunEffect(
                        _arenaManager,
                        _arenaManager.ArenaQueue.activeEntity.arenaEntity.OccupiedNode,
                        choosedFirstNode
                    );
                }
                else
                {
                    await warMachineConfig.RunEffectByGameObject(
                        _arenaManager,
                        _arenaManager.ArenaQueue.activeEntity.arenaEntity.OccupiedNode,
                        _arenaManager.clickedFortification
                    );
                }
                _arenaManager.NextCreature(false, false); // EndRunWarMachine();
                break;
            case ArenaTypeRunEffect.Choosed:
                if (warMachineConfig.TypeWarMachine != TypeWarMachine.Catapult)
                {
                    foreach (var node in resultChoosed.ChoosedNodes)
                    {
                        if (!node.StateArenaNode.HasFlag(StateArenaNode.Related))
                        {
                            _arenaManager.FightingOccupiedNodes.Add(node);
                            _arenaManager.SetColorAllowFightNode(node, LevelManager.Instance.ConfigGameSettings.colorAllowAttackCreature);
                        }
                    }
                }
                else
                {
                    _arenaManager.town.ArenaEntityTownMB.SetStatusColliders(true);
                }
                break;
            case ArenaTypeRunEffect.AutoAll:
                List<UniTask> listTasks = new();
                foreach (var nodeForAction in resultChoosed.ChoosedNodes)
                {
                    listTasks.Add(warMachineConfig.RunEffect(_arenaManager, _arenaManager.ArenaQueue.activeEntity.arenaEntity.OccupiedNode, nodeForAction));
                }
                await UniTask.WhenAll(listTasks);
                _arenaManager.NextCreature(false, false); // EndRunWarMachine();
                break;
        }
    }

    public override async UniTask ClickCreature()
    {
        await base.ClickCreature();
        if (!_arenaManager.FightingOccupiedNodes.Contains(OccupiedNode)) return;

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
            Debug.Log($"Choose warMachine node for attack!");
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
            }
        }
    }

    public override async UniTask GoAttack(GridArenaNode nodeFromAttack, GridArenaNode nodeToAttack)
    {
        var entityForAttack = nodeToAttack.OccupiedUnit;
        var typeAttack = _arenaManager.ArenaQueue.activeEntity.arenaEntity.Data.typeAttack;

        for (int i = 0; i < Data.countAttack; i++)
        {
            if (
                nodeToAttack.OccupiedUnit != null
                && !nodeToAttack.OccupiedUnit.Death
                && !Death
                )
            {
                await ArenaWarMachineMonoBehavior.RunAttackShoot(nodeToAttack);
                CalculateAttack(nodeFromAttack, nodeToAttack);
            }
        }
    }

    public override async UniTask ClickButtonAction()
    {
        Debug.Log($"{this.GetType()} ClickButtonAction {_arenaManager.clickedNode}");
        var warMachine = ((ScriptableAttributeWarMachine)((EntityCreature)Entity).ConfigAttribute);
        if (_arenaManager.clickedFortification == null && warMachine.TypeWarMachine == TypeWarMachine.Catapult)
            return;
        if ((_arenaManager.clickedNode == null || _arenaManager.clickedNode.OccupiedUnit == null) && warMachine.TypeWarMachine != TypeWarMachine.Catapult)
            return;

        _arenaManager.isRunningAction = true;
        await AudioManager.Instance.Click();


        _arenaManager._buttonWarMachine.SetActive(false);
        // // await ((ScriptableAttributeWarMachine)ArenaQueue.activeEntity.arenaEntity.Entity.ScriptableDataAttribute)
        // //     .AddEffect();
        switch (warMachine.TypeWarMachine)
        {
            case TypeWarMachine.Catapult:
                await warMachine.RunEffectByGameObject(_arenaManager, OccupiedNode, _arenaManager.clickedFortification);
                break;
            default:
                await warMachine.RunEffect(_arenaManager, OccupiedNode, _arenaManager.clickedNode);
                break;
        }

        _arenaManager.town.ArenaEntityTownMB.SetStatusColliders(false);
        EndRunWarMachine();
    }

    private void EndRunWarMachine()
    {
        // Clear clicked node.
        _arenaManager.clickedNode = null;
        _arenaManager.ClearAttackNode();

        // Next creature.
        // await GoEntity();
        _arenaManager.NextCreature(false, false);

        // DrawCursor
        if (_arenaManager.clickedNode != null) _arenaManager.DrawButtonAction();

        _arenaManager.isRunningAction = false;
    }

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
}
