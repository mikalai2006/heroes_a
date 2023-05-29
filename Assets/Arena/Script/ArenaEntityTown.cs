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

[Serializable]
public class ArenaEntityTownData
{
    public bool status;
    public List<ArenaCreature> ShootEntities = new();
}

[Serializable]
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
    // public Transform Door;

    public SerializableDictionary<Transform, int> FortificationsGameObject = new();
    private EntityTown _town;
    public EntityTown Town => _town;

    public void Init(
        GridArenaNode node,
        EntityTown entityTown,
        ArenaManager arenaManager
        )
    {
        OccupiedNode = node;
        _town = entityTown;
        _arenaManager = arenaManager;

    }

    public async UniTask OpenDoor()
    {
        await ArenaEntityTownMB.OpenDoor();
    }

    public void CloseDoor()
    {
        ArenaEntityTownMB.CloseDoor();
    }
    public async UniTask SetShootTown()
    {
        // Door = FortificationsGameObject.Where(t => t.Key.name == "Door").First().Key;

        if (Town.Data.level > 0)
        {
            var castle = Town.Data.Generals.Where(t => t.Value.ConfigData.TypeBuild == TypeBuild.Castle).First();
        }

        Town.Data.level = 1;
        Debug.Log($"Town.Data.level={Town.Data.level}");

        switch (Town.Data.level)
        {
            case 0:
                break;
            case 1:
                var nodeObj3 = _arenaManager.GridArenaHelper.GridTile.GetGridObject(new Vector3Int(15, 7));
                await CreateShooter(nodeObj3);
                break;
            case 2:
                var nodeObj = _arenaManager.GridArenaHelper.GridTile.GetGridObject(new Vector3Int(12, 12));
                await CreateShooter(nodeObj);
                var nodeObj2 = _arenaManager.GridArenaHelper.GridTile.GetGridObject(new Vector3Int(12, 0));
                await CreateShooter(nodeObj2);
                break;
            default:

                break;
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

        _arenaManager.CreateButtonCatapult(clickedObject);
    }
    public void AttackFortification(GameObject fortification)
    {
        Debug.Log($"Click fortification {fortification.name}");
    }
    internal void AddFortification(Transform child)
    {
        FortificationsGameObject.Add(child, Town.Data.level);
    }

    #region CreateDestroy
    public async UniTask CreateShooter(GridArenaNode node)
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
