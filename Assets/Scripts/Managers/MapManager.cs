using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
//using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

[System.Serializable]
public struct Cursors
{
    [SerializeField] public TileBase left;
    [SerializeField] public TileBase right;
    [SerializeField] public TileBase top;
    [SerializeField] public TileBase bottom;
    [SerializeField] public TileBase center;
    [SerializeField] public TileBase cornerLeftTop;
    [SerializeField] public TileBase cornerLeftBottom;
    [SerializeField] public TileBase cornerRightTop;
    [SerializeField] public TileBase cornerRightBottom;
    
    [SerializeField] public TileBase VtoDTopLeft;
    [SerializeField] public TileBase VtoDTopRight;
    [SerializeField] public TileBase VtoDBottomLeft;
    [SerializeField] public TileBase VtoDBottomRight;

    [SerializeField] public TileBase HtoDTopLeft;
    [SerializeField] public TileBase HtoDTopRight;
    [SerializeField] public TileBase HtoDBottomLeft;
    [SerializeField] public TileBase HtoDBottomRight;

    [SerializeField] public TileBase DtoHTopLeft;
    [SerializeField] public TileBase DtoHTopRight;
    [SerializeField] public TileBase DtoHBottomLeft;
    [SerializeField] public TileBase DtoHBottomRight;

    [SerializeField] public TileBase DtoVTopLeft;
    [SerializeField] public TileBase DtoVTopRight;
    [SerializeField] public TileBase DtoVBottomLeft;
    [SerializeField] public TileBase DtoVBottomRight;

    [SerializeField] public TileBase Attack;
    [SerializeField] public TileBase GoMapObject;


}
public class MapManager : MonoBehaviour, IDataGame
{
    public UIMenuAppView UIMenu;

    [SerializeField] private UnitManager UnitManager;

    //private int _width;
    //private int _height;

    [SerializeField] public DataGameMode gameModeData;
    //[SerializeField] private int _cellSize = 1;
    private bool _isWater = false;
    private int _countArea;

    [SerializeField] private Tilemap _tileMap;

    [SerializeField] public GameObject _textMesh;
    [SerializeField] public Tilemap _tileMapText;
    [SerializeField] public Dictionary<Vector3, GameObject> listTextMesh = new Dictionary<Vector3, GameObject>();
#if UNITY_EDITOR
#endif

    [Header("Cursor")]
    [Space(10)]
    [SerializeField] private Tilemap _tileMapCursor;
    [SerializeField] public Cursors _cursorSprites;

    private TileLandscape _tileBg;

    [Header("Road setting")]
    [Space(10)]
    public Tilemap _tileMapSky;
    [SerializeField] private RuleTile _tileSky;

    [Header("Road setting")]
    [Space(10)]
    [SerializeField] Tilemap _tileMapRoad;
    [SerializeField] private TileLandscape _tileRoad;

    [Header("Nature settings")]
    [Space(10)]
    [SerializeField] Tilemap _tileMapNature;

    //[Header("Generate settings")]
    //[Space(10)]
    //[SerializeField] public float _koofNature;
    //[SerializeField] public int countResourceFixed;


    [Header("Walkable Nature settings")]
    [Space(10)]
    [SerializeField] TileBase _tileCreek;
    [SerializeField] Tilemap _tileMapWalkedNature;

    private Dictionary<TypeGround, TileLandscape> _dataTypeGround;
    //private Dictionary<TileBase, TileLandscape> _dataFromTiles;
    private GridTileHelper gridTileHelper;

    public List<GridTileNatureNode> _listNatureNode = new List<GridTileNatureNode>();

    private Dictionary<Vector3Int, GridTileNode> _transferOpenList;
    private Dictionary<Vector3Int, GridTileNode> _transferClosedList;
    private Dictionary<Vector3Int, GridTileNode> openList;
    private Dictionary<Vector3Int, GridTileNode> closedList;
    private float maxSizeOneWorld;

    public void LoadDataGame(DataGame data)
    {
        LoadMap(data);
    }

    public void SaveDataGame(ref DataGame data)
    {
        data.dataMap.mapNode = gridTileHelper?.GetAllGridNodes();
        data.dataMap.natureNode = _listNatureNode;
        //data.dataMap.width = _width;
        //data.dataMap.height = _height;
        data.dataMap.GameModeData = gameModeData;
        data.dataMap.isWater = _isWater;
        data.dataMap.countArea = _countArea;
    }

    private void LoadMap(DataGame data)
    {
        //_width = data.dataMap.width;
        //_height = data.dataMap.height;
        gameModeData = data.dataMap.GameModeData;
        _isWater = data.dataMap.isWater;
        _countArea = data.dataMap.countArea;

        // Clear all tilemap.
        InitSetting();

        // Create grid tile nodes.
        gridTileHelper = new GridTileHelper(gameModeData.width, gameModeData.height);

        for (int i = 0; i < data.dataMap.mapNode.Count; i++)
        {
            //Debug.Log($"data map::: [count={data.dataMap.mapNode.Count}] { node.x} - {node.y}");
            GridTileNode node = gridTileHelper.GridTile.getGridObject(new Vector3Int(data.dataMap.mapNode[i].x, data.dataMap.mapNode[i].y));
            node.typeGround = data.dataMap.mapNode[i].typeGround;
            node.keyArea = data.dataMap.mapNode[i].keyArea;
            node.State = data.dataMap.mapNode[i].State;

            if (data.dataMap.mapNode[i].Road) node.SetAsRoad();
        }

        for (int x = 0; x < gameModeData.width; x++)
        {
            for (int y = 0; y < gameModeData.height; y++)
            {

                GridTileNode tileNode = gridTileHelper.GridTile.getGridObject(new Vector3Int(x, y));

                RuleTile drawRule = _dataTypeGround[tileNode.typeGround].tileRule;

                Vector3Int pos = new Vector3Int(x, y, 0);

                _tileMap.SetTile(pos, drawRule);
                //tileNode.SetState(true);
            }
        }

        setSizeTileMap();

        for (int i = 0; i < data.dataMap.natureNode.Count; i++)
        {
            GridTileNode tileNode = gridTileHelper.GridTile.getGridObject(new Vector3Int(data.dataMap.natureNode[i].x, data.dataMap.natureNode[i].y));

            //Debug.Log($"GetNature: [{i}-{data.dataMap.natureNode[i].n}-{data.dataMap.natureNode[i].idNature}]");
            if (data.dataMap.natureNode[i].idNature == "creek")
            {

                _tileMapWalkedNature.SetTile(tileNode._position, _tileCreek);
                continue;
            }
            TileNature natureTile = ResourceSystem.Instance.GetNature(data.dataMap.natureNode[i].idNature);
            if (natureTile == null)
            {
                Debug.Log($"None resource for tile nature: [{i}-{data.dataMap.natureNode[i].idNature}]");
                continue;
            }
            _tileMapNature.SetTile(tileNode._position, natureTile);
            //_tileMap.SetTile(tileNode._position, _tileBg.tileRule);
        }

        // Spawn town.
        foreach (SaveDataUnit<DataTown> unitTown in DataManager.Instance.DataPlay.Units.towns)
        {
            GridTileNode tileNode = gridTileHelper.GridTile.getGridObject(new Vector3Int(unitTown.position.x, unitTown.position.y));

            if (unitTown.idObject == "") continue;

            ScriptableTown scriptableData = ResourceSystem.Instance.GetUnit<ScriptableTown>(unitTown.idObject);
            
            if (scriptableData == null)
            {
                Debug.Log($"None town data for : [{unitTown.idUnit}-{unitTown.data.name}]");
                continue;
            }
            UnitBase Town = UnitManager.SpawnUnitToNode(scriptableData, tileNode);
            LevelManager.Instance.GetPlayer(unitTown.data.idPlayer).AddTown(Town);
        }

        foreach (SaveDataUnit<DataHero> unitHero in DataManager.Instance.DataPlay.Units.heroes)
        {
            GridTileNode tileNode = gridTileHelper.GridTile.getGridObject(new Vector3Int(unitHero.position.x, unitHero.position.y));

            if (unitHero.idObject == "") continue;

            ScriptableHero scriptableData = ResourceSystem.Instance.GetUnit<ScriptableHero>(unitHero.idObject);

            if (scriptableData == null)
            {
                Debug.Log($"None hero data for : [{unitHero.idUnit}-{unitHero.data.name}]");
                continue;
            }
            UnitBase Hero = UnitManager.SpawnUnitToNode(scriptableData, tileNode);
            Hero.OnLoadUnit(unitHero);
            LevelManager.Instance.GetPlayer(unitHero.data.idPlayer).AddHero((Hero)Hero);
        }

    }

    private void InitSetting()
    {

        _tileMap.ClearAllTiles();
        _tileMapCursor.ClearAllTiles();
        _tileMapRoad.ClearAllTiles();
        _tileMapNature.ClearAllTiles();
        _tileMapWalkedNature.ClearAllTiles();

        _dataTypeGround = ResourceSystem.Instance.AllLandscape();

        _tileBg = _dataTypeGround.Values.Where(t => t.typeGround == TypeGround.Sand).First();

        // Reset unitManager.
        UnitManager.ResetUnitManager();
    }

    public IEnumerator NewMap()
    {
        Application.targetFrameRate = 60;

        InitSetting();

        //_width = LevelManager.Instance.width;
        //_height = LevelManager.Instance.height;
        gameModeData = LevelManager.Instance.gameModeData;

        _isWater = LevelManager.Instance.isWater;
        _countArea = LevelManager.Instance.CountArea;

        // Create grid tile nodes.
        gridTileHelper = new GridTileHelper(gameModeData.width, gameModeData.height);

        // Add water if this setting is exist.
        if (!_isWater)
        {
            _dataTypeGround.Remove(TypeGround.Water);
        }

        //CreateGrid();
        yield return StartCoroutine(CreateAreas());
        yield return StartCoroutine(CreateTerrain());
        NormalizeArea();
        yield return StartCoroutine(CreateBordersRect());
        yield return StartCoroutine(CreateBorders());

        //CreateCreeks();
        yield return StartCoroutine(CreateTowns());
        //CreatePortals();
        yield return StartCoroutine(CreateLandscape());
        yield return StartCoroutine(CreateRoads());

        yield return StartCoroutine(CreateMines());

        yield return StartCoroutine(CreateExplore());
        yield return StartCoroutine(CreateSkillSchool());
        yield return StartCoroutine(CreateEveryWeekDayResource());
        yield return StartCoroutine(CreateFreeResource());

        yield return StartCoroutine(CreateArtifacts());

        //AnalyzeLanscape();
        //CreateWarriors();

        Debug.LogWarning($"LEVEL::: \n {LevelManager.Instance.ToString()}");
        Application.targetFrameRate = -1;

        yield return null;
    }

    private void setSizeTileMap()
    {

        BoxCollider2D colliderTileMap = _tileMap.GetComponent<BoxCollider2D>();
        colliderTileMap.offset = new Vector2((float)gameModeData.width / 2, (float)gameModeData.height / 2);
        colliderTileMap.size = new Vector2(gameModeData.width, gameModeData.height);
        CompositeCollider2D composeColiiderTileMap = _tileMap.GetComponent<CompositeCollider2D>();
        //composeColiiderTileMap.
    }

    /// <summary>
    /// Create grid for map.
    /// </summary>
    //private void CreateGrid()
    //{
    //    UIMenu.SetProgressText("Grid");

    //    for (int x = 0; x < _width; x++)
    //    {
    //        for (int y = 0; y < _height; y++)
    //        {
    //            GridTileNode tileNode = new GridTileNode(gridTileHelper.GridTile, x, y);

    //            gridTileHelper.GridTile.SetValue(x, y, tileNode);
    //        }
    //    }
    //}

    private void NormalizeArea()
    {
        _countArea = LevelManager.Instance.level.listArea.Count;

        //Debug.Log($"Area before::: {LevelManager.Instance.ToString()}");

        List<Area> areaList = LevelManager.Instance.level.listArea.Where(t => t.countNode < 30).ToList();

        while(areaList.Count > 0)
        {
            Area area = areaList[0];

            List<GridTileNode> nodes = gridTileHelper.GetAllGridNodes().Where(t =>t.keyArea == area.id).ToList();

            for(int y = 0; y < nodes.Count; y++)
            {
                GridTileNode node = nodes[y];
                _tileMap.SetTile(node._position, _dataTypeGround[node.typeGround].tileRule);

                List<TileNature> listTileForDraw = ResourceSystem.Instance.GetNature().Where(t =>
                    t.typeGround == node.typeGround
                    && !t.isWalkable
                ).ToList();

                TileNature tileForDraw = listTileForDraw[Random.Range(0, listTileForDraw.Count)];

                _tileMapNature.SetTile(node._position, tileForDraw);

                _listNatureNode.Add(new GridTileNatureNode(node, tileForDraw.idObject, false, tileForDraw.name));

                SetColorForTile(node._position, Color.green);

                SetDisableNode(node, tileForDraw.listTypeNoPath, Color.white);
                
            }

            LevelManager.Instance.RemoveArea(area);

            areaList.RemoveAt(0);

        }

        _countArea = LevelManager.Instance.level.listArea.Count;
        //Debug.Log($"Area after::: {LevelManager.Instance.ToString()}");
    }

    /// <summary>
    /// Create areas.
    /// </summary>
    /// <returns></returns>
    public IEnumerator CreateAreas()
    {
        List<TileLandscape> listTileData = _dataTypeGround.Values.Where(t => t.typeGround != TypeGround.None && t.typeGround != TypeGround.Sand).ToList();
        
        maxSizeOneWorld = 1f / (float)_countArea;

        closedList = new Dictionary<Vector3Int, GridTileNode>();

        openList = gridTileHelper.GetAllGridNodes().ToDictionary(t => t._position, t => t);

        for (int i = 0; i < _countArea; i++)
        {

            TileLandscape randomTileData = listTileData[Random.Range(0, listTileData.Count)];

            GridTileNode randomNode = openList.ElementAt(Random.Range(0, openList.Count)).Value;

            List<GridTileNode> listNeighbors = gridTileHelper.GetNeighbourListWithTypeGround(randomNode);
            
            while (listNeighbors.Count != 4)
            {
                randomNode = openList.ElementAt(Random.Range(0, openList.Count)).Value;

                listNeighbors = gridTileHelper.GetNeighbourListWithTypeGround(randomNode);
            }

            LevelManager.Instance.AddArea(i, randomTileData.typeGround);

            yield return StartCoroutine(CreateArea(i, randomNode, randomTileData));

            randomNode.isCreated = true;

        }

        if (openList.Count > 0)
        {
            int keyEmprtyArea = _countArea;

            listTileData = _dataTypeGround.Values.Where(t => t.typeGround != TypeGround.None).ToList();

            while (openList.Count > 0)
            {
                TileLandscape randomTileData = listTileData[Random.Range(0, listTileData.Count)];

                LevelManager.Instance.AddArea(keyEmprtyArea, randomTileData.typeGround);

                yield return StartCoroutine(CreateArea(keyEmprtyArea, openList.ElementAt(0).Value, randomTileData));
                keyEmprtyArea++;
            }
        }

        Debug.LogWarning($"AllTileNodes::: noCreated-{openList.Count}[Created={closedList.Count}]");
        
        closedList.Clear();
        openList.Clear();
    }

    public IEnumerator CreateArea(int keyArea, GridTileNode startNode, TileLandscape tileData)
    {
        float bombTimer = 0;

        UIMenu.SetProgressText("Create area " + tileData.typeGround);

        Area area = LevelManager.Instance.GetArea(keyArea);

        area.startPosition = startNode._position;

        _transferOpenList = new Dictionary<Vector3Int, GridTileNode>();
        _transferOpenList.Add(startNode._position, startNode);
        _transferClosedList = new Dictionary<Vector3Int, GridTileNode>();

        startNode.level = 0;
        startNode.keyArea = keyArea;
        float maxCountNode = (
            (gridTileHelper.gridTile.GetHeight() * gridTileHelper.gridTile.GetWidth() * maxSizeOneWorld) -
            (gridTileHelper.gridTile.GetHeight() * gridTileHelper.gridTile.GetWidth() * .03f)
            );

        while (_transferOpenList.Count > 0 && openList.Count > 0)
        {
            
            GridTileNode currentNode = gridTileHelper.GetRandomTileNode(_transferOpenList); // _transferOpenList.ElementAt(0).Value;

            Vector3Int currentNodePosition = currentNode._position; // new Vector3Int(currentNode.x, currentNode.y);

            if (bombTimer > 1)
            {
                // float procentCreatedNode = ((float)_transferClosedList.Count / maxCountNode);
                float procentCreatedNode = ((float)closedList.Count / (gridTileHelper.gridTile.GetHeight() * gridTileHelper.gridTile.GetWidth()));

                UIMenu.SetProgressValue(procentCreatedNode * 100f);
                bombTimer = 0;
                yield return null;
            }
            bombTimer += Time.deltaTime;

            currentNode.isCreated = true;
            currentNode.typeGround = tileData.typeGround;

            _transferOpenList.Remove(currentNodePosition);
            openList.Remove(currentNodePosition);

            _transferClosedList.Add(currentNodePosition, currentNode);
            closedList.Add(currentNodePosition, currentNode);

            if (area.countNode <= maxCountNode)
            {
                List<GridTileNode> listNeighbors = gridTileHelper.GetNeighbourList(currentNode);//.OrderBy(t => Random.value).ToList();
                currentNode.countRelatedNeighbors = listNeighbors.Count;

                for (int x = 0; x < listNeighbors.Count; x++)
                {
                    GridTileNode neighbourNode = listNeighbors[x];

                    Vector3Int neighbourPosition = neighbourNode._position; // new Vector3Int(neighbourNode.x, neighbourNode.y);

                    //List<GridTileNode> listCreatedNeighbours = listNeighbors.Where(t => t.isCreated).ToList();

                    if (
                        closedList.ContainsKey(neighbourPosition)
                        && _transferClosedList.ContainsKey(neighbourPosition)
                    )
                    {
                        //if (neighbourNode.typeGround == currentNode.typeGround && neighbourNode.keyArea == currentNode.keyArea)
                        //{
                        //    // neighbourNode.countRelatedNeighbors += 1;
                        //    currentNode.countRelatedNeighbors += 1;
                        //}
                        continue;
                    }

                    if (neighbourNode.isCreated)
                    {
                        if (!closedList.ContainsKey(neighbourPosition)) closedList.Add(neighbourPosition, neighbourNode);
                        _transferClosedList.Add(neighbourPosition, neighbourNode);
                        continue;
                    }
                    if (!_transferOpenList.ContainsKey(neighbourPosition)) //  && listNeighbors.Count == 4
                    {
                        neighbourNode.level = currentNode.level + 1;
                        neighbourNode.keyArea = currentNode.keyArea;
                        _transferOpenList.Add(neighbourPosition, neighbourNode);
                        area.countNode++;
                        //neighbourNode.countRelatedNeighbors += 1;
                        //currentNode.countRelatedNeighbors += 1;
                    }

                }

            } else
            {
                List<GridTileNode> listNeighbors = gridTileHelper.GetNeighbourListWithTypeGround(currentNode);//.OrderBy(t => Random.value).ToList();
                currentNode.countRelatedNeighbors = listNeighbors.Count;

            }

        }
        //Debug.Log($"_maxSizeOneWorld={_maxSizeOneWorld}, created = { ((float)_transferClosedList.Count / ((float)gridTile.GetHeight() * (float)gridTile.GetWidth()))}");
        //if (maxSizeOneWorld <= currentCountNode && _transferOpenList.Count == 0)
        //{
        //Debug.LogWarning($"" +
        //    $"Exist _transferOpenList=" +
        //    $"area.countNode={area.countNode}" +
        //    $"maxCountNode={maxCountNode}" +
        //    $"closed[{_transferClosedList.Count}]" +
        //    $"open[{_transferOpenList.Count}]" +
        //    $"openList[{openList.Count}]" +
        //    $"closedList[{closedList.Count}]");
        yield return null;
        //break;
        //}
        // Debug.Log($"bombTimer {keyArea}[{area.countNode}] = {bombTimer}");
        ////UIMenu.SetProgressValue(keyArea);
        //yield return null;
    }

    private IEnumerator CreateTerrain()
    {
        UIMenu.SetProgressText("Terrain");

        // Generate grid areas for tile node data.
        //gridTileHelper.CreateAreas(
        //    _countArea,
        //    1f / (float)_countArea,
        //    _dataTypeGround.Values.Where(t => t.typeGround != TypeGround.None && t.typeGround != TypeGround.Sand).ToList(),
        //    _tileBg
        //);

        //Debug.Log($"LevelManager::: {LevelManager.Instance.ToString()}");

        // Test
#if UNITY_EDITOR
        List<Color> listColor = new List<Color> { Color.red, Color.blue, Color.green, Color.gray, Color.cyan, Color.yellow, Color.white };
#endif
        float timer = 0;
        for (int x = 0; x < gameModeData.width; x++)
        {
            for (int y = 0; y < gameModeData.height; y++)
            {

                GridTileNode tileNode = gridTileHelper.GridTile.getGridObject(new Vector3Int(x, y));

                RuleTile drawRule = _dataTypeGround[tileNode.typeGround].tileRule;

                Vector3Int pos = new Vector3Int(x, y, 0);

                _tileMap.SetTile(pos, drawRule);
                
                tileNode.SetState(TypeStateNode.Enable);

                if (x == 0 || y == 0 || x == gameModeData.width - 1 || y == gameModeData.height - 1)
                {
                    tileNode.isEdge = true;
                }
                _tileMap.SetTile(pos, drawRule);
                _tileMapSky.SetTile(pos, _tileSky);
                // SetTextMeshNode(tileNode);

#if UNITY_EDITOR
                //Color a = listColor[tileNode.keyArea > 6 ? 0 : tileNode.keyArea];
                //a.a = ((tileNode.countRelatedNeighbors * tileNode.level) * .01f);
                //SetColorForTile(pos, a);
                //if (tileNode.level == 0)
                //{
                //    SetColorForTile(pos, Color.black);
                //}
#endif

            }
            if (timer > 1f)
            {
                UIMenu.SetProgressValue(((float)x / (float)gameModeData.width) * 100f);
                yield return null;
                timer = 0;
            }
            timer += Time.deltaTime;
        }
        //yield return null;
        setSizeTileMap();
    }

    private IEnumerator CreateBordersRect()
    {
        UIMenu.SetProgressText("BordersRect");

        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        stopWatch.Start();

        int i = 0;

        foreach (GridTileNode tileNode in gridTileHelper.GetAllGridNodes().Where(t =>
            t.isEdge
            && gridTileHelper.CalculateNeighbours(t) >= 5
            && t.Empty
            && t.Enable
        ))
        {
            i++;

            TileLandscape _tileData = _dataTypeGround[tileNode.typeGround];

            List<TileNature> listNature = ResourceSystem.Instance.GetNature().Where(t =>
                        t.typeGround == _tileData.typeGround
                        && t.isCorner
                    ).ToList();

            TileNature cornerTiles = listNature[Random.Range(0, listNature.Count)];

            _tileMapNature.SetTile(tileNode._position, cornerTiles);

            _listNatureNode.Add(new GridTileNatureNode(tileNode, cornerTiles.idObject, false, cornerTiles.name));

            SetDisableNode(tileNode, cornerTiles.listTypeNoPath, Color.blue);

            if (i % 10 == 0)
            {
                UIMenu.SetProgressValue(i);
                yield return null;
            }
        }

        stopWatch.Stop();
        System.TimeSpan timeTaken = stopWatch.Elapsed;
        Debug.Log($"BordersRect Stopwatch::: {timeTaken.ToString(@"m\:ss\.ffff")}");
        yield return null;
    }

    private IEnumerator CreateBorders()
    {
        UIMenu.SetProgressText("Borders");

        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        stopWatch.Start();

        int i = 0;

        List<GridTileNode> nodes = gridTileHelper.GetAllGridNodes().Where(t =>
            gridTileHelper.CalculateNeighboursByArea(t) < 4
            && t.Empty
            && t.Enable
            //&& !t.isEdge
            && (
                t.x > 1 &&
                t.x < gameModeData.width - 2 &&
                t.y > 1 &&
                t.y < gameModeData.height - 2
            )
        ).ToList();

        foreach (GridTileNode tileNode in nodes)
        {
            i++;
            if (tileNode.Empty && tileNode.Enable) {

                TileLandscape _tileData = _dataTypeGround[tileNode.typeGround];
                List<TileNature> listNature = ResourceSystem.Instance.GetNature().Where(t =>
                            t.typeGround == _tileData.typeGround
                            && t.isCorner
                        ).ToList();
                TileNature cornerTiles = listNature[Random.Range(0, listNature.Count)];

                _tileMapNature.SetTile(tileNode._position, cornerTiles);

                _listNatureNode.Add(new GridTileNatureNode(tileNode, cornerTiles.idObject, false, cornerTiles.name));

                SetDisableNode(tileNode, cornerTiles.listTypeNoPath, Color.blue);

            }
            if (i % 10 == 0)
            {
                UIMenu.SetProgressValue(i);
                yield return null;
            }
        }

        stopWatch.Stop();
        System.TimeSpan timeTaken = stopWatch.Elapsed;
        Debug.Log($"Border Stopwatch::: {timeTaken.ToString(@"m\:ss\.ffff")}");

        yield return StartCoroutine(CreateMountains());

        yield return null;
    }

    private IEnumerator CreateMountains()
    {
        UIMenu.SetProgressText("Mountains");

        if (LevelManager.Instance.gameModeData.noiseScaleMontain == 0 || LevelManager.Instance.gameModeData.koofMountains == 0)
        {
            yield return null;
        }
        // Random value for noise.
        var xOffSet = Random.Range(-10000f, 10000f);
        var zOffSet = Random.Range(-10000f, 10000f);

        for (int x = 0; x < gameModeData.width; x++)
        {
            for (int y = 0; y < gameModeData.height; y++)
            {
                GridTileNode currentNode = gridTileHelper.GetNode(x, y);

                float noiseValue = Mathf.PerlinNoise(x * LevelManager.Instance.gameModeData.noiseScaleMontain + xOffSet, y * LevelManager.Instance.gameModeData.noiseScaleMontain + zOffSet);

                bool _isMountain = noiseValue < LevelManager.Instance.gameModeData.koofMountains;

                //Area area = LevelManager.Instance.GetArea(currentNode.keyArea);

                //float minCountNoMountain = area.countNode * LevelManager.Instance.koofMountains;

                // Create Mountain.
                if (_isMountain && currentNode.Empty && currentNode.Enable && gridTileHelper.GetNeighbourListWithTypeGround(currentNode).Count > 3)
                {
                    TileLandscape _tileData = _dataTypeGround[currentNode.typeGround];

                    List<TileNature> listTileForDraw = ResourceSystem.Instance.GetNature().Where(t =>
                        t.typeGround == _tileData.typeGround
                        && !t.isWalkable
                    ).ToList(); //  _tileData.cornerTiles.Concat(_tileData.natureTiles).ToList();

                    TileNature tileForDraw = listTileForDraw[Random.Range(0, listTileForDraw.Count)];

                    if (currentNode.Empty && currentNode.Enable)
                    {
                        _tileMapNature.SetTile(currentNode._position, tileForDraw);
                        
                        _listNatureNode.Add(new GridTileNatureNode(currentNode, tileForDraw.idObject, false, tileForDraw.name));

                        SetDisableNode(currentNode, tileForDraw.listTypeNoPath, Color.black);

                    }

                    //area.countMountain++;
                }
                //UIMenu.SetProgressValue(((float)((x * y) / (_width * _height)) * 100));
                //yield return null;
            }
            //UIMenu.SetProgressValue(((float)x / (float)_width) * 100);
            //yield return null;
        }
        yield return null;
    }

    private IEnumerator CreateTowns()
    {
        UIMenu.SetProgressText("Towns");
        // List<UnitBase> towns = new List<UnitBase>();

        for (int x = 0; x < LevelManager.Instance.level.listArea.Count; x++)
        {
            Area area = LevelManager.Instance.level.listArea[x];

            float minCountNodeAreaForTown = ((float)area.countNode / (float)gridTileHelper.gridTile.SizeGrid);

            // For small area cancel spawn town.
            if (minCountNodeAreaForTown < LevelManager.Instance.gameModeData.koofMinTown || !area.isFraction)
            {
                continue;
            }

            //Debug.LogWarning($"Spawn town for {area.typeGround}-{area.isFraction}");

            List<GridTileNode> listGridNode = gridTileHelper.GetAllGridNodes().Where(t =>
                t.Empty
                && !t.Protected
                && t.Enable
                && t.keyArea == area.id
                && gridTileHelper.CalculateNeighbours(t) == 8
            // && gridTileHelper.GetDistanceBetweeenPoints(t._position, LevelManager.Instance.GetArea(t.keyArea).startPosition) < 10 
            //&& t.level < 10
            ).OrderBy(t => Random.value).ToList();

            if (listGridNode.Count() > 0)
            {
                GridTileNode node = listGridNode[listGridNode.Count - 1];
                //Create town.
                ScriptableTown town = UnitManager.SpawnTown(node, area.id);
                // towns.Add(town);
                area.startPosition = node._position;

                // Spawn mines.
                for (int i = 0; i < town.mines.Count; i++)
                {
                    var listNodes = gridTileHelper.GetAllGridNodes().Where(t =>
                    t.keyArea == area.id
                    && t.Empty
                    && t.Enable
                    && gridTileHelper.GetDistanceBetweeenPoints(t._position, node._position) >= 4
                    && gridTileHelper.GetDistanceBetweeenPoints(t._position, node._position) <= 10
                    && gridTileHelper.CalculateNeighboursByArea(t) == 8
                   ).ToList();
                    if (listNodes.Count > 0)
                    {
                        GridTileNode nodeForSpawn = listNodes.OrderBy(t => Random.value).First();

                        if (nodeForSpawn != null)
                        {
                            UnitBase createdMine = UnitManager.SpawnUnitToNode(town.mines[i], nodeForSpawn);
                        }
                    }
                }

            }
            yield return null;
        }

    }

    public void CreateCreeks(GridTileNode startNode)
    {
        GridTileNode randomNode = gridTileHelper.GetAllGridNodes().Where(t =>
            t.Disable
            && t.keyArea == startNode.keyArea
            && (gridTileHelper.GetDistanceBetweeenPoints(t._position,startNode._position) > 5
            && gridTileHelper.GetDistanceBetweeenPoints(t._position, startNode._position) < 15)
        ).OrderBy(t => Random.value).First();

        List<GridTileNode> path = gridTileHelper.FindPath(
            startNode._position,
            randomNode._position,
            false,
            false,
            true
            );

        if (path != null)
        {
            foreach (GridTileNode node in path)
            {
                _tileMapWalkedNature.SetTile(node._position, _tileCreek);
                _listNatureNode.Add(new GridTileNatureNode(node, "creek", true, "creek"));
            }
        }

    }
    
    private IEnumerator CreateMines()
    {
        UIMenu.SetProgressText("Create mines");

        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        stopWatch.Start();

        float timer = 0;

        for (int keyArea = 0; keyArea < LevelManager.Instance.level.listArea.Count; keyArea++)
        {
            Area area = LevelManager.Instance.level.listArea[keyArea];

            List<GridTileNode> nodes = gridTileHelper.GetAllGridNodes().Where(node =>
                node.Empty
                && node.Enable
                && !node.Road
                && !node.Protected
                && node.keyArea == area.id
                && gridTileHelper.GetDistanceBetweeenPoints(node._position, area.startPosition) > 10
                && gridTileHelper.GetNeighbourList(node).Count >= 4
            ).OrderBy(t => Random.value).ToList();

            if (nodes.Count > 0)
            {
                int countLandscape = Mathf.CeilToInt(LevelManager.Instance.gameModeData.koofMines * .1f * area.countNode);
                area.Stat.countMineN = countLandscape;
                int countCreated = 0;

                while (countCreated < countLandscape && nodes.Count > 0)
                {
                    GridTileNode currentNode = nodes[Random.Range(0, nodes.Count)];

                    NeighboursNature disableNeighbours = gridTileHelper.GetDisableNeighbours(currentNode);

                    GridTileNode nodeWarrior = GetNodeWarrior(currentNode);

                    //Debug.Log($"Count mine area[{area.id}]max[{countLandscape}]create[{countCreated}][{natureNode.bottom.Count}][{natureNode.top.Count}]");

                    if (
                        currentNode != null
                        && nodeWarrior != null
                        && disableNeighbours.bottom.Count == 0
                        && disableNeighbours.top.Count >= 2
                        && gridTileHelper.CalculateNeighbours(currentNode) >= 5
                        )
                    {
                        UnitBase unit = UnitManager.SpawnMine(currentNode, TypeMine.Free);

                        BaseWarriors warrior = (BaseWarriors)UnitManager.SpawnWarrior(nodeWarrior);

                        nodeWarrior.SetProtectedNeigbours(warrior, currentNode);

                        nodes.Remove(currentNode);

                        countCreated++;

                        area.Stat.countMine++;

                        List<GridTileNode> listExistExitNode = gridTileHelper.IsExistExit(currentNode);

                        if (listExistExitNode.Count > 1)
                        {
                            CreatePortal(currentNode, listExistExitNode);
                            // Debug.Log($"Need portal::: keyArea{currentNode.keyArea}[{currentNode._position}]- {listExistExitNode.Count}");

                        }
                        //else
                        //{
                        //    Debug.Log($"NoExit::: keyArea{currentNode.keyArea}[{currentNode._position}]- Null");

                        //}

                    }
                    else
                    {
                        nodes.Remove(currentNode);
                        continue;
                    }

                    timer += Time.deltaTime;
                    if (timer > 1f)
                    {
                        UIMenu.SetProgressValue((float)((float)(countCreated) / (float)countLandscape) * 100f);
                        yield return null;
                        timer = 0;
                    }
                }

            }
        }
        stopWatch.Stop();
        System.TimeSpan timeTaken = stopWatch.Elapsed;
        Debug.Log($"Mines Watch time::: {timeTaken.ToString(@"m\:ss\.ffff")}");
        yield return null;
    }

    private IEnumerator CreateEveryWeekDayResource()
    {
        UIMenu.SetProgressText("Create resources");

        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        stopWatch.Start();

        float timer = 0;

        for (int x = 0; x < LevelManager.Instance.level.listArea.Count; x++)
        {
            Area area = LevelManager.Instance.level.listArea[x];

            List<GridTileNode> nodes = gridTileHelper.GetAllGridNodes().Where(t =>
                    t.Empty
                    && t.Enable
                    && !t.Road
                    && !t.Protected
                    && t.keyArea == area.id
                    && gridTileHelper.CalculateNeighbours(t) == 8
                    && gridTileHelper.GetDistanceBetweeenPoints(t._position, area.startPosition) > 5
                ).OrderBy(t => Random.value).ToList();

            if (nodes.Count > 0)
            {
                int maxCountResource = Mathf.CeilToInt(LevelManager.Instance.gameModeData.koofResource * area.countNode);
                area.Stat.countEveryResourceN = maxCountResource;
                int countCreated = 0;

                while(countCreated < maxCountResource && nodes.Count > 0)
                {
                    GridTileNode currentNode = nodes[Random.Range(0, nodes.Count)];

                    GridTileNode nodeWarrior = GetNodeWarrior(currentNode);

                    if (nodeWarrior != null && currentNode != null && gridTileHelper.CalculateNeighbours(currentNode) == 8)
                    {

                        BaseWarriors warrior = UnitManager.SpawnWarrior(nodeWarrior);

                        UnitBase unit = UnitManager.SpawnResource(
                            currentNode,
                            new List<TypeWork>() { TypeWork.EveryDay, TypeWork.EveryWeek }
                        );

                        area.Stat.countEveryResource++;

                        nodeWarrior.SetProtectedNeigbours(warrior, currentNode);

                        nodes.Remove(currentNode);

                        countCreated++;

                        List<GridTileNode> listExistExitNode = gridTileHelper.IsExistExit(currentNode);

                        if (listExistExitNode.Count > 1)
                        {
                            CreatePortal(currentNode, listExistExitNode);
                        }
                        //else
                        //{
                        //    Debug.Log($"NoExit::: keyArea{currentNode.keyArea}[{currentNode._position}]- Null");
                        //}

                    } else
                    {
                        nodes.Remove(currentNode);
                        // break;
                    }

                    timer += Time.deltaTime;

                    if (timer > 1f)
                    {
                        UIMenu.SetProgressValue(((float)countCreated / (float)maxCountResource) * 100f);
                        yield return null;
                        timer = 0;
                    }
                }

            }
        }

        stopWatch.Stop();
        System.TimeSpan timeTaken = stopWatch.Elapsed;
        Debug.Log($"EveryResource Stopwatch::: {timeTaken.ToString(@"m\:ss\.ffff")}");
    }

    private GridTileNode GetNodeWarrior(GridTileNode currentNode)
    {
        GridTileNode nodeWarrior = gridTileHelper.GridTile.getGridObject(currentNode._position + new Vector3Int(0, -1, 0));

        //if (currentNode.OccupiedUnit.typeInput == TypeInput.Down)
        //{
        //    return nodeWarrior;
        //}
        if (nodeWarrior == null || !nodeWarrior.Empty || nodeWarrior.Disable || nodeWarrior.Protected)
        {
            nodeWarrior = gridTileHelper.GridTile.getGridObject(currentNode._position + new Vector3Int(1, 0, 0));
        }
        if (nodeWarrior == null || !nodeWarrior.Empty || nodeWarrior.Disable || nodeWarrior.Protected)
        {
            nodeWarrior = gridTileHelper.GridTile.getGridObject(currentNode._position + new Vector3Int(-1, 0, 0));
        }

        if (nodeWarrior == null || !nodeWarrior.Empty || nodeWarrior.Disable || nodeWarrior.Protected)
        {
            nodeWarrior = gridTileHelper.GridTile.getGridObject(currentNode._position + new Vector3Int(0, 1, 0));
        }
        if (nodeWarrior == null || !nodeWarrior.Empty || nodeWarrior.Disable || nodeWarrior.Protected)
        {
            nodeWarrior = gridTileHelper.GridTile.getGridObject(currentNode._position + new Vector3Int(1, 1, 0));
        }
        if (nodeWarrior == null || !nodeWarrior.Empty || nodeWarrior.Disable || nodeWarrior.Protected)
        {
            nodeWarrior = gridTileHelper.GridTile.getGridObject(currentNode._position + new Vector3Int(1, -1, 0));
        }
        if (nodeWarrior == null || !nodeWarrior.Empty || nodeWarrior.Disable || nodeWarrior.Protected)
        {
            nodeWarrior = gridTileHelper.GridTile.getGridObject(currentNode._position + new Vector3Int(-1, 1, 0));
        }
        if (nodeWarrior == null || !nodeWarrior.Empty || nodeWarrior.Disable || nodeWarrior.Protected)
        {
            nodeWarrior = gridTileHelper.GridTile.getGridObject(currentNode._position + new Vector3Int(-1, -1, 0));
        }

        return nodeWarrior;
    }

    private IEnumerator CreateFreeResource()
    {
        UIMenu.SetProgressText("Create free resources");

        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        stopWatch.Start();

        float timer = 0;

        for (int x = 0; x < LevelManager.Instance.level.listArea.Count; x++)
        {
            Area area = LevelManager.Instance.level.listArea[x];

            List<GridTileNode> nodes = gridTileHelper.GetAllGridNodes().Where(t =>
                t.Empty
                && t.Enable
                && !t.Protected
                && t.keyArea == area.id
                && gridTileHelper.CalculateNeighbours(t) > 2
                && gridTileHelper.GetDistanceBetweeenPoints(t._position, area.startPosition) > 5
            ).OrderBy(t => Random.value).ToList();

            if (nodes.Count > 0)
            {
                int maxCountResource = Mathf.CeilToInt(LevelManager.Instance.gameModeData.koofFreeResource * area.countNode);

                area.Stat.countFreeResourceN = maxCountResource;

                int countCreated = 0;

                while (countCreated < maxCountResource && nodes.Count > 0)
                {
                    GridTileNode currentNode = nodes[Random.Range(0, nodes.Count)];

                    
                    UnitBase unit = UnitManager.SpawnResource(currentNode, new List<TypeWork>() { TypeWork.One });

                    nodes.Remove(currentNode);

                    countCreated++;

                    area.Stat.countFreeResource++;
                    //if (isWalk)
                    //{} else
                    //{
                    //    nodes.Remove(currentNode);
                    //}

                    List<GridTileNode> listExistExitNode = gridTileHelper.IsExistExit(currentNode);
                    //bool isWalk = true;
                    if (listExistExitNode.Count > 1)
                    {
                        CreatePortal(currentNode, listExistExitNode);
                    }
                    timer += Time.deltaTime;


                    if (timer > 1f)
                    {
                        UIMenu.SetProgressValue(((float)(countCreated) / (float)maxCountResource) * 100f);
                        yield return null;
                        timer = 0;
                    }
                }

            }
        }

        stopWatch.Stop();
        System.TimeSpan timeTaken = stopWatch.Elapsed;
        Debug.Log($"FreeResource Stopwatch::: {timeTaken.ToString(@"m\:ss\.ffff")}");
    }

    private IEnumerator CreateArtifacts()
    {
        UIMenu.SetProgressText("Create artifacts");

        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        stopWatch.Start();

        float timer = 0;

        for (int x = 0; x < LevelManager.Instance.level.listArea.Count; x++)
        {
            Area area = LevelManager.Instance.level.listArea[x];

            List<GridTileNode> nodes = gridTileHelper.GetAllGridNodes().Where(t =>
                t.Empty
                && t.Enable
                && !t.Protected
                && t.keyArea == area.id
                && gridTileHelper.CalculateNeighbours(t) < 2
                && gridTileHelper.GetDistanceBetweeenPoints(t._position, area.startPosition) > 10
            ).OrderBy(t => Random.value).ToList();

            if (nodes.Count > 0)
            {
                int maxCount = Mathf.CeilToInt(LevelManager.Instance.gameModeData.koofArtifacts * area.countNode);

                area.Stat.countArtifactN = maxCount;

                int countCreated = 0;

                while (countCreated < maxCount && nodes.Count > 0)
                {
                    GridTileNode currentNode = nodes[Random.Range(0, nodes.Count)];
                    GridTileNode nodeWarrior = GetNodeWarrior(currentNode);

                    if (nodeWarrior != null
                        && nodeWarrior.Empty
                        && currentNode != null
                        //&& gridTileHelper.CalculateNeighbours(currentNode) < 3
                        )
                    {
                        UnitBase unit = UnitManager.SpawnMapObjectToPosition(currentNode, MapObjectType.Artifact);

                        BaseWarriors warrior = (BaseWarriors)UnitManager.SpawnWarrior(nodeWarrior);

                        nodeWarrior.SetProtectedNeigbours(warrior, currentNode);

                        nodes.Remove(currentNode);

                        countCreated++;

                        area.Stat.countArtifact++;

                        List<GridTileNode> listExistExitNode = gridTileHelper.IsExistExit(currentNode);
                        if (listExistExitNode.Count > 1)
                        {
                            CreatePortal(currentNode, listExistExitNode);
                        }

                        timer += Time.deltaTime;

                    } else
                    {
                        nodes.Remove(currentNode);
                    }

                    if (timer > 1f)
                    {
                        UIMenu.SetProgressValue(((float)countCreated / (float)maxCount) * 100f);
                        yield return null;
                        timer = 0;
                    }
                }

            }
        }

        stopWatch.Stop();
        System.TimeSpan timeTaken = stopWatch.Elapsed;
        Debug.Log($"Artifacts Stopwatch::: {timeTaken.ToString(@"m\:ss\.ffff")}");
    }

    private IEnumerator CreateLandscape()
    {
        UIMenu.SetProgressText("Landscape");

        for (int x = 0; x < LevelManager.Instance.level.listArea.Count; x++)
        {
            Area area = LevelManager.Instance.level.listArea[x];

            List<GridTileNode> nodes = gridTileHelper.GetAllGridNodes().Where(t =>
                t.Empty
                && t.Enable
                && !t.Road
                && !t.Protected
                && t.keyArea == area.id
                && gridTileHelper.CalculateNeighbours(t) > 6
            ).OrderBy(t => Random.value).ToList();
            if (nodes.Count > 0)
            {
                int countLandscape = Mathf.CeilToInt(LevelManager.Instance.gameModeData.koofNature * nodes.Count * .1f);
                //Debug.LogWarning($"count lands={countLandscape}");
                int countCreated = 0;
                //for (int i = 0; i < countLandscape; i++)
                while(countCreated < countLandscape && nodes.Count > 0)
                {
                    GridTileNode currentNode = nodes[0];
                    if (currentNode != null && gridTileHelper.CalculateNeighbours(currentNode) > 6)
                    {
                        //TileBase _tile = _tileMap.GetTile(tileNode._position);
                        TileLandscape _tileData = _dataTypeGround[currentNode.typeGround];
                        List<TileNature> listNature = ResourceSystem.Instance.GetNature().Where(t =>
                            t.typeGround == _tileData.typeGround
                            && !t.isCorner
                        ).ToList().ToList();
                        TileNature tileForDraw = listNature[Random.Range(0, listNature.Count)]; // _tileData.natureTiles[Random.Range(0, _tileData.natureTiles.Count)];

                        _tileMapNature.SetTile(currentNode._position, tileForDraw);

                        _listNatureNode.Add(new GridTileNatureNode(currentNode, tileForDraw.idObject, false, tileForDraw.name));

                        SetDisableNode(currentNode, tileForDraw.listTypeNoPath, Color.green);

                        nodes.Remove(currentNode);

                        countCreated++;
                    } else
                    {
                        nodes.Remove(currentNode);
                        // break;
                    }
                    //UIMenu.SetProgressValue(((countCreated) / (float)nodes.Count) * 100);
                    //yield return null;
                }

            }
            UIMenu.SetProgressValue(((float)x / (float)LevelManager.Instance.level.listArea.Count) * 100f);
            yield return null;
        }
        yield return null;
    }

    private IEnumerator CreateExplore()
    {
        UIMenu.SetProgressText("Explore");

        for (int x = 0; x < LevelManager.Instance.level.listArea.Count; x++)
        {
            Area area = LevelManager.Instance.level.listArea[x];

            List<GridTileNode> nodes = gridTileHelper.GetAllGridNodes().Where(t =>
                t.Empty
                && t.Enable
                && !t.Road
                && !t.Protected
                && t.keyArea == area.id
                && gridTileHelper.CalculateNeighbours(t) == 8
                && gridTileHelper.GetDistanceBetweeenPoints(t._position, area.startPosition) > 10
            ).OrderBy(t => Random.value).ToList();

            if (nodes.Count > 0)
            {
                int maxCountExplore = Mathf.CeilToInt(LevelManager.Instance.gameModeData.koofExplore * area.countNode);
                area.Stat.countExploreN = maxCountExplore;
                int countCreated = 0;

                while (countCreated < maxCountExplore && nodes.Count > 0)
                {
                    GridTileNode currentNode = nodes[Random.Range(0, nodes.Count)];
                    GridTileNode nodeWarrior = GetNodeWarrior(currentNode);

                    if (nodeWarrior != null
                        && nodeWarrior.Empty
                        && currentNode != null
                        && gridTileHelper.CalculateNeighbours(currentNode) == 8
                        )
                    {
                        UnitBase unit = UnitManager.SpawnMapObjectToPosition(currentNode, MapObjectType.Explore);

                        BaseWarriors warrior = (BaseWarriors)UnitManager.SpawnWarrior(nodeWarrior);
                        
                        nodeWarrior.SetProtectedNeigbours(warrior, currentNode);

                        nodes.Remove(currentNode);

                        countCreated++;

                        area.Stat.countExplore++;

                        List<GridTileNode> listExistExitNode = gridTileHelper.IsExistExit(currentNode);
                        if (listExistExitNode.Count > 1)
                        {
                            CreatePortal(currentNode, listExistExitNode);
                        }
                    }
                    else
                    {
                        nodes.Remove(currentNode);
                    }

                    //UIMenu.SetProgressValue((countCreated / maxCountExplore) * 100f);
                    //yield return null;
                }

            }
            UIMenu.SetProgressValue(((float)x / (float)LevelManager.Instance.level.listArea.Count) * 100f);
            yield return null;
        }
    }

    private IEnumerator CreateSkillSchool()
    {
        UIMenu.SetProgressText("SkillSchool");

        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        stopWatch.Start();

        float timer = 0;

        for (int x = 0; x < LevelManager.Instance.level.listArea.Count; x++)
        {
            Area area = LevelManager.Instance.level.listArea[x];

            List<GridTileNode> nodes = gridTileHelper.GetAllGridNodes().Where(t =>
                t.Empty
                && t.Enable
                && !t.Road
                && !t.Protected
                && t.keyArea == area.id
                && gridTileHelper.CalculateNeighbours(t) == 8
                && gridTileHelper.GetDistanceBetweeenPoints(t._position, area.startPosition) > 10
            ).OrderBy(t => Random.value).ToList();

            if (nodes.Count > 0)
            {
                int maxCountSchool = Mathf.CeilToInt(LevelManager.Instance.gameModeData.koofSchoolSkills * area.countNode);
                area.Stat.countSkillSchoolN = maxCountSchool;
                int countCreated = 0;

                while (countCreated < maxCountSchool && nodes.Count > 0)
                {
                    GridTileNode currentNode = nodes[Random.Range(0, nodes.Count)];

                    GridTileNode nodeWarrior = GetNodeWarrior(currentNode);

                    if (nodeWarrior != null && currentNode != null && gridTileHelper.CalculateNeighbours(currentNode) == 8)
                    {
                        UnitBase unit = UnitManager.SpawnMapObjectToPosition(currentNode, MapObjectType.SkillSchool);
                        
                        BaseWarriors warrior = (BaseWarriors)UnitManager.SpawnWarrior(nodeWarrior);

                        nodeWarrior.SetProtectedNeigbours(warrior, currentNode);

                        nodes.Remove(currentNode);

                        countCreated++;

                        area.Stat.countSkillSchool++;

                        List<GridTileNode> listExistExitNode = gridTileHelper.IsExistExit(currentNode);
                        if (listExistExitNode.Count > 1)
                        {
                            CreatePortal(currentNode, listExistExitNode);
                        }

                    }
                    else
                    {
                        nodes.Remove(currentNode);
                        //break;
                    }

                    timer += Time.deltaTime;

                    if (timer > 1f)
                    {
                        UIMenu.SetProgressValue(((float)countCreated / (float)maxCountSchool) * 100f);
                        yield return null;
                        timer = 0;
                    }
                }

            }
        }
        stopWatch.Stop();
        System.TimeSpan timeTaken = stopWatch.Elapsed;
        Debug.Log($"SkillSchool Stopwatch::: {timeTaken.ToString(@"m\:ss\.ffff")}");
    }

    private bool CreatePortal(GridTileNode node, List<GridTileNode> potentialNode)
    {
        List<GridTileNode> _potentialNode = potentialNode.Where(t =>
            t != node
            && gridTileHelper.CalculateNeighbours(t) > 4
            && t.keyArea == node.keyArea
            && t.Empty
            && t.Enable
        ).ToList();
        if (_potentialNode.Count > 0)
        {
            GridTileNode nodeExitPortal = _potentialNode[Random.Range(0, _potentialNode.Count - 1)];

            Area currentArea = LevelManager.Instance.GetArea(nodeExitPortal.keyArea);
            // If town is not exists for area, get random town.
            if (currentArea.town == null)
            {
                // Debug.LogWarning($"Area [{currentArea.id}] not town!");
                currentArea = LevelManager.Instance.level.listArea.Where(t => t.town != null).OrderBy(t => Random.value).First();
            }

            GridTileNode townNode = gridTileHelper.GetNode(currentArea.startPosition.x, currentArea.startPosition.y);

            BaseMonolith monolith = (BaseMonolith)currentArea.portal;

            if (monolith == null)
            {
                List<GridTileNode> listAroundTownNodes = gridTileHelper.IsExistExit(townNode);
                List<GridTileNode> listPotentialAroundTownNodes = listAroundTownNodes.Where(t =>
                    t != node
                    //&& gridTileHelper.CalculateNeighbours(t) > 4
                    && t.keyArea == node.keyArea
                    && t.Empty
                    && t.Enable
                    && !t.Protected
                    && t != townNode
                    && gridTileHelper.GetDistanceBetweeenPoints(t._position, townNode._position) > 3
                ).ToList();

                if (listPotentialAroundTownNodes.Count > 0)
                {

                    GridTileNode nodeInputPortal = listPotentialAroundTownNodes[Random.Range(0, listPotentialAroundTownNodes.Count - 1)];

                    GridTileNode nodeWarrior = GetNodeWarrior(nodeInputPortal);
                    if (nodeWarrior != null)
                    {
                        monolith = (BaseMonolith)UnitManager.SpawnUnitByTypeUnit(nodeInputPortal, TypeUnit.Monolith);
                        currentArea.portal = monolith;

                        BaseWarriors warrior = (BaseWarriors)UnitManager.SpawnWarrior(nodeWarrior);

                        nodeWarrior.SetProtectedNeigbours(warrior, nodeInputPortal);
                    }
                    //MonolithData monolithData = new MonolithData();
                    //monolithData.position = nodeForEndPortal._position;
                    //monolith.Init(monolithData);

                }
            }

            if (monolith != null)
            {
                BaseMonolith monolithExit = (BaseMonolith)UnitManager.SpawnUnitByTypeUnit(nodeExitPortal, TypeUnit.Monolith);

                GridTileNode nodeWarrior = GetNodeWarrior(nodeExitPortal);
                if (nodeWarrior != null)
                {
                    BaseWarriors warrior = (BaseWarriors)UnitManager.SpawnWarrior(nodeWarrior);

                    nodeWarrior.SetProtectedNeigbours(warrior, nodeExitPortal);
                }
                //MonolithData monolithExitData = new MonolithData();
                //monolithExitData.position = monolithExit.Position;
                //monolithExit.Init(monolithExitData);

                monolith.Data.portalPoints.Add(monolithExit.Position);
                return true;
            }
        } else
        {
            return false;
        }
        return false;
    }

    private IEnumerator CreateRoads()
    {
        UIMenu.SetProgressText("Roads");
        //List<Area> area = LevelManager.Instance.level.countArea;
        //Dictionary<UnitBase, Vector3Int> portals = UnitManager.Instance._portalList;
        List<Area> listArea = LevelManager.Instance.level.listArea.Where(t => 
            t.portal != null
            || t.town != null
        ).ToList();
        for (int i = 0; i < listArea.Count - 1; i++)
        {

            Area area = listArea[i];
            Area areaNext = listArea[i + 1];

            yield return StartCoroutine(onDrawRoad(
                new Vector3Int(area.startPosition.x, area.startPosition.y, 0),
                new Vector3Int(areaNext.startPosition.x, areaNext.startPosition.y, 0)
            ));

            if (listArea[i].portal != null && listArea[i].town != null)
            {
                yield return StartCoroutine(onDrawRoad(
                    new Vector3Int(listArea[i].town.Position.x, listArea[i].town.Position.y, 0),
                    new Vector3Int(listArea[i].portal.Position.x, listArea[i].portal.Position.y, 0)
                ));
            }
            if (listArea[i + 1].portal != null && listArea[i + 1].town != null)
            {
                // TODO None town.
                yield return StartCoroutine(onDrawRoad(
                    new Vector3Int(listArea[i + 1].town.Position.x, listArea[i + 1].town.Position.y, 0),
                    new Vector3Int(listArea[i + 1].portal.Position.x, listArea[i + 1].portal.Position.y, 0)
                ));
            }
            UIMenu.SetProgressValue(((float)i / (float)listArea.Count) * 100f);
        }

        Area areaFirst = listArea[0];
        Area areaLast = listArea[listArea.Count - 1];
        yield return StartCoroutine(onDrawRoad(
            new Vector3Int(areaFirst.startPosition.x, areaFirst.startPosition.y, 0),
            new Vector3Int(areaLast.startPosition.x, areaLast.startPosition.y, 0)
        ));
        if (listArea[0].portal != null && listArea[0].town != null)
        {
            yield return StartCoroutine(onDrawRoad(
                new Vector3Int(listArea[0].town.Position.x, listArea[0].town.Position.y, 0),
                new Vector3Int(listArea[0].portal.Position.x, listArea[0].portal.Position.y, 0)
            ));
        }
    }

    private IEnumerator onDrawRoad(Vector3Int start, Vector3Int end)
    {
        if (start == null || end == null) yield break;

        List<GridTileNode> path = gridTileHelper.FindPath(start, end, true, true);

        if (path != null)
        {

            for (int i = 0; i < path.Count - 1; i++)
            {
                GridTileNode pathNode = path[i];
                GridTileNode pathNodeNext = path[i + 1];
                Vector3Int pos = new Vector3Int(pathNode.x, pathNode.y, 0);
                _tileMapRoad.SetTile(pos, _tileRoad.tileRule);
                pathNode.SetAsRoad();
                //_tileMap.SetColor(pos, Color.yellow);
                //if (pathNode.countRelatedNeighbors < 2)
                //{
                //    UnitManager.Instance.SpawnMapObjectToPosition(pathNode, MapObjectType.Enemy);
                //    SetTextMeshNode(pathNode);
                //}


                //if (i == path.Count - 2)
                //{
                //    pathNode.SetWalkable(false);
                //    Vector3Int posLast = new Vector3Int(pathNodeNext.x, pathNodeNext.y, 0);
                //    _tileMapRoad.SetTile(posLast, _tileRoad.tileRule);
                //}

                //Debug.Log($"Grid {_map.GetGridObjectByXZ(i,i)}");

                //Debug.DrawLine(
                //    new Vector3(pathNode.x + (.5f * _cellSize), pathNode.z + (.5f * _cellSize), 0.05f),
                //    new Vector3(pathNodeNext.x + (.5f * _cellSize), pathNodeNext.z + (.5f * _cellSize), 0.05f),
                //    Color.white, 2f
                //    );
            }

            //print("Draw road  from " + start + " to " + end);

        } else
        {

            //GridTileNode startNode = gridTileHelper.GetNode(start.x, start.y);
            //GridTileNode endNode = gridTileHelper.GetNode(end.x, end.y);

            //GridTileNode nodeStartPortal = GetNodeForUnitSpawn(start, startNode.keyArea, 5);
            //GridTileNode nodeEndPortal = GetNodeForUnitSpawn(end, endNode.keyArea, 5);

            //// Spawn portal.
            //if (nodeStartPortal != null && nodeEndPortal != null)
            //{
            //    UnitManager.Instance.SpawnUnitByTypeUnit(nodeStartPortal, TypeUnit.Monolith);
            //    UnitManager.Instance.SpawnUnitByTypeUnit(nodeEndPortal, TypeUnit.Monolith);
            //} else
            //{
            //    Debug.Log($"Not found node for Monolith start {startNode.keyArea} end {endNode.keyArea}");
            //}
        }

        yield return null;
    }

    //public void SetSlowNode(GridTileNode node)
    //{
    //    List<GridTileNode> listNeighbors = gridTileHelper.GetNeighbourList(node, true);
    //    foreach (GridTileNode tileNode in listNeighbors)
    //    {
    //        if (!tileNode.Walkable) continue;
    //        tileNode.koofPath = 20;
    //    }
    //}

    //public void SetProtectedNodes(GridTileNode warriorNode, UnitBase warriorUnit)
    //{
    //    List<GridTileNode> nodes = gridTileHelper.GetNeighbourList(warriorNode, true);

    //    warriorNode.SetIsWarrior(warriorUnit);

    //    if (nodes == null || nodes.Count == 0) return;

    //    for (int i = 0; i < nodes.Count; i++)
    //    {
    //        nodes[i].SetIsWarrior(warriorUnit);
    //        SetColorForTile(nodes[i]._position, Color.red);
    //    }
    //}

    public void SetDisableNode(GridTileNode node, List<TypeNoPath> listNoPath, Color color)
    {
        color = color == null ? Color.black : color;

        node.SetState(TypeStateNode.Disabled);
        //node.countRelatedNeighbors = 0;
        SetColorForTile(node._position, color);
        //Color colorc = Color.magenta;
        //colorc.a = 0.2f;
        //SetTextMeshNode(node);
        //ChangeCountNeighbors(node);
        // SetSlowNode(node);
        if (listNoPath == null) return;

        if (listNoPath.Count > 0)
        {
            List<GridTileNode> list = GetNodeListAsNoPath(node, listNoPath);
            if (list.Count > 0)
            {
                foreach (GridTileNode nodePath in list)
                {
                    SetDisableNode(nodePath, null, color);
                    // nodesCreated.Add(node);
                }
            }
        }
    }

//    public void ChangeCountNeighbors(GridTileNode node)
//    {
//        List<GridTileNode> listNeighbors = gridTileHelper.GetNeighbourList(node, true);
//        foreach (GridTileNode tileNode in listNeighbors)
//        {
//            if (!tileNode.Walkable) continue;
//            if (tileNode.countRelatedNeighbors > 0) tileNode.countRelatedNeighbors--;
//#if UNITY_EDITOR
//            if (tileNode.Walkable && tileNode.countRelatedNeighbors != 0)
//            {
//                //SetColorForTile(tileNode._position, Color.yellow);
//                //Color a = Color.magenta;
//                //a.a = tileNode.countRelatedNeighbors * .2f + .2f;
//                //SetColorForTile(tileNode._position, a);

//            }
//            //SetTextMeshNode(tileNode);
//#endif

//        }
//        if (node.countRelatedNeighbors > 0) node.countRelatedNeighbors = 0;
//        //SetTextMeshNode(node);
//    }

    public List<GridTileNode> GetNodeListAsNoPath(GridTileNode node, List<TypeNoPath> listNoPath)
    {
        Vector3Int pos = node._position;
        List <GridTileNode> list = new List<GridTileNode> ();
        for (int i = 0; i < listNoPath.Count; i++)
        {
            Vector3Int noPathPos = new Vector3Int(pos.x, pos.y);
            switch (listNoPath[i])
            {
                case TypeNoPath.Top:
                    noPathPos.y += 1;
                    break;
                case TypeNoPath.Left:
                    noPathPos.x -= 1;
                    break;
                case TypeNoPath.Right:
                    noPathPos.x += 1;
                    break;
                case TypeNoPath.RightTop:
                    noPathPos.x += 1;
                    noPathPos.y += 1;
                    break;
                case TypeNoPath.LeftTop:
                    noPathPos.x -= 1;
                    noPathPos.y += 1;
                    break;
                case TypeNoPath.RightBottom:
                    noPathPos.x += 1;
                    noPathPos.y -= 1;
                    break;
                case TypeNoPath.Right2Bottom:
                    noPathPos.x += 2;
                    noPathPos.y -= 1;
                    break;
                case TypeNoPath.Left2Bottom:
                    noPathPos.x -= 2;
                    noPathPos.y -= 1;
                    break;
                case TypeNoPath.LeftBottom:
                    noPathPos.x -= 1;
                    noPathPos.y -= 1;
                    break;
                case TypeNoPath.Bottom:
                    noPathPos.y -= 1;
                    break;
                case TypeNoPath.Top2:
                    noPathPos.y += 2;
                    break;
                case TypeNoPath.Left2:
                    noPathPos.x -= 2;
                    break;
                case TypeNoPath.Right2:
                    noPathPos.x += 2;
                    break;
                case TypeNoPath.Right2Top:
                    noPathPos.x += 2;
                    noPathPos.y += 1;
                    break;
                case TypeNoPath.Left2Top:
                    noPathPos.x -= 2;
                    noPathPos.y += 1;
                    break;
                case TypeNoPath.Right2Top2:
                    noPathPos.x += 2;
                    noPathPos.y += 2;
                    break;
                case TypeNoPath.Left2Top2:
                    noPathPos.x -= 2;
                    noPathPos.y += 2;
                    break;
                case TypeNoPath.LeftTop2:
                    noPathPos.x -= 1;
                    noPathPos.y += 2;
                    break;
                case TypeNoPath.RightTop2:
                    noPathPos.x += 1;
                    noPathPos.y += 2;
                    break;
            }

            if (noPathPos.x >= 0 && noPathPos.x < gameModeData.width && noPathPos.y >= 0 && noPathPos.y < gameModeData.height)
            {
                GridTileNode noPathTile = gridTileHelper.GridTile.getGridObject(noPathPos); // GetMapObjectByPosition(noPathPos.x, noPathPos.y);
                list.Add(noPathTile);
                ////if (
                ////    node.typeGround == noPathTile.typeGround &&
                ////    node.keyArea == noPathTile.keyArea
                ////    )
                ////{
                //noPathTile.SetWalkable(false);
                //noPathTile._noPath = true;
                //ChangeCountNeighbors(node);
                //SetColorForTile(noPathPos, Color.magenta);
                ////}
            }
        }
        return list;
    }

    //public GridTileNode GetTileForUnitSpawn(int level, int keyArea)
    //{

    //    var elem = gridTileHelper.GetAllGridNodes().Where(t => t.Walkable && t.level == level && t.keyArea == keyArea);

    //    GridTileNode tileNode = elem.OrderBy(t => Random.value).First();

    //    //List<GridTileNode> listNeighbors = gridTileHelper.GetNeighbourList(tileNode);
    //    //while (
    //    //    listNeighbors.Count != 4
    //    //    )
    //    //{
    //    //    gridTileHelper.GetAllGridNodes().Where(t => t.Walkable && t.level == level++ && t.keyArea == keyArea).First();

    //    //    tileNode = elem.OrderBy(t => Random.value).First();
    //    //}

    //    return tileNode;
    //}


    //public Vector3Int GetPosForUnitSpawn()
    //{
    //    var elem = _map.Where(t => t.Value.Walkable);

    //    Vector3Int baseTile = elem.Count() > 0 ? elem.OrderBy(t => Random.value).First().Key : Vector3Int.zero;

    //    return baseTile;
    //}

    //public GridTileNode GetNodeForUnitSpawn(Vector3Int pos, int keyArea, float minDistance = 3f, float maxDistance = 100f)
    //{
    //    var listNodes = gridTileHelper.GetAllGridNodes().Where(t =>
    //        t.keyArea == keyArea
    //        && t.Empty
    //        && t.Enable
    //        && gridTileHelper.GetDistanceBetweeenPoints(t._position, pos) >= minDistance
    //        && gridTileHelper.GetDistanceBetweeenPoints(t._position, pos) <= maxDistance
    //        && gridTileHelper.CalculateNeighboursByArea(t) > 5
    //       //t.level > minLevel &&
    //       //t.level < maxlevel
    //       ).ToList();
    //    GridTileNode nodeForSpawn = listNodes.OrderBy(t => Random.value).First();
    //    //while (nodeForSpawn == null || listNodes.Count > 0) {
    //    //    if (listNodes.Count != 0) {
    //    //        GridTileNode nodeTest = listNodes.OrderBy(t => Random.value).First();
    //    //        listNodes.Remove(nodeTest);
    //    //        List<GridTileNode> listNeigbours = gridTileHelper.GetNeighbourList(nodeTest, true);
    //    //        if (
    //    //            listNeigbours.Where(t => t.Empty).Count() == 8
    //    //            //&&
    //    //            //gridTileHelper.FindPath(nodeTest._position, pos, false, true).Count() > 0
    //    //            ) {
    //    //            nodeForSpawn = nodeTest;
    //    //            }
    //    //    } else
    //    //    {
    //    //        break;
    //    //    }
    //    //}
    //    return nodeForSpawn; //elem.Count() > 0 ? elem.OrderBy(t => Random.value).First() : null;
    //    //if (elem.Count() > 0)
    //    //{


    //    //    List<GridTileNode> listNodes = new List<GridTileNode>();
    //    //    for (int i = 0; i < countReturnValue; i++)
    //    //    {
    //    //        listNodes.Add(elem.ElementAt(Random.Range(0, elem.Count())));
    //    //    }
    //    //    return listNodes;
    //    //} else { return null; }
    //}
    //private void CreateTile(int tileId, int x, int y)
    //{
    //    Vector3Int pos = new Vector3Int(x, y, 0);

    //    TileData tile = tileset[tileId];
    //    _tileMap.SetTile(pos, tile.tileRule);
    //    BaseTile baseTile = new BaseTile(tile, pos);
    //    _map.Add(pos, baseTile);

    //    if (baseTile.Walkable)
    //    {
    //        //SetColorForTile(pos, Color.green);
    //        //_listWalkableTile.Add(pos, baseTile);
    //    }


    //}


    //public TileLandscape GetTileData(Vector3Int tilePosition)
    //{
    //    TileBase tile = _tileMap.GetTile(tilePosition);

    //    if (tile == null)
    //        return null;
    //    else
    //        return _dataFromTiles[tile];

    //}


    /// <summary>
    /// Get service path finding.
    /// </summary>
    /// <returns>PathFinding</returns>
    public GridTileHelper GridTileHelper()
    {
        return gridTileHelper;
    }

    //public Tilemap GetTileMap()
    //{
    //    return _tileMap;
    //}

    //public TileBase GetTileAtPosition(Vector2 pos)
    //{
    //    Vector3Int tilePos = _tileMap.WorldToCell(pos);

    //    return _tileMap.GetTile(tilePos);

    //}

    //public GridTileNode GetMapObjectByPosition(int x, int y)
    //{
    //    //Debug.Log($"Find plane: [{x},0,{z}]");
    //    if (x >= 0 && x < _width && y >= 0 && y < _height)
    //    {
    //        Vector3Int pos = new Vector3Int(x, y, 0);
    //        return gridTileHelper.GetGridTile().getGridObject(pos);
    //    } else
    //    {
    //        return null;
    //    }
    //}
    //public TileLandscape GetTileDataByPos(Vector3Int pos)
    //{
    //    TileBase tileBase = _tileMap.GetTile(pos);

    //    return _dataFromTiles[tileBase];
    //}


    
    public void ChangePath()
    {
        //Vector2 posMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Ray hit = Camera.main.ScreenPointToRay(posMouse);

        Vector2 posMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tilePos = _tileMap.WorldToCell(posMouse);

        //Ray ray = Camera.main.ScreenPointToRay(posMouse);
        //RaycastHit2D hit2d = Physics2D.GetRayIntersection(ray);
        //Collider2D colliderHit = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        //if (hit2d.collider != null)
        //{
        //    Debug.Log($"Hit 2D click {hit2d.collider} colliderHit = {colliderHit}");
        //}


        //if (Physics.Raycast(posMouse, Vector2.down, Mathf.Infinity))
        //{
        //    Debug.Log($"Click tile");
        //}
        TileBase clickedTile = _tileMap.GetTile(tilePos);
        GridTileNode node = gridTileHelper.GetNode(tilePos.x, tilePos.y);

        if (clickedTile != null && !node.Disable)
        {
            if (!node.OccupiedUnit || node.Protected) {

                //Debug.Log($"Click unit={node.OccupiedUnit?.name}, warrior={node.ProtectedUnit?.name}");
                //List<GridTileNode> paths = 
                //if (node.WarriorUnit != null)
                //{
                //    LevelManager.Instance.ActivePlayer.FindPathForHero(node.WarriorUnit.Position, false, true);

                //} else
                //{
                LevelManager.Instance.ActivePlayer.FindPathForHero(tilePos, false, true);
                //}
                //// gridTileHelper.FindPath(tilePos, false, true);
                //if (paths != null)
                //{
 
                //    UnitManager.Instance.ChangePathForHero(paths);

                //    //for (int i = 0; i < paths.Count - 1; i++)
                //    //{
                //    //    GridTileNode pathNode = paths[i];
                //    //    GridTileNode pathNodeNext = paths[i + 1];

                //    //    //Debug.Log($"Grid {_map.GetGridObjectByXZ(i,i)}");

                //    //    Debug.DrawLine(
                //    //        new Vector3(pathNode.x + .5f, pathNode.y + .5f, 0),
                //    //        new Vector3(pathNodeNext.x + .5f, pathNodeNext.y + .5f, 0),
                //    //        Color.white, .5f
                //    //        );
                //    //    SetColorForTile(pathNode._position, Color.cyan);
                //    //    //_player.transform.position = new Vector3(pathNodeNext.x, _player.transform.position.y, pathNodeNext.z);
                //    //    // _player.GetComponent<Rigidbody>().MovePosition(new Vector3(pathNodeNext.x, _player.transform.position.y, pathNodeNext.z));
                //    //}
                //}
            }

        }

        Debug.Log($"Click {clickedTile.ToString()} \n {node.ToString()}");
    }


    public void DrawCursor(List<GridTileNode> paths, Hero hero)
    {
        _tileMapCursor.ClearAllTiles();
        
        if (paths.Count == 0) { return; }

        float countDistance = hero.Data.hit;
        //Debug.Log($"hero.HeroData.hit.Value={hero.HeroData.hit.Value}");

        for (int i = 0; i < paths.Count - 1; i++)
        {
            GridTileNode node = paths[i];
            GridTileNode nodeNext = paths[i + 1];
            GridTileNode nodePrev = i > 0 ? paths[i - 1] : null;
            //_tileMapCursor.SetTile(nodeNext._position, _cursorRule);
            //continue;

            TileBase _tile = null;

            if (node.x > nodeNext.x)
            {
                // left
                _tile = _cursorSprites.left;
                if (nodePrev != null && node.y != nodePrev.y)
                {
                    if (node.y > nodePrev.y)
                    {
                        _tile = _cursorSprites.HtoDBottomLeft;

                    }
                    else
                    {
                        _tile = _cursorSprites.HtoDTopLeft;
                    }
                }

            }
            else if (node.x < nodeNext.x)
            {
                // right
                _tile = _cursorSprites.right;
                if (nodePrev != null && node.y != nodePrev.y)
                {
                    if (node.y > nodePrev.y)
                    {
                        _tile = _cursorSprites.HtoDBottomRight;

                    }
                    else
                    {
                        _tile = _cursorSprites.HtoDTopRight;
                    }
                }

            }
            else if (node.y < nodeNext.y)
            {
                // top
                _tile = _cursorSprites.top;
                if (nodePrev != null && node.x != nodePrev.x)
                {
                    if (node.x > nodePrev.x)
                    {
                        _tile = _cursorSprites.VtoDTopRight;

                    }
                    else
                    {
                        _tile = _cursorSprites.VtoDTopLeft;
                    }
                }

            }
            else if (node.y > nodeNext.y)
            {
                // bottom
                _tile = _cursorSprites.bottom;
                if (nodePrev != null && node.x != nodePrev.x)
                {
                    if (node.x > nodePrev.x)
                    {
                        _tile = _cursorSprites.VtoDBottomLeft;

                    }
                    else
                    {
                        _tile = _cursorSprites.VtoDBottomRight;
                    }
                }

            }

            if (node.x > nodeNext.x && node.y > nodeNext.y)
            {
                //if (nodePrev != null && node.x == nodePrev.x)
                //{
                //    _tile = _cursorSprites.HtoDBottomLeft;
                //} else
                //{
                //    _tile = _cursorSprites.cornerLeftBottom;
                //}
                _tile = _cursorSprites.cornerLeftBottom;
            } else if (node.x < nodeNext.x && node.y < nodeNext.y)
            {
                _tile = _cursorSprites.cornerRightTop;
            }
            else if (node.x < nodeNext.x && node.y > nodeNext.y)
            {
                _tile = _cursorSprites.cornerRightBottom;
            }
            else if (node.x > nodeNext.x && node.y < nodeNext.y)
            {
                _tile = _cursorSprites.cornerLeftTop;
            }


            if (_tile != null)
            {
                
                //_tile.transform = Matrix4x4.Translate(new Vector3(0,0,0)) * Matrix4x4.Scale(new Vector3(0, -1, 0));
                _tileMapCursor.SetTile(node._position, _tile);
                if (countDistance < 0) {
                    SetColorCursor(node._position, Color.red);
                }

                countDistance -= hero.CalculateHitByNode(node);
            }

            if (i == paths.Count - 2)
            {
                //newTile = _cursorSprites.center;
                if (nodeNext.ProtectedUnit)
                {
                    _tileMapCursor.SetTile(nodeNext._position, _cursorSprites.Attack);

                }
                else if (nodeNext.OccupiedUnit)
                {
                    _tileMapCursor.SetTile(nodeNext._position, _cursorSprites.GoMapObject);

                } else
                {
                    _tileMapCursor.SetTile(nodeNext._position, _cursorSprites.center);
                }

                if (countDistance < 0)
                {
                    SetColorCursor(nodeNext._position, Color.red);
                }
                countDistance -= hero.CalculateHitByNode(node);
            }


        }
    }

    private void SetColorCursor(Vector3Int pos, Color color)
    {
        SetColorForTile(pos, color, _tileMapCursor);
    }

    //public BaseTile GetTileAtPosition(Vector2 pos)
    //{
    //    if (_tiles.TryGetValue(pos, out var tile))
    //    {
    //        return tile;
    //    };
    //    return null;
    //}



    public void SetColorForTile(Vector3Int pos, Color color, Tilemap __tileMap = null)
    {
        Tilemap tileMap = _tileMap;

        if (__tileMap != null)
        {
            tileMap = __tileMap;
        }
        //TileBase _tile = tileMap.GetTile(pos);
        //TileData _tileData = _dataFromTiles[_tile];
        //Debug.Log($"SetUnitOnTile: {_tileData.color}");
        tileMap.SetTileFlags(pos, TileFlags.None);
        tileMap.SetColor(pos, color);
        tileMap.SetTileFlags(pos, TileFlags.LockColor);
    }
    public List<GridTileNode> DrawSky(GridTileNode node, int distance)
    {
        List<GridTileNode> listNode = gridTileHelper.GetNeighboursAtDistance(node, distance);
        for (int i = 0; i < listNode.Count; i++)
        {
            _tileMapSky.SetTile(listNode[i]._position, null);
        }
        return listNode;
    }

    public void ResetSky(SerializableShortPosition positions)
    {
        for (int x = 0; x < gameModeData.width; x++)
        {
            for (int y = 0; y < gameModeData.height; y++)
            {
                Vector3Int position = new Vector3Int(x, y);
                if (positions.ContainsKey(position))
                {
                    _tileMapSky.SetTile(position, null);
                } else
                {
                    _tileMapSky.SetTile(position, _tileSky);
                }
            }
        }
    }

    public void SetTextMeshNode(GridTileNode tileNode, string textString = "")
    {
        Vector3 posText = tileNode._position;
        GameObject text;
        if (!listTextMesh.TryGetValue(posText, out text))
        {
            text = Instantiate(_textMesh, posText, Quaternion.identity);
            text.transform.SetParent(_tileMapText.transform);
            listTextMesh.Add(posText, text);
        }
        text.GetComponent<TextMeshPro>().text = textString != "" ? textString : string.Format("nei: {0} l:{1} np: {2} w: {3}",
            tileNode.countRelatedNeighbors,
            tileNode.level,
            tileNode.Protected,
            tileNode.Empty
            );
    }


}
