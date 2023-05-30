using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ArenaEntityTownData
{
    public bool status;
    public List<ArenaCreature> ShootEntities = new();
}

public class ArenaEntityTown
{
    public ArenaEntityTownData Data = new();
    private ArenaManager _arenaManager;
    [NonSerialized] public GridArenaNode OccupiedNode = null;
    [NonSerialized] private List<GridArenaNode> _relatedNodes = new();
    public List<GridArenaNode> RelatedNodes => _relatedNodes;
    // public ScriptableEntityTown ConfigDataTown;
    private Vector3 _centerNode;
    public Vector3 CenterNode => _centerNode;
    private Vector3 _positionPrefab;
    public Vector3 PositionPrefab => _positionPrefab;
    [NonSerialized] public ArenaEntityTownMB ArenaEntityTownMB;

    public SerializableDictionary<Transform, int> FortificationsGameObject = new();
    public SerializableDictionary<string, GridArenaNode> FortificationsNodes = new();
    public SerializableDictionary<string, ArenaShootTown> FortificationsShoots = new();
    private EntityTown _town;
    public EntityTown Town => _town;
    public bool isMoat = false;

    public void Init(
        GridArenaNode node,
        EntityTown entityTown,
        ArenaManager arenaManager
        )
    {
        OccupiedNode = node;
        _town = entityTown;
        _arenaManager = arenaManager;

        Debug.Log($"Town.Data.level={Town.Data.level}");
        if (Town.Data.level > -1)
        {

            // Disable indestructible nodes.
            _arenaManager.GridArenaHelper.GetNode(11, 11).StateArenaNode |= StateArenaNode.Disable;
            _arenaManager.GridArenaHelper.GetNode(10, 9).StateArenaNode |= StateArenaNode.Disable;
            _arenaManager.GridArenaHelper.GetNode(10, 8).StateArenaNode |= StateArenaNode.Disable;
            _arenaManager.GridArenaHelper.GetNode(9, 5).StateArenaNode |= StateArenaNode.Disable;
            _arenaManager.GridArenaHelper.GetNode(10, 3).StateArenaNode |= StateArenaNode.Disable;
            _arenaManager.GridArenaHelper.GetNode(11, 2).StateArenaNode |= StateArenaNode.Disable;

            // Destructible Wall nodes.
            // (11, 10) (9, 7) (10, 4) (11, 1)
            _arenaManager.GridArenaHelper.GetNode(11, 10).StateArenaNode |= StateArenaNode.Wall | StateArenaNode.Disable;
            _arenaManager.GridArenaHelper.GetNode(9, 7).StateArenaNode |= StateArenaNode.Wall | StateArenaNode.Disable;
            _arenaManager.GridArenaHelper.GetNode(10, 4).StateArenaNode |= StateArenaNode.Wall | StateArenaNode.Disable;
            _arenaManager.GridArenaHelper.GetNode(11, 1).StateArenaNode |= StateArenaNode.Wall | StateArenaNode.Disable;

            SetFortificationNodes();
        }
        if (Town.Data.level > 0)
        {
            isMoat = true;

            // Moat nodes.
            // 10,11; 10,10; 9,9; 9,8; 8,7; 9,6; 8,5; 9,4; 9,3; 10,2; 10,1;
            _arenaManager.GridArenaHelper.GetNode(10, 11).StateArenaNode |= StateArenaNode.Moating;
            _arenaManager.GridArenaHelper.GetNode(10, 10).StateArenaNode |= StateArenaNode.Moating;
            _arenaManager.GridArenaHelper.GetNode(9, 9).StateArenaNode |= StateArenaNode.Moating;
            _arenaManager.GridArenaHelper.GetNode(9, 8).StateArenaNode |= StateArenaNode.Moating;
            _arenaManager.GridArenaHelper.GetNode(8, 7).StateArenaNode |= StateArenaNode.Moating;
            // GridArenaHelper.GetNode(9, 6).StateArenaNode |= StateArenaNode.Moating;
            _arenaManager.GridArenaHelper.GetNode(8, 5).StateArenaNode |= StateArenaNode.Moating;
            _arenaManager.GridArenaHelper.GetNode(9, 4).StateArenaNode |= StateArenaNode.Moating;
            _arenaManager.GridArenaHelper.GetNode(9, 3).StateArenaNode |= StateArenaNode.Moating;
            _arenaManager.GridArenaHelper.GetNode(10, 2).StateArenaNode |= StateArenaNode.Moating;
            _arenaManager.GridArenaHelper.GetNode(10, 1).StateArenaNode |= StateArenaNode.Moating;
        }


        // Bridge.
        var bridgeNode = _arenaManager.GridArenaHelper.GetNode(10, 6);
        bridgeNode.StateArenaNode |= StateArenaNode.Bridge;
        FortificationsNodes.Add("0_Bridge", bridgeNode);
    }

    public async UniTask OpenBridge()
    {
        await ArenaEntityTownMB.OpenBridge();
    }

    public void CloseBridge()
    {
        ArenaEntityTownMB.CloseBridge();
    }

    public void SetFortificationNodes()
    {
        if (Town.Data.level > -1)
        {
            var nodeWall1 = _arenaManager.GridArenaHelper.GetNode(11, 10);
            FortificationsNodes.Add("0_Wall1", nodeWall1);
            var nodeWall2 = _arenaManager.GridArenaHelper.GetNode(9, 7);
            FortificationsNodes.Add("0_Wall2", nodeWall2);
            var nodeWall3 = _arenaManager.GridArenaHelper.GetNode(10, 4);
            FortificationsNodes.Add("0_Wall3", nodeWall3);
            var nodeWall4 = _arenaManager.GridArenaHelper.GetNode(11, 1);
            FortificationsNodes.Add("0_Wall4", nodeWall4);
        }
    }

    public async UniTask SetShootTown()
    {
        if (Town.Data.level >= 1)
        {
            var nodeObj3 = _arenaManager.GridArenaHelper.GridTile.GetGridObject(new Vector3Int(15, 7));
            var shootKeep = await CreateShooter(nodeObj3);
            FortificationsShoots.Add("1_TowerKeep", shootKeep);
        }

        if (Town.Data.level >= 2)
        {
            var nodeObj = _arenaManager.GridArenaHelper.GridTile.GetGridObject(new Vector3Int(12, 12));
            var shootTower1 = await CreateShooter(nodeObj);
            FortificationsShoots.Add("2_Tower1", shootTower1);
            var nodeObj2 = _arenaManager.GridArenaHelper.GridTile.GetGridObject(new Vector3Int(12, 0));
            var shootTower2 = await CreateShooter(nodeObj2);
            FortificationsShoots.Add("2_Tower2", shootTower2);
        }
    }

    // public async void ClickEntityTown(InputAction.CallbackContext context)
    // {
    //     await AudioManager.Instance.Click();
    //     if (_arenaManager.FightingOccupiedNodes.Contains(OccupiedNode))
    //     {

    //         Debug.Log($"Choose spell for spell!");
    //         _arenaManager.CreateButtonSpell(OccupiedNode);
    //     }
    //     else
    //     {
    //         Debug.Log($"Spell not maybe to cancel!");
    //         await _arenaManager.ClickArena(OccupiedNode.position);
    //     }
    //     await UniTask.Delay(1);
    // }

    public void ClickFortification(GameObject clickedObject)
    {
        Debug.Log($"Click fortification {clickedObject.name}");
        _arenaManager.clickedNode = null;

        _arenaManager.CreateButtonCatapult(clickedObject);
    }

    internal async void AddFortification(Transform child)
    {
        var nameSplitArr = child.name.Split("_");
        int levelObject = Int32.Parse(nameSplitArr[0]);
        var countHPTownObject = 2;
        if (Town.Data.level >= levelObject)
        {
            if (child.name.IndexOf("Wall") >= 0 && Town.Data.level >= 2)
            {
                countHPTownObject = 3;
            }
            FortificationsGameObject.Add(child, countHPTownObject);
        }
        await ArenaEntityTownMB.ResreshObject(child, countHPTownObject);
    }

    public async UniTask AttackFortification(Transform transform, int damage)
    {
        Debug.Log($"AttackFortification::: object={transform.name}, damage={damage}");

        FortificationsGameObject[transform] = FortificationsGameObject[transform] - damage;
        if (FortificationsGameObject[transform] < 0) FortificationsGameObject[transform] = 0;

        await ArenaEntityTownMB.ResreshObject(transform, FortificationsGameObject[transform]);

        if (FortificationsGameObject[transform] == 0)
        {
            // remove disabled status wall node.
            if (FortificationsNodes.ContainsKey(transform.name))
            {
                var node = FortificationsNodes[transform.name];
                node.StateArenaNode &= ~StateArenaNode.Disable;
                node.StateArenaNode &= ~StateArenaNode.Wall;
                node.StateArenaNode &= ~StateArenaNode.Bridge;
                node.StateArenaNode &= ~StateArenaNode.OpenBridge;
            }

            // remove status bridge.
            if (transform.name.IndexOf("Bridge") != -1)
            {
                await OpenBridge();
            }

            // remove shoot town.
            if (transform.name.IndexOf("Tower") != -1)
            {
                var shooter = FortificationsShoots[transform.name];
                _arenaManager.ArenaQueue.RemoveEntity(shooter);
                GameObject.Destroy(shooter.ArenaShootTownMB);
                FortificationsShoots.Remove(transform.name);
                shooter = null;
            }
        }

        await UniTask.Delay(1);
    }

    #region CreateDestroy
    public async UniTask<ArenaShootTown> CreateShooter(GridArenaNode node)
    {
        EntityCreature creatureShoot = new EntityCreature(Town.ConfigData.shootCreature);
        var newShootTown = new ArenaShootTown();
        newShootTown.Init(_arenaManager, _arenaManager.DialogArenaData.enemy);
        newShootTown.SetEntity(creatureShoot, node);
        newShootTown.TypeArenaPlayer = TypeArenaPlayer.Right;
        newShootTown.Data.speed = 1001;
        newShootTown.SetPosition(node);
        await newShootTown.CreateMapGameObject();
        _arenaManager.ArenaQueue.AddEntity(newShootTown);
        return newShootTown;
    }

    public async UniTask CreateGameObject()
    {
        AssetReferenceGameObject gameObj = null;
        if (Town.ConfigData != null && Town.ConfigData.ArenaPrefab.RuntimeKeyIsValid())
        {
            gameObj = Town.ConfigData.ArenaPrefab;
        }

        if (gameObj == null)
        {
            Debug.LogWarning($"Not found Prefab {Town.ConfigData.name}!");
            return;
        }

        var arenaUnits = GameObject.FindGameObjectWithTag("ArenaUnits");

        var asset = Addressables.InstantiateAsync(
            gameObj,
            OccupiedNode.center,
            Quaternion.identity,
            arenaUnits.transform
        );
        await asset.Task;

        ArenaEntityTownMB = asset.Result.GetComponent<ArenaEntityTownMB>();
        ArenaEntityTownMB.Init(this);
        await UniTask.Delay(1);
    }
    #endregion
}
