using Cysharp.Threading.Tasks;

using Loader;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TMPro;

using UnityEngine;
using UnityEngine.Tilemaps;

// [System.Serializable]
// public class ResourceMap
// {
//     public List<ResourceMapItem> Resourcemap;
// }
// [System.Serializable]
// public struct ResourceMapItem
// {
//     public string id;
//     public List<ResourceMapItemVariantList> variants;
// }
// [System.Serializable]
// public struct ResourceMapItemVariantList
// {
//     public List<ResourceMapItemVariantItem> list;
//     public float probability;
// }
// [System.Serializable]
// public struct ResourceMapItemVariantItem
// {
//     public string idso;
//     public int value;
// }
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
public class MapManager : MonoBehaviour, ISaveDataGame, ILoadGame
{
    // public LocalizationManager LocalizationManager;
    [SerializeField] public DataGameMode gameModeData;
    private bool _isWater = false;
    public int countArea;
    public Tilemap _tileMap;
    public Tilemap _tileTest;
    [SerializeField] public GameObject _textMesh;
    [SerializeField] public Tilemap _tileMapText;
    [SerializeField] public Dictionary<Vector3, GameObject> listTextMesh = new Dictionary<Vector3, GameObject>();
    [SerializeField] public Tilemap BlokUnits;
    [Header("Cursor")]
    [Space(10)]
    [SerializeField] private Tilemap _tileMapCursor;

    [Header("Road setting")]
    [Space(10)]
    public Tilemap _tileMapSky;
    [SerializeField] public RuleTile _tileSky;

    [Header("Road setting")]
    [Space(10)]
    [SerializeField] Tilemap _tileMapRoad;
    [SerializeField] private TileLandscape _tileRoad;

    [Header("Nature settings")]
    [Space(10)]
    public Tilemap _tileMapNature;

    [Header("Walkable Nature settings")]
    [Space(10)]
    [SerializeField] TileBase _tileCreek;
    [SerializeField] Tilemap _tileMapWalkedNature;

    public Dictionary<TypeGround, TileLandscape> _dataTypeGround;
    public GridTileHelper gridTileHelper;
    public List<GridTileNatureNode> _listNatureNode = new List<GridTileNatureNode>();

    [Space(10)]
    [SerializeField] public Cursors _cursorSprites;

    public void SaveDataGame(ref DataGame data)
    {
        data.dataMap.mapNode = gridTileHelper?.GetAllGridNodes();
        data.dataMap.natureNode = _listNatureNode;
        //data.dataMap.GameModeData = gameModeData;
        data.dataMap.isWater = _isWater;
        data.dataMap.countArea = countArea;
    }

    public void LoadGameData(DataPlay dataPlay, DataGame dataGame)
    {
        LevelManager.Instance.LoadLevel(dataPlay, dataGame);

        gameModeData = dataGame.dataMap.GameModeData;
        _listNatureNode = dataGame.dataMap.natureNode;
        _isWater = dataGame.dataMap.isWater;
        countArea = dataGame.dataMap.countArea;

        // Clear all tilemap.
        InitSetting();


        // Create grid tile nodes.
        gridTileHelper = new GridTileHelper(gameModeData.width, gameModeData.height);

        for (int i = 0; i < dataGame.dataMap.mapNode.Count; i++)
        {
            //Debug.Log($"data map::: [count={data.dataMap.mapNode.Count}] { node.x} - {node.y}");
            GridTileNode node = gridTileHelper.GridTile
                .GetGridObject(new Vector3Int(dataGame.dataMap.mapNode[i].X, dataGame.dataMap.mapNode[i].Y));
            node.TypeGround = dataGame.dataMap.mapNode[i].TypeGround;
            node.KeyArea = dataGame.dataMap.mapNode[i].KeyArea;
            // node.State = dataGame.dataMap.mapNode[i].State;
            if (dataGame.dataMap.mapNode[i].StateNode.HasFlag(StateNode.Disable))
            {
                node.StateNode = dataGame.dataMap.mapNode[i].StateNode;
            }

            if (dataGame.dataMap.mapNode[i].StateNode.HasFlag(StateNode.Road))
            {
                _tileMapRoad.SetTile(node.position, _tileRoad.tileRule);
                node.SetAsRoad();
            }
        }

        for (int x = 0; x < gameModeData.width; x++)
        {
            for (int y = 0; y < gameModeData.height; y++)
            {

                GridTileNode tileNode = gridTileHelper.GridTile.GetGridObject(new Vector3Int(x, y));

                RuleTile drawRule = _dataTypeGround[tileNode.TypeGround].tileRule;

                Vector3Int pos = new Vector3Int(x, y, 0);

                _tileMap.SetTile(pos, drawRule);
                //tileNode.SetState(true);
            }
        }

        setSizeTileMap();

        for (int i = 0; i < dataGame.dataMap.natureNode.Count; i++)
        {
            GridTileNode tileNode = gridTileHelper.GridTile
                .GetGridObject(new Vector3Int(dataGame.dataMap.natureNode[i].x, dataGame.dataMap.natureNode[i].y));

            //Debug.Log($"GetNature: [{i}-{data.dataMap.natureNode[i].n}-{data.dataMap.natureNode[i].idNature}]");
            if (dataGame.dataMap.natureNode[i].idNature == "creek")
            {

                _tileMapWalkedNature.SetTile(tileNode.position, _tileCreek);
                continue;
            }
            TileNature natureTile = ResourceSystem.Instance.GetNature(dataGame.dataMap.natureNode[i].idNature);
            if (natureTile == null)
            {
                Debug.Log($"None resource for tile nature: [{i}-{dataGame.dataMap.natureNode[i].idNature}]");
                continue;
            }
            _tileMapNature.SetTile(tileNode.position, natureTile);
            //_tileMap.SetTile(tileNode._position, _tileBg.tileRule);
        }

        // Spawn town.
        foreach (SaveDataUnit<DataTown> unitTown in dataPlay.entity.towns)
        {
            GridTileNode tileNode = gridTileHelper.GridTile.GetGridObject(new Vector3Int(unitTown.position.x, unitTown.position.y));

            if (unitTown.idObject == "") continue;

            EntityTown town = new EntityTown(TypeGround.None, unitTown);
            UnitManager.SpawnEntityMapObjectToNode(tileNode, town);
            if (unitTown.data.idPlayer >= 0)
            {
                town.SetPlayer(LevelManager.Instance.GetPlayer(unitTown.data.idPlayer));
            }
        }

        foreach (SaveDataUnit<DataHero> unitHero in dataPlay.entity.heroes)
        {
            GridTileNode tileNode = gridTileHelper.GridTile.GetGridObject(new Vector3Int(unitHero.position.x, unitHero.position.y));

            if (unitHero.idObject == "") continue;

            EntityHero hero = new EntityHero(TypeFaction.Neutral, unitHero);
            UnitManager.SpawnEntityMapObjectToNode(tileNode, hero);
            if (unitHero.data.idPlayer >= 0)
            {
                hero.SetPlayer(LevelManager.Instance.GetPlayer(unitHero.data.idPlayer));
            }
        }

        foreach (SaveDataUnit<DataEntityMapObject> item in dataPlay.entity.mapObjects)
        {
            GridTileNode tileNode = gridTileHelper.GridTile.GetGridObject(new Vector3Int(item.position.x, item.position.y));

            if (item.idObject == "") continue;

            EntityMapObject entity = new EntityMapObject(
                null,
                item
            );
            UnitManager.SpawnEntityMapObjectToNode(tileNode, entity);
        }

        foreach (SaveDataUnit<DataMine> item in dataPlay.entity.mines)
        {
            GridTileNode tileNode = gridTileHelper.GridTile.GetGridObject(new Vector3Int(item.position.x, item.position.y));

            if (item.idObject == "") continue;

            EntityMine entity = new EntityMine(null, item);
            UnitManager.SpawnEntityMapObjectToNode(tileNode, entity);
            if (item.data.idPlayer >= 0)
            {
                entity.SetPlayer(LevelManager.Instance.GetPlayer(item.data.idPlayer));
            }
        }

        foreach (SaveDataUnit<DataArtifact> item in dataPlay.entity.artifacts)
        {
            GridTileNode tileNode = gridTileHelper.GridTile.GetGridObject(new Vector3Int(item.position.x, item.position.y));

            if (item.idObject == "") continue;
            EntityArtifact entity = new EntityArtifact(null, item);
            UnitManager.SpawnEntityMapObjectToNode(tileNode, entity);
        }

        // foreach (SaveDataUnit<DataResourceMapObject> item in DataManager.Instance.DataPlay.Units.resourcesmap)
        // {
        //     GridTileNode tileNode = gridTileHelper.GridTile.GetGridObject(new Vector3Int(item.position.x, item.position.y));

        //     if (item.idObject == "") continue;

        //     ScriptableMapObject scriptableData = ResourceSystem.Instance.GetUnit<ScriptableMapObject>(item.idObject);

        //     if (scriptableData == null)
        //     {
        //         Debug.Log($"None resource map data for : [{item.idUnit}]");
        //         continue;
        //     }
        //     BaseResourceMapObject unit = (BaseResourceMapObject)await UnitManager.SpawnUnitToNode(scriptableData, tileNode);
        //     // if (item.data.idPlayer >= 0)
        //     // {
        //     //     Player player = LevelManager.Instance.GetPlayer(item.data.idPlayer);
        //     //     unit.SetPlayer(player);
        //     // }
        // }

        foreach (SaveDataUnit<DataExplore> item in dataPlay.entity.explorers)
        {
            GridTileNode tileNode = gridTileHelper.GridTile.GetGridObject(new Vector3Int(item.position.x, item.position.y));

            if (item.idObject == "") continue;
            EntityExpore entity = new EntityExpore(null, item);
            UnitManager.SpawnEntityMapObjectToNode(tileNode, entity);
        }

        foreach (SaveDataUnit<DataEntityDwelling> item in dataPlay.entity.dwellings)
        {
            GridTileNode tileNode = gridTileHelper.GridTile.GetGridObject(new Vector3Int(item.position.x, item.position.y));

            if (item.idObject == "") continue;
            EntityDwelling entity = new EntityDwelling(null, item);
            UnitManager.SpawnEntityMapObjectToNode(tileNode, entity);
        }
        // foreach (SaveDataUnit<DataSkillSchool> item in dataPlay.entity.skillSchools)
        // {
        //     GridTileNode tileNode = gridTileHelper.GridTile.GetGridObject(new Vector3Int(item.position.x, item.position.y));

        //     if (item.idObject == "") continue;
        //     EntitySkillSchool entity = new EntitySkillSchool(tileNode, item);
        //     UnitManager.SpawnEntityToNode(tileNode, entity);

        // }

        foreach (SaveDataUnit<DataMonolith> item in dataPlay.entity.monoliths)
        {
            GridTileNode tileNode = gridTileHelper.GridTile.GetGridObject(new Vector3Int(item.position.x, item.position.y));

            if (item.idObject == "") continue;
            EntityMonolith entity = new EntityMonolith(null, item);
            UnitManager.SpawnEntityMapObjectToNode(tileNode, entity);

        }

        foreach (SaveDataUnit<DataCreature> item in dataPlay.entity.creatures)
        {
            GridTileNode nodeWarrior = gridTileHelper.GridTile.GetGridObject(new Vector3Int(item.position.x, item.position.y));
            GridTileNode currentNode = gridTileHelper.GridTile.GetGridObject(new Vector3Int(item.data.protectedNode.x, item.data.protectedNode.y));

            if (item.idObject == "") continue;

            EntityCreature warrior = new EntityCreature(null, item);
            nodeWarrior.SetProtectedNeigbours(warrior, currentNode);
            // UnitManager.SpawnEntityToNode(tileNode, entity);
            // BaseWarriors warrior = (BaseWarriors)await UnitManager.SpawnUnitToNode(scriptableData, tileNode);
            // tileNode.SetProtectedNeigbours(entity, protectedNode);
            // // Hero.OnLoadUnit(unitHero);
            // // LevelManager.Instance.GetPlayer(unitHero.data.idPlayer).AddHero((Hero)Hero);
        }
    }

    private void InitSetting()
    {

        // string fullPath = System.IO.Path.Combine(Application.persistentDataPath, "resourcemap.json");

        // ResourceMap loadedData = new();

        // if (System.IO.File.Exists(fullPath))
        // {
        //     try
        //     {
        //         string dataToLoad = "";

        //         using (System.IO.FileStream stream = new System.IO.FileStream(fullPath, System.IO.FileMode.Open))
        //         {
        //             using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
        //             {
        //                 dataToLoad = reader.ReadToEnd();
        //             }
        //         }

        //         loadedData = JsonUtility.FromJson<ResourceMap>(dataToLoad);
        //         Debug.Log($"{dataToLoad}");

        //     }
        //     catch (System.Exception e)
        //     {
        //         Debug.LogError("Error Load file::: " + fullPath + "\n" + e);
        //     }

        //     if (loadedData != null)
        //     {

        //         Debug.Log($"{loadedData.ToString()}");
        //         foreach (var it in loadedData.Resourcemap)
        //         {
        //             Debug.Log($"{it.id}");

        //         }
        //     }
        // }

        _tileMap.ClearAllTiles();
        _tileMapSky.ClearAllTiles();
        _tileMapCursor.ClearAllTiles();
        _tileMapRoad.ClearAllTiles();
        _tileMapNature.ClearAllTiles();
        _tileMapWalkedNature.ClearAllTiles();

        _dataTypeGround = ResourceSystem.Instance
            .GetAllAssetsByLabel<TileLandscape>("landscape")
            .ToDictionary(t => t.typeGround, t => t);

        // Reset unitManager.
        ResetUnitManager();
    }

    public async UniTask NewMap()
    {
        Application.targetFrameRate = 60;

#if UNITY_EDITOR
        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        stopWatch.Start();
#endif
        InitSetting();

        gameModeData = LevelManager.Instance.GameModeData;

        _isWater = LevelManager.Instance.isWater;

        countArea = LevelManager.Instance.CountArea;

        // Create grid tile nodes.
        gridTileHelper = new GridTileHelper(gameModeData.width, gameModeData.height);

        // Add water if this setting is exist.
        if (!_isWater)
        {
            _dataTypeGround.Remove(TypeGround.Water);
        }

        var operations = new Queue<ILoadingOperation>();
        operations.Enqueue(new CreateAreasOperation(this));
        operations.Enqueue(new CreateTerrainOperation(this));
        operations.Enqueue(new CreateBordersOperation(this));
        operations.Enqueue(new CreateTownOperation(this));
        operations.Enqueue(new CreateNatureOperation(this));

        operations.Enqueue(new CreateRoadOperation(this));

        operations.Enqueue(new CreateMinesOperation(this));
        operations.Enqueue(new CreateExploreOperation(this));
        operations.Enqueue(new CreateSkillSchoolOperation(this));

        operations.Enqueue(new CreateResourceEveryWeekOperation(this));
        operations.Enqueue(new CreateResourceOperation(this));
        operations.Enqueue(new CreateArtifactOperation(this));
        operations.Enqueue(new CreateDwellingOperation(this));
        await GameManager.Instance.LoadingScreenProvider.LoadAndDestroy(operations);

        Application.targetFrameRate = -1;

#if UNITY_EDITOR
        stopWatch.Stop();
        System.TimeSpan timeTaken = stopWatch.Elapsed;
        Debug.LogWarning($"Time Generation Map::: {timeTaken.ToString(@"m\:ss\.ffff")}");
        Debug.LogWarning($"LEVELINFO::: \n {LevelManager.Instance.ToString()}");
#endif
        // ResourceSystem.Instance.DestroyAssetsByLabel("nature");
        // ResourceSystem.Instance.DestroyAssetsByLabel("landscape");

        setSizeTileMap();
        // await UniTask.Delay(1);
    }

    private void setSizeTileMap()
    {
        BoxCollider2D colliderTileMap = _tileMap.GetComponent<BoxCollider2D>();
        colliderTileMap.offset = new Vector2((float)gameModeData.width / 2, (float)gameModeData.height / 2);
        colliderTileMap.size = new Vector2(gameModeData.width, gameModeData.height);
        CompositeCollider2D composeColiiderTileMap = _tileMap.GetComponent<CompositeCollider2D>();
    }


    /// <summary>
    /// Create creek for related object.
    /// </summary>
    /// <param name="startNode"></param>
    public void CreateCreeks(GridTileNode startNode)
    {
        GridTileNode randomNode = gridTileHelper.GetAllGridNodes().Where(t =>
            // t.Disable
            t.StateNode.HasFlag(StateNode.Disable)
            && t.KeyArea == startNode.KeyArea
            && (gridTileHelper.GetDistanceBetweeenPoints(t.position, startNode.position) > 5
            && gridTileHelper.GetDistanceBetweeenPoints(t.position, startNode.position) < 15)
        ).OrderBy(t => Random.value).First();

        List<GridTileNode> path = gridTileHelper.FindPath(
            startNode.position,
            randomNode.position,
            false,
            true
            );

        if (path != null)
        {
            foreach (GridTileNode node in path)
            {
                _tileMapWalkedNature.SetTile(node.position, _tileCreek);
                _listNatureNode.Add(new GridTileNatureNode(node, "creek", true, "creek"));
            }
        }

    }

    /// <summary>
    /// Create Warrior for related object.
    /// </summary>
    /// <param name="currentNode">node</param>
    /// <returns>nodeWarrior</returns>
    public GridTileNode GetNodeWarrior(GridTileNode currentNode)
    {
        GridTileNode nodeWarrior = gridTileHelper.GridTile.GetGridObject(currentNode.position + new Vector3Int(0, -1, 0));

        if (nodeWarrior == null || !nodeWarrior.IsAllowSpawn)
        {
            nodeWarrior = gridTileHelper.GridTile.GetGridObject(currentNode.position + new Vector3Int(1, 0, 0));
        }
        if (nodeWarrior == null || !nodeWarrior.IsAllowSpawn)
        {
            nodeWarrior = gridTileHelper.GridTile.GetGridObject(currentNode.position + new Vector3Int(-1, 0, 0));
        }

        if (nodeWarrior == null || !nodeWarrior.IsAllowSpawn)
        {
            nodeWarrior = gridTileHelper.GridTile.GetGridObject(currentNode.position + new Vector3Int(0, 1, 0));
        }
        if (nodeWarrior == null || !nodeWarrior.IsAllowSpawn)
        {
            nodeWarrior = gridTileHelper.GridTile.GetGridObject(currentNode.position + new Vector3Int(1, 1, 0));
        }
        if (nodeWarrior == null || !nodeWarrior.IsAllowSpawn)
        {
            nodeWarrior = gridTileHelper.GridTile.GetGridObject(currentNode.position + new Vector3Int(1, -1, 0));
        }
        if (nodeWarrior == null || !nodeWarrior.IsAllowSpawn)
        {
            nodeWarrior = gridTileHelper.GridTile.GetGridObject(currentNode.position + new Vector3Int(-1, 1, 0));
        }
        //nodeWarrior.Empty || nodeWarrior.Disable || nodeWarrior.Protected
        if (nodeWarrior == null || !nodeWarrior.IsAllowSpawn)
        {
            nodeWarrior = gridTileHelper.GridTile.GetGridObject(currentNode.position + new Vector3Int(-1, -1, 0));
        }

        return nodeWarrior;
    }

    public bool CreatePortal(GridTileNode node, List<GridTileNode> potentialNode)
    {
        List<GridTileNode> _potentialNode = potentialNode.Where(t =>
            t != node
            && gridTileHelper.CalculateNeighbours(t) > 4
            && t.KeyArea == node.KeyArea
            // && t.Empty
            // && t.Enable
            && t.StateNode.HasFlag(StateNode.Empty)
        // && gridTileHelper.GetDisableNeighbours(t).bottom.Count == 0
        // && gridTileHelper.GetDisableNeighbours(t).top.Count < 2
        ).ToList();
        SetColorForTile(node.position, Color.yellow);
        if (_potentialNode.Count > 0)
        {
            GridTileNode nodeExitPortal = _potentialNode[Random.Range(0, _potentialNode.Count - 1)];

            Area currentArea = LevelManager.Instance.GetArea(nodeExitPortal.KeyArea);

            // If town is not exists for area, get random town.
            if (currentArea.town == null)
            {
                currentArea = LevelManager.Instance.Level
                    .listArea.Where(t => t.town != null).OrderBy(t => Random.value).First();
            }

            GridTileNode townNode = gridTileHelper.GetNode(
                currentArea.startPosition.x, currentArea.startPosition.y
            );

            EntityMonolith monolith = currentArea.portal;


            List<ScriptableEntityMapObject> listConfigPortals = ResourceSystem.Instance
                .GetEntityByType<ScriptableEntityMapObject>(TypeEntity.MapObject)
                .Where(t => t.TypeMapObject == TypeMapObject.Portal)
                .ToList();

            if (monolith == null)
            {
                List<GridTileNode> listAroundTownNodes = gridTileHelper.IsExistExit(townNode);
                List<GridTileNode> listPotentialAroundTownNodes = listAroundTownNodes.Where(t =>
                    t != node
                    //&& gridTileHelper.CalculateNeighbours(t) > 4
                    && t.KeyArea == node.KeyArea
                    && t.StateNode.HasFlag(StateNode.Empty)
                    // && t.Empty
                    // && t.Enable
                    // && !t.Protected
                    && t != townNode
                    && gridTileHelper.GetDistanceBetweeenPoints(t.position, townNode.position) > 4
                ).ToList();

                if (listPotentialAroundTownNodes.Count > 0)
                {
                    GridTileNode nodeInputPortal
                        = listPotentialAroundTownNodes[Random.Range(0, listPotentialAroundTownNodes.Count - 1)];

                    GridTileNode nodeWarrior = GetNodeWarrior(nodeInputPortal);
                    if (nodeWarrior != null)
                    {
                        var configData
                            = listConfigPortals[UnityEngine.Random.Range(0, listConfigPortals.Count)];

                        var factory = new EntityMapObjectFactory();
                        monolith = (EntityMonolith)factory.CreateMapObject(
                            TypeMapObject.Portal,
                            configData
                            );
                        UnitManager.SpawnEntityMapObjectToNode(nodeInputPortal, monolith);
                        nodeInputPortal.AddStateNode(StateNode.Teleport);

                        // monolith = new EntityMonolith(nodeInputPortal, configData);
                        currentArea.portal = (EntityMonolith)monolith;

                        BaseEntity warrior = UnitManager.SpawnEntityCreature(nodeWarrior);

                        nodeWarrior.SetProtectedNeigbours(warrior, nodeInputPortal);
                    }
                    else
                    {

                    }
                    //MonolithData monolithData = new MonolithData();
                    //monolithData.position = nodeForEndPortal._position;
                    //monolith.Init(monolithData);

                }
            }

            if (monolith != null)
            {
                var configData
                    = listConfigPortals[UnityEngine.Random.Range(0, listConfigPortals.Count)];

                var factory = new EntityMapObjectFactory();
                EntityMonolith configMonolithExit = (EntityMonolith)factory.CreateMapObject(
                    TypeMapObject.Portal,
                    configData
                    );
                UnitManager.SpawnEntityMapObjectToNode(nodeExitPortal, configMonolithExit);
                nodeExitPortal.AddStateNode(StateNode.Teleport);
                // BaseEntity configMonolithExit = new EntityMonolith(nodeExitPortal);
                // UnitManager.SpawnMapObjectAsync(nodeExitPortal, TypeMapObject.Monolith);

                GridTileNode nodeWarrior = GetNodeWarrior(nodeExitPortal);
                if (nodeWarrior != null)
                {
                    BaseEntity warrior = UnitManager.SpawnEntityCreature(nodeWarrior);

                    nodeWarrior.SetProtectedNeigbours(warrior, nodeExitPortal);
                }
                // //MonolithData monolithExitData = new MonolithData();
                // //monolithExitData.position = monolithExit.Position;
                // //monolithExit.Init(monolithExitData);

                monolith.Data.portalPoints.Add(configMonolithExit.Position);
                configMonolithExit.Data.portalPoints.Add(monolith.Position);
                return true;
            }
        }
        else
        {
            return false;
        }
        return false;
    }

    public async UniTask OnDrawRoad(Vector3Int start, Vector3Int end)
    {
        if (start == null || end == null) return;

        List<GridTileNode> path = gridTileHelper.FindPath(start, end, true, false);

        if (path != null)
        {
            for (int i = 0; i < path.Count - 1; i++)
            {
                GridTileNode pathNode = path[i];
                GridTileNode pathNodeNext = path[i + 1];
                Vector3Int pos = new Vector3Int(pathNode.X, pathNode.Y, 0);
                _tileMapRoad.SetTile(pos, _tileRoad.tileRule);
                pathNode.SetAsRoad();
            }
        }
        else
        {
        }
        await UniTask.Delay(1);
    }


    /// <summary>
    /// Get service path finding.
    /// </summary>
    /// <returns>PathFinding</returns>
    public GridTileHelper GridTileHelper()
    {
        return gridTileHelper;
    }

    public void ChangePath()
    {
        Vector2 posMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tilePos = _tileMap.WorldToCell(posMouse);

        TileBase clickedTile = _tileMap.GetTile(tilePos);
        GridTileNode node = gridTileHelper.GetNode(tilePos.x, tilePos.y);

        if (clickedTile != null && !node.StateNode.HasFlag(StateNode.Disable))
        {
            if (node.OccupiedUnit == null || node.StateNode.HasFlag(StateNode.Protected))
            {
                LevelManager.Instance.ActivePlayer.FindPathForHero(tilePos, true);
            }

        }

        Debug.Log($"Click {clickedTile.ToString()} \n {node.ToString()}");
    }

    public void ResetCursor()
    {
        _tileMapCursor.ClearAllTiles();
    }

    public void DrawCursor(List<GridTileNode> paths, EntityHero hero)
    {
        ResetCursor();
        if (paths == null) { return; }
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

            if (node.X > nodeNext.X)
            {
                // left
                _tile = _cursorSprites.left;
                if (nodePrev != null && node.Y != nodePrev.Y)
                {
                    if (node.Y > nodePrev.Y)
                    {
                        _tile = _cursorSprites.HtoDBottomLeft;

                    }
                    else
                    {
                        _tile = _cursorSprites.HtoDTopLeft;
                    }
                }

            }
            else if (node.X < nodeNext.X)
            {
                // right
                _tile = _cursorSprites.right;
                if (nodePrev != null && node.Y != nodePrev.Y)
                {
                    if (node.Y > nodePrev.Y)
                    {
                        _tile = _cursorSprites.HtoDBottomRight;

                    }
                    else
                    {
                        _tile = _cursorSprites.HtoDTopRight;
                    }
                }

            }
            else if (node.Y < nodeNext.Y)
            {
                // top
                _tile = _cursorSprites.top;
                if (nodePrev != null && node.X != nodePrev.X)
                {
                    if (node.X > nodePrev.X)
                    {
                        _tile = _cursorSprites.VtoDTopRight;

                    }
                    else
                    {
                        _tile = _cursorSprites.VtoDTopLeft;
                    }
                }

            }
            else if (node.Y > nodeNext.Y)
            {
                // bottom
                _tile = _cursorSprites.bottom;
                if (nodePrev != null && node.X != nodePrev.X)
                {
                    if (node.X > nodePrev.X)
                    {
                        _tile = _cursorSprites.VtoDBottomLeft;

                    }
                    else
                    {
                        _tile = _cursorSprites.VtoDBottomRight;
                    }
                }

            }

            if (node.X > nodeNext.X && node.Y > nodeNext.Y)
            {
                //if (nodePrev != null && node.x == nodePrev.x)
                //{
                //    _tile = _cursorSprites.HtoDBottomLeft;
                //} else
                //{
                //    _tile = _cursorSprites.cornerLeftBottom;
                //}
                _tile = _cursorSprites.cornerLeftBottom;
            }
            else if (node.X < nodeNext.X && node.Y < nodeNext.Y)
            {
                _tile = _cursorSprites.cornerRightTop;
            }
            else if (node.X < nodeNext.X && node.Y > nodeNext.Y)
            {
                _tile = _cursorSprites.cornerRightBottom;
            }
            else if (node.X > nodeNext.X && node.Y < nodeNext.Y)
            {
                _tile = _cursorSprites.cornerLeftTop;
            }


            if (_tile != null)
            {

                //_tile.transform = Matrix4x4.Translate(new Vector3(0,0,0)) * Matrix4x4.Scale(new Vector3(0, -1, 0));
                _tileMapCursor.SetTile(node.position, _tile);
                if (countDistance < 0)
                {
                    SetColorCursor(node.position, Color.red);
                }

                countDistance -= hero.CalculateHitByNode(node);
            }

            if (i == paths.Count - 2)
            {
                //newTile = _cursorSprites.center;
                if (nodeNext.ProtectedUnit != null)
                {
                    _tileMapCursor.SetTile(nodeNext.position, _cursorSprites.Attack);

                }
                else if (nodeNext.OccupiedUnit != null)
                {
                    _tileMapCursor.SetTile(nodeNext.position, _cursorSprites.GoMapObject);

                }
                else
                {
                    _tileMapCursor.SetTile(nodeNext.position, _cursorSprites.center);
                }

                if (countDistance < 0)
                {
                    SetColorCursor(nodeNext.position, Color.red);
                }
                countDistance -= hero.CalculateHitByNode(node);
            }


        }
    }

    private void SetColorCursor(Vector3Int pos, Color color)
    {
        SetColorForTile(pos, color, _tileMapCursor);
    }

    // public void ResetTestTileMap()
    // {
    //     _tileTest.ClearAllTiles();
    // }

    // public void SetColorForTest(Vector3Int pos, Color color)
    // {
    //     color.a = .3f;
    //     Tilemap tileMap = _tileTest;
    //     tileMap.SetTile(pos, _tileSky);
    //     tileMap.SetTileFlags(pos, TileFlags.None);
    //     tileMap.SetColor(pos, color);
    //     tileMap.SetTileFlags(pos, TileFlags.LockColor);
    // }

    public void SetColorForTile(Vector3Int pos, Color color, Tilemap __tileMap = null)
    {
        Tilemap tileMap = _tileMap;

        if (__tileMap != null)
        {
            tileMap = __tileMap;
        }

        tileMap.SetTileFlags(pos, TileFlags.None);
        tileMap.SetColor(pos, color);
        tileMap.SetTileFlags(pos, TileFlags.LockColor);
    }
    public List<GridTileNode> DrawSky(Vector3Int startPosition, int distance)
    {
        GridTileNode startNode
            = gridTileHelper.GridTile.GetGridObject(startPosition);
        List<GridTileNode> listNode = gridTileHelper.GetNeighboursAtDistance(startNode, distance);
        for (int i = 0; i < listNode.Count; i++)
        {
            _tileMapSky.SetTile(listNode[i].position, null);
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
                }
                else
                {
                    _tileMapSky.SetTile(position, _tileSky);
                }
            }
        }
    }

    // public void SetTextMeshNode(GridTileNode tileNode, string textString = "")
    // {
    //     Vector3 posText = tileNode.position;
    //     GameObject text;
    //     if (!listTextMesh.TryGetValue(posText, out text))
    //     {
    //         text = Instantiate(_textMesh, posText, Quaternion.identity);
    //         text.transform.SetParent(_tileMapText.transform);
    //         listTextMesh.Add(posText, text);
    //     }
    //     text.GetComponent<TextMeshPro>().text = textString != "" ? textString : string.Format("nei: {0} l:{1} np: {2} w: {3}",
    //         tileNode.countRelatedNeighbors,
    //         tileNode.level,
    //         tileNode.Protected,
    //         tileNode.Empty
    //         );
    // }

    public void ResetUnitManager()
    {
        Helpers.DestroyChildren(BlokUnits.transform);
    }
}
