using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Cysharp.Threading.Tasks;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Tilemaps;

[Serializable]
public struct CursorArena
{
    public RuleTile NotAllow;
    public RuleTile GoGround;
    public RuleTile GoFlying;
    public RuleTile FightFromLeft;
    public RuleTile FightFromTopLeft;
    public RuleTile FightFromTop;
    public RuleTile FightFromTopRight;
    public RuleTile FightFromRight;
    public RuleTile FightFromBottomRight;
    public RuleTile FightFromBottom;
    public RuleTile FightFromBottomLeft;
    public RuleTile Shoot;
    public RuleTile ShootHalf;
    public RuleTile Spell;
    public RuleTile Catapult;
    public RuleTile FirstAidTent;
}

[Serializable]
public struct AttackItemNode
{
    public GridArenaNode nodeFromAttack;
    public GridArenaNode nodeToAttack;
}

public enum TypeModeArena
{
    Running = 1,
    Stopped = 2,
}

public class ArenaManager : MonoBehaviour
{
    public CursorArena CursorRule;
    public RuleTile activeCursor;
    public TypeModeArena typeMode;
    // [SerializeField] private Tilemap _tileMapCursor;
    // public static event Action OnSetNextCreature;
    public static event Action OnChangeNodesForAttack;
    public static event Action OnRefreshButton;
    // public static event Action OnEndBattle;
    // public static event Action OnRunFromBattle;
    public static event Action OnAutoNextCreature;
    public static event Action OnHideSpellInfo;
    public static event Action OnChooseCreatureForSpell;
    public static event Action OnShowState;
    [SerializeField] private int width = 15;
    [SerializeField] private int height = 11;
    [SerializeField] public Tilemap tileMapArenaGrid;
    [SerializeField] public Tilemap tileMapArenaUnits;
    [SerializeField] public Tilemap _tileMapDistance;
    [SerializeField] private Tilemap _tileMapShadow;
    [SerializeField] private Tilemap _tileMapUnitActive;
    [SerializeField] private Tilemap _tileMapAllowAttackNode;
    [SerializeField] private Tilemap _tileMapAllowAttack;
    [SerializeField] private Tilemap _tileMapPath;
    [SerializeField] private Tilemap _tileMapObstacles;

    [SerializeField] public GameObject _textPrefab;
    [SerializeField] public GameObject _textCanvas;
    private Dictionary<Vector3, GameObject> _listText = new Dictionary<Vector3, GameObject>();

    private GridArenaHelper _gridArenaHelper;
    public GridArenaHelper GridArenaHelper => _gridArenaHelper;
    [SerializeField] public Tile _tileHex;
    [SerializeField] public Tile _tileHexShadow;
    [SerializeField] public Tile _tileHexActive;
    private int zCoord = -14;
    // private SerializableDictionary<int, List<Vector2>> schemaCreatures = new();

    public ArenaHeroEntity heroLeft;
    public ArenaHeroEntity heroRight;
    public ArenaEntityTown ArenaTown = null;
    public GameObject clickedFortification;
    public GameObject buttonAction;
    public GameObject _buttonAction;
    public GameObject buttonSpell;
    public GameObject _buttonSpell;
    public GameObject buttonWarMachine;
    public GameObject _buttonWarMachine;
    private int maxCountObstacle = 0;
    // [NonSerialized] public List<ArenaEntity> ArenaEnteties;
    [NonSerialized] public List<GridArenaNode> DistanceNodes = new();
    [NonSerialized] public List<GridArenaNode> PathNodes = new();
    [NonSerialized] public List<GridArenaNode> FightingOccupiedNodes = new();
    // [NonSerialized] public List<GridArenaNode> AllowRunSpellNodes = new();
    [NonSerialized] public List<GridArenaNode> AllMovedNodes = new();
    [NonSerialized] public List<GridArenaNode> AllowMovedNodes = new();
    public List<GridArenaNode> AllowPathNodes => PathNodes.Concat(AllowMovedNodes).ToList();
    // 1 - node attack, 2 - occupied,related node
    public List<AttackItemNode> NodesForAttackActiveCreature = new();
    public int KeyNodeFromAttack = -1;
    public GridArenaNode clickedNode;
    [NonSerialized] public ArenaEntityBase AttackedCreature;
    // [NonSerialized] public GridArenaNode NodeForSpell;
    // [NonSerialized] public GridArenaNode NodeForAttack;
    public ArenaQueue ArenaQueue = new ArenaQueue();
    public ArenaStat ArenaStat = new ArenaStat();
    [SerializeField] private Camera _camera;
    public bool isRunningAction;
    public DialogArenaData DialogArenaData;
    public ScriptableAttributeSpell ChoosedSpell
        => ArenaQueue.ActiveHero != null
            && ArenaQueue.ActiveHero.Entity != null
            && ArenaQueue.ActiveHero.Entity.SpellBook != null
                ? ArenaQueue.ActiveHero.Entity.SpellBook.ChoosedSpell
                : null;
    private InputManager inputManager;


    private void OnEnable()
    {
        inputManager = new InputManager();
        inputManager.Enable();
        inputManager.Click += OnClick;
    }

    private void OnDisable()
    {
        inputManager.Click -= OnClick;
        inputManager.Disable();
    }

    private void Start()
    {
        _buttonAction = GameObject.Instantiate(
            buttonAction,
            new Vector3(0, 0, -5),
            Quaternion.identity,
            transform
        );
        _buttonSpell = GameObject.Instantiate(
            buttonSpell,
            new Vector3(0, 0, -5),
            Quaternion.identity,
            transform
        );
        _buttonWarMachine = GameObject.Instantiate(
            buttonWarMachine,
            new Vector3(0, 0, -5),
            Quaternion.identity,
            transform
        );
        // inputManager.OnStartTouch += OnClick;
        UIArena.OnNextCreature += RunNextCreature;
        UIArena.OnOpenSpellBook += OpenSpellBook;
        UIArena.OnClickAttack += ActionClickButton;
        UIDialogSpellBook.OnClickSpell += ChooseSpell;
        UIArena.OnCancelSpell += EndSpell;
        UIArena.OnClickAutoBattle += ClickButtonAutoBattle;

        // CreateArena();
        maxCountObstacle = LevelManager.Instance.ConfigGameSettings.arenaMaxCountObstacles;

        ArenaStat.Init(this);
    }

    private void OnDestroy()
    {
        UIArena.OnNextCreature -= RunNextCreature;
        UIArena.OnOpenSpellBook -= OpenSpellBook;
        UIArena.OnClickAttack -= ActionClickButton;
        UIDialogSpellBook.OnClickSpell -= ChooseSpell;
        UIArena.OnCancelSpell -= EndSpell;
        UIArena.OnClickAutoBattle -= ClickButtonAutoBattle;
    }

    public void DisableInputSystem()
    {
        inputManager.Disable();
    }
    public void EnableInputSystem()
    {
        inputManager.Enable();
    }

    private async void OpenSpellBook()
    {
        inputManager.Disable();

        var dialogWindow = new DialogSpellBookOperation(ArenaQueue.ActiveHero.Entity);
        var result = await dialogWindow.ShowAndHide();

        inputManager.Enable();
    }

    public async UniTask CreateMoveArea()
    {
#if UNITY_EDITOR
        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        stopWatch.Start();
#endif
        ArenaEntityBase activeArenaEntity = ArenaQueue.activeEntity.arenaEntity;
        ScriptableAttributeCreature activeEntityCreature = (ScriptableAttributeCreature)activeArenaEntity.Entity.ScriptableDataAttribute;

        ResetArenaState();
        DistanceNodes = GridArenaHelper.GetNeighboursAtDistance(
            activeArenaEntity.OccupiedNode,
            activeArenaEntity.Speed
            );

        _gridArenaHelper.CreateWeightCellByX(DistanceNodes, activeArenaEntity.OccupiedNode);

        foreach (var neiNode in DistanceNodes)
        {
            // _tileMapDistance.SetTile(neiNode.position, _tileHexShadow);

            // // Set formal disable node for pathfinding.
            if (activeEntityCreature.CreatureParams.Size > 1)
            {
                if (neiNode.OccupiedUnit != null)
                {
                    if (
                        neiNode.LeftNode != null
                        && neiNode.LeftNode.OccupiedUnit == null
                        && activeArenaEntity.TypeArenaPlayer == TypeArenaPlayer.Right
                        && neiNode.OccupiedUnit != activeArenaEntity.OccupiedNode.OccupiedUnit
                        && !neiNode.LeftNode.StateArenaNode.HasFlag(StateArenaNode.Bridge)
                        )
                    {
                        neiNode.LeftNode.StateArenaNode |= StateArenaNode.Moved;
                        AllMovedNodes.Add(neiNode.LeftNode);
                    }
                    else if (
                        neiNode.RightNode != null
                        && activeArenaEntity.TypeArenaPlayer == TypeArenaPlayer.Left
                        && neiNode.RightNode.OccupiedUnit == null
                        && neiNode.OccupiedUnit != activeArenaEntity.OccupiedNode.OccupiedUnit
                        && !neiNode.RightNode.StateArenaNode.HasFlag(StateArenaNode.Bridge)
                        )
                    {
                        neiNode.RightNode.StateArenaNode |= StateArenaNode.Moved;
                        AllMovedNodes.Add(neiNode.RightNode);
                    }
                }
            }
        };

        // foreach (var neiNode in GridArenaHelper.GetAllGridNodes())
        // {
        //     SetTextMeshNode(neiNode);
        // };

        // ResetTextMeshNode();
        var nodes = GridArenaHelper.GetNeighboursAtDistanceAndFindPath(DistanceNodes, activeArenaEntity.OccupiedNode);

        _gridArenaHelper.CreateWeightCellByX(nodes, activeArenaEntity.OccupiedNode);

        var settingGame = LevelManager.Instance.ConfigGameSettings;

        foreach (var neiNode in nodes)
        {
            if (neiNode.weight >= activeEntityCreature.CreatureParams.Size)
            {
                if (settingGame.showShadowGrid)
                {
                    FillShadowNode(_tileMapShadow, neiNode);
                };
                // _tileMapShadow.SetTile(neiNode.position, _tileHexShadow);
                PathNodes.Add(neiNode);
            }
        };

        if (activeEntityCreature.CreatureParams.Size >= 2)
        {
            foreach (var neiNode in AllMovedNodes)
            {
                if (neiNode.weight >= 2)
                {
                    if (settingGame.showShadowGrid)
                    {
                        FillShadowNode(_tileMapShadow, neiNode);
                    }
                    AllowMovedNodes.Add(neiNode);
                }
            };
        }
        // // Draw help text.
        // ResetTextMeshNode();
        // foreach (var neiNode in GridArenaHelper.GetAllGridNodes())
        // {
        //     SetTextMeshNode(neiNode);
        // };

        // lighting allow fighting nodes.
        // await ArenaQueue.activeEntity.arenaEntity.GetFightingNodes();

#if UNITY_EDITOR
        stopWatch.Stop();
        System.TimeSpan timeTaken = stopWatch.Elapsed;
        Debug.LogWarning($"Time Generation Step::: {timeTaken.ToString(@"m\:ss\.ffff")}");
#endif
        await UniTask.Delay(1);
    }

    private async void RunNextCreature(bool wait, bool def)
    {
        await NextCreature(wait, def);
    }

    private async void ClickButtonAutoBattle()
    {
        await ArenaQueue.ActiveHero.SetStatusAutoRun(!ArenaQueue.ActiveHero.Data.autoRun);
        OnRefreshButton?.Invoke();
    }

    public async UniTask NextCreature(bool wait, bool def)
    {
        // Check end battle.
        var countCreaturesLeft = ArenaQueue.ListEntities.Where(t => t.arenaEntity.TypeArenaPlayer == TypeArenaPlayer.Left).Count();
        var countCreaturesRight = ArenaQueue.ListEntities.Where(t => t.arenaEntity.TypeArenaPlayer == TypeArenaPlayer.Right).Count();

        if (countCreaturesLeft == 0 || countCreaturesRight == 0)
        {
            if (countCreaturesLeft == 0)
            {
                heroLeft.typearenaHeroStatus = TypearenaHeroStatus.Defendied;
                heroRight.typearenaHeroStatus = TypearenaHeroStatus.Victorious;
            }
            else
            {
                heroLeft.typearenaHeroStatus = TypearenaHeroStatus.Victorious;
                heroRight.typearenaHeroStatus = TypearenaHeroStatus.Defendied;
            }
            await CalculateStat();
            return;
        }
        else
        {
            ArenaQueue.NextCreature(wait, def);

            await ArenaQueue.activeEntity.arenaEntity.GetFightingNodes();

            // lighting active creature.
            LightingActiveNode();

            OnAutoNextCreature?.Invoke();

            if (!ArenaQueue.activeEntity.arenaEntity.Data.isRun)
            {
                await NextCreature(false, false);
            }
            else if (ArenaQueue.ActiveHero.Data.autoRun)
            {
                // Run AI
                await ArenaQueue.ActiveHero.AI.Run();
            }
        }
    }

    private void FillShadowNode(Tilemap tilemap, GridArenaNode node)
    {
        var settingGame = LevelManager.Instance.ConfigGameSettings;
        // if (settingGame.showShadowGrid)
        // {
        tilemap.SetTile(node.position, _tileHexShadow);
        tilemap.SetTileFlags(node.position, TileFlags.None);
        tilemap.SetColor(node.position, settingGame.colorShadow);
        tilemap.SetTileFlags(node.position, TileFlags.LockColor);
        // }
    }

    public void DrawNotAllowButton()
    {
        RuleTile ruleCursor = CursorRule.NotAllow;
        Vector3 positionButton = new Vector3(clickedNode.center.x, clickedNode.center.y, zCoord);
        _buttonAction.SetActive(true);
        _buttonSpell.SetActive(false);
        _buttonWarMachine.SetActive(false);
        _buttonAction.GetComponent<SpriteRenderer>().sprite = ruleCursor.m_DefaultSprite;
        _buttonAction.transform.position = positionButton;
        activeCursor = ruleCursor;
    }

    public void DrawButtonAction()
    {
        if (clickedNode == null) return;
        RuleTile ruleCursor = CursorRule.NotAllow;
        Vector3 positionButton = new Vector3(clickedNode.center.x, clickedNode.center.y, zCoord);
        // create direction icon.
        var scale = new Vector3(1, 1, 1);

        if (ChoosedSpell == null)
        {
            ArenaEntityBase activeArenaEntity = ArenaQueue.activeEntity.arenaEntity;
            ScriptableAttributeCreature activeCreatureData
                = (ScriptableAttributeCreature)activeArenaEntity.Entity.ScriptableDataAttribute;

            if (AllowPathNodes.Contains(clickedNode))
            {
                ruleCursor = activeCreatureData.CreatureParams.Movement == MovementType.Flying
                    ? CursorRule.GoFlying
                    : CursorRule.GoGround;
            }
            if (NodesForAttackActiveCreature.Count > 0)
            {
                if (KeyNodeFromAttack != -1 && AttackedCreature != null)
                {
                    var nodesForAttack = NodesForAttackActiveCreature[KeyNodeFromAttack];

                    switch (ArenaQueue.activeEntity.arenaEntity.Data.typeAttack)
                    {
                        case TypeAttack.Attack:
                            Vector3 difPos = nodesForAttack.nodeFromAttack.center - nodesForAttack.nodeToAttack.center;
                            if (difPos.x > 0 && difPos.y > 0)
                            {
                                ruleCursor = CursorRule.FightFromTopRight;
                            }
                            else if (difPos.x < 0 && difPos.y > 0)
                            {
                                ruleCursor = CursorRule.FightFromTopLeft;
                            }
                            else if (difPos.x > 0 && difPos.y == 0)
                            {
                                ruleCursor = CursorRule.FightFromRight;
                            }
                            else if (difPos.x < 0 && difPos.y == 0)
                            {
                                ruleCursor = CursorRule.FightFromLeft;
                            }
                            else if (difPos.x < 0 && difPos.y < 0)
                            {
                                ruleCursor = CursorRule.FightFromBottomLeft;
                            }
                            else if (difPos.x > 0 && difPos.y < 0)
                            {
                                ruleCursor = CursorRule.FightFromBottomRight;
                            }
                            break;
                        case TypeAttack.AttackShoot:
                            if (nodesForAttack.nodeToAttack.DistanceTo(ArenaQueue.activeEntity.arenaEntity.OccupiedNode) <= ArenaQueue.activeEntity.arenaEntity.Speed)
                            {
                                ruleCursor = CursorRule.Shoot;
                            }
                            else
                            {
                                ruleCursor = CursorRule.ShootHalf;
                            }
                            positionButton = new Vector3(nodesForAttack.nodeToAttack.center.x, nodesForAttack.nodeToAttack.center.y, zCoord);
                            break;
                        case TypeAttack.Aid:
                            ruleCursor = CursorRule.FirstAidTent;
                            positionButton = new Vector3(nodesForAttack.nodeToAttack.center.x, nodesForAttack.nodeToAttack.center.y, zCoord);
                            break;
                        default:
                            ruleCursor = CursorRule.FightFromLeft;
                            break;
                    }
                }
            }

            // Check move cursor for inverse direction.
            if (
                (ruleCursor == CursorRule.GoFlying || ruleCursor == CursorRule.GoGround)
                &&
                activeArenaEntity.OccupiedNode.center.x > clickedNode.center.x
            )
            {
                scale = new Vector3(-1, 1, 1);
            }
        }

        _buttonAction.SetActive(true);
        _buttonSpell.SetActive(false);
        _buttonWarMachine.SetActive(false);
        _buttonAction.GetComponent<SpriteRenderer>().sprite = ruleCursor.m_DefaultSprite;
        _buttonAction.transform.position = positionButton;
        _buttonAction.transform.localScale = scale;
        activeCursor = ruleCursor;
        // _tileMapCursor.SetTile(clickedNode.position, ruleCursor);
    }

    public async void ActionClickButton()
    {
        await ArenaQueue.activeEntity.arenaEntity.ClickButtonAction();
    }

    public async UniTask DrawPath(GridArenaNode node)
    {
        // GridArenaNode node = GridArenaHelper.GridTile.GetGridObject(new Vector3Int(positionClick.x, positionClick.y));
        if (node == null) return;


        if (AllowPathNodes.Contains(node))
        {
            if (clickedNode != node)
            {
                //Find path.
                var path = GridArenaHelper.FindPath(ArenaQueue.activeEntity.arenaEntity.OccupiedNode.position, node.position, PathNodes);
                if (path == null)
                {
                    return;
                }

                // Draw path.
                _tileMapPath.ClearAllTiles();
                ((ArenaCreature)ArenaQueue.activeEntity.arenaEntity).SetPath(path);
                var settingGame = LevelManager.Instance.ConfigGameSettings;
                if (settingGame.showPath)
                {
                    foreach (var pathNode in path)
                    {
                        // _tileMapPath.SetTile(pathNode.position, _tileHexShadow);
                        FillShadowNode(_tileMapPath, pathNode);
                    };
                }

                if (settingGame.showShadowCursor)
                {
                    var endNode = path[path.Count - 1];
                    // _tileMapPath.SetTile(endNode.position, _tileHexShadow);
                    FillShadowNode(_tileMapPath, endNode);
                    if (((EntityCreature)ArenaQueue.activeEntity.arenaEntity.Entity).ConfigAttribute.CreatureParams.Size == 2)
                    {
                        var relatedNewNode = ArenaQueue.activeEntity.arenaEntity.TypeArenaPlayer == TypeArenaPlayer.Left
                            ? endNode.LeftNode
                            : endNode.RightNode;
                        FillShadowNode(_tileMapPath, relatedNewNode);
                        // _tileMapPath.SetTile(relatedNewNode.position, _tileHexShadow);
                    }
                }
                // Set active click node.
                clickedNode = node;
            }
        }
        else
        {
            // Click not allowed node.
            _tileMapPath.ClearAllTiles();
            clickedNode = node;
        }
        await UniTask.Delay(1);
    }

    public void ResetArenaState()
    {
        // Clear clicked node.
        // clickedNode = null;
        ClearAttackNode();
        // isRunningAction = false;
        _tileMapAllowAttack.ClearAllTiles();


        NodesForAttackActiveCreature.Clear();
        AllowPathNodes.Clear();
        FightingOccupiedNodes.Clear();
        DistanceNodes.Clear();
        PathNodes.Clear();
        if (ChoosedSpell != null)
        {
            ArenaQueue.ActiveHero.Entity.SpellBook.ChooseSpell(null);
        }

        foreach (var neiNode in AllMovedNodes)
        {
            neiNode.StateArenaNode ^= StateArenaNode.Moved;
        };

        if (ArenaTown != null && ArenaTown.ArenaEntityTownMB != null)
        {
            ArenaTown.ArenaEntityTownMB.SetStatusColliders(false);
        }
        AllMovedNodes.Clear();
        AllowMovedNodes.Clear();
        ResetButton();
        // _tileMapAllowAttack.ClearAllTiles();
        _tileMapDistance.ClearAllTiles();
        _tileMapPath.ClearAllTiles();
        _tileMapShadow.ClearAllTiles();
        _tileMapAllowAttackNode.ClearAllTiles();
        // _tileMapCursor.ClearAllTiles();
    }

    private void ResetButton()
    {
        _buttonAction.SetActive(false);
        _buttonSpell.SetActive(false);
        _buttonWarMachine.SetActive(false);
    }

    public async void OnClick(InputAction.CallbackContext context)
    {
        // if (!context.started) return;
        if (context.performed && !isRunningAction)
        {
            var pos = context.ReadValue<Vector2>();
            var rayHit = Physics2D.GetRayIntersection(_camera.ScreenPointToRay(pos));

            if (!rayHit.collider)
            {
                return;
            }

            Vector2 posMouse = _camera.ScreenToWorldPoint(pos);
            Vector3Int tilePos = tileMapArenaGrid.WorldToCell(posMouse);

            GridArenaNode node = GridArenaHelper.GridTile.GetGridObject(tilePos);
            Debug.Log($"Click node::: {node}");

            if (rayHit.collider.gameObject == tileMapArenaGrid.gameObject)
            {
                ClearAttackNode();
                if (ChoosedSpell != null && node != null)
                {
                    clickedNode = node;
                    if (ChoosedSpell.typeSpellTarget == TypeSpellTarget.Node)
                    {
                        CreateButtonSpell(node);
                    }
                    else
                    {
                        DrawNotAllowButton();
                    }
                    OnChooseCreatureForSpell?.Invoke();
                    return;
                }
                else
                {
                    await DrawPath(node);
                    DrawButtonAction();
                }
            }
            else if (rayHit.collider.gameObject == _buttonAction.gameObject)
            {
                // await ClickButton();
                await ArenaQueue.activeEntity.arenaEntity.ClickButtonAction();
            }
            else if (rayHit.collider.gameObject == _buttonSpell.gameObject)
            {
                await ClickButtonSpell();
            }
            else if (rayHit.collider.gameObject == _buttonWarMachine.gameObject)
            {
                await ArenaQueue.activeEntity.arenaEntity.ClickButtonAction();
            }
        }
    }

    public void ClearAttackNode()
    {
        // Debug.Log("Clear attack Node");
        _tileMapAllowAttackNode.ClearAllTiles();
        _tileMapPath.ClearAllTiles();
        NodesForAttackActiveCreature.Clear();
        KeyNodeFromAttack = -1;
        AttackedCreature = null;
        _buttonAction.SetActive(false);
        OnChangeNodesForAttack?.Invoke();
    }

    public void DrawAttackNodes()
    {
        _tileMapAllowAttackNode.ClearAllTiles();
        if (LevelManager.Instance.ConfigGameSettings.paintAllowAttackNode)
        {
            foreach (var node in NodesForAttackActiveCreature)
            {
                _tileMapAllowAttackNode.SetTile(node.nodeFromAttack.position, _tileHexActive);
                _tileMapAllowAttackNode.SetTileFlags(node.nodeFromAttack.position, TileFlags.None);
                _tileMapAllowAttackNode.SetColor(node.nodeFromAttack.position, LevelManager.Instance.ConfigGameSettings.colorAllowAttackNode);
                _tileMapAllowAttackNode.SetTileFlags(node.nodeFromAttack.position, TileFlags.LockColor);
            }
        }
        OnChangeNodesForAttack?.Invoke();
    }

    public async void ChooseNextPositionForAttack()
    {
        if (NodesForAttackActiveCreature.Count() == 0)
        {
            DrawNotAllowButton();
            return;
        }
        int nextIndex = KeyNodeFromAttack + 1;
        int indexNextAttackNode = nextIndex >= NodesForAttackActiveCreature.Count()
            ? 0
            : nextIndex;
        KeyNodeFromAttack = indexNextAttackNode;
        // Debug.Log($"{NodesForAttackActiveCreature.Count()}/{indexNextAttackNode}");

        if (ArenaQueue.activeEntity.arenaEntity.Data.typeAttack == TypeAttack.Attack)
        {
            await DrawPath(NodesForAttackActiveCreature[indexNextAttackNode].nodeFromAttack);
        }
        if (clickedNode != null) DrawButtonAction();
    }

    private async void ChooseSpell()
    {
        //ClearAttackNode();
        _tileMapAllowAttack.ClearAllTiles();
        //FightingOccupiedNodes.Clear();
        FightingOccupiedNodes.Clear();
        var arenaEntity = ArenaQueue.activeEntity.arenaEntity;

        var occupiedNodes = await ChoosedSpell.ChooseTarget(this, ArenaQueue.ActiveHero);

        foreach (var node in occupiedNodes)
        {
            if (!node.StateArenaNode.HasFlag(StateArenaNode.Related))
            {
                FightingOccupiedNodes.Add(node);
                if (ChoosedSpell.typeSpellRun != TypeSpellRun.Immediately)
                {
                    SetColorAllowFightNode(node, LevelManager.Instance.ConfigGameSettings.colorNodeRunSpell);
                }
            }
        }

        if (ArenaQueue.ActiveHero != null)
        {
            // if expert school, fun for all.
            var baseSSkill = ChoosedSpell.SchoolMagic.BaseSecondarySkill;
            if (
                ArenaQueue.ActiveHero.Entity.Data.SSkills.ContainsKey(baseSSkill.TypeTwoSkill)
                && ChoosedSpell.typeSpellRun == TypeSpellRun.Collective
                )
            {
                var dataSSkill = ArenaQueue.ActiveHero.Entity.Data.SSkills[baseSSkill.TypeTwoSkill];
                if (dataSSkill.level >= 2)
                {
                    List<UniTask> listTasks = new();
                    for (int i = 0; i < FightingOccupiedNodes.Count; i++)
                    {
                        // listTasks.Add(FightingOccupiedNodes.ElementAt(i).OccupiedUnit.RunSpell());
                        listTasks.Add(ArenaQueue.ActiveHero.Entity.SpellBook.RunSpellCombat(FightingOccupiedNodes.ElementAt(i), ArenaQueue.ActiveHero, this));
                    }
                    await ArenaQueue.ActiveHero.ArenaHeroMonoBehavior.RunSpellAnimation();
                    await UniTask.WhenAll(listTasks);
                    ArenaQueue.Refresh();
                    EndSpell();
                }
            }
        }
        // if immediately run spell.
        if (ChoosedSpell != null && ChoosedSpell.typeSpellRun == TypeSpellRun.Immediately)
        {
            await ArenaQueue.ActiveHero.ArenaHeroMonoBehavior.RunSpellAnimation();
            await ArenaQueue.ActiveHero.Entity.SpellBook
                .RunSpellCombat(FightingOccupiedNodes[UnityEngine.Random.Range(0, FightingOccupiedNodes.Count - 1)], ArenaQueue.ActiveHero, this);
            ArenaQueue.Refresh();
            EndSpell();
        }
    }

    internal void CreateButtonSpell(GridArenaNode nodeForSpell)
    {
        //clickedNode = null;
        ClearAttackNode();

        // AttackedCreature = clickedEntity;
        clickedNode = nodeForSpell; // NodeForSpell = nodeForSpell;


        var positionButton = new Vector3(clickedNode.center.x, clickedNode.center.y, zCoord);

        if (
            FightingOccupiedNodes.Count > 0
            && clickedNode != null
            && FightingOccupiedNodes.Contains(clickedNode)
            )
        {
            _buttonSpell.SetActive(true);
            _buttonAction.SetActive(false);
            _buttonSpell.transform.position = positionButton;
        }
        OnChooseCreatureForSpell?.Invoke();
    }

    public async UniTask ClickButtonSpell()
    {
        Debug.Log("ClickButtonSpell");
        isRunningAction = true;
        OnHideSpellInfo?.Invoke();

        await AudioManager.Instance.Click();
        // await NodeForSpell.OccupiedUnit.RunSpell();
        await ArenaQueue.ActiveHero.ArenaHeroMonoBehavior.RunSpellAnimation();
        await ArenaQueue.ActiveHero.Entity.SpellBook.RunSpellCombat(clickedNode, ArenaQueue.ActiveHero, this);
        _buttonSpell.SetActive(false);

        ArenaQueue.Refresh();
        EndSpell();
        isRunningAction = false;
    }

    private async void EndSpell()
    {
        _tileMapAllowAttack.ClearAllTiles();
        _buttonSpell.SetActive(false);
        ArenaQueue.ActiveHero.Entity.SpellBook.ChooseSpell(null);
        await ArenaQueue.activeEntity.arenaEntity.GetFightingNodes();
        clickedNode = null;
        OnAutoNextCreature?.Invoke();
    }

    internal void CreateButtonCatapult(GameObject gameObject)
    {
        // clickedNode = null;
        ClearAttackNode();

        clickedFortification = gameObject;

        var positionButton = new Vector3(
            gameObject.transform.position.x,
            gameObject.transform.position.y,
            zCoord);
        var warMachine = ((ScriptableAttributeWarMachine)((EntityCreature)ArenaQueue.activeEntity.arenaEntity.Entity).ConfigAttribute);

        RuleTile cursor = CursorRule.Catapult;
        if (gameObject != null)
        {
            _buttonWarMachine.SetActive(true);
            _buttonAction.SetActive(false);
            _buttonSpell.SetActive(false);
            _buttonWarMachine.GetComponent<SpriteRenderer>().sprite = cursor.m_DefaultSprite;
            _buttonWarMachine.transform.position = positionButton;
        }
        OnChooseCreatureForSpell?.Invoke();
    }

    internal async UniTask CalculateStat()
    {
        inputManager.Disable();

        var creatures = heroLeft.Entity.Data.Creatures.Values.ToList();
        foreach (var creatureItem in creatures)
        {
            var key = heroLeft.Entity.Data.Creatures.FirstOrDefault(x => x.Value == creatureItem).Key;
            // empty slot.
            if (creatureItem == null) continue;

            // if creature is deaded.
            if (heroLeft.Data.ArenaCreatures[creatureItem].Death)
            {
                ArenaStat.AddDeadedCreature(heroLeft.Data.ArenaCreatures[creatureItem], heroLeft.Entity.Data.Creatures[key].Data.value);
                heroLeft.Entity.Data.Creatures[key] = null;
                continue;
            }

            var newValue = heroLeft.Data.ArenaCreatures[creatureItem].Data.quantity;
            if (heroLeft.Entity.Data.Creatures[key].Data.value - newValue > 0)
            {
                ArenaStat.AddDeadedCreature(heroLeft.Data.ArenaCreatures[creatureItem], heroLeft.Entity.Data.Creatures[key].Data.value - newValue);
            }
            heroLeft.Entity.Data.Creatures[key].Data.value = newValue;
        }

        if (heroRight.Entity != null)
        {
            var creaturesEnemy = heroRight.Entity.Data.Creatures.Values.ToList();
            foreach (var creatureItem in creaturesEnemy)
            {
                var key = heroRight.Entity.Data.Creatures.FirstOrDefault(x => x.Value == creatureItem).Key;
                // empty slot.
                if (creatureItem == null) continue;

                // if creature is deaded.
                if (heroRight.Data.ArenaCreatures[creatureItem].Death)
                {
                    ArenaStat.AddDeadedCreature(heroRight.Data.ArenaCreatures[creatureItem], heroRight.Entity.Data.Creatures[key].Data.value);
                    heroRight.Entity.Data.Creatures[key] = null;
                    continue;
                }

                var newValue = heroRight.Data.ArenaCreatures[creatureItem].Data.quantity;
                if (heroRight.Entity.Data.Creatures[key].Data.value - newValue > 0)
                {
                    ArenaStat.AddDeadedCreature(heroRight.Data.ArenaCreatures[creatureItem], heroRight.Entity.Data.Creatures[key].Data.value - newValue);
                }
                heroRight.Entity.Data.Creatures[key].Data.value = newValue;
            }
        }
        else if (DialogArenaData.creature != null)
        {
            var newValue = heroRight.Data.ArenaCreatures.Values.Select(t => t.Data.quantity).Sum();
            if (DialogArenaData.creature.Data.value - newValue > 0)
            {
                ArenaStat.AddDeadedCreature(heroRight.Data.ArenaCreatures.First().Value, DialogArenaData.creature.Data.value - newValue);
            }
            DialogArenaData.creature.Data.value = newValue;
        }
        else if (DialogArenaData.creaturesBank != null)
        {
            var creaturesBank = DialogArenaData.creaturesBank.Data.Defenders.Values.ToList();
            foreach (var creatureItem in creaturesBank)
            {
                var key = DialogArenaData.creaturesBank.Data.Defenders.FirstOrDefault(x => x.Value == creatureItem).Key;
                // empty slot.
                if (creatureItem == null) continue;

                // if creature is deaded.
                if (heroRight.Data.ArenaCreatures[creatureItem].Death)
                {
                    ArenaStat.AddDeadedCreature(heroRight.Data.ArenaCreatures[creatureItem], DialogArenaData.creaturesBank.Data.Defenders[key].Data.value);
                    DialogArenaData.creaturesBank.Data.Defenders[key].SetValueCreature(0);
                    continue;
                }

                var newValue = heroRight.Data.ArenaCreatures[creatureItem].Data.quantity;
                if (DialogArenaData.creaturesBank.Data.Defenders[key].Data.value - newValue > 0)
                {
                    ArenaStat.AddDeadedCreature(heroRight.Data.ArenaCreatures[creatureItem], DialogArenaData.creaturesBank.Data.Defenders[key].Data.value - newValue);
                }
                DialogArenaData.creaturesBank.Data.Defenders[key].Data.value = newValue;
            }
        }

        ArenaStat.isWinRightHero = ArenaQueue.ListEntities.Where(t => t.arenaEntity.TypeArenaPlayer == TypeArenaPlayer.Left).Count() == 0;
        ArenaStat.isWinLeftHero = ArenaQueue.ListEntities.Where(t => t.arenaEntity.TypeArenaPlayer == TypeArenaPlayer.Right).Count() == 0;
        OnShowState?.Invoke();
        await UniTask.Delay(1);
    }


    #region  Helpers
    public void LightingActiveNode()
    {
        _tileMapUnitActive.ClearAllTiles();

        if (LevelManager.Instance.ConfigGameSettings.paintActiveCreature)
        {
            Color color = LevelManager.Instance.ConfigGameSettings.colorActiveCreature;
            ArenaEntityBase activeArenaEntity = ArenaQueue.activeEntity.arenaEntity;

            if (activeArenaEntity.OccupiedNode != null)
            {
                GridArenaNode nodeActiveCreature = activeArenaEntity.OccupiedNode;
                GridArenaNode relatedNodeActiveCreature = activeArenaEntity.RelatedNode;

                _tileMapUnitActive.SetTile(nodeActiveCreature.position, _tileHexActive);
                _tileMapUnitActive.SetTileFlags(nodeActiveCreature.position, TileFlags.None);
                _tileMapUnitActive.SetColor(nodeActiveCreature.position, color);
                _tileMapUnitActive.SetTileFlags(nodeActiveCreature.position, TileFlags.LockColor);

                if (relatedNodeActiveCreature != null)
                {
                    _tileMapUnitActive.SetTile(relatedNodeActiveCreature.position, _tileHexActive);
                    _tileMapUnitActive.SetTileFlags(relatedNodeActiveCreature.position, TileFlags.None);
                    _tileMapUnitActive.SetColor(relatedNodeActiveCreature.position, color);
                    _tileMapUnitActive.SetTileFlags(relatedNodeActiveCreature.position, TileFlags.LockColor);
                }
            }
        }
    }

    public void SetColorAllowFightNode(GridArenaNode node, Color color)
    {
        // // _tileMapUnitActive.ClearAllTiles();
        // _tileMapUnitActive.SetTile(node.position, _tileHexShadow);

        if (LevelManager.Instance.ConfigGameSettings.paintAllowAttackCreature)
        {
            _tileMapAllowAttack.SetTile(node.position, _tileHexActive);
            _tileMapAllowAttack.SetTileFlags(node.position, TileFlags.None);
            _tileMapAllowAttack.SetColor(node.position, color);
            _tileMapAllowAttack.SetTileFlags(node.position, TileFlags.LockColor);
        }
        // Color color = Color.red;
        // color.a = .5f;
        // _tileMapUnitActive.SetTileFlags(node.position, TileFlags.None);
        // _tileMapUnitActive.SetColor(node.position, color);
        // _tileMapUnitActive.SetTileFlags(node.position, TileFlags.LockColor);
    }

    public void SetTextMeshNode(GridArenaNode tileNode, string textString = "")
    {
        Vector3 posText = new Vector3(tileNode.center.x, tileNode.center.y);
        GameObject text;
        if (!_listText.TryGetValue(posText, out text))
        {
            text = Instantiate(_textPrefab, posText, Quaternion.identity, _textCanvas.transform);
            _listText.Add(posText, text);
        }
        text.gameObject.transform.GetComponentInChildren<TextMeshProUGUI>().text
            = textString != "" ? textString : string.Format("{0}:{1}\r\n L{2} W{3}",
            tileNode.position.x,
            tileNode.position.y,
            tileNode.level,
            tileNode.weight
            );
        text.gameObject.SetActive(true);
    }

    public void ResetTextMeshNode()
    {
        foreach (var node in _listText)
        {
            node.Value.gameObject.SetActive(false);
        }
    }
    #endregion

    #region Create Arena
    public async void CreateArena(DialogArenaData data)
    {
        DialogArenaData = data;

        // Create grid and helper.
        _gridArenaHelper = new GridArenaHelper(width + 1, height + 2, this);

        for (int x = 0; x < width + 1; x++)
        {
            for (int y = 0; y < height + 2; y++)
            {
                var nodeObj = GridArenaHelper.GridTile.GetGridObject(new Vector3Int(x, y));
                var bounds = tileMapArenaUnits.GetBoundsLocal(nodeObj.position);
                nodeObj.SetCenter(bounds.center);

                if (y == 0 || y > height || x >= width)
                {
                    nodeObj.StateArenaNode |= StateArenaNode.Disable;
                }
            }
        }

        CreateGrid();

        await CreateHeroes();

        if (DialogArenaData.town != null)
        {
            await CreateTown();
        }

        // CreateSchemaCreatures();

        await CreateCreatures();

        if (DialogArenaData.creaturesBank == null)
        {
            await CreateWarMachine();

            CreateObstacles();
        }

        setSizeTileMap();

        await NextCreature(false, false);
    }

    public void CreateGrid()
    {
        var settingGame = LevelManager.Instance.ConfigGameSettings;
        if (settingGame.showGrid)
        {
            for (int x = 0; x < width + 1; x++)
            {
                for (int y = 0; y < height + 2; y++)
                {
                    var nodeObj = GridArenaHelper.GridTile.GetGridObject(new Vector3Int(x, y));

                    if (y == 0 || y > height || x >= width)
                    {
                    }
                    else
                    {
                        tileMapArenaGrid.SetTile(nodeObj.position, _tileHex);
                    }
                }
            }
        }
        else
        {
            tileMapArenaGrid.ClearAllTiles();
        }
    }

    private void setSizeTileMap()
    {
        BoxCollider2D colliderTileMap = tileMapArenaGrid.GetComponent<BoxCollider2D>();
        colliderTileMap.offset = new Vector2(7.25f, 4.6f);
        colliderTileMap.size = new Vector2(width + 0.5f, height - 3);
    }

    private SerializableDictionary<int, List<Vector2Int>> CreateSchemaCreatures(int x)
    {
        SerializableDictionary<int, List<Vector2Int>> schemaCreatures = new();
        if (DialogArenaData.creaturesBank != null)
        {
            schemaCreatures.Clear();
            schemaCreatures.Add(1, new List<Vector2Int>() {
                new Vector2Int(5, 8)
            });
            schemaCreatures.Add(2, new List<Vector2Int>() {
                new Vector2Int(5, 8),
                new Vector2Int(9, 8)
            });
            schemaCreatures.Add(3, new List<Vector2Int>() {
                new Vector2Int(5, 8),
                new Vector2Int(9, 8),
                new Vector2Int(4, 6)
            });
            schemaCreatures.Add(4, new List<Vector2Int>() {
                new Vector2Int(5, 8),
                new Vector2Int(9, 8),
                new Vector2Int(4, 6),
                new Vector2Int(7, 6)
            });
            schemaCreatures.Add(5, new List<Vector2Int>() {
                new Vector2Int(5, 8),
                new Vector2Int(9, 8),
                new Vector2Int(4, 6),
                new Vector2Int(7, 6),
                new Vector2Int(10, 6)
                });
            schemaCreatures.Add(6, new List<Vector2Int>() {
                new Vector2Int(5, 8),
                new Vector2Int(9, 8),
                new Vector2Int(4, 6),
                new Vector2Int(7, 6),
                new Vector2Int(10, 6),
                new Vector2Int(5, 4)
                });
            schemaCreatures.Add(7, new List<Vector2Int>() {
                new Vector2Int(5, 8),
                new Vector2Int(9, 8),
                new Vector2Int(4, 6),
                new Vector2Int(7, 6),
                new Vector2Int(10, 6),
                new Vector2Int(5, 4),
                new Vector2Int(9, 4)
                });
        }
        else
        {
            schemaCreatures.Clear();
            schemaCreatures.Add(1, new List<Vector2Int>() { new Vector2Int(x, 6) });
            schemaCreatures.Add(2, new List<Vector2Int>() { new Vector2Int(x, 9), new Vector2Int(x, 3) });
            schemaCreatures.Add(3, new List<Vector2Int>() { new Vector2Int(x, 9), new Vector2Int(x, 6), new Vector2Int(x, 3) });
            schemaCreatures.Add(4, new List<Vector2Int>() { new Vector2Int(x, 9), new Vector2Int(x, 7), new Vector2Int(x, 6), new Vector2Int(x, 3) });
            schemaCreatures.Add(5, new List<Vector2Int>() {
                new Vector2Int(x, 9),
                new Vector2Int(x, 7),
                new Vector2Int(x, 6),
                new Vector2Int(x, 5),
                new Vector2Int(x, 3)
                });
            schemaCreatures.Add(6, new List<Vector2Int>() {
                new Vector2Int(x, 11),
                new Vector2Int(x, 9),
                new Vector2Int(x, 7),
                new Vector2Int(x, 5),
                new Vector2Int(x, 3),
                new Vector2Int(x, 1)
                });
            schemaCreatures.Add(7, new List<Vector2Int>() {
                new Vector2Int(x, 11),
                new Vector2Int(x, 9),
                new Vector2Int(x, 7),
                new Vector2Int(x, 6),
                new Vector2Int(x, 5),
                new Vector2Int(x, 3),
                new Vector2Int(x, 1)
                });
        }
        return schemaCreatures;
    }

    private async UniTask CreateWarMachine()
    {
        if (
            heroLeft.Entity.Data.WarMachines.ContainsKey(TypeWarMachine.Catapult)
            && DialogArenaData.town != null
            && DialogArenaData.town.Data.level != -1
            )
        {
            var catapult = (EntityCreature)heroLeft.Entity.Data.WarMachines[TypeWarMachine.Catapult];
            var GridGameObject = new ArenaWarMachine();
            GridGameObject.Init(this, heroLeft);
            var nodeObj = GridArenaHelper.GridTile.GetGridObject(new Vector3Int(0, 4));
            GridGameObject.TypeArenaPlayer = TypeArenaPlayer.Left;
            GridGameObject.SetEntity(catapult, nodeObj);
            GridGameObject.SetPosition(nodeObj);
            GridGameObject.Data.typeAttack = TypeAttack.AttackCatapult;

            await GridGameObject.CreateMapGameObject(nodeObj);

            ArenaQueue.AddEntity(GridGameObject);
        }
        if (heroLeft.Entity.Data.WarMachines.ContainsKey(TypeWarMachine.Ballista))
        {
            var ballista = (EntityCreature)heroLeft.Entity.Data.WarMachines[TypeWarMachine.Ballista];
            var GridGameObject = new ArenaWarMachine();
            GridGameObject.Init(this, heroLeft);
            var nodeObj = GridArenaHelper.GridTile.GetGridObject(new Vector3Int(0, 8));
            GridGameObject.TypeArenaPlayer = TypeArenaPlayer.Left;
            GridGameObject.SetEntity(ballista, nodeObj);
            GridGameObject.SetPosition(nodeObj);
            GridGameObject.Data.typeAttack = TypeAttack.AttackShoot;

            await GridGameObject.CreateMapGameObject(nodeObj);

            ArenaQueue.AddEntity(GridGameObject);
        }
        if (heroLeft.Entity.Data.WarMachines.ContainsKey(TypeWarMachine.AmmoCart))
        {
            var ammoCart = (EntityCreature)heroLeft.Entity.Data.WarMachines[TypeWarMachine.AmmoCart];
            var GridGameObject = new ArenaWarMachine();
            GridGameObject.Init(this, heroLeft);
            var nodeObj = GridArenaHelper.GridTile.GetGridObject(new Vector3Int(0, 10));
            GridGameObject.TypeArenaPlayer = TypeArenaPlayer.Left;
            GridGameObject.SetEntity(ammoCart, nodeObj);
            GridGameObject.SetPosition(nodeObj);
            GridGameObject.Data.typeAttack = TypeAttack.Attack;

            await GridGameObject.CreateMapGameObject(nodeObj);

            ArenaQueue.AddEntity(GridGameObject);
        }
        if (heroLeft.Entity.Data.WarMachines.ContainsKey(TypeWarMachine.FirstAidTent))
        {
            var firstAidTent = (EntityCreature)heroLeft.Entity.Data.WarMachines[TypeWarMachine.FirstAidTent];
            var GridGameObject = new ArenaWarMachine();
            GridGameObject.Init(this, heroLeft);
            var nodeObj = GridArenaHelper.GridTile.GetGridObject(new Vector3Int(0, 2));
            GridGameObject.TypeArenaPlayer = TypeArenaPlayer.Left;
            GridGameObject.SetEntity(firstAidTent, nodeObj);
            GridGameObject.SetPosition(nodeObj);
            GridGameObject.Data.typeAttack = TypeAttack.Aid;

            await GridGameObject.CreateMapGameObject(nodeObj);

            ArenaQueue.AddEntity(GridGameObject);
        }
        if (heroRight != null && heroRight.Data.typeArenaHero != TypeArenaHero.Hidden)
        {
            if (heroRight.Entity.Data.WarMachines.ContainsKey(TypeWarMachine.AmmoCart))
            {
                var ammoCart = (EntityCreature)heroRight.Entity.Data.WarMachines[TypeWarMachine.AmmoCart];
                var GridGameObject = new ArenaWarMachine();
                GridGameObject.Init(this, heroRight);
                var nodeObj = GridArenaHelper.GridTile.GetGridObject(new Vector3Int(width - 1, 10));
                GridGameObject.TypeArenaPlayer = TypeArenaPlayer.Right;
                GridGameObject.SetEntity(ammoCart, nodeObj);
                GridGameObject.SetPosition(nodeObj);
                GridGameObject.Data.typeAttack = TypeAttack.Attack;

                await GridGameObject.CreateMapGameObject(nodeObj);

                ArenaQueue.AddEntity(GridGameObject);
            }
            if (heroRight.Entity.Data.WarMachines.ContainsKey(TypeWarMachine.Ballista))
            {
                var ballista = (EntityCreature)heroRight.Entity.Data.WarMachines[TypeWarMachine.Ballista];
                var GridGameObject = new ArenaWarMachine();
                GridGameObject.Init(this, heroRight);
                var nodeObj = GridArenaHelper.GridTile.GetGridObject(new Vector3Int(width - 1, 8));
                GridGameObject.TypeArenaPlayer = TypeArenaPlayer.Right;
                GridGameObject.SetEntity(ballista, nodeObj);
                GridGameObject.SetPosition(nodeObj);
                GridGameObject.Data.typeAttack = TypeAttack.AttackShoot;

                await GridGameObject.CreateMapGameObject(nodeObj);

                ArenaQueue.AddEntity(GridGameObject);
            }
            if (heroRight.Entity.Data.WarMachines.ContainsKey(TypeWarMachine.FirstAidTent))
            {
                var firstAidTent = (EntityCreature)heroRight.Entity.Data.WarMachines[TypeWarMachine.FirstAidTent];
                var GridGameObject = new ArenaWarMachine();
                GridGameObject.Init(this, heroRight);
                var nodeObj = GridArenaHelper.GridTile.GetGridObject(new Vector3Int(width - 1, 2));
                GridGameObject.TypeArenaPlayer = TypeArenaPlayer.Right;
                GridGameObject.SetEntity(firstAidTent, nodeObj);
                GridGameObject.SetPosition(nodeObj);
                GridGameObject.Data.typeAttack = TypeAttack.Aid;

                await GridGameObject.CreateMapGameObject(nodeObj);

                ArenaQueue.AddEntity(GridGameObject);
            }
        }
    }

    private void CreateObstacles()
    {
        if (maxCountObstacle == 0) return;

        // Create not removed obstacle.
        var spriteHeadObstacle = DialogArenaData.ArenaSetting.ObstaclesNotRemove
            .OrderBy(t => UnityEngine.Random.value)
            .First();
        var nodeForHeadObstacle = GridArenaHelper.GetNode(7, 6);
        // var worktextureHeadSprite = Helpers.DuplicateTexture(spriteHeadObstacle.texture);
        var tileHeadSprite = ScriptableObject.CreateInstance<Tile>();
        tileHeadSprite.sprite = spriteHeadObstacle;
        _tileMapObstacles.SetTile(nodeForHeadObstacle.position, tileHeadSprite);
        CreateObstacle(nodeForHeadObstacle, spriteHeadObstacle, false);

        // Create shallow obstacles.
        var potentialSprites = DialogArenaData.ArenaSetting.Obstacles
            .OrderBy(t => UnityEngine.Random.value)
            .ToList()
            .GetRange(0, maxCountObstacle);
        var potentialNodes = GridArenaHelper.GetAllGridNodes()
            .Where(t =>
                // t.Neighbours().Count == 6
                t.position.x > 3 && t.position.x < width - 3
                && t.position.y > 3 && t.position.y < height - 3
                && !t.StateArenaNode.HasFlag(StateArenaNode.Disable)
                && !t.StateArenaNode.HasFlag(StateArenaNode.Spellsed)
                && !t.StateArenaNode.HasFlag(StateArenaNode.Obstacles)
            )
            .OrderBy(t => UnityEngine.Random.value)
            .ToList();

        int counCreatedtObstacle = 0;
        while (counCreatedtObstacle < maxCountObstacle && potentialNodes.Count > 0)
        {
            int countCreate = CreateObstacle(potentialNodes[0], potentialSprites[UnityEngine.Random.Range(0, potentialSprites.Count)], true);
            counCreatedtObstacle += countCreate;
            potentialNodes.RemoveAt(0);
        }
    }

    private int CreateObstacle(GridArenaNode nodeForObstacle, Sprite spriteForObstacle, bool isDestructure)
    {
        int counCreatedtObstacle = 0;
        if (
            nodeForObstacle.Neighbours().Count == 6
            && !nodeForObstacle.StateArenaNode.HasFlag(StateArenaNode.Disable)
            && !nodeForObstacle.StateArenaNode.HasFlag(StateArenaNode.Spellsed)
            && !nodeForObstacle.StateArenaNode.HasFlag(StateArenaNode.Occupied)
        )
        {
            var worktexture = Helpers.DuplicateTexture(spriteForObstacle.texture);
            var tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = spriteForObstacle;
            // Calculate area sprite.
            var extents = spriteForObstacle.bounds.extents;
            var minX = nodeForObstacle.center.x - extents.x;
            var maxX = nodeForObstacle.center.x + extents.x;
            var minY = nodeForObstacle.center.y - extents.y;
            var maxY = nodeForObstacle.center.y + extents.y;

            int maxDistance = Mathf.RoundToInt(Math.Max(extents.x, extents.y));

            // Get neighbours by distance.
            var neighboursDistance = GridArenaHelper.GetNeighboursAtDistance(nodeForObstacle, maxDistance);
            if (neighboursDistance.Where(t =>
                t.StateArenaNode.HasFlag(StateArenaNode.Obstacles)
                || t.StateArenaNode.HasFlag(StateArenaNode.Occupied)
                || t.StateArenaNode.HasFlag(StateArenaNode.Disable)
                || t.StateArenaNode.HasFlag(StateArenaNode.Spellsed)
                )
                .Count() == 0
            )
            {
                _tileMapObstacles.SetTile(nodeForObstacle.position, tile);
                nodeForObstacle.StateArenaNode |= StateArenaNode.Disable;
                nodeForObstacle.StateArenaNode |= StateArenaNode.Obstacles;
                if (isDestructure)
                {
                    nodeForObstacle.StateArenaNode |= StateArenaNode.Spellsed;
                }
                counCreatedtObstacle++;
                foreach (var neiNode in neighboursDistance)
                {
                    var difX = (int)((extents.x + (neiNode.center.x - nodeForObstacle.center.x)) * spriteForObstacle.pixelsPerUnit);
                    var difY = (int)((extents.y + (neiNode.center.y - nodeForObstacle.center.y)) * spriteForObstacle.pixelsPerUnit);
                    // Debug.Log($"difX={difX}:difY={difY}||{worktexture.GetPixel(difX, difY).a != 0}||{neiNode.center}---{nodeForObstacle.center}");
                    var pixelCenter = worktexture.GetPixel(difX, difY);
                    if (
                        neiNode.center.x > minX
                        && neiNode.center.x < maxX
                        && neiNode.center.y < maxY
                        && neiNode.center.y > minY
                        && pixelCenter.a != 0f
                        && !neiNode.StateArenaNode.HasFlag(StateArenaNode.Obstacles)
                    )
                    {
                        if (isDestructure)
                        {
                            neiNode.StateArenaNode |= StateArenaNode.Spellsed;
                        }
                        neiNode.StateArenaNode |= StateArenaNode.Disable;
                        neiNode.StateArenaNode |= StateArenaNode.Obstacles;
                        // _tileMapObstacles.SetTile(neiNode.position, _tileHexShadow);
                        // _tileMapObstacles.SetTileFlags(neiNode.position, TileFlags.None);
                        // _tileMapObstacles.SetColor(neiNode.position, nodeForObstacle == neiNode ? Color.magenta : Color.yellow);
                        // _tileMapObstacles.SetTileFlags(neiNode.position, TileFlags.LockColor);
                    }
                }
            }
        }
        return counCreatedtObstacle;
    }

    private async UniTask CreateTown()
    {
        maxCountObstacle = 0;

        if (DialogArenaData.town.Data.level > -1)
        {
            ArenaTown = new ArenaEntityTown();
            var node = GridArenaHelper.GetNode(new Vector3Int(10, 5));
            ArenaTown.Init(node, DialogArenaData.town, this);
            await ArenaTown.CreateGameObject();
            await ArenaTown.SetShootTown();
        }
        else
        {
            ArenaTown = null;
        }

        // if (DialogArenaData.town.HeroInTown != null)
        // {
        //     // enemy = DialogArenaData.town.HeroInTown;
        //     enemy = new ArenaHeroEntity(tileMapArenaUnits);
        //     enemy.SetEntity(this, DialogArenaData.town.HeroInTown, TypeArenaHero.Visible, new Vector3(width + 1.5f, 8.5f));
        // }
    }

    private async UniTask CreateHeroes()
    {
        var positionRightHero = new Vector3(width + 1.5f, 8.5f);
        var positionLeftHero = new Vector3(-2, 8.5f);

        heroLeft = new ArenaHeroEntity(tileMapArenaUnits);
        await heroLeft.SetEntity(this, DialogArenaData.heroAttacking, TypeArenaHero.Visible, positionLeftHero);

        if (DialogArenaData.heroDefending != null)
        {
            heroRight = new ArenaHeroEntity(tileMapArenaUnits);
            await heroRight.SetEntity(this, DialogArenaData.heroDefending, TypeArenaHero.Visible, positionRightHero);
        }
        else if (
            DialogArenaData.town != null
            && DialogArenaData.town.Data.level > -1
            && DialogArenaData.town.HeroInTown != null
            )
        {
            heroRight = new ArenaHeroEntity(tileMapArenaUnits);
            await heroRight.SetEntity(this, DialogArenaData.town.HeroInTown, TypeArenaHero.Visible, positionRightHero);
        }
        else
        {
            heroRight = new ArenaHeroEntity(tileMapArenaUnits);
            await heroRight.SetEntity(this, null, TypeArenaHero.Hidden, positionRightHero);
        }

    }

    private async UniTask CreateCreatures()
    {
        var heroCreatures = heroLeft.Entity.Data.Creatures.Where(t => t.Value != null).ToList();
        var schemaCreaturesHero = CreateSchemaCreatures(1)[heroCreatures.Count];
        for (int i = 0; i < heroCreatures.Count; i++)
        {
            var creature = heroCreatures[i];
            var GridGameObject = new ArenaCreature();
            GridGameObject.Init(this, heroLeft);
            var size = ((EntityCreature)creature.Value).ConfigAttribute.CreatureParams.Size;
            var x = DialogArenaData.creaturesBank != null
                ? size > 1 ? schemaCreaturesHero[i].x + 1 : schemaCreaturesHero[i].x
                : size - schemaCreaturesHero[i].x;
            var nodeObj = GridArenaHelper.GridTile.GetGridObject(new Vector3Int(x, schemaCreaturesHero[i].y));
            GridGameObject.TypeArenaPlayer = TypeArenaPlayer.Left;
            GridGameObject.SetEntity(creature.Value, nodeObj);
            GridGameObject.SetPosition(nodeObj);

            await GridGameObject.CreateMapGameObject(nodeObj);

            ArenaQueue.AddEntity(GridGameObject);
        }

        if (heroRight.Data.typeArenaHero == TypeArenaHero.Visible)
        {
            var enemyCreatures = heroRight.Entity.Data.Creatures.Where(t => t.Value != null).ToList();
            var schemaEnemyCreatures = CreateSchemaCreatures(width)[enemyCreatures.Count];
            for (int i = 0; i < enemyCreatures.Count; i++)
            {
                var creature = enemyCreatures[i];
                var GridGameObject = new ArenaCreature();
                GridGameObject.Init(this, heroRight);
                var size = ((EntityCreature)creature.Value).ConfigAttribute.CreatureParams.Size;
                var nodeObj = GridArenaHelper.GridTile.GetGridObject(new Vector3Int(schemaEnemyCreatures[i].x - size, schemaEnemyCreatures[i].y));
                GridGameObject.TypeArenaPlayer = TypeArenaPlayer.Right;
                GridGameObject.SetEntity(creature.Value, nodeObj);
                GridGameObject.SetPosition(nodeObj);

                await GridGameObject.CreateMapGameObject(nodeObj);

                ArenaQueue.AddEntity(GridGameObject);
            }
        }
        else
        {
            if (DialogArenaData.creaturesBank != null)
            {
                var creatures = DialogArenaData.creaturesBank.Data.Defenders;
                var _schemaCreaturesBank = new List<Vector2Int>() {
                    new Vector2Int(15, 6),
                    new Vector2Int(1, 1),
                    new Vector2Int(1, 11),
                    new Vector2Int(15, 1),
                    new Vector2Int(15, 11)
                };
                for (int i = 0; i < creatures.Count; i++)
                {
                    var creatureEntity = creatures[i];
                    if (creatureEntity != null && creatureEntity.Data.value != 0)
                    {
                        var creatureConfig = creatureEntity.ConfigAttribute;
                        var GridGameObject = new ArenaCreature();
                        GridGameObject.Init(this, heroRight);
                        var size = creatureConfig.CreatureParams.Size;
                        var dif = _schemaCreaturesBank[i].x < 2 && size > 1 ? 1 : 0;
                        var position = new Vector3Int(Mathf.Abs(size - _schemaCreaturesBank[i].x - dif), _schemaCreaturesBank[i].y);
                        var nodeObj = GridArenaHelper.GridTile.GetGridObject(position);
                        GridGameObject.TypeArenaPlayer = TypeArenaPlayer.Right;
                        GridGameObject.SetEntity(creatureEntity, nodeObj);
                        GridGameObject.SetPosition(nodeObj);

                        await GridGameObject.CreateMapGameObject(nodeObj);

                        ArenaQueue.AddEntity(GridGameObject);
                    }
                }
            }
            else
            {
                var allAIHeroCreatures = heroCreatures.Select(t => t.Value.totalAI).Sum() * heroLeft.Entity.Streight;
                var allAICreatures = DialogArenaData.creature.totalAI;

                // Calculate count stack.
                int procentHp = allAIHeroCreatures / allAICreatures * 100;
                var maxCountStack = 1;
                switch (procentHp)
                {
                    case int n when (n >= 200):
                        maxCountStack = UnityEngine.Random.Range(1, 3);
                        break;
                    case int n when (n >= 150 && n < 200):
                        maxCountStack = UnityEngine.Random.Range(2, 4);
                        break;
                    case int n when (n >= 100 && n < 150):
                        maxCountStack = UnityEngine.Random.Range(3, 5);
                        break;
                    case int n when (n >= 67 && n < 100):
                        maxCountStack = UnityEngine.Random.Range(4, 6);
                        break;
                    case int n when (n >= 50 && n < 67):
                        maxCountStack = UnityEngine.Random.Range(5, 7);
                        break;
                    case int n when (n < 50):
                        maxCountStack = UnityEngine.Random.Range(6, 7);
                        break;
                }

                // Create stack creatures.
                // SerializableDictionary<int, EntityCreature> enemyCreaturesAll = new();
                if (DialogArenaData.creature.Data.value < maxCountStack)
                {
                    maxCountStack = DialogArenaData.creature.Data.value;
                }
                int countCreatureOneStack = DialogArenaData.creature.Data.value / maxCountStack;
                int remainder = DialogArenaData.creature.Data.value % maxCountStack;

                // Debug.Log($"Battles::: {allAIHeroCreatures}/{allAICreatures} [{procentHp}]({maxCountStack})");

                var _schemaCreatures = CreateSchemaCreatures(width)[maxCountStack];
                for (int i = 0; i < maxCountStack; i++)
                {
                    var creatureConfig = DialogArenaData.creature.ConfigAttribute;
                    var creatureEntity = new EntityCreature(creatureConfig);
                    creatureEntity.SetValueCreature(i < maxCountStack - 1 ? countCreatureOneStack : countCreatureOneStack + remainder);

                    var GridGameObject = new ArenaCreature();
                    GridGameObject.Init(this, heroRight);
                    var size = creatureConfig.CreatureParams.Size;
                    var nodeObj = GridArenaHelper.GridTile.GetGridObject(new Vector3Int(_schemaCreatures[i].x - size, _schemaCreatures[i].y));
                    GridGameObject.TypeArenaPlayer = TypeArenaPlayer.Right;
                    GridGameObject.SetEntity(creatureEntity, nodeObj);
                    GridGameObject.SetPosition(nodeObj);

                    await GridGameObject.CreateMapGameObject(nodeObj);

                    ArenaQueue.AddEntity(GridGameObject);
                }
            }

        }
        // SetColorDisableNode();
    }
    #endregion

    // public void ResetPathColor()
    // {
    //     _tileMapAllowAttack.ClearAllTiles();
    // }

    // public void SetColorPathNode(GridArenaNode node)
    // {
    //     ArenaEntity activeArenaEntity = ArenaQueue.activeEntity;

    //     _tileMapPathColor.SetTile(node.position, _tileHexActive);
    //     _tileMapPathColor.SetTileFlags(node.position, TileFlags.None);
    //     Color color = Color.blue;
    //     color.a = .6f;
    //     _tileMapPathColor.SetColor(node.position, color);
    //     _tileMapPathColor.SetTileFlags(node.position, TileFlags.LockColor);
    // }

    // public void SetColorDisableNode()
    // {
    //     List<GridArenaNode> disableNodes = GridArenaHelper.GetAllGridNodes()
    //     .Where(t => !t.StateArenaNode.HasFlag(StateArenaNode.Empty)).ToList();
    //     _tileMapDisableNode.ClearAllTiles();

    //     foreach (var node in disableNodes)
    //     {
    //         _tileMapDisableNode.SetTile(node.position, _tileHexActive);
    //         _tileMapDisableNode.SetTileFlags(node.position, TileFlags.None);
    //         Color color = Color.red;
    //         color.a = .6f;
    //         _tileMapDisableNode.SetColor(node.position, color);
    //         _tileMapDisableNode.SetTileFlags(node.position, TileFlags.LockColor);
    //     }
    // }

    // private void DrawText(string text)
    // {
    //     GameObject labelObject = Instantiate<GameObject>(_textPrefab, new Vector2(0, 0), Quaternion.identity, _textCanvas.transform);
    //     TextMeshProUGUI label = labelObject.gameObject.transform.GetComponentInChildren<TextMeshProUGUI>();

    //     // // label.rectTransform.SetParent(_textCanvas.transform, false);
    //     // // label.rectTransform.anchoredPosition =
    //     // //     new Vector2(0, 0);
    //     label.text = text;//x.ToString() + "\n" + z.ToString();
    // }
    // private void CreateWeight()
    // {
    //     // Get All empty nodes.
    //     // List<GridArenaNode> allEmptyNodes = _gridArenaHelper.GetAllGridNodes()
    //     //     .Where(t => !t.StateArenaNode.HasFlag(StateArenaNode.Disable) && !t.StateArenaNode.HasFlag(StateArenaNode.Occupied))
    //     //     .ToList();
    //     // foreach (var n in allEmptyNodes) {

    //     // }

    //     for (int y = height - 1; y >= 0; y--)
    //     {
    //         for (int x = width - 1; x >= 0; x--)
    //         {
    //             if (_gridArenaHelper.GetNode())
    //         }
    //     }
    // }

    // private async UniTask GetFightingNodesForShootTown()
    // {
    //     ClearAttackNode();
    //     _tileMapAllowAttack.ClearAllTiles();
    //     FightingOccupiedNodes.Clear();

    //     var arenaEntity = ArenaQueue.activeEntity.arenaEntity;

    //     var nodesForAttack = GridArenaHelper
    //         .GetAllGridNodes()
    //         .Where(t =>
    //             t.OccupiedUnit != null
    //             && t.OccupiedUnit.TypeArenaPlayer != ArenaQueue.activeEntity.arenaEntity.TypeArenaPlayer
    //         )
    //         .ToList(); ;

    //     foreach (var node in nodesForAttack)
    //     {
    //         if (!node.StateArenaNode.HasFlag(StateArenaNode.Related))
    //         {
    //             FightingOccupiedNodes.Add(node);
    //             SetColorAllowFightNode(node, LevelManager.Instance.ConfigGameSettings.colorAllowAttackCreature);
    //         }
    //     }

    //     // Check type run.
    //     int levelSSkill = 0;
    //     if (ArenaQueue.ActiveHero != null)
    //     {
    //         levelSSkill = ArenaQueue.ActiveHero.Data.SSkills.ContainsKey(TypeSecondarySkill.Artillery)
    //             ? ArenaQueue.ActiveHero.Data.SSkills[TypeSecondarySkill.Artillery].level + 1
    //             : 0;
    //     }
    //     ArenaTypeRunEffect typeRunEffect = ArenaTypeRunEffect.AutoChoose;
    //     switch (levelSSkill)
    //     {
    //         case 0:
    //             typeRunEffect = ArenaTypeRunEffect.AutoChoose;
    //             break;
    //         case 1:
    //         case 2:
    //         case 3:
    //             typeRunEffect = ArenaTypeRunEffect.Choosed;
    //             break;
    //     }

    //     switch (typeRunEffect)
    //     {
    //         case ArenaTypeRunEffect.AutoChoose:
    //             // TODO choose target algoritm.
    //             Debug.Log($"Auto run shoot town! Artillery={levelSSkill}");
    //             // await ArenaQueue.activeEntity.arenaEntity.GoAttack(
    //             //     ArenaQueue.activeEntity.arenaEntity.OccupiedNode,
    //             //     nodesForAttack[UnityEngine.Random.Range(0, nodesForAttack.Count)]
    //             // );
    //             break;
    //         case ArenaTypeRunEffect.Choosed:
    //             // TODO choose target algoritm.
    //             Debug.Log("Create button shoot town!");
    //             break;
    //     }

    //     await UniTask.Yield();
    // }

    // private async UniTask GetFightingNodesForWarMachine()
    // {
    //     ClearAttackNode();
    //     _tileMapAllowAttack.ClearAllTiles();
    //     FightingOccupiedNodes.Clear();

    //     var arenaEntity = ArenaQueue.activeEntity.arenaEntity;
    //     var warMachineConfig = ((ScriptableAttributeWarMachine)((EntityCreature)ArenaQueue.activeEntity.arenaEntity.Entity).ConfigAttribute);
    //     var resultChoosed = await warMachineConfig
    //             .ChooseTarget(this, ArenaQueue.ActiveHero);

    //     switch (resultChoosed.TypeRunEffect)
    //     {
    //         case ArenaTypeRunEffect.AutoChoose:
    //             if (warMachineConfig.TypeWarMachine != TypeWarMachine.Catapult)
    //             {
    //                 var choosedFirstNode = resultChoosed.ChoosedNodes
    //                     .OrderBy(t => -t.position.y)
    //                     .First();
    //                 await warMachineConfig.RunEffect(
    //                     this,
    //                     ArenaQueue.activeEntity.arenaEntity.OccupiedNode,
    //                     choosedFirstNode
    //                 );
    //             }
    //             else
    //             {
    //                 await warMachineConfig.RunEffectByGameObject(
    //                     this,
    //                     ArenaQueue.activeEntity.arenaEntity.OccupiedNode,
    //                     clickedFortification
    //                 );
    //             }
    //             EndRunWarMachine();
    //             break;
    //         case ArenaTypeRunEffect.Choosed:
    //             if (warMachineConfig.TypeWarMachine != TypeWarMachine.Catapult)
    //             {
    //                 foreach (var node in resultChoosed.ChoosedNodes)
    //                 {
    //                     if (!node.StateArenaNode.HasFlag(StateArenaNode.Related))
    //                     {
    //                         FightingOccupiedNodes.Add(node);
    //                         SetColorAllowFightNode(node, LevelManager.Instance.ConfigGameSettings.colorAllowAttackCreature);
    //                     }
    //                 }
    //             }
    //             else
    //             {
    //                 town.ArenaEntityTownMonobehavior.SetStatusColliders(true);
    //             }
    //             break;
    //         case ArenaTypeRunEffect.AutoAll:
    //             List<UniTask> listTasks = new();
    //             foreach (var nodeForAction in resultChoosed.ChoosedNodes)
    //             {
    //                 listTasks.Add(warMachineConfig.RunEffect(this, ArenaQueue.activeEntity.arenaEntity.OccupiedNode, nodeForAction));
    //             }
    //             await UniTask.WhenAll(listTasks);
    //             EndRunWarMachine();
    //             break;
    //     }
    //     // }
    //     // else if (warMachineConfig.TypeWarMachine == TypeWarMachine.Catapult)
    //     // {
    //     //     town.ArenaEntityTownMonobehavior.SetStatusColliders(true);
    //     //     await warMachineConfig.RunEffectByGameObject(
    //     //         this,
    //     //         ArenaQueue.activeEntity.arenaEntity.OccupiedNode,
    //     //         clickedFortification
    //     //     );
    //     // }
    // }

    // internal void CreateButtonWarMachine(GridArenaNode nodeForAttack)
    // {
    //     // clickedNode = null;
    //     ClearAttackNode();

    //     // AttackedCreature = clickedEntity;
    //     clickedNode = nodeForAttack;

    //     var positionButton = new Vector3(nodeForAttack.center.x, nodeForAttack.center.y, zCoord);
    //     var warMachine = ((ScriptableAttributeWarMachine)((EntityCreature)ArenaQueue.activeEntity.arenaEntity.Entity).ConfigAttribute);

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

    // public async UniTask ClickButton()
    // {
    //     Debug.Log("ClickButton");
    //     isRunningAction = true;

    //     await AudioManager.Instance.Click();
    //     if (activeCursor == CursorRule.NotAllow)
    //     {
    //         isRunningAction = false;
    //         return;
    //     };

    //     if (
    //         ((AttackedCreature != null && ArenaQueue.activeEntity.arenaEntity.Data.typeAttack == TypeAttack.Attack)
    //         || AttackedCreature == null)
    //         && ArenaQueue.activeEntity.arenaEntity.Data.speed > 0
    //         )
    //     {
    //         // Move creature.
    //         await ((ArenaCreature)ArenaQueue.activeEntity.arenaEntity).ArenaMonoBehavior.MoveCreature();
    //     }

    //     // Attack, if exist KeyNodeFromAttack
    //     if (AttackedCreature != null)
    //     {
    //         var nodes = NodesForAttackActiveCreature[KeyNodeFromAttack];
    //         if (nodes.nodeFromAttack.OccupiedUnit != null)
    //         {
    //             await nodes.nodeFromAttack.OccupiedUnit.GoAttack(nodes.nodeFromAttack, nodes.nodeToAttack);
    //         }
    //     }

    //     // Clear clicked node.
    //     clickedNode = null;
    //     ClearAttackNode();

    //     // Next creature.
    //     // await GoEntity();
    //     NextCreature(false, false);

    //     // DrawCursor
    //     if (clickedNode != null) DrawButtonAction();

    //     isRunningAction = false;
    // }


    // private async UniTask GetFightingNodes()
    // {
    //     switch (ArenaQueue.activeEntity.arenaEntity.Data.typeAttack)
    //     {
    //         case TypeAttack.AttackWarMachine:
    //             await ArenaQueue.activeEntity.arenaEntity.GetFightingNodes();
    //             break;
    //         case TypeAttack.AttackShootTown:
    //             await ArenaQueue.activeEntity.arenaEntity.GetFightingNodes();
    //             break;
    //         default:
    //             await GoEntity();
    //             break;
    //     }
    // }

    // public void CreateButtonAttackNode(ArenaEntityBase clickedEntity)
    // {
    //     clickedNode = null;
    //     if (AttackedCreature != clickedEntity && AttackedCreature != null)
    //     {
    //         ClearAttackNode();
    //     }
    //     // else
    //     // {
    //     AttackedCreature = clickedEntity;
    //     // }

    //     var allowNodes = AllowPathNodes.Concat(AllowMovedNodes).ToList();

    //     var neighbourNodesEnemyEntity = ArenaQueue.activeEntity.arenaEntity.OccupiedNode
    //         .Neighbours()
    //         // GridArenaHelper
    //         // .GetNeighbourList(ArenaQueue.activeEntity.arenaEntity.OccupiedNode)
    //         .Where(t => t.OccupiedUnit != null && t.OccupiedUnit.TypeArenaPlayer != ArenaQueue.activeEntity.arenaEntity.TypeArenaPlayer);
    //     // var activeSpell = ArenaQueue.ActiveHero.SpellBook != null
    //     //     ? ArenaQueue.ActiveHero.SpellBook.ChoosedSpell
    //     //     : null;
    //     if (
    //         (ArenaQueue.activeEntity.arenaEntity.Data.shoots == 0
    //         || neighbourNodesEnemyEntity.Count() > 0)
    //         && ChoosedSpell == null
    //         )
    //     {
    //         List<GridArenaNode> neighbours = clickedEntity.OccupiedNode
    //             .Neighbours()
    //             // GridArenaHelper.GetNeighbourList(clickedEntity.OccupiedNode)
    //             .Where(t => t.OccupiedUnit == null || t.OccupiedUnit == ArenaQueue.activeEntity.arenaEntity)
    //             .ToList();
    //         var allowNeighbours = neighbours.Intersect(allowNodes).ToList();
    //         foreach (var node in allowNeighbours)
    //         {
    //             NodesForAttackActiveCreature.Add(new AttackItemNode()
    //             {
    //                 nodeFromAttack = node,
    //                 nodeToAttack = clickedEntity.OccupiedNode
    //             });
    //         }

    //         if (clickedEntity.RelatedNode != null)
    //         {
    //             List<GridArenaNode> neighboursRelatedNode
    //                 = clickedEntity.RelatedNode.Neighbours()
    //                 // GridArenaHelper.GetNeighbourList(clickedEntity.RelatedNode)
    //                 .Where(t => t.OccupiedUnit == null || t.OccupiedUnit == ArenaQueue.activeEntity.arenaEntity)
    //                 .ToList();
    //             // neighbours = neighbours.Concat(neighboursRelatedNode).ToList();
    //             var allowNeighboursRelatedNode = neighboursRelatedNode.Intersect(allowNodes).ToList();
    //             foreach (var node in allowNeighboursRelatedNode)
    //             {
    //                 NodesForAttackActiveCreature.Add(new AttackItemNode()
    //                 {
    //                     nodeFromAttack = node,
    //                     nodeToAttack = clickedEntity.RelatedNode
    //                 });
    //             }
    //         }
    //     }
    //     else
    //     {
    //         NodesForAttackActiveCreature.Add(new AttackItemNode()
    //         {
    //             nodeFromAttack = ArenaQueue.activeEntity.arenaEntity.OccupiedNode,
    //             nodeToAttack = clickedEntity.OccupiedNode
    //         });
    //     }

    //     // NodesForAttackActiveCreature = neighbours.Intersect(allowNodes).ToList();
    //     _tileMapAllowAttackNode.ClearAllTiles();
    //     if (LevelManager.Instance.ConfigGameSettings.paintAllowAttackNode)
    //     {
    //         foreach (var node in NodesForAttackActiveCreature)
    //         {
    //             _tileMapAllowAttackNode.SetTile(node.nodeFromAttack.position, _tileHexActive);
    //             _tileMapAllowAttackNode.SetTileFlags(node.nodeFromAttack.position, TileFlags.None);
    //             _tileMapAllowAttackNode.SetColor(node.nodeFromAttack.position, LevelManager.Instance.ConfigGameSettings.colorAllowAttackNode);
    //             _tileMapAllowAttackNode.SetTileFlags(node.nodeFromAttack.position, TileFlags.LockColor);
    //         }
    //     }

    //     // GridArenaNode randomFirstNode
    //     //     = NodesForAttackActiveCreature[UnityEngine.Random.Range(0, NodesForAttackActiveCreature.Count)];
    //     // clickedNode = NodesForAttackActiveCreature[1];
    //     ChooseNextPositionForAttack();
    //     OnChangeNodesForAttack?.Invoke();
    // }
}
