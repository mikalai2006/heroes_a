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
    public List<ArenaEntity> ShootEntities = new();
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
    [NonSerialized] public ArenaEntityTownMonobehavior ArenaEntityTownMonobehavior;
    // public TypeArenaPlayer TypeArenaPlayer;
    // private EntityHero _hero;
    // public EntityHero Hero => _hero;
    private EntityTown _town;
    public EntityTown Town => _town;

    public ArenaEntityTown(
        GridArenaNode node,
        EntityTown entityTown,
        ArenaManager arenaManager
        )
    {
        OccupiedNode = node;
        _town = entityTown;
        _arenaManager = arenaManager;
        // ConfigDataTown = _town.ConfigData;
        // node.SetSpellsUnit(this);

        if (Town.Data.level > 0)
        {
            var castle = Town.Data.Generals.Where(t => t.Value.ConfigData.TypeBuild == TypeBuild.Castle).First();

        }
        EntityCreature creatureShoot = new EntityCreature(Town.ConfigData.shootCreature);
        var GridGameObject = new ArenaEntity(_arenaManager, Town.HeroInTown);
        var size = creatureShoot.ConfigAttribute.CreatureParams.Size;
        GridGameObject.TypeArenaPlayer = TypeArenaPlayer.Right;
        GridGameObject.SetEntity(creatureShoot, null);
        GridGameObject.Data.typeAttack = TypeAttack.AttackShootTown;
        // GridGameObject.CreateMapGameObject(nodeObj);
        Data.ShootEntities.Add(GridGameObject);
        _arenaManager.ArenaQueue.AddEntity(GridGameObject);
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

    #region CreateDestroy
    public async UniTask CreateGameObject()
    {
        await LoadGameObject();
    }
    // public void AddRelatedNode(GridArenaNode node)
    // {
    //     _relatedNodes.Add(node);
    // }

    // public void DestroyObject()
    // {
    //     GameObject.Destroy(ArenaEntityTownMonobehavior);
    //     // OccupiedNode.SetSpellsUnit(null);
    //     // OccupiedNode.SetSpellsStatus(false);
    // }

    private async UniTask LoadGameObject()
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

        ArenaEntityTownMonobehavior = asset.Result.GetComponent<ArenaEntityTownMonobehavior>();
        ArenaEntityTownMonobehavior.Init(this);
    }

    // public virtual void LoadedAsset(AsyncOperationHandle<GameObject> handle)
    // {
    //     if (handle.Status == AsyncOperationStatus.Succeeded)
    //     {
    //         var r_asset = handle.Result;
    //         ArenaEntityTownMonobehavior = r_asset.GetComponent<ArenaEntityTownMonobehavior>();
    //         ArenaEntityTownMonobehavior.Init(this);
    //     }
    //     else
    //     {
    //         Debug.LogError($"Error Load prefab::: {handle.Status}");
    //     }
    // }

    internal async void SetRoundData()
    {
        Debug.Log("TODO SetRound");

    }
    #endregion
}
