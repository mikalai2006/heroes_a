using System;
using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class ArenaManager : MonoBehaviour
{
    public UIArena uiArena;
    public static event Action OnSetNextCreature;
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
    [SerializeField] public GameObject _testObj;
    [SerializeField] public GameObject _testCreatureObj;
    [SerializeField] public GameObject _arenaGameObject;

    private EntityHero hero;
    private GameObject heroGameObject;
    private EntityHero enemy;
    private GameObject enemyGameObject;
    public List<ArenaEntity> ArenaEnteties;
    public List<GridArenaNode> DistanceNodes;
    public List<GridArenaNode> PathNodes;
    public List<GridArenaNode> MovedNodes = new List<GridArenaNode>();

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
        // inputManager.OnStartTouch += OnClick;
        UIArena.OnNextCreature += NextCreature;
        UIArena.OnOpenSpellBook += OpenSpellBook;

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

        ResetTileMaps();
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
                        MovedNodes.Add(neiNode.LeftNode);
                    }
                    else if (
                        neiNode.RightNode != null
                        && activeArenaEntity.TypeArenaPlayer == TypeArenaPlayer.Left
                        && neiNode.RightNode.OccupiedUnit == null
                        && neiNode.OccupiedUnit != activeArenaEntity.OccupiedNode.OccupiedUnit
                        )
                    {
                        neiNode.RightNode.StateArenaNode |= StateArenaNode.Moved;
                        MovedNodes.Add(neiNode.RightNode);
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
            foreach (var neiNode in MovedNodes)
            {
                if (neiNode.weight >= 2)
                {
                    _tileMapShadow.SetTile(neiNode.position, _tileHexShadow);
                }
            };
        }
        foreach (var neiNode in GridArenaHelper.GetAllGridNodes())
        {
            SetTextMeshNode(neiNode);
        };

        // lighting active creature.
        SetColorActiveNode();
#if UNITY_EDITOR
        stopWatch.Stop();
        System.TimeSpan timeTaken = stopWatch.Elapsed;
        Debug.LogWarning($"Time Generation Step::: {timeTaken.ToString(@"m\:ss\.ffff")}");
#endif
        await UniTask.Delay(1);
    }

    public void SetColorActiveNode()
    {
        ArenaEntity activeArenaEntity = ArenaQueue.activeEntity;

        GridArenaNode nodeActiveCreature = activeArenaEntity.OccupiedNode;
        _tileMapUnitActive.ClearAllTiles();

        _tileMapUnitActive.SetTile(nodeActiveCreature.position, _tileHexActive);
        _tileMapUnitActive.SetTileFlags(nodeActiveCreature.position, TileFlags.None);
        Color color = Color.magenta;
        color.a = .6f;
        _tileMapUnitActive.SetColor(nodeActiveCreature.position, color);
        _tileMapUnitActive.SetTileFlags(nodeActiveCreature.position, TileFlags.LockColor);
    }
    public void ResetPathColor()
    {
        _tileMapPathColor.ClearAllTiles();
    }
    public void SetColorPathNode(GridArenaNode node)
    {
        ArenaEntity activeArenaEntity = ArenaQueue.activeEntity;

        _tileMapPathColor.SetTile(node.position, _tileHexActive);
        _tileMapPathColor.SetTileFlags(node.position, TileFlags.None);
        Color color = Color.blue;
        color.a = .6f;
        _tileMapPathColor.SetColor(node.position, color);
        _tileMapPathColor.SetTileFlags(node.position, TileFlags.LockColor);
    }
    public void SetColorDisableNode()
    {
        List<GridArenaNode> disableNodes = GridArenaHelper.GetAllGridNodes()
        .Where(t => !t.StateArenaNode.HasFlag(StateArenaNode.Empty)).ToList();
        _tileMapDisableNode.ClearAllTiles();

        foreach (var node in disableNodes)
        {
            _tileMapDisableNode.SetTile(node.position, _tileHexActive);
            _tileMapDisableNode.SetTileFlags(node.position, TileFlags.None);
            Color color = Color.red;
            color.a = .6f;
            _tileMapDisableNode.SetColor(node.position, color);
            _tileMapDisableNode.SetTileFlags(node.position, TileFlags.LockColor);
        }
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
        SetColorDisableNode();
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

    public async void ClickArena(Vector3Int positionClick)
    {
        TileBase clickedTile = _tileMapArenaGrid.GetTile(positionClick);
        GridArenaNode node = GridArenaHelper.GridTile.GetGridObject(new Vector3Int(positionClick.x, positionClick.y));
        if (node != null)
        {
            Debug.Log($"Click node::: {node.ToString()}");

            // _tileMapShadow.ClearAllTiles();
            // var nodes = GridArenaHelper.GetNeighbourList(node);
            // foreach (var neiNode in nodes)
            // {
            //     _tileMapShadow.SetTile(neiNode.position, _tileHexShadow);
            //     // Debug.Log($"Neig node::: {neiNode?.position}");
            // };

            // Debug.Log($"Neighbours==={node.position}=========================");
            // var nodes = GridArenaHelper.GetNeighboursAtDistance(node, 2); //.FindPath(new Vector3Int(0, 0, 0), positionClick);
            // foreach (var neiNode in nodes)
            // {
            //     _tileMapShadow.SetTile(neiNode.position, _tileHexShadow);
            //     // Debug.Log($"{neiNode?.position}");
            // };

            var path = GridArenaHelper.FindPath(ArenaQueue.activeEntity.OccupiedNode.position, positionClick, PathNodes);
            if (path == null)
            {
                return;
            }

            _tileMapPath.ClearAllTiles();
            ArenaQueue.activeEntity.SetPath(path);
            foreach (var pathNode in path)
            {
                _tileMapPath.SetTile(pathNode.position, _tileHexShadow);

                // Debug.Log($"Path node::: {pathNode.position}");
            };
            await ArenaQueue.activeEntity.ArenaMonoBehavior.StartMove();
            await GoEntity();
            // NextCreature();
            // OnSetNextCreature?.Invoke();
            // await UniTask.Delay(1);
        }
    }

    private void ResetTileMaps()
    {
        DistanceNodes.Clear();
        PathNodes.Clear();

        foreach (var neiNode in MovedNodes)
        {
            neiNode.StateArenaNode ^= StateArenaNode.Moved;
        };
        MovedNodes.Clear();

        ResetTextMeshNode();
        _tileMapDistance.ClearAllTiles();
        _tileMapPath.ClearAllTiles();
        _tileMapShadow.ClearAllTiles();
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        // if (!context.started) return;
        if (context.performed)
        {
            var pos = context.ReadValue<Vector2>();
            var rayHit = Physics2D.GetRayIntersection(_camera.ScreenPointToRay(pos));

            if (!rayHit.collider) return;

            if (rayHit.collider.gameObject == _tileMapArenaGrid.gameObject)
            {
                Vector2 posMouse = _camera.ScreenToWorldPoint(pos);
                Vector3Int tilePos = _tileMapArenaGrid.WorldToCell(posMouse);
                Debug.Log($"Click Grid::: {rayHit.collider.gameObject.name} / {pos}");
                ClickArena(tilePos);
            }
        }
    }
}