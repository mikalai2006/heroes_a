using System;
using UnityEngine;

/// <summary>
/// Nice, easy to understand enum-based game manager. For larger and more complex games, look into
/// state machines. But this will serve just fine for most games.
/// </summary>
public class GameManager : StaticInstance<GameManager> {

    public static event Action<GameState> OnBeforeStateChanged;
    public static event Action<GameState> OnAfterStateChanged;

    [SerializeField] public MapManager mapManager;

    public GameState State { get; private set; }

    // Kick the game off with the first state
    void Start() {
        //QualitySettings.vSyncCount = 0;
#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
	    Debug.unityLogger.logEnabled = false;
#endif


        ChangeState(GameState.StartApp);

    }

    public void ChangeState(GameState newState, object Params = null)
    {

        //Debug.Log($"New state: {newState}");
        OnBeforeStateChanged?.Invoke(newState);

        State = newState;
        switch (newState) {
            case GameState.StartApp:
                HandleStarting();
                break;
            case GameState.NewGame:
                HandleNewGame();
                break;
            case GameState.LoadGame:
                HandleLoadGame();
                break;
            case GameState.CreateLevel:
                HandleCreateLevel();
                break;
            case GameState.CreateMap:
                HandleCreateMap();
                break;
            //case GameState.SpawningTowns:
            //    HandleSpawningTowns();
            //    break;
            //case GameState.SpawningMapObjects:
            //    HandleSpawningMapObjects();
            //    break;
            //case GameState.SpawningPortals:
            //    HandleSpawningPortals();
            //    break;
            //case GameState.SpawningRoads:
            //    HandleSpawningRoads();
            //    break;
            //case GameState.SpawningEnemies:
            //    HandleSpawningEnemies();
            //    break;
            case GameState.StepNextPlayer:
                HandleSetActivePlayer();
                break;
            case GameState.ChooseHero:
                HandleChooseHero();
                break;
            case GameState.ChooseTown:
                ChooseTown(Params);
                break;
            case GameState.EnemyTurn:
                break;
            case GameState.Win:
                break;
            case GameState.Lose:
                break;
            case GameState.ShowMenu:
                break;
            case GameState.ChangeResources:
                HandleSaveGame();
                break;
            case GameState.StartMoveHero:
                HandleStartMoveHero();
                break;
            case GameState.StopMoveHero:
                HandleStopMoveHero();
                break;
            case GameState.CreatePathHero:
                
                break;
            case GameState.SaveGame:
                HandleSaveGame();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnAfterStateChanged?.Invoke(newState);
    }

    private void HandleNewGame()
    {
        ChangeState(GameState.CreateLevel);

    }
    private void HandleCreateLevel()
    {
        LevelManager.Instance.NewLevel();
        ChangeState(GameState.CreateMap);
    }
    private void HandleCreateMap()
    {
        StartCoroutine(mapManager.NewMap());
    }

    private void HandleLoadGame()
    {
        DataManager.Instance.Load();
        ChangeState(GameState.StepNextPlayer);
    }

    private void HandleSaveGame()
    {
        DataManager.Instance.Save();
    }

    private void HandleStarting()
    {
        //UIManager.Instance.ShowStartMenu();
    }
    private void HandleChooseHero()
    {
        
    }

    private void HandleStartMoveHero()
    {
        //ChangeState(GameState.MoveHero);
    }
    private void HandleStopMoveHero()
    {
        //ChangeState(GameState.MoveHero);
    }
    private void ChooseTown(object town)
    {
        //ChangeState(GameState.ShowMenu);
    }


    //private void HandleSpawningPortals()
    //{
    //    for (int i = 0; i < countArea; i++)
    //    {

    //        UnitManager.Instance.SpawnUnitsByTypeAndCount(TypeUnit.Portal, i, 1);

    //    }

    //    ChangeState(GameState.SpawningRoads);
    //}
    //private void HandleSpawningMapObjects()
    //{
    //    //UnitManager.Instance.SpawnUnitsByTypeAndCount(TypeUnit.MapObject, 5, 5);

    //    ChangeState(GameState.SpawningEnemies);
    //}
    //private void HandleSpawningRoads()
    //{
    //    Grid2DManager.Instance.onDrawRoads();

    //    ChangeState(GameState.SpawningMapObjects);

    //}


    //private void HandleSpawningEnemies() {
        
    //    // Spawn enemies
        
    //    ChangeState(GameState.HeroTurn);
    //}

    private void HandleSetActivePlayer() {
        // If you're making a turn based game, this could show the turn menu, highlight available units etc

        // Keep track of how many units need to make a move, once they've all finished, change the state. This could
        // be monitored in the unit manager or the units themselves.
        LevelManager.Instance.StepNextPlayer();

    }

}

/// <summary>
/// This is obviously an example and I have no idea what kind of game you're making.
/// You can use a similar manager for controlling your menu states or dynamic-cinematics, etc
/// </summary>
[Serializable]
public enum GameState {
    StartApp = 0,
    CreateMap = 1,
    StepNextPlayer = 2,
    EnemyTurn = 3,
    Win = 4,
    Lose = 5,
    ShowMenu = 6,
    ChooseTown = 8,
    CreateLevel = 9,
    ChooseHero = 10,

    ChangeResources = 100,

    StartMoveHero = 200,
    StopMoveHero = 201,
    CreatePathHero = 202,

    SaveGame = 1000,

    LoadGame = 1010,

    NewGame = 1020,
}