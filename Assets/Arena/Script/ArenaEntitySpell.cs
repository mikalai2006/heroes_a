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
public class ArenaEntitySpellData
{
    // int - quantity round
    public Dictionary<ScriptableAttributeSpell, int> SpellsState = new();
}

[Serializable]
public class ArenaEntitySpell
{
    public ArenaEntitySpellData Data = new();

    private ArenaManager _arenaManager;
    // public static event Action OnChangeParamsCreature;

    [NonSerialized] public GridArenaNode OccupiedNode = null;
    [NonSerialized] private List<GridArenaNode> _relatedNodes = null;
    public List<GridArenaNode> RelatedNodes => _relatedNodes;
    public ScriptableAttributeSpell ConfigDataSpell;
    private Vector3 _centerNode;
    public Vector3 CenterNode => _centerNode;
    private Vector3 _positionPrefab;
    public Vector3 PositionPrefab => _positionPrefab;
    [NonSerialized] public ArenaSpellMonoBehavior ArenaSpellMonoBehavior;
    public TypeArenaPlayer TypeArenaPlayer;
    private EntityHero _hero;

    public ArenaEntitySpell(
        GridArenaNode node,
        ScriptableAttributeSpell configData,
        EntityHero heroRunSpell,
        ArenaManager arenaManager
        )
    {
        _arenaManager = arenaManager;
        OccupiedNode = node;
        _hero = heroRunSpell;
        ConfigDataSpell = configData;
        TypeArenaPlayer = arenaManager.ArenaQueue.activeEntity.arenaEntity.TypeArenaPlayer;
        node.SetSpellsUnit(this);
    }

    public async void ClickEntitySpell(InputAction.CallbackContext context)
    {
        Debug.Log("ClickEntitySpell");
        await AudioManager.Instance.Click();
        if (_arenaManager.FightingOccupiedNodes.Contains(OccupiedNode))
        {

            Debug.Log($"Choose spell for spell!");
            _arenaManager.CreateButtonSpell(OccupiedNode);
        }
        else
        {
            Debug.Log($"Spell not maybe to cancel!");
            await _arenaManager.ClickArena(OccupiedNode.position);
        }
        await UniTask.Delay(1);
    }

    #region CreateDestroy
    public void CreateMapGameObject()
    {
        LoadGameObject();
    }

    public void DestroyMapObject()
    {
        GameObject.Destroy(ArenaSpellMonoBehavior);
        OccupiedNode.SetSpellsUnit(null);
    }

    private void LoadGameObject()
    {
        AssetReferenceGameObject gameObj = null;
        if (ConfigDataSpell != null && ConfigDataSpell.AnimatePrefab.RuntimeKeyIsValid())
        {
            gameObj = ConfigDataSpell.AnimatePrefab;
        }

        if (gameObj == null)
        {
            Debug.LogWarning($"Not found Prefab {ConfigDataSpell.name}!");
            return;
        }

        var arenaUnits = GameObject.FindGameObjectWithTag("ArenaUnits");

        Addressables.InstantiateAsync(
            gameObj,
            OccupiedNode.center,
            Quaternion.identity,
            arenaUnits.transform
            ).Completed += LoadedAsset;
    }

    public virtual void LoadedAsset(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            var r_asset = handle.Result;
            ArenaSpellMonoBehavior = r_asset.GetComponent<ArenaSpellMonoBehavior>();
            // Debug.Log($"Spawn Entity::: {r_asset.name}");
            ArenaSpellMonoBehavior.Init(this);
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
