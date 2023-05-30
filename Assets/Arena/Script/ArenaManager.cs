using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Cysharp.Threading.Tasks;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
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

public class ArenaManager : MonoBehaviour
{
    public UIArena uiArena;
    public CursorArena CursorRule;
    public RuleTile activeCursor;
    // [SerializeField] private Tilemap _tileMapCursor;
    // public static event Action OnSetNextCreature;
    public static event Action OnChangeNodesForAttack;
    // public static event Action OnEndBattle;
    // public static event Action OnRunFromBattle;
    public static event Action OnAutoNextCreature;
    public static event Action OnHideSpellInfo;
    public static event Action OnChooseCreatureForSpell;
    public static event Action OnShowState;
    [SerializeField] private int width = 15;
    [SerializeField] private int height = 11;
    [SerializeField] private Tilemap _tileMapArenaGrid;
    [SerializeField] public Tilemap tileMapArenaUnits;
    [SerializeField] public Tilemap _tileMapDistance;
    [SerializeField] private Tilemap _tileMapShadow;
    [SerializeField] private Tilemap _tileMapUnitActive;
    [SerializeField] private Tilemap _tileMapAllowAttackNode;
    [SerializeField] private Tilemap _tileMapAllowAttack;
    [SerializeField] private Tilemap _tileMapPath;

    [SerializeField] public GameObject _textPrefab;
    [SerializeField] public GameObject _textCanvas;
    private Dictionary<Vector3, GameObject> _listText = new Dictionary<Vector3, GameObject>();

    private GridArenaHelper _gridArenaHelper;
    public GridArenaHelper GridArenaHelper => _gridArenaHelper;
    [SerializeField] public Tile _tileHex;
    [SerializeField] public Tile _tileHexShadow;
    [SerializeField] public Tile _tileHexActive;
    private int zCoord = -14;
    private SerializableDictionary<int, List<int>> schemaCreatures = new();

    private EntityHero hero;
    private EntityHero enemy;
    public ArenaEntityTown ArenaTown = null;
    public GameObject clickedFortification;
    public GameObject buttonAction;
    public GameObject _buttonAction;
    public GameObject buttonSpell;
    public GameObject _buttonSpell;
    public GameObject buttonWarMachine;
    public GameObject _buttonWarMachine;
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
    [SerializeField] private Camera _camera;
    public bool isRunningAction;
    public DialogArenaData DialogArenaData;
    public ScriptableAttributeSpell ChoosedSpell
        => ArenaQueue.ActiveHero != null && ArenaQueue.ActiveHero.SpellBook != null ?
            ArenaQueue.ActiveHero.SpellBook.ChoosedSpell
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
        UIArena.OnNextCreature += NextCreature;
        UIArena.OnOpenSpellBook += OpenSpellBook;
        UIArena.OnClickAttack += ActionClickButton;
        UIDialogSpellBook.OnClickSpell += ChooseSpell;
        UIArena.OnCancelSpell += EndSpell;

        // CreateArena();

    }
    private void OnDestroy()
    {
        UIArena.OnNextCreature -= NextCreature;
        UIArena.OnOpenSpellBook -= OpenSpellBook;
        UIArena.OnClickAttack -= ActionClickButton;
        UIDialogSpellBook.OnClickSpell -= ChooseSpell;
        UIArena.OnCancelSpell -= EndSpell;
    }

    private async void OpenSpellBook()
    {
        inputManager.Disable();

        var dialogWindow = new DialogSpellBookOperation(ArenaQueue.ActiveHero);
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

        foreach (var neiNode in nodes)
        {
            if (neiNode.weight >= activeEntityCreature.CreatureParams.Size)
            {
                FillShadowNode(_tileMapShadow, neiNode);
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
                    FillShadowNode(_tileMapShadow, neiNode);
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

    public async void NextCreature(bool wait, bool def)
    {
        // Check end battle.
        var countCreaturesLeft = ArenaQueue.ListEntities.Where(t => t.arenaEntity.TypeArenaPlayer == TypeArenaPlayer.Left).Count();
        var countCreaturesRight = ArenaQueue.ListEntities.Where(t => t.arenaEntity.TypeArenaPlayer == TypeArenaPlayer.Right).Count();
        if (countCreaturesLeft == 0 || countCreaturesRight == 0)
        {
            await CalculateStat();
            return;
        }

        ArenaQueue.NextCreature(wait, def);

        await ArenaQueue.activeEntity.arenaEntity.GetFightingNodes();

        // lighting active creature.
        LightingActiveNode();

        OnAutoNextCreature?.Invoke();

        if (!ArenaQueue.activeEntity.arenaEntity.Data.isRun)
        {
            NextCreature(false, false);
        }
    }

    private void FillShadowNode(Tilemap tilemap, GridArenaNode node)
    {
        var settingGame = LevelManager.Instance.ConfigGameSettings;
        if (settingGame.showShadowGrid)
        {
            tilemap.SetTile(node.position, _tileHexShadow);
            tilemap.SetTileFlags(node.position, TileFlags.None);
            tilemap.SetColor(node.position, settingGame.colorShadow);
            tilemap.SetTileFlags(node.position, TileFlags.LockColor);
        }
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
            ArenaQueue.ActiveHero.SpellBook.ChooseSpell(null);
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
            Vector3Int tilePos = _tileMapArenaGrid.WorldToCell(posMouse);

            GridArenaNode node = GridArenaHelper.GridTile.GetGridObject(tilePos);
            Debug.Log($"Click node::: {node}");

            if (rayHit.collider.gameObject == _tileMapArenaGrid.gameObject)
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
        int nextIndex = KeyNodeFromAttack + 1;
        int indexNextAttackNode = nextIndex >= NodesForAttackActiveCreature.Count()
            ? 0
            : nextIndex;
        KeyNodeFromAttack = indexNextAttackNode;
        Debug.Log($"{NodesForAttackActiveCreature.Count()}/{indexNextAttackNode}");

        if (ArenaQueue.activeEntity.arenaEntity.Data.typeAttack == TypeAttack.Attack)
        {
            await DrawPath(NodesForAttackActiveCreature[indexNextAttackNode].nodeFromAttack);
        }
        if (clickedNode != null) DrawButtonAction();
    }

    private async void ChooseSpell()
    {
        //ClearAttackNode();
        //_tileMapAllowAttack.ClearAllTiles();
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
                ArenaQueue.ActiveHero.Data.SSkills.ContainsKey(baseSSkill.TypeTwoSkill)
                && ChoosedSpell.typeSpellRun == TypeSpellRun.Collective
                )
            {
                var dataSSkill = ArenaQueue.ActiveHero.Data.SSkills[baseSSkill.TypeTwoSkill];
                if (dataSSkill.level >= 2)
                {
                    List<UniTask> listTasks = new();
                    for (int i = 0; i < FightingOccupiedNodes.Count; i++)
                    {
                        // listTasks.Add(FightingOccupiedNodes.ElementAt(i).OccupiedUnit.RunSpell());
                        listTasks.Add(ArenaQueue.ActiveHero.SpellBook.RunSpellCombat(FightingOccupiedNodes.ElementAt(i), this));
                    }
                    await ArenaQueue.ActiveHero.ArenaHeroEntity.ArenaHeroMonoBehavior.RunSpellAnimation();
                    await UniTask.WhenAll(listTasks);
                    ArenaQueue.Refresh();
                    EndSpell();
                }
            }
        }
        // if immediately run spell.
        if (ChoosedSpell != null && ChoosedSpell.typeSpellRun == TypeSpellRun.Immediately)
        {
            await ArenaQueue.ActiveHero.ArenaHeroEntity.ArenaHeroMonoBehavior.RunSpellAnimation();
            await ArenaQueue.ActiveHero.SpellBook
                .RunSpellCombat(FightingOccupiedNodes[UnityEngine.Random.Range(0, FightingOccupiedNodes.Count - 1)], this);
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
        await ArenaQueue.ActiveHero.ArenaHeroEntity.ArenaHeroMonoBehavior.RunSpellAnimation();
        await ArenaQueue.ActiveHero.SpellBook.RunSpellCombat(clickedNode, this);
        _buttonSpell.SetActive(false);

        ArenaQueue.Refresh();
        EndSpell();
        isRunningAction = false;
    }

    private async void EndSpell()
    {
        _tileMapAllowAttack.ClearAllTiles();
        _buttonSpell.SetActive(false);
        ArenaQueue.ActiveHero.SpellBook.ChooseSpell(null);
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
        var settingGame = LevelManager.Instance.ConfigGameSettings;
        _gridArenaHelper = new GridArenaHelper(width + 1, height + 2, this);

        for (int x = 0; x < width + 1; x++)
        {
            for (int y = 0; y < height + 2; y++)
            {
                // Vector3Int pos = new Vector3Int(x - (x % 2 == 0 ? 0 : 1), y, 0);
                var nodeObj = GridArenaHelper.GridTile.GetGridObject(new Vector3Int(x, y));
                var bounds = tileMapArenaUnits.GetBoundsLocal(nodeObj.position);
                nodeObj.SetCenter(bounds.center);

                if (y == 0 || y > height || x >= width)
                {
                    nodeObj.StateArenaNode |= StateArenaNode.Disable;
                }
                else
                {
                    if (settingGame.showGrid)
                    {
                        _tileMapArenaGrid.SetTile(nodeObj.position, _tileHex);
                    }
                }
                // SetTextMeshNode(nodeObj);
            }
        }

        if (ResourceSystem.Instance != null)
        {
            if (DialogArenaData.town != null)
            {
                await CreateTown();
            }

            CreateHeroes();

            CreateSchemaCreatures();

            await CreateCreatures();

            CreateSchema();
            // CreateObstacles();
            await CreateWarMachine();

            NextCreature(false, false);

            setSizeTileMap();
        }
    }
    private void setSizeTileMap()
    {
        BoxCollider2D colliderTileMap = _tileMapArenaGrid.GetComponent<BoxCollider2D>();
        colliderTileMap.offset = new Vector2(7.25f, 4.6f);
        colliderTileMap.size = new Vector2(width + 0.5f, height - 3);
    }

    private void CreateSchemaCreatures()
    {
        schemaCreatures.Clear();
        schemaCreatures.Add(1, new List<int>() { 6 });
        schemaCreatures.Add(2, new List<int>() { 9, 3 });
        schemaCreatures.Add(3, new List<int>() { 9, 6, 3 });
        schemaCreatures.Add(4, new List<int>() { 9, 7, 6, 3 });
        schemaCreatures.Add(5, new List<int>() { 9, 7, 6, 5, 3 });
        schemaCreatures.Add(6, new List<int>() { 11, 9, 7, 5, 3, 1 });
        schemaCreatures.Add(7, new List<int>() { 11, 9, 7, 6, 5, 3, 1 });
    }

    private void CreateSchema()
    {
        if (DialogArenaData.town != null)
        {
        }
        else if (DialogArenaData.creatureBank != null)
        {

        }
    }

    private async UniTask CreateWarMachine()
    {
        if (
            hero.Data.WarMachines.ContainsKey(TypeWarMachine.Catapult)
            && DialogArenaData.town != null
            && DialogArenaData.town.Data.level != -1
            )
        {
            var catapult = (EntityCreature)hero.Data.WarMachines[TypeWarMachine.Catapult];
            var GridGameObject = new ArenaWarMachine();
            GridGameObject.Init(this, hero);
            var nodeObj = GridArenaHelper.GridTile.GetGridObject(new Vector3Int(0, 4));
            GridGameObject.TypeArenaPlayer = TypeArenaPlayer.Left;
            GridGameObject.SetEntity(catapult, nodeObj);
            GridGameObject.SetPosition(nodeObj);
            GridGameObject.Data.typeAttack = TypeAttack.AttackCatapult;

            await GridGameObject.CreateMapGameObject(nodeObj);

            ArenaQueue.AddEntity(GridGameObject);
        }
        if (hero.Data.WarMachines.ContainsKey(TypeWarMachine.Ballista))
        {
            var ballista = (EntityCreature)hero.Data.WarMachines[TypeWarMachine.Ballista];
            var GridGameObject = new ArenaWarMachine();
            GridGameObject.Init(this, hero);
            var nodeObj = GridArenaHelper.GridTile.GetGridObject(new Vector3Int(0, 8));
            GridGameObject.TypeArenaPlayer = TypeArenaPlayer.Left;
            GridGameObject.SetEntity(ballista, nodeObj);
            GridGameObject.SetPosition(nodeObj);
            GridGameObject.Data.typeAttack = TypeAttack.AttackShoot;

            await GridGameObject.CreateMapGameObject(nodeObj);

            ArenaQueue.AddEntity(GridGameObject);
        }
        if (enemy.Data.WarMachines.ContainsKey(TypeWarMachine.Ballista))
        {
            var ballista = (EntityCreature)enemy.Data.WarMachines[TypeWarMachine.Ballista];
            var GridGameObject = new ArenaWarMachine();
            GridGameObject.Init(this, enemy);
            var nodeObj = GridArenaHelper.GridTile.GetGridObject(new Vector3Int(width - 1, 8));
            GridGameObject.TypeArenaPlayer = TypeArenaPlayer.Right;
            GridGameObject.SetEntity(ballista, nodeObj);
            GridGameObject.SetPosition(nodeObj);
            GridGameObject.Data.typeAttack = TypeAttack.AttackShoot;

            await GridGameObject.CreateMapGameObject(nodeObj);

            ArenaQueue.AddEntity(GridGameObject);
        }
        if (hero.Data.WarMachines.ContainsKey(TypeWarMachine.AmmoCart))
        {
            var ammoCart = (EntityCreature)hero.Data.WarMachines[TypeWarMachine.AmmoCart];
            var GridGameObject = new ArenaWarMachine();
            GridGameObject.Init(this, hero);
            var nodeObj = GridArenaHelper.GridTile.GetGridObject(new Vector3Int(0, 10));
            GridGameObject.TypeArenaPlayer = TypeArenaPlayer.Left;
            GridGameObject.SetEntity(ammoCart, nodeObj);
            GridGameObject.SetPosition(nodeObj);
            GridGameObject.Data.typeAttack = TypeAttack.Attack;

            await GridGameObject.CreateMapGameObject(nodeObj);

            ArenaQueue.AddEntity(GridGameObject);
        }
        if (enemy.Data.WarMachines.ContainsKey(TypeWarMachine.AmmoCart))
        {
            var ammoCart = (EntityCreature)enemy.Data.WarMachines[TypeWarMachine.AmmoCart];
            var GridGameObject = new ArenaWarMachine();
            GridGameObject.Init(this, enemy);
            var nodeObj = GridArenaHelper.GridTile.GetGridObject(new Vector3Int(width - 1, 10));
            GridGameObject.TypeArenaPlayer = TypeArenaPlayer.Right;
            GridGameObject.SetEntity(ammoCart, nodeObj);
            GridGameObject.SetPosition(nodeObj);
            GridGameObject.Data.typeAttack = TypeAttack.Attack;

            await GridGameObject.CreateMapGameObject(nodeObj);

            ArenaQueue.AddEntity(GridGameObject);
        }
        if (hero.Data.WarMachines.ContainsKey(TypeWarMachine.FirstAidTent))
        {
            var firstAidTent = (EntityCreature)hero.Data.WarMachines[TypeWarMachine.FirstAidTent];
            var GridGameObject = new ArenaWarMachine();
            GridGameObject.Init(this, hero);
            var nodeObj = GridArenaHelper.GridTile.GetGridObject(new Vector3Int(0, 2));
            GridGameObject.TypeArenaPlayer = TypeArenaPlayer.Left;
            GridGameObject.SetEntity(firstAidTent, nodeObj);
            GridGameObject.SetPosition(nodeObj);
            GridGameObject.Data.typeAttack = TypeAttack.Aid;

            await GridGameObject.CreateMapGameObject(nodeObj);

            ArenaQueue.AddEntity(GridGameObject);
        }
        if (enemy.Data.WarMachines.ContainsKey(TypeWarMachine.FirstAidTent))
        {
            var firstAidTent = (EntityCreature)enemy.Data.WarMachines[TypeWarMachine.FirstAidTent];
            var GridGameObject = new ArenaWarMachine();
            GridGameObject.Init(this, enemy);
            var nodeObj = GridArenaHelper.GridTile.GetGridObject(new Vector3Int(width - 1, 2));
            GridGameObject.TypeArenaPlayer = TypeArenaPlayer.Right;
            GridGameObject.SetEntity(firstAidTent, nodeObj);
            GridGameObject.SetPosition(nodeObj);
            GridGameObject.Data.typeAttack = TypeAttack.Aid;

            await GridGameObject.CreateMapGameObject(nodeObj);

            ArenaQueue.AddEntity(GridGameObject);
        }
    }

    private void CreateObstacles()
    {
        var potentialNodes = GridArenaHelper.GetAllGridNodes()
            .Where(t =>
                // t.Neighbours().Count == 6
                t.position.x > 2 && t.position.x < width - 2
                && t.position.y > 2 && t.position.y < height - 2
            )
            .OrderBy(t => UnityEngine.Random.value)
            .ToList();
        int counCreatedtObstacle = 0;
        int maxCountObstacle = 3;
        while (counCreatedtObstacle < maxCountObstacle)
        {

            var nodeForObstacle = potentialNodes[0];
            if (nodeForObstacle.Neighbours().Count == 6)
            {
                nodeForObstacle.StateArenaNode |= StateArenaNode.Spellsed;
                counCreatedtObstacle++;
            }
        }

    }

    private async UniTask CreateTown()
    {
        if (DialogArenaData.town.Data.level > -1)
        {
            var townEntity = DialogArenaData.town;
            var node = GridArenaHelper.GetNode(new Vector3Int(10, 5));
            ArenaTown = new ArenaEntityTown();
            ArenaTown.Init(node, townEntity, this);
            await ArenaTown.CreateGameObject();
            await ArenaTown.SetShootTown();
        }
        else
        {
            ArenaTown = null;
        }

        if (DialogArenaData.town.HeroInTown != null)
        {
            enemy = DialogArenaData.town.HeroInTown;
            var enemyArena = new ArenaHeroEntity(tileMapArenaUnits);
            enemyArena.SetEntity(enemy);
            enemyArena.SetPosition(new Vector3(width + 1.5f, 8.5f));
            enemyArena.CreateMapGameObject();
            enemy.ArenaHeroEntity = enemyArena;
        }
    }

    private void CreateHeroes()
    {
        // TODO - use only from DialogArenaData
        hero = LevelManager.Instance.ActivePlayer != null
            ? LevelManager.Instance.ActivePlayer.ActiveHero
            : DialogArenaData.hero; // new EntityHero(TypeFaction.Castle, heroes[0]);
        var heroArena = new ArenaHeroEntity(tileMapArenaUnits);
        heroArena.SetEntity(hero);
        heroArena.SetPosition(new Vector3(-2, 8.5f));
        heroArena.CreateMapGameObject();
        hero.ArenaHeroEntity = heroArena;

        if (DialogArenaData.enemy != null)
        {
            enemy = DialogArenaData.enemy;
            var enemyArena = new ArenaHeroEntity(tileMapArenaUnits);
            enemyArena.SetEntity(enemy);
            enemyArena.SetPosition(new Vector3(width + 1.5f, 8.5f));
            enemyArena.CreateMapGameObject();
            enemy.ArenaHeroEntity = enemyArena;
        }
    }

    private async UniTask CreateCreatures()
    {
        var heroCreatures = hero.Data.Creatures.Where(t => t.Value != null).ToList();
        var schemaCreaturesHero = schemaCreatures[heroCreatures.Count];
        for (int i = 0; i < heroCreatures.Count; i++)
        {
            var creature = heroCreatures[i];
            var GridGameObject = new ArenaCreature();
            GridGameObject.Init(this, hero);
            var size = ((EntityCreature)creature.Value).ConfigAttribute.CreatureParams.Size;
            var nodeObj = GridArenaHelper.GridTile.GetGridObject(new Vector3Int(size - 1, schemaCreaturesHero[i]));
            GridGameObject.TypeArenaPlayer = TypeArenaPlayer.Left;
            GridGameObject.SetEntity(creature.Value, nodeObj);
            GridGameObject.SetPosition(nodeObj);

            await GridGameObject.CreateMapGameObject(nodeObj);

            ArenaQueue.AddEntity(GridGameObject);
        }

        if (enemy != null)
        {
            var enemyCreatures = enemy.Data.Creatures.Where(t => t.Value != null).ToList();
            var schemaEnemyCreatures = schemaCreatures[enemyCreatures.Count];
            for (int i = 0; i < enemyCreatures.Count; i++)
            {
                var creature = enemyCreatures[i];
                var GridGameObject = new ArenaCreature();
                GridGameObject.Init(this, enemy);
                var size = ((EntityCreature)creature.Value).ConfigAttribute.CreatureParams.Size;
                var nodeObj = GridArenaHelper.GridTile.GetGridObject(new Vector3Int(width - size, schemaEnemyCreatures[i]));
                GridGameObject.TypeArenaPlayer = TypeArenaPlayer.Right;
                GridGameObject.SetEntity(creature.Value, nodeObj);
                GridGameObject.SetPosition(nodeObj);

                await GridGameObject.CreateMapGameObject(nodeObj);

                ArenaQueue.AddEntity(GridGameObject);
            }
        }
        else
        {
            var allAIHeroCreatures = heroCreatures.Select(t => t.Value.totalAI).Sum() * hero.Streight;
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

            var _schemaCreatures = schemaCreatures[maxCountStack];
            for (int i = 0; i < maxCountStack; i++)
            {
                var creatureConfig = DialogArenaData.creature.ConfigAttribute;
                var creatureEntity = new EntityCreature(creatureConfig);
                creatureEntity.SetValueCreature(i < maxCountStack - 1 ? countCreatureOneStack : countCreatureOneStack + remainder);

                var GridGameObject = new ArenaCreature();
                GridGameObject.Init(this, null);
                var size = creatureConfig.CreatureParams.Size;
                var nodeObj = GridArenaHelper.GridTile.GetGridObject(new Vector3Int(width - size, _schemaCreatures[i]));
                GridGameObject.TypeArenaPlayer = TypeArenaPlayer.Right;
                GridGameObject.SetEntity(creatureEntity, nodeObj);
                GridGameObject.SetPosition(nodeObj);

                await GridGameObject.CreateMapGameObject(nodeObj);

                ArenaQueue.AddEntity(GridGameObject);
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
