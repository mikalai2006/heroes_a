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
    public static event Action OnAutoNextCreature;
    [SerializeField] private int width = 15;
    [SerializeField] private int height = 11;
    [SerializeField] private Tilemap _tileMapArenaGrid;
    [SerializeField] public Tilemap tileMapArenaUnits;
    [SerializeField] public Tilemap _tileMapDistance;
    [SerializeField] private Tilemap _tileMapShadow;
    [SerializeField] private Tilemap _tileMapUnitActive;
    [SerializeField] private Tilemap _tileMapDisableNode;
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

    private EntityHero hero;
    // private GameObject heroGameObject;
    private EntityHero enemy;
    public GameObject buttonAction;
    public GameObject _buttonAction;
    public GameObject buttonSpell;
    public GameObject _buttonSpell;
    private GridArenaNode clickedNode;
    // [NonSerialized] public List<ArenaEntity> ArenaEnteties;
    [NonSerialized] public List<GridArenaNode> DistanceNodes = new();
    [NonSerialized] public List<GridArenaNode> PathNodes = new();
    [NonSerialized] public List<GridArenaNode> FightingOccupiedNodes = new();
    [NonSerialized] public List<GridArenaNode> AllMovedNodes = new();
    [NonSerialized] public List<GridArenaNode> AllowMovedNodes = new();
    private List<GridArenaNode> AllowPathNodes => PathNodes.Concat(AllowMovedNodes).ToList();
    // 1 - node attack, 2 - occupied,related node
    public List<AttackItemNode> NodesForAttackActiveCreature = new();
    private int KeyNodeFromAttack = -1;
    [NonSerialized] public ArenaEntity AttackedCreature;
    [SerializeField] private List<ScriptableEntityHero> heroes;
    public ArenaQueue ArenaQueue = new ArenaQueue();
    [SerializeField] private Camera _camera;
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
        // inputManager.OnStartTouch += OnClick;
        UIArena.OnNextCreature += NextCreature;
        UIArena.OnOpenSpellBook += OpenSpellBook;
        UIArena.OnClickAttack += ActionClickButton;
        UIDialogSpellBook.OnClickSpell += ChooseSpell;
        UIArena.OnCancelSpell += CancelSpell;

        CreateArena();

        if (ResourceSystem.Instance != null)
        {

            // Create test hero.
            hero = new EntityHero(TypeFaction.Castle, heroes[0]);
            enemy = new EntityHero(TypeFaction.Castle, heroes[1]);

            CreateHero();

            CreateCreatures();

            // ArenaQueue.SetActiveEntity(ArenaQueue.First);

            // await GoEntity();
            NextCreature(false, false);

            setSizeTileMap();

        }
    }
    private void OnDestroy()
    {
        UIArena.OnNextCreature -= NextCreature;
        UIArena.OnOpenSpellBook -= OpenSpellBook;
        UIArena.OnClickAttack -= ActionClickButton;
        UIDialogSpellBook.OnClickSpell -= ChooseSpell;
        UIArena.OnCancelSpell -= CancelSpell;
    }

    private async void OpenSpellBook()
    {
        inputManager.Disable();

        var dialogWindow = new DialogSpellBookOperation(ArenaQueue.ActiveHero);
        var result = await dialogWindow.ShowAndHide();

        inputManager.Enable();
    }

    public async void NextCreature(bool wait, bool def)
    {
        ArenaQueue.NextCreature(wait, def);
        await GoEntity();
        OnAutoNextCreature?.Invoke();
    }

    private async UniTask GoEntity()
    {
#if UNITY_EDITOR
        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        stopWatch.Start();
#endif
        ArenaEntity activeArenaEntity = ArenaQueue.activeEntity.arenaEntity;
        ScriptableAttributeCreature activeEntityCreature = (ScriptableAttributeCreature)activeArenaEntity.Entity.ScriptableDataAttribute;

        ResetArenaState();
        DistanceNodes = GridArenaHelper.GetNeighboursAtDistance(
            activeArenaEntity.OccupiedNode,
            activeArenaEntity.Data.speed
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
                        && activeArenaEntity.TypeArenaPlayer == TypeArenaPlayer.Right
                        && neiNode.LeftNode.OccupiedUnit == null
                        && neiNode.OccupiedUnit != activeArenaEntity.OccupiedNode.OccupiedUnit
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
                _tileMapShadow.SetTile(neiNode.position, _tileHexShadow);
                PathNodes.Add(neiNode);
            }
        };

        if (activeEntityCreature.CreatureParams.Size >= 2)
        {
            foreach (var neiNode in AllMovedNodes)
            {
                if (neiNode.weight >= 2)
                {
                    _tileMapShadow.SetTile(neiNode.position, _tileHexShadow);
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

        // lighting active creature.
        LightingActiveNode();

        // lighting allow fighting nodes.
        GetFightingNodes();

#if UNITY_EDITOR
        stopWatch.Stop();
        System.TimeSpan timeTaken = stopWatch.Elapsed;
        Debug.LogWarning($"Time Generation Step::: {timeTaken.ToString(@"m\:ss\.ffff")}");
#endif
        await UniTask.Delay(1);
    }

    private void GetFightingNodes()
    {
        _tileMapAllowAttack.ClearAllTiles();

        var arenaEntity = ArenaQueue.activeEntity.arenaEntity;

        var creatureData = ((ScriptableAttributeCreature)arenaEntity.Entity.ScriptableDataAttribute);

        var occupiedNodes = GridArenaHelper
            .GetAllGridNodes()
            .Where(t =>
                t.OccupiedUnit != null
                && t.OccupiedUnit.TypeArenaPlayer != arenaEntity.TypeArenaPlayer
                // && !t.StateArenaNode.HasFlag(StateArenaNode.Deathed)
                );

        List<GridArenaNode> neighboursNodesEnemy = new();
        if (creatureData.CreatureParams.Shoots != 0)
        {
            neighboursNodesEnemy = GridArenaHelper
                .GetNeighbourList(arenaEntity.OccupiedNode)
                .Where(t =>
                    t.OccupiedUnit != null
                    && t.OccupiedUnit.TypeArenaPlayer != arenaEntity.TypeArenaPlayer
                    // && !t.StateArenaNode.HasFlag(StateArenaNode.Deathed)
                    )
                .ToList();
        }

        foreach (var node in occupiedNodes)
        {
            if (
                (arenaEntity.Data.shoots == 0
                ||
                (creatureData.CreatureParams.Shoots != 0 && neighboursNodesEnemy.Count() > 0))
                )
            {
                arenaEntity.SetTypeAttack(TypeAttack.Attack);
                var neighbours = GridArenaHelper.GetNeighbourList(node);
                if (AllowPathNodes.Intersect(neighbours).Count() > 0)
                {
                    FightingOccupiedNodes.Add(node);
                    SetColorAllowFightNode(node, LevelManager.Instance.ConfigGameSettings.colorAllowAttackCreature);
                }
            }
            else
            {
                arenaEntity.SetTypeAttack(TypeAttack.AttackShoot);
                FightingOccupiedNodes.Add(node);
                SetColorAllowFightNode(node, LevelManager.Instance.ConfigGameSettings.colorAllowAttackCreature);
            }
        }
    }

    private void setSizeTileMap()
    {
        BoxCollider2D colliderTileMap = _tileMapArenaGrid.GetComponent<BoxCollider2D>();
        colliderTileMap.offset = new Vector2(7.25f, 3.75f);
        colliderTileMap.size = new Vector2(width + 1, height - 2);
    }

    private void CreateHero()
    {
        var heroArena = new ArenaHeroEntity(tileMapArenaUnits);
        heroArena.SetEntity(hero);
        heroArena.SetPosition(new Vector3(-2, 8.5f));
        heroArena.CreateMapGameObject();

        var enemyArena = new ArenaHeroEntity(tileMapArenaUnits);
        enemyArena.SetEntity(enemy);
        enemyArena.SetPosition(new Vector3(width + 1.5f, 8.5f));
        enemyArena.CreateMapGameObject();
    }

    private void CreateCreatures()
    {
        foreach (var creature in hero.Data.Creatures)
        {
            if (creature.Value != null)
            {
                var GridGameObject = new ArenaEntity(this, hero);
                var size = ((ScriptableAttributeCreature)creature.Value.ScriptableDataAttribute).CreatureParams.Size;
                var nodeObj = GridArenaHelper.GridTile.GetGridObject(new Vector3Int(size - 1, creature.Key));
                GridGameObject.TypeArenaPlayer = TypeArenaPlayer.Left;
                GridGameObject.SetEntity(creature.Value, nodeObj);
                GridGameObject.SetPosition(nodeObj);

                GridGameObject.CreateMapGameObject(nodeObj);

                ArenaQueue.AddEntity(GridGameObject);
            }
        }

        foreach (var creature in enemy.Data.Creatures)
        {
            if (creature.Value != null)
            {
                var GridGameObject = new ArenaEntity(this, enemy);
                var size = ((ScriptableAttributeCreature)creature.Value.ScriptableDataAttribute).CreatureParams.Size;
                var nodeObj = GridArenaHelper.GridTile.GetGridObject(new Vector3Int(width - size, creature.Key));
                GridGameObject.TypeArenaPlayer = TypeArenaPlayer.Right;
                GridGameObject.SetEntity(creature.Value, nodeObj);
                GridGameObject.SetPosition(nodeObj);

                GridGameObject.CreateMapGameObject(nodeObj);

                ArenaQueue.AddEntity(GridGameObject);
            }
        }
        // SetColorDisableNode();
    }

    private void CreateArena()
    {
        // Create grid and helper.
        _gridArenaHelper = new GridArenaHelper(width, height, this);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);

                _tileMapArenaGrid.SetTile(pos, _tileHex);
                var nodeObj = GridArenaHelper.GridTile.GetGridObject(new Vector3Int(x, y));
                var bounds = tileMapArenaUnits.GetBoundsLocal(pos);
                nodeObj.SetCenter(bounds.center);

                // SetTextMeshNode(nodeObj);
            }
        }
    }

    private void DrawButtonAction()
    {
        RuleTile ruleCursor = CursorRule.NotAllow;

        ScriptableAttributeSpell activeSpell = ArenaQueue.ActiveHero != null
            ? ArenaQueue.ActiveHero.Data.SpellBook.ChoosedSpell
            : null;
        Vector3 positionButton = new Vector3(clickedNode.center.x, clickedNode.center.y, zCoord);

        if (activeSpell == null)
        {
            ScriptableAttributeCreature activeCreatureData
                = (ScriptableAttributeCreature)ArenaQueue.activeEntity.arenaEntity.Entity.ScriptableDataAttribute;

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
                    // var neighboursNodesEnemy = GridArenaHelper
                    //     .GetNeighbourList(ArenaQueue.activeEntity.OccupiedNode)
                    //     .Where(t => t.OccupiedUnit != null && t.OccupiedUnit.TypeArenaPlayer != ArenaQueue.activeEntity.TypeArenaPlayer);
                    if (ArenaQueue.ActiveHero.Data.SpellBook.ChoosedSpell == null)
                    {
                        ruleCursor = CursorRule.FightFromLeft;
                        if (ArenaQueue.activeEntity.arenaEntity.Data.shoots > 0 && ArenaQueue.activeEntity.arenaEntity.Data.typeAttack == TypeAttack.AttackShoot)
                        {
                            // check distance.
                            if (nodesForAttack.nodeToAttack.DistanceTo(ArenaQueue.activeEntity.arenaEntity.OccupiedNode) <= ArenaQueue.activeEntity.arenaEntity.Data.speed)
                            {
                                ruleCursor = CursorRule.Shoot;
                            }
                            else
                            {
                                ruleCursor = CursorRule.ShootHalf;
                            }
                            positionButton = new Vector3(nodesForAttack.nodeToAttack.center.x, nodesForAttack.nodeToAttack.center.y, zCoord);
                        }
                        else
                        {
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
                            // _buttonAction.transform.position = new Vector3(clickedNode.center.x, clickedNode.center.y, -5);
                        }
                    }
                    else
                    {
                        // Spell cursor.
                        ruleCursor = CursorRule.Spell;
                        positionButton = new Vector3(nodesForAttack.nodeToAttack.center.x, nodesForAttack.nodeToAttack.center.y, zCoord);
                    }
                }
            }
        }

        _buttonAction.SetActive(true);
        _buttonSpell.SetActive(false);
        _buttonAction.GetComponent<SpriteRenderer>().sprite = ruleCursor.m_DefaultSprite;
        _buttonAction.transform.position = positionButton;
        activeCursor = ruleCursor;
        // _tileMapCursor.SetTile(clickedNode.position, ruleCursor);
    }

    public async void ActionClickButton()
    {
        await ClickButton();
    }

    public async UniTask ClickButton()
    {
        await AudioManager.Instance.Click();
        if (activeCursor == CursorRule.NotAllow) return;
        Debug.Log($"Click button:::");

        if (
            (AttackedCreature != null && ArenaQueue.activeEntity.arenaEntity.Data.typeAttack == TypeAttack.Attack)
            || AttackedCreature == null
            )
        {
            // Move creature.
            await ArenaQueue.activeEntity.arenaEntity.ArenaMonoBehavior.MoveCreature();
        }

        // Attack, if exist KeyNodeFromAttack
        if (AttackedCreature != null)
        {
            var nodes = NodesForAttackActiveCreature[KeyNodeFromAttack];
            if (nodes.nodeFromAttack.OccupiedUnit != null)
            {
                await nodes.nodeFromAttack.OccupiedUnit.GoAttack(nodes.nodeFromAttack, nodes.nodeToAttack);
            }
        }

        // Clear clicked node.
        clickedNode = null;
        ClearAttackNode();

        // Next creature.
        // await GoEntity();
        NextCreature(false, false);

        // DrawCursor
        if (clickedNode != null) DrawButtonAction();
    }

    public async UniTask ClickArena(Vector3Int positionClick)
    {
        await AudioManager.Instance.Click();

        GridArenaNode node = GridArenaHelper.GridTile.GetGridObject(new Vector3Int(positionClick.x, positionClick.y));

        var activeSpell = ArenaQueue.ActiveHero != null
            ? ArenaQueue.ActiveHero.Data.SpellBook.ChoosedSpell
            : null;
        if (activeSpell != null && node != null)
        {
            clickedNode = node;
            DrawButtonAction();
            return;
        }

        if (node != null)
        {
            // Debug.Log($"Click node::: {node.ToString()}");

            if (AllowPathNodes.Contains(node))
            {

                if (clickedNode != node)
                {
                    //Find path.
                    var path = GridArenaHelper.FindPath(ArenaQueue.activeEntity.arenaEntity.OccupiedNode.position, positionClick, PathNodes);
                    if (path == null)
                    {
                        return;
                    }

                    // Draw path.
                    _tileMapPath.ClearAllTiles();
                    ArenaQueue.activeEntity.arenaEntity.SetPath(path);
                    foreach (var pathNode in path)
                    {
                        _tileMapPath.SetTile(pathNode.position, _tileHexShadow);
                    };

                    // Set active click node.
                    clickedNode = node;
                }
                // else
                // {
                //     // // Move creature.
                //     // await ArenaQueue.activeEntity.ArenaMonoBehavior.MoveCreature();

                //     // // Attack, if exist KeyNodeFromAttack
                //     // if (KeyNodeFromAttack != -1)
                //     // {
                //     //     var nodes = NodesForAttackActiveCreature[KeyNodeFromAttack];
                //     //     if (nodes.nodeFromAttack.OccupiedUnit != null)
                //     //     {
                //     //         await nodes.nodeFromAttack.OccupiedUnit.GoAttack(nodes.nodeToAttack);
                //     //     }
                //     // }

                //     // // Next creature.
                //     // await GoEntity();

                //     // // Clear clicked node.
                //     // clickedNode = null;
                //     // ClearAttackNode();
                // }
            }
            else
            {
                // Click not allowed node.
                _tileMapPath.ClearAllTiles();
                clickedNode = node;
            }

            // DrawCursor
            if (clickedNode != null) DrawButtonAction();
        }
        await UniTask.Delay(1);
    }

    private void ResetArenaState()
    {
        NodesForAttackActiveCreature.Clear();
        AllowPathNodes.Clear();
        FightingOccupiedNodes.Clear();
        DistanceNodes.Clear();
        PathNodes.Clear();

        foreach (var neiNode in AllMovedNodes)
        {
            neiNode.StateArenaNode ^= StateArenaNode.Moved;
        };
        AllMovedNodes.Clear();
        AllowMovedNodes.Clear();
        _buttonAction.SetActive(false);
        _buttonSpell.SetActive(false);

        // _tileMapAllowAttack.ClearAllTiles();
        _tileMapDistance.ClearAllTiles();
        _tileMapPath.ClearAllTiles();
        _tileMapShadow.ClearAllTiles();
        _tileMapDisableNode.ClearAllTiles();
        // _tileMapCursor.ClearAllTiles();
    }

    public async void OnClick(InputAction.CallbackContext context)
    {
        // if (!context.started) return;
        if (context.performed)
        {
            var pos = context.ReadValue<Vector2>();
            var rayHit = Physics2D.GetRayIntersection(_camera.ScreenPointToRay(pos));

            if (!rayHit.collider) return;

            Vector2 posMouse = _camera.ScreenToWorldPoint(pos);
            Vector3Int tilePos = _tileMapArenaGrid.WorldToCell(posMouse);

            GridArenaNode node = GridArenaHelper.GridTile.GetGridObject(tilePos);

            if (rayHit.collider.gameObject == _tileMapArenaGrid.gameObject)
            {
                ClearAttackNode();
                await ClickArena(tilePos);
            }
            else if (rayHit.collider.gameObject == _buttonAction.gameObject)
            {
                await ClickButton();
            }
            else if (rayHit.collider.gameObject == _buttonSpell.gameObject)
            {
                await ClickButtonSpell();
            }
        }
    }

    public void ClearAttackNode()
    {
        // Debug.Log("Clear attack Node");
        _tileMapDisableNode.ClearAllTiles();
        _tileMapPath.ClearAllTiles();
        NodesForAttackActiveCreature.Clear();
        KeyNodeFromAttack = -1;
        AttackedCreature = null;
        _buttonAction.SetActive(false);
        OnChangeNodesForAttack?.Invoke();
    }

    public void CreateAttackNode(ArenaEntity clickedEntity)
    {
        clickedNode = null;
        if (AttackedCreature != clickedEntity && AttackedCreature != null)
        {
            ClearAttackNode();
        }
        // else
        // {
        AttackedCreature = clickedEntity;
        // }

        var allowNodes = AllowPathNodes.Concat(AllowMovedNodes).ToList();

        var neighbourNodesEnemyEntity = GridArenaHelper
            .GetNeighbourList(ArenaQueue.activeEntity.arenaEntity.OccupiedNode)
            .Where(t => t.OccupiedUnit != null && t.OccupiedUnit.TypeArenaPlayer != ArenaQueue.activeEntity.arenaEntity.TypeArenaPlayer);
        if (
            (ArenaQueue.activeEntity.arenaEntity.Data.shoots == 0
            || neighbourNodesEnemyEntity.Count() > 0)
            && ArenaQueue.ActiveHero.Data.SpellBook.ChoosedSpell == null
            )
        {
            List<GridArenaNode> neighbours = GridArenaHelper
                .GetNeighbourList(clickedEntity.OccupiedNode)
                .Where(t => t.OccupiedUnit == null || t.OccupiedUnit == ArenaQueue.activeEntity.arenaEntity)
                .ToList();
            var allowNeighbours = neighbours.Intersect(allowNodes).ToList();
            foreach (var node in allowNeighbours)
            {
                NodesForAttackActiveCreature.Add(new AttackItemNode()
                {
                    nodeFromAttack = node,
                    nodeToAttack = clickedEntity.OccupiedNode
                });
            }

            if (clickedEntity.RelatedNode != null)
            {
                List<GridArenaNode> neighboursRelatedNode
                    = GridArenaHelper.GetNeighbourList(clickedEntity.RelatedNode)
                    .Where(t => t.OccupiedUnit == null || t.OccupiedUnit == ArenaQueue.activeEntity.arenaEntity)
                    .ToList();
                // neighbours = neighbours.Concat(neighboursRelatedNode).ToList();
                var allowNeighboursRelatedNode = neighboursRelatedNode.Intersect(allowNodes).ToList();
                foreach (var node in allowNeighboursRelatedNode)
                {
                    NodesForAttackActiveCreature.Add(new AttackItemNode()
                    {
                        nodeFromAttack = node,
                        nodeToAttack = clickedEntity.RelatedNode
                    });
                }
            }
        }
        else
        {
            NodesForAttackActiveCreature.Add(new AttackItemNode()
            {
                nodeFromAttack = ArenaQueue.activeEntity.arenaEntity.OccupiedNode,
                nodeToAttack = clickedEntity.OccupiedNode
            });
        }

        // NodesForAttackActiveCreature = neighbours.Intersect(allowNodes).ToList();
        _tileMapDisableNode.ClearAllTiles();
        if (LevelManager.Instance.ConfigGameSettings.paintAllowAttackNode)
        {
            foreach (var node in NodesForAttackActiveCreature)
            {
                _tileMapDisableNode.SetTile(node.nodeFromAttack.position, _tileHexActive);
                _tileMapDisableNode.SetTileFlags(node.nodeFromAttack.position, TileFlags.None);
                _tileMapDisableNode.SetColor(node.nodeFromAttack.position, LevelManager.Instance.ConfigGameSettings.colorAllowAttackNode);
                _tileMapDisableNode.SetTileFlags(node.nodeFromAttack.position, TileFlags.LockColor);
            }
        }

        // GridArenaNode randomFirstNode
        //     = NodesForAttackActiveCreature[UnityEngine.Random.Range(0, NodesForAttackActiveCreature.Count)];
        // clickedNode = NodesForAttackActiveCreature[1];
        ChooseNextPositionForAttack();
        OnChangeNodesForAttack?.Invoke();
    }

    public async void ChooseNextPositionForAttack()
    {
        int nextIndex = KeyNodeFromAttack + 1;
        int indexNextAttackNode = nextIndex >= NodesForAttackActiveCreature.Count()
            ? 0
            : nextIndex;
        KeyNodeFromAttack = indexNextAttackNode;
        await ClickArena(NodesForAttackActiveCreature[indexNextAttackNode].nodeFromAttack.position);
    }

    #region  Helpers

    public void LightingActiveNode()
    {
        _tileMapUnitActive.ClearAllTiles();

        if (LevelManager.Instance.ConfigGameSettings.paintActiveCreature)
        {
            Color color = LevelManager.Instance.ConfigGameSettings.colorActiveCreature;
            ArenaEntity activeArenaEntity = ArenaQueue.activeEntity.arenaEntity;

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

    private void SetColorAllowFightNode(GridArenaNode node, Color color)
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

    private void ChooseSpell()
    {
        ClearAttackNode();
        _tileMapAllowAttack.ClearAllTiles();
        FightingOccupiedNodes.Clear();

        var arenaEntity = ArenaQueue.activeEntity.arenaEntity;

        var activeSpell = ArenaQueue.ActiveHero != null
            ? ArenaQueue.ActiveHero.Data.SpellBook.ChoosedSpell
            : null;

        var occupiedNodes = GridArenaHelper
            .GetAllGridNodes()
            .Where(t =>
                t.OccupiedUnit != null
                && (
                    (
                        activeSpell.typeAchievement == TypeSpellAchievement.Enemy
                        && t.OccupiedUnit.TypeArenaPlayer != ArenaQueue.activeEntity.arenaEntity.TypeArenaPlayer
                    )
                    || (
                        activeSpell.typeAchievement == TypeSpellAchievement.Friendly
                        && t.OccupiedUnit.TypeArenaPlayer == ArenaQueue.activeEntity.arenaEntity.TypeArenaPlayer
                    )
                )
                );

        foreach (var node in occupiedNodes)
        {
            FightingOccupiedNodes.Add(node);
            SetColorAllowFightNode(node, LevelManager.Instance.ConfigGameSettings.colorNodeRunSpell);
        }
    }

    internal void CreateButtonSpell(ArenaEntity clickedEntity)
    {
        clickedNode = null;
        ClearAttackNode();

        AttackedCreature = clickedEntity;

        var positionButton = new Vector3(clickedEntity.OccupiedNode.center.x, clickedEntity.OccupiedNode.center.y, zCoord);

        if (
            FightingOccupiedNodes.Count > 0
            && AttackedCreature != null
            && FightingOccupiedNodes.Contains(clickedEntity.OccupiedNode)
            )
        {
            _buttonSpell.SetActive(true);
            _buttonAction.SetActive(false);
            _buttonSpell.transform.position = positionButton;
        }
    }

    private async UniTask ClickButtonSpell()
    {
        await AudioManager.Instance.Click();
        _buttonSpell.SetActive(false);
        await AttackedCreature.GoRunSpell();
        ArenaQueue.ActiveHero.Data.SpellBook.ChooseSpell(null);
        await GoEntity();
    }

    private async void CancelSpell()
    {
        _buttonSpell.SetActive(false);
        ArenaQueue.ActiveHero.Data.SpellBook.ChooseSpell(null);
        await GoEntity();
    }

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
    #endregion
}