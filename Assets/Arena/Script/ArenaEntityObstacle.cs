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
using UnityEngine.Tilemaps;

[Serializable]
public class ArenaEntityObstacleData
{
    // int - quantity round
    // public Dictionary<ScriptableAttributeSpell, int> SpellsState = new();
    public bool isDestroyed;
}

[Serializable]
public class ArenaEntityObstacle
{
    public ArenaEntityObstacleData Data = new();
    private ArenaManager _arenaManager;
    [NonSerialized] public GridArenaNode OccupiedNode = null;
    [NonSerialized] private List<GridArenaNode> _relatedNodes = new();
    public List<GridArenaNode> RelatedNodes => _relatedNodes;
    public ScriptableAttributeSpell ConfigDataObtacle;
    private Vector3 _centerNode;
    public Vector3 CenterNode => _centerNode;
    private Vector3 _positionPrefab;
    public Vector3 PositionPrefab => _positionPrefab;
    [NonSerialized] public ArenaEntityObstacleMonobehavior ArenaEntityObstacleMonobehavior;
    public TypeArenaPlayer TypeArenaPlayer;
    private ArenaHeroEntity _hero;
    public ArenaHeroEntity Hero => _hero;

    public ArenaEntityObstacle(
        GridArenaNode node,
        ScriptableAttributeSpell configData,
        ArenaHeroEntity heroRunSpell,
        ArenaManager arenaManager
        )
    {
        _arenaManager = arenaManager;
        OccupiedNode = node;
        _hero = heroRunSpell;
        ConfigDataObtacle = configData;
        TypeArenaPlayer = arenaManager.ArenaQueue.activeEntity.arenaEntity.TypeArenaPlayer;
        // node.SetSpellsUnit(this);
    }

    public async void ClickEntityObstacle(InputAction.CallbackContext context)
    {
        Debug.Log("ClickEntitySpell");
        await AudioManager.Instance.Click();
        if (_arenaManager.FightingOccupiedNodes.Contains(_arenaManager.clickedNode))
        {

            Debug.Log($"Choose spell for spell!");
            _arenaManager.CreateButtonSpell(OccupiedNode);
        }
        else
        {
            Debug.Log($"Spell not maybe to cancel!");
            await _arenaManager.DrawPath(OccupiedNode);
        }
        await UniTask.Delay(1);
    }

    #region CreateDestroy
    public void CreateMapGameObject()
    {
        LoadGameObject();
    }
    public void AddRelatedNode(GridArenaNode node)
    {
        _relatedNodes.Add(node);
    }

    public void DestroyMapObject()
    {
        GameObject.Destroy(ArenaEntityObstacleMonobehavior);
        OccupiedNode.SetSpellsUnit(null);
        OccupiedNode.SetSpellsStatus(false);
    }

    private void LoadGameObject()
    {
        // AssetReferenceGameObject gameObj = null;
        // if (ConfigDataSpell != null && ConfigDataSpell.AnimatePrefab.RuntimeKeyIsValid())
        // {
        //     gameObj = ConfigDataSpell.AnimatePrefab;
        // }

        // if (gameObj == null)
        // {
        //     Debug.LogWarning($"Not found Prefab {ConfigDataSpell.name}!");
        //     return;
        // }

        // var arenaUnits = GameObject.FindGameObjectWithTag("ArenaUnits");

        // Addressables.InstantiateAsync(
        //     gameObj,
        //     OccupiedNode.center,
        //     Quaternion.identity,
        //     arenaUnits.transform
        //     ).Completed += LoadedAsset;
    }

    public virtual void LoadedAsset(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            var r_asset = handle.Result;
            ArenaEntityObstacleMonobehavior = r_asset.GetComponent<ArenaEntityObstacleMonobehavior>();
            // ArenaSpellMonoBehavior.Init(this);
        }
        else
        {
            Debug.LogError($"Error Load prefab::: {handle.Status}");
        }
    }

    internal async void SetRoundData()
    {
        Debug.Log("TODO SetRound");
        var spells = OccupiedNode.SpellsState.Keys.ToList();
        foreach (var spell in spells)
        {
            OccupiedNode.SpellsState[spell] -= 1;
            if (OccupiedNode.SpellsState[spell] <= 0)
            {
                await spell.RemoveEffect(OccupiedNode, _hero);
                OccupiedNode.SpellsState.Remove(spell);
            }
        }
    }
    #endregion
}
