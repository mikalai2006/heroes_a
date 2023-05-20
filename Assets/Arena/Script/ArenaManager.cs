using System;
using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

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
    // [SerializeField] private Tilemap _tileMapCursor;
    // public static event Action OnSetNextCreature;
    public static event Action OnChangeNodesForAttack;
    [SerializeField] private int width = 15;
    [SerializeField] private int height = 11;
    [SerializeField] private Tilemap _tileMapArenaGrid;
    [SerializeField] public Tilemap tileMapArenaUnits;
    [SerializeField] public Tilemap _tileMapDistance;
    [SerializeField] private Tilemap _tileMapShadow;
    [SerializeField] private Tilemap _tileMapUnitActive;
    [SerializeField] private Tilemap _tileMapDisableNode;
    [SerializeField] private Tilemap _tileMapPathColor;
    [SerializeField] private Tilemap _tileMapPath;

    [SerializeField] public GameObject _textPrefab;
    [SerializeField] public GameObject _textCanvas;
    private Dictionary<Vector3, GameObject> _listText = new Dictionary<Vector3, GameObject>();

    private GridArenaHelper _gridArenaHelper;
    public GridArenaHelper GridArenaHelper => _gridArenaHelper;
    [SerializeField] public Tile _tileHex;
    [SerializeField] public Tile _tileHexShadow;
    [SerializeField] public Tile _tileHexActive;

    private EntityHero hero;
    // private GameObject heroGameObject;
    private EntityHero enemy;
    public GameObject buttonAction;
    public GameObject _buttonAction;
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

    [SerializeField] private Color _colorActiveCreature;
    [SerializeField] private Color _colorAllowAttack;

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
        // inputManager.OnStartTouch += OnClick;
        UIArena.OnNextCreature += NextCreature;
        UIArena.OnOpenSpellBook += OpenSpellBook;
        UIArena.OnClickNextNodeForAttack += ChooseNextPositionForAttack;

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
            NextCreature();

            setSizeTileMap();

        }
    }
    private void OnDestroy()
    {
        UIArena.OnNextCreature -= NextCreature;
        UIArena.OnOpenSpellBook -= OpenSpellBook;
        UIArena.OnClickNextNodeForAttack -= ChooseNextPositionForAttack;
    }

    private async void OpenSpellBook()
    {
        var dialogWindow = new DialogSpellBookOperation(hero);
        var result = await dialogWindow.ShowAndHide();
    }

    private async void NextCreature()
    {
        ArenaQueue.NextCreature();
        await GoEntity();
    }

    private async UniTask GoEntity()
    {
#if UNITY_EDITOR
        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        stopWatch.Start();
#endif
        ArenaEntity activeArenaEntity = ArenaQueue.activeEntity;
        ScriptableAttributeCreature activeEntityCreature = (ScriptableAttributeCreature)activeArenaEntity.Entity.ScriptableDataAttribute;

        ResetArenaState();
        DistanceNodes = GridArenaHelper.GetNeighboursAtDistance(
            activeArenaEntity.OccupiedNode,
            activeEntityCreature.CreatureParams.Speed
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
        // List<GridArenaNode> allowPathNodes = PathNodes.Concat(MovedNodes).ToList();
        var creatureData = ((ScriptableAttributeCreature)ArenaQueue.activeEntity.Entity.ScriptableDataAttribute);
        var occupiedNodes = GridArenaHelper
            .GetAllGridNodes()
            .Where(t => t.OccupiedUnit != null && t.OccupiedUnit.TypeArenaPlayer != ArenaQueue.activeEntity.TypeArenaPlayer);
        var neighboursNodesEnemy = GridArenaHelper
            .GetNeighbourList(ArenaQueue.activeEntity.OccupiedNode)
            .Where(t => t.OccupiedUnit != null && t.OccupiedUnit.TypeArenaPlayer != ArenaQueue.activeEntity.TypeArenaPlayer);
        foreach (var node in occupiedNodes)
        {
            if (
                ArenaQueue.activeEntity.Data.shoots == 0
                ||
                (creatureData.CreatureParams.Shoots != 0 && neighboursNodesEnemy.Count() > 0)
                )
            {
                ArenaQueue.activeEntity.SetTypeAttack(TypeAttack.Attack);
                var neighbours = GridArenaHelper.GetNeighbourList(node);
                if (AllowPathNodes.Intersect(neighbours).Count() > 0)
                {
                    FightingOccupiedNodes.Add(node);
                    SetColorAllowFightNode(node);
                }
            }
            else
            {
                ArenaQueue.activeEntity.SetTypeAttack(TypeAttack.Shoot);
                FightingOccupiedNodes.Add(node);
                SetColorAllowFightNode(node);
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
        heroArena.SetPosition(new Vector3(-1, 8.5f));
        heroArena.CreateMapGameObject();
        // heroGameObject = GameObject.Instantiate(hero.ConfigData.ClassHero.ArenaPrefab, new Vector3(-1, 8.5f), Quaternion.identity, _tileMapArenaUnits.transform);

        // enemyGameObject = GameObject.Instantiate(enemy.ConfigData.ClassHero.ArenaPrefab, new Vector3(15, 8.5f), Quaternion.identity, _tileMapArenaUnits.transform);
        // enemyGameObject.transform.localScale = new Vector3(-1, 1, 1);
        var enemyArena = new ArenaHeroEntity(tileMapArenaUnits);
        enemyArena.SetEntity(enemy);
        enemyArena.SetPosition(new Vector3(15, 8.5f));
        enemyArena.CreateMapGameObject();

    }

    private void CreateCreatures()
    {
        foreach (var creature in hero.Data.Creatures)
        {
            if (creature.Value != null)
            {
                var GridGameObject = new ArenaEntity(this);
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
                var GridGameObject = new ArenaEntity(this);
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
        int zCoord = -14;
        // _tileMapCursor.ClearAllTiles();
        RuleTile ruleCursor = CursorRule.NotAllow;

        ScriptableAttributeCreature activeCreatureData
            = (ScriptableAttributeCreature)ArenaQueue.activeEntity.Entity.ScriptableDataAttribute;

        var positionButton = new Vector3(clickedNode.center.x, clickedNode.center.y, zCoord);
        if (AllowPathNodes.Contains(clickedNode))
        {
            ruleCursor = activeCreatureData.CreatureParams.Movement == MovementType.Flying
                ? CursorRule.GoFlying
                : CursorRule.GoGround;
        }
        if (NodesForAttackActiveCreature.Count > 0)
        {
            ruleCursor = CursorRule.FightFromLeft;
            if (KeyNodeFromAttack != -1 && AttackedCreature != null)
            {
                var nodesForAttack = NodesForAttackActiveCreature[KeyNodeFromAttack];
                // var neighboursNodesEnemy = GridArenaHelper
                //     .GetNeighbourList(ArenaQueue.activeEntity.OccupiedNode)
                //     .Where(t => t.OccupiedUnit != null && t.OccupiedUnit.TypeArenaPlayer != ArenaQueue.activeEntity.TypeArenaPlayer);

                if (ArenaQueue.activeEntity.Data.shoots > 0 && ArenaQueue.activeEntity.Data.typeAttack == TypeAttack.Shoot)
                {
                    // check distance.
                    if (nodesForAttack.nodeToAttack.DistanceTo(ArenaQueue.activeEntity.OccupiedNode) <= activeCreatureData.CreatureParams.Speed)
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
        }

        _buttonAction.SetActive(true);
        _buttonAction.GetComponent<SpriteRenderer>().sprite = ruleCursor.m_DefaultSprite;
        _buttonAction.transform.position = positionButton;
        // _tileMapCursor.SetTile(clickedNode.position, ruleCursor);
    }

    public async UniTask ClickButton(Vector3Int positionClick)
    {
        GridArenaNode node = GridArenaHelper.GridTile.GetGridObject(new Vector3Int(positionClick.x, positionClick.y));

        if (node != null)
        {
            Debug.Log($"Click button::: {node.ToString()}");

            // if (AllowPathNodes.Contains(node))
            // {
            if (
                (AttackedCreature != null && ArenaQueue.activeEntity.Data.typeAttack == TypeAttack.Attack)
                || AttackedCreature == null
                )
            {
                // Move creature.
                await ArenaQueue.activeEntity.ArenaMonoBehavior.MoveCreature();
            }

            // Attack, if exist KeyNodeFromAttack
            if (AttackedCreature != null)
            {
                var nodes = NodesForAttackActiveCreature[KeyNodeFromAttack];
                if (nodes.nodeFromAttack.OccupiedUnit != null)
                {
                    if (ArenaQueue.activeEntity.Data.typeAttack == TypeAttack.Attack)
                    {
                        await nodes.nodeFromAttack.OccupiedUnit.GoAttack(nodes.nodeToAttack);
                    }
                    else
                    {
                        await nodes.nodeFromAttack.OccupiedUnit.GoAttackShoot(nodes.nodeToAttack);
                    }
                }
            }

            // Clear clicked node.
            clickedNode = null;
            ClearAttackNode();

            // Next creature.
            await GoEntity();

            // }
            // else
            // {
            //     // Click not allowed node.
            //     _tileMapPath.ClearAllTiles();
            //     clickedNode = node;
            // }

            // DrawCursor
            if (clickedNode != null) DrawButtonAction();
        }
    }

    public async UniTask ClickArena(Vector3Int positionClick)
    {
        GridArenaNode node = GridArenaHelper.GridTile.GetGridObject(new Vector3Int(positionClick.x, positionClick.y));

        if (node != null)
        {
            Debug.Log($"Click node::: {node.ToString()}");

            if (AllowPathNodes.Contains(node))
            {

                if (clickedNode != node)
                {
                    //Find path.
                    var path = GridArenaHelper.FindPath(ArenaQueue.activeEntity.OccupiedNode.position, positionClick, PathNodes);
                    if (path == null)
                    {
                        return;
                    }

                    // Draw path.
                    _tileMapPath.ClearAllTiles();
                    ArenaQueue.activeEntity.SetPath(path);
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

        // ResetTextMeshNode();
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
                await ClickButton(tilePos);
            }
        }
    }

    public void ClearAttackNode()
    {
        // Debug.Log("Clear attack Node");
        _tileMapDisableNode.ClearAllTiles();
        NodesForAttackActiveCreature.Clear();
        KeyNodeFromAttack = -1;
        OnChangeNodesForAttack?.Invoke();
        AttackedCreature = null;
    }

    public void CreateAttackNode(ArenaEntity clickedEntity)
    {
        clickedNode = null;
        if (AttackedCreature != clickedEntity && AttackedCreature != null)
        {
            ClearAttackNode();
        }
        else
        {
            AttackedCreature = clickedEntity;
        }

        var allowNodes = AllowPathNodes.Concat(AllowMovedNodes).ToList();

        var neighbourNodesEnemyEntity = GridArenaHelper
            .GetNeighbourList(ArenaQueue.activeEntity.OccupiedNode)
            .Where(t => t.OccupiedUnit != null && t.OccupiedUnit.TypeArenaPlayer != ArenaQueue.activeEntity.TypeArenaPlayer);
        if (ArenaQueue.activeEntity.Data.shoots == 0 || neighbourNodesEnemyEntity.Count() > 0)
        {
            List<GridArenaNode> neighbours = GridArenaHelper
                .GetNeighbourList(clickedEntity.OccupiedNode)
                .Where(t => t.OccupiedUnit == null || t.OccupiedUnit == ArenaQueue.activeEntity)
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
                    .Where(t => t.OccupiedUnit == null || t.OccupiedUnit == ArenaQueue.activeEntity)
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
                nodeFromAttack = ArenaQueue.activeEntity.OccupiedNode,
                nodeToAttack = clickedEntity.OccupiedNode
            });
        }

        // NodesForAttackActiveCreature = neighbours.Intersect(allowNodes).ToList();
        _tileMapDisableNode.ClearAllTiles();
        foreach (var node in NodesForAttackActiveCreature)
        {
            _tileMapDisableNode.SetTile(node.nodeFromAttack.position, _tileHexActive);
            _tileMapDisableNode.SetTileFlags(node.nodeFromAttack.position, TileFlags.None);
            _tileMapDisableNode.SetColor(node.nodeFromAttack.position, _colorAllowAttack);
            _tileMapDisableNode.SetTileFlags(node.nodeFromAttack.position, TileFlags.LockColor);
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
        ArenaEntity activeArenaEntity = ArenaQueue.activeEntity;

        GridArenaNode nodeActiveCreature = activeArenaEntity.OccupiedNode;
        GridArenaNode relatedNodeActiveCreature = activeArenaEntity.RelatedNode;

        _tileMapUnitActive.SetTile(nodeActiveCreature.position, _tileHexActive);
        _tileMapUnitActive.SetTileFlags(nodeActiveCreature.position, TileFlags.None);
        _tileMapUnitActive.SetColor(nodeActiveCreature.position, _colorActiveCreature);
        _tileMapUnitActive.SetTileFlags(nodeActiveCreature.position, TileFlags.LockColor);

        if (relatedNodeActiveCreature != null)
        {
            _tileMapUnitActive.SetTile(relatedNodeActiveCreature.position, _tileHexActive);
            _tileMapUnitActive.SetTileFlags(relatedNodeActiveCreature.position, TileFlags.None);
            _tileMapUnitActive.SetColor(relatedNodeActiveCreature.position, _colorActiveCreature);
            _tileMapUnitActive.SetTileFlags(relatedNodeActiveCreature.position, TileFlags.LockColor);
        }
    }

    private void SetColorAllowFightNode(GridArenaNode node)
    {
        // _tileMapUnitActive.ClearAllTiles();
        _tileMapUnitActive.SetTile(node.position, _tileHexShadow);

        // Color color = Color.red;
        // color.a = .5f;
        // _tileMapUnitActive.SetTileFlags(node.position, TileFlags.None);
        // _tileMapUnitActive.SetColor(node.position, color);
        // _tileMapUnitActive.SetTileFlags(node.position, TileFlags.LockColor);
    }

    public void ResetPathColor()
    {
        _tileMapPathColor.ClearAllTiles();
    }

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

    // public void SetTextMeshNode(GridArenaNode tileNode, string textString = "")
    // {
    //     Vector3 posText = new Vector3(tileNode.center.x, tileNode.center.y);
    //     GameObject text;
    //     if (!_listText.TryGetValue(posText, out text))
    //     {
    //         text = Instantiate(_textPrefab, posText, Quaternion.identity, _textCanvas.transform);
    //         _listText.Add(posText, text);
    //     }
    //     text.gameObject.transform.GetComponentInChildren<TextMeshProUGUI>().text
    //         = textString != "" ? textString : string.Format("{0}:{1}\r\n L{2} W{3}",
    //         tileNode.position.x,
    //         tileNode.position.y,
    //         tileNode.level,
    //         tileNode.weight
    //         );
    //     text.gameObject.SetActive(true);
    // }

    // public void ResetTextMeshNode()
    // {
    //     foreach (var node in _listText)
    //     {
    //         node.Value.gameObject.SetActive(false);
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
    #endregion
}