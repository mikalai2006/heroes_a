using System;
using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;

public class ArenaShootTown : ArenaEntityBase
{
    [NonSerialized] public ArenaShootTownMB ArenaShootTownMB;
    private Transform transform;

    public override void Init(ArenaManager arenaManager, EntityHero hero)
    {
        base.Init(arenaManager, hero);
    }
    public void SetEntity(BaseEntity entity, GridArenaNode node)
    {
        _entity = entity;
        _occupiedNode = node;
        _configData = Entity.ScriptableData;

        var creatureData = ((ScriptableAttributeCreature)Entity.ScriptableDataAttribute);
        Data.shoots = 1000;
        Data.speed = creatureData.CreatureParams.Speed;
        Data.typeMove = creatureData.CreatureParams.Movement;
        Data.typeAttack = TypeAttack.AttackShootTown;
        Data.quantity = ((EntityCreature)Entity).Data.value;

        Data.damageMin = creatureData.CreatureParams.DamageMin;
        Data.damageMax = creatureData.CreatureParams.DamageMax;

        Data.defense = creatureData.CreatureParams.Defense;
        Data.attack = creatureData.CreatureParams.Attack;

        Data.HP = 1;//creatureData.CreatureParams.HP;
        Data.totalHP = Data.maxHP = creatureData.CreatureParams.HP * ((EntityCreature)Entity).Data.value;

        Data.isRun = true;
    }
    // public void SetPosition(Transform transform)
    // {
    //     var creatureData = ((ScriptableAttributeCreature)Entity.ScriptableDataAttribute);

    //     _centerNode = transform.position - new Vector3(0, 0.5f, 0);
    //     _positionPrefab = transform.position - new Vector3(0, 0.5f, 0);
    //     this.transform = transform;
    // }
    public void SetPosition(GridArenaNode node)
    {
        OccupiedNode.SetOcuppiedUnit(null);
        // var creatureData = ((ScriptableAttributeCreature)Entity.ScriptableDataAttribute);

        _centerNode = node.center;
        _positionPrefab = node.center;
        node.SetOcuppiedUnit(this);
        _occupiedNode = node;
    }

    public async UniTask CreateMapGameObject()
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

        // var asset = Addressables.InstantiateAsync(
        //     gameObj,
        //     PositionPrefab,
        //     Quaternion.identity,
        //     transform
        //     // _arenaManager.tileMapArenaUnits.transform
        //     );
        // await asset.Task;
        // var comp = asset.Result.GetComponent<ArenaMonoBehavior>();
        // // var shoots = comp.ShootPrefab;
        // GameObject.Destroy(comp);
        // asset.Result.AddComponent<ArenaShootTownMB>();
        // ArenaShootTownMB = asset.Result.GetComponent<ArenaShootTownMB>();
        // ArenaShootTownMB.Init(this);

        var asset = Addressables.InstantiateAsync(
            gameObj,
            PositionPrefab, // + new Vector3(0, -.25f, 0),
            Quaternion.identity,
            _arenaManager.tileMapArenaUnits.transform
            );
        await asset.Task;
        var comp = asset.Result.GetComponent<ArenaMonoBehavior>();
        // var shoots = comp.ShootPrefab;
        GameObject.Destroy(comp);
        asset.Result.AddComponent<ArenaShootTownMB>();
        ArenaShootTownMB = asset.Result.GetComponent<ArenaShootTownMB>();
        ArenaShootTownMB.Init(this);
    }

    public override async UniTask GetFightingNodes()
    {
        _arenaManager.ClearAttackNode();
        // _tileMapAllowAttack.ClearAllTiles();
        _arenaManager.FightingOccupiedNodes.Clear();

        var arenaEntity = _arenaManager.ArenaQueue.activeEntity.arenaEntity;

        var nodesForAttack = _arenaManager.GridArenaHelper
            .GetAllGridNodes()
            .Where(t =>
                t.OccupiedUnit != null
                && t.OccupiedUnit.TypeArenaPlayer != _arenaManager.ArenaQueue.activeEntity.arenaEntity.TypeArenaPlayer
            )
            .ToList(); ;

        foreach (var node in nodesForAttack)
        {
            if (!node.StateArenaNode.HasFlag(StateArenaNode.Related))
            {
                _arenaManager.FightingOccupiedNodes.Add(node);
                _arenaManager.SetColorAllowFightNode(node, LevelManager.Instance.ConfigGameSettings.colorAllowAttackCreature);
            }
        }

        // Check type run.
        int levelSSkill = 0;
        if (_arenaManager.ArenaQueue.ActiveHero != null)
        {
            levelSSkill = _arenaManager.ArenaQueue.ActiveHero.Data.SSkills.ContainsKey(TypeSecondarySkill.Artillery)
                ? _arenaManager.ArenaQueue.ActiveHero.Data.SSkills[TypeSecondarySkill.Artillery].level + 1
                : 0;
        }
        ArenaTypeRunEffect typeRunEffect = ArenaTypeRunEffect.AutoChoose;
        switch (levelSSkill)
        {
            case 0:
                typeRunEffect = ArenaTypeRunEffect.AutoChoose;
                break;
            case 1:
            case 2:
            case 3:
                typeRunEffect = ArenaTypeRunEffect.Choosed;
                break;
        }

        switch (typeRunEffect)
        {
            case ArenaTypeRunEffect.AutoChoose:
                // TODO choose target algoritm.
                Debug.Log($"Auto run shoot town! Artillery={levelSSkill}");
                // await ArenaQueue.activeEntity.arenaEntity.GoAttack(
                //     ArenaQueue.activeEntity.arenaEntity.OccupiedNode,
                //     nodesForAttack[UnityEngine.Random.Range(0, nodesForAttack.Count)]
                // );
                break;
            case ArenaTypeRunEffect.Choosed:
                // TODO choose target algoritm.
                Debug.Log("Create button shoot town!");
                break;
        }

        await UniTask.Yield();
    }

    public override void CreateButtonAttackNode(GridArenaNode clickedNode)
    {
        _arenaManager.clickedNode = OccupiedNode;
        if (_arenaManager.AttackedCreature != this && _arenaManager.AttackedCreature != null)
        {
            _arenaManager.ClearAttackNode();
        }
        // _arenaManager.AttackedCreature = this;
        _arenaManager.clickedNode = OccupiedNode;

        var allowNodes = _arenaManager.AllowPathNodes.Concat(_arenaManager.AllowMovedNodes).ToList();

        _arenaManager.NodesForAttackActiveCreature.Add(new AttackItemNode()
        {
            nodeFromAttack = _arenaManager.ArenaQueue.activeEntity.arenaEntity.OccupiedNode,
            nodeToAttack = OccupiedNode
        });

        // _arenaManager.ChooseNextPositionForAttack();
        _arenaManager.DrawAttackNodes();
        _arenaManager.DrawButtonAction();
    }


    public override void CreateButton(GridArenaNode occupiedNode)
    {
        base.CreateButton(occupiedNode);
    }
}
