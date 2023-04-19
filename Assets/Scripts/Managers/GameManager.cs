using System;
using UnityEngine;
using Login;
using System.Collections.Generic;
using Loader;
using AppInfo;
using UnityEngine.Localization;

public class GameManager : StaticInstance<GameManager>
{
    public static event Action<GameState> OnBeforeStateChanged;
    public static event Action<GameState> OnAfterStateChanged;
    [SerializeField] public MapManager MapManager;
    [SerializeField] public LevelManager LevelManager;
    public LoadingScreenProvider LoadingScreenProvider { get; private set; }
    public LoginWindowProvider LoginWindowProvider { get; private set; }
    public AssetProvider AssetProvider { get; private set; }
    public GameState State { get; private set; }
    public AppInfoContainer AppInfo;

    void Start()
    {
#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;
#endif
        //QualitySettings.vSyncCount = 0;

        ChangeState(GameState.StartApp);

    }

    public void Init()
    {
        LoadingScreenProvider = new LoadingScreenProvider();
        LoginWindowProvider = new LoginWindowProvider();
        AssetProvider = new AssetProvider();
    }

    public void ChangeState(GameState newState, object Params = null)
    {

        //Debug.Log($"New state: {newState}");
        OnBeforeStateChanged?.Invoke(newState);

        State = newState;
        switch (newState)
        {
            case GameState.StartApp:
                HandleStartApp();
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
            case GameState.StepNextPlayer:
                HandleSetActivePlayer();
                break;
            case GameState.StartGame:
                HandleStartGame();
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
                // HandleSaveGame();
                break;
            case GameState.StartMoveHero:
                HandleStartMoveHero();
                break;
            case GameState.StopMoveHero:
                HandleStopMoveHero();
                break;
            case GameState.ChangeHeroParams:
                // HandleStopMoveHero();
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

    private async void HandleNewGame()
    {
        LevelManager.Instance.NewLevel();

        var operations = new Queue<ILoadingOperation>();
        operations.Enqueue(new GameInitOperation());
        await LoadingScreenProvider.LoadAndDestroy(operations);

        await MapManager.NewMap();

        ChangeState(GameState.StepNextPlayer);
    }
    private void HandleCreateLevel()
    {
        LevelManager.Instance.NewLevel();
        ChangeState(GameState.CreateMap);
    }
    private void HandleCreateMap()
    {
        // await MapManager.NewMap();
    }

    private async void HandleLoadGame()
    {

        // LevelManager.Instance.NewLevel();

        var operations = new Queue<ILoadingOperation>();
        operations.Enqueue(new GameInitOperation());
        await LoadingScreenProvider.LoadAndDestroy(operations);
        DataManager.Instance.Load();

        // await MapManager.NewMap();
        ChangeState(GameState.StartGame);
    }

    private void HandleSaveGame()
    {
        DataManager.Instance.Save();
    }

    private void HandleStartApp()
    {
        //UIManager.Instance.ShowStartMenu();
    }
    private void HandleStartGame()
    {
        //UIManager.Instance.ShowStartMenu();
    }
    private void HandleChooseHero()
    {

    }

    private async void HandleStartMoveHero()
    {
        await LevelManager.Instance.ActivePlayer.ActiveHero.StartMove();
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

    private void HandleSetActivePlayer()
    {
        LevelManager.Instance.StepNextPlayer();
    }

}

[Serializable]
public enum GameState
{
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
    ChangeHeroParams = 11,
    StartGame = 12,
    ChangeResources = 100,

    StartMoveHero = 200,
    StopMoveHero = 201,
    CreatePathHero = 202,

    SaveGame = 1000,

    LoadGame = 1010,

    NewGame = 1020,
}