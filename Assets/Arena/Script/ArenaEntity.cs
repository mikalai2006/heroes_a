using System;
using System.Collections.Generic;
using System.Linq;

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
public enum TypeDirection
{
    Right = 0,
    Left = 1
}

[Serializable]
public class ArenaEntity
{
    private ArenaManager _arenaManager;

    [SerializeField]
    public ScriptableEntity _configData;
    public ScriptableEntity ConfigData => _configData;
    [NonSerialized] public GridArenaNode OccupiedNode = null;
    [NonSerialized] private GridArenaNode _relatedNode = null;
    public GridArenaNode RelatedNode => _relatedNode;
    [NonSerialized] public GridArenaNode ProtectedNode = null;
    [NonSerialized] private List<GridArenaNode> _path = null;
    public List<GridArenaNode> Path => _path;
    private Vector3 _centerNode;
    public Vector3 CenterNode => _centerNode;
    private Vector3 _positionPrefab;
    public Vector3 PositionPrefab => _positionPrefab;
    [NonSerialized] public ArenaMonoBehavior ArenaMonoBehavior;
    public TypeArenaPlayer TypeArenaPlayer;
    public BaseEntity Entity { get; private set; }
    public TypeDirection Direction;

    public ArenaEntity(ArenaManager arenaManager)
    {
        _arenaManager = arenaManager;
    }

    public void SetEntity(BaseEntity entity, GridArenaNode node)
    {
        Entity = entity;
        _configData = Entity.ScriptableData;
        OccupiedNode = node;
        // Debug.Log($"SetEntity::: {entity.ScriptableDataAttribute.name}");
        Direction = TypeArenaPlayer == TypeArenaPlayer.Left ? TypeDirection.Right : TypeDirection.Left;
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
        OccupiedNode = node;
    }

    private void NormalizeNode(GridArenaNode node)
    {
        if (RelatedNode != null && RelatedNode.OccupiedUnit == this)
        {
            RelatedNode.SetOcuppiedUnit(null);
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
        }
        // // disable old related data.
        // if (OccupiedNode.RightNode != null && OccupiedNode.RightNode.OccupiedUnit == this)
        // {
        //     OccupiedNode.RightNode.SetOcuppiedUnit(null);
        //     Debug.Log($"Clear related::: right {OccupiedNode.RightNode}");
        // }
        // else if (OccupiedNode.LeftNode != null && OccupiedNode.LeftNode.OccupiedUnit == this)
        // {
        //     OccupiedNode.LeftNode.SetOcuppiedUnit(null);
        //     Debug.Log($"Clear related::: left {OccupiedNode.LeftNode}");
        // }

        // // // set new related data.
        // var prevNode = node.cameFromNode != null ? node.cameFromNode : OccupiedNode;
        // var direction = _arenaManager.GridArenaHelper.GetDirection(prevNode, node);
        // if (direction == -1 && node.RightNode != null && node.RightNode.OccupiedUnit == null)
        // {
        //     // _positionPrefab = node.LeftNode.positionPrefab;
        //     //OccupiedNode = node.LeftNode;
        //     node.RightNode.SetOcuppiedUnit(this);
        //     Debug.Log($"Set related::: right {node.RightNode}");
        // }
        // else if (direction == 1 && node.LeftNode != null && node.LeftNode.OccupiedUnit == null)
        // {
        //     Debug.Log($"Set related::: left {node.LeftNode}");
        //     // _positionPrefab = node.RightNode.positionPrefab;
        //     //OccupiedNode = node.RightNode;
        //     node.LeftNode.SetOcuppiedUnit(this);
        // }
    }

    public TypeDirection Rotate(GridArenaNode node)
    {
        var prevNode = node.cameFromNode;
        if (prevNode != null)
        {
            var difference = node.center.x - prevNode.center.x;
            if (difference < 0)
            {
                Direction = TypeDirection.Left;
                // RelatedNode.SetOcuppiedUnit(null);
                // _relatedNode = node.RightNode;
                // RelatedNode.SetOcuppiedUnit(this);
                // Debug.Log($"Rotate Left::: {node.RightNode}");
            }
            else if (difference > 0)
            {
                Direction = TypeDirection.Right;
                // RelatedNode.SetOcuppiedUnit(null);
                // _relatedNode = node.LeftNode;
                // RelatedNode.SetOcuppiedUnit(this);
                // Debug.Log($"Rotate Right::: {node.LeftNode}");
            }
        }

        // if (((ScriptableAttributeCreature)Entity.ScriptableDataAttribute).CreatureParams.Size == 2)
        // {
        //     if (difference < 0)
        //     {
        //         OccupiedNode = OccupiedNode.LeftNode;
        //     }
        //     else
        //     {
        //         OccupiedNode = OccupiedNode.RightNode;
        //     }
        // }

        // _positionPrefab = OccupiedNode.positionPrefab;
        return Direction;
    }

    public void ChangePosition(GridArenaNode node)
    {
        // Debug.Log($"ChangePosition::: {OccupiedNode}");
        // OccupiedNode.SetOcuppiedUnit(null);
        // // OccupiedNode = node;
        // // node.SetOcuppiedUnit(this);
        // NormalizeNode(node);
        // _positionPrefab = node.positionPrefab;
        SetPosition(node);
        // _arenaManager.SetColorActiveNode();
        // _arenaManager.SetColorDisableNode();
    }

    public async void ClickCreature()
    {
        if (_arenaManager.FightingOccupiedNodes.Contains(OccupiedNode))
        {
            Debug.Log($"Fight node!");
            _arenaManager.CreateAttackNode(this);
        }
        else
        {
            Debug.Log($"Creature not maybe to attack!");
        }
        await UniTask.Delay(1);
    }

    public void DoHero(Player player)
    {

    }

    public void DestroyMapObject()
    {
        // OccupiedNode.SetOcuppiedUnit(null);
        // MapObjectGameObject.DestroyGameObject();
        // // MapObjectGameObject.DestroyMapObject();
        // UnitManager.MapObjects.Remove(IdMapObject);
    }
    public void CreateMapGameObject(GridArenaNode node)
    {
        // Debug.LogWarning($"CreateMapGameObject::: {Entity.ScriptableData.name}");
        // _positionPrefab = node.positionPrefab;
        // OccupiedNode = node;
        // Entity.SetMapObject(this);
        LoadGameObject();
    }


    public void SetPath(List<GridArenaNode> path)
    {
        _path = path;
    }

    #region LoadAsset
    private void LoadGameObject()
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
            Debug.LogWarning($"Not found ArenaPrefab {ConfigData.name}!");
            return;
        }

        Addressables.InstantiateAsync(
            gameObj,
            PositionPrefab, // + new Vector3(0, -.25f, 0),
            Quaternion.identity,
            _arenaManager.tileMapArenaUnits.transform
            ).Completed += LoadedAsset;
    }

    public virtual void LoadedAsset(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            var r_asset = handle.Result;
            ArenaMonoBehavior = r_asset.GetComponent<ArenaMonoBehavior>();
            // Debug.Log($"Spawn Entity::: {r_asset.name}");
            ArenaMonoBehavior.Init(this);
        }
        else
        {
            Debug.LogError($"Error Load prefab::: {handle.Status}");
        }
    }

    internal async UniTask GoAttack(GridArenaNode nodeForAttack)
    {
        var entityForAttack = nodeForAttack.OccupiedUnit;
        Debug.Log($"Attack [{this.Entity.ScriptableDataAttribute.name}] / [{entityForAttack.Entity.ScriptableDataAttribute.name}]");

        await ArenaMonoBehavior.RunAttack(nodeForAttack);

        await UniTask.Delay(1);
    }

    #endregion
}
