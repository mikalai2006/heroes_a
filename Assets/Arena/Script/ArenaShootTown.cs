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
    public bool isHead;

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
        Data.typeAttack = TypeAttack.AttackShoot;
        Data.quantity = ((EntityCreature)Entity).Data.value;

        Data.countAttack = 1; // TODO - option

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
        node.StateArenaNode |= StateArenaNode.Excluded;
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
        var asset = Addressables.InstantiateAsync(
            gameObj,
            PositionPrefab,
            Quaternion.identity,
            arenaManager.tileMapArenaUnits.transform
            );
        await asset.Task;
        var comp = asset.Result.GetComponent<ArenaCreatureMB>();
        var shoots = comp.ShootPrefab;
        GameObject.Destroy(comp);
        asset.Result.AddComponent<ArenaShootTownMB>();
        ArenaShootTownMB = asset.Result.GetComponent<ArenaShootTownMB>();
        ArenaShootTownMB.Init(this, shoots);
    }

    public override async UniTask GetFightingNodes()
    {
        arenaManager.ClearAttackNode();
        arenaManager.FightingOccupiedNodes.Clear();

        var arenaEntity = arenaManager.ArenaQueue.activeEntity.arenaEntity;

        var nodesForAttack = arenaManager.GridArenaHelper
            .GetAllGridNodes()
            .Where(t =>
                t.OccupiedUnit != null
                && t.OccupiedUnit.TypeArenaPlayer != arenaManager.ArenaQueue.activeEntity.arenaEntity.TypeArenaPlayer
            )
            .ToList(); ;

        foreach (var node in nodesForAttack)
        {
            if (!node.StateArenaNode.HasFlag(StateArenaNode.Related))
            {
                arenaManager.FightingOccupiedNodes.Add(node);
                arenaManager.SetColorAllowFightNode(node, LevelManager.Instance.ConfigGameSettings.colorAllowAttackCreature);
            }
        }

        // Check type run.
        int levelSSkill = 0;
        if (arenaManager.ArenaQueue.ActiveHero != null)
        {
            levelSSkill = arenaManager.ArenaQueue.ActiveHero.Data.SSkills.ContainsKey(TypeSecondarySkill.Artillery)
                ? arenaManager.ArenaQueue.ActiveHero.Data.SSkills[TypeSecondarySkill.Artillery].level + 1
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
                // Debug.Log($"Auto run shoot town! Artillery={levelSSkill}");
                //TODO AI choose target.
                var nodeToAttack = nodesForAttack[UnityEngine.Random.Range(0, nodesForAttack.Count)];
                arenaManager.clickedNode = nodeToAttack;
                arenaManager.AttackedCreature = nodeToAttack.OccupiedUnit;
                CreateButtonAttackNode(nodeToAttack);
                await ClickButtonAction();
                break;
            case ArenaTypeRunEffect.Choosed:
                // TODO choose target algoritm.
                // Debug.Log("Create button shoot town!");
                break;
        }

        await UniTask.Yield();
    }

    public override async UniTask ClickButtonAction()
    {
        arenaManager.isRunningAction = true;
        // await AudioManager.Instance.Click();

        if (arenaManager.activeCursor == arenaManager.CursorRule.NotAllow)
        {
            arenaManager.isRunningAction = false;
            return;
        };

        if (arenaManager.AttackedCreature != null)
        {
            var nodes = arenaManager.NodesForAttackActiveCreature[arenaManager.KeyNodeFromAttack];
            if (nodes.nodeFromAttack.OccupiedUnit != null)
            {
                await nodes.nodeFromAttack.OccupiedUnit.GoAttack(nodes.nodeFromAttack, nodes.nodeToAttack);
            }
        }

        // Clear clicked node.
        arenaManager.clickedNode = null;
        arenaManager.ClearAttackNode();

        // Next creature.
        // await GoEntity();
        arenaManager.NextCreature(false, false);

        // EndRunWarMachine();
        arenaManager.isRunningAction = false;
    }

    public override async UniTask GoAttack(GridArenaNode nodeFromAttack, GridArenaNode nodeToAttack)
    {
        for (int i = 0; i < Data.countAttack; i++)
        {
            if (
                nodeToAttack.OccupiedUnit != null
                && !nodeToAttack.OccupiedUnit.Death
                && !Death
                )
            {
                await ArenaShootTownMB.RunAttackShoot(nodeToAttack);
                CalculateAttack(nodeFromAttack, nodeToAttack);
            }
        }
    }

    public override void CalculateAttack(GridArenaNode nodeFromAttack, GridArenaNode nodeToAttack)
    {
        var town = arenaManager.ArenaTown.Town;
        if (town == null) return;
        int countBuildingInTown = town.Data.Generals.Count + town.Data.Armys.Count;

        int baseDamage = UnityEngine.Random.Range(Data.damageMin, Data.damageMax);

        int dopDamage = isHead
            ? UnityEngine.Random.Range(2, 4) * countBuildingInTown
            : UnityEngine.Random.Range(2, 4) * Mathf.CeilToInt(countBuildingInTown / 2);
        int totalDamage = baseDamage + dopDamage;

        // Debug.Log($"{Entity.ScriptableDataAttribute.name} run damage {totalDamage}/ base={baseDamage}/dopDamage={dopDamage}");

        nodeToAttack.OccupiedUnit.SetDamage(totalDamage);
    }

    internal void SetData(bool isHeadTower)
    {
        isHead = isHeadTower;
        var town = arenaManager.ArenaTown.Town;
        if (town == null) return;
        int countBuildingInTown = town.Data.Generals.Count + town.Data.Armys.Count;

        Data.damageMin = isHead ? 10 : 6;
        Data.damageMax = isHead ? 20 : 12;

        int dopDamage = isHead
            ? UnityEngine.Random.Range(2, 4) * countBuildingInTown
            : UnityEngine.Random.Range(2, 4) * Mathf.CeilToInt(countBuildingInTown / 2);
        Data.DamageModificators.Add(((EntityCreature)Entity).ConfigAttribute, dopDamage);
    }
}
