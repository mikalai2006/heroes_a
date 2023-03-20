using UnityEngine;

/// <summary>
/// Controller responsible for game phase.
/// </summary>
public class GameController : SubController<UIGame>
{
    // Reference to current game data.
    private DataGame gameData;

    private void OnDestroy() => GameManager.OnAfterStateChanged -= OnChangeGameState;

    private void Start()
    {
        GameManager.OnAfterStateChanged += OnChangeGameState;
    }

    private void OnChangeGameState(GameState state) {
        switch (state)
        {
            case GameState.CreatePathHero:
                ui.GameView.Aside.OnToogleEnableBtnGoHero();
                break;
            case GameState.ChangeResources:
                ui.GameView.Aside.OnRedrawResource();
                break;
            case GameState.StartMoveHero:
                ui.GameView.Aside.StartMoveHero();
                break;
            case GameState.StopMoveHero:
                ui.GameView.Aside.StopMoveHero();
                break;
            case GameState.StepNextPlayer:
                ui.GameView.Aside.NextStep();
                break;
        }
    }

    public override void EngageController()
    {
        // New game need fresh data.
        gameData = new DataGame();

        //// Restarting game time.
        //ui.GameView.UpdateTime(0);

        base.EngageController();
        
        ui.GameView.Init();

        // Attaching UI events.
        //ui.GameView.OnFinishClicked += FinishGame;
        //ui.GameView.OnMenuClicked += GoToMenu;
        ui.GameView.Aside.OnShowSetting += ShowSettingMenu;
        ui.GameView.Aside.OnClickNextStep += NextStep;
        ui.GameView.SettingMenu.OnHideSetting += HideSettingMenu;
        ui.GameView.SettingMenu.OnSave += SaveGame;
        ui.GameView.Aside.OnShowTown += ShowTown;

        NextStep();
    }

    public override void DisengageController()
    {
        // Detaching UI events.
        //ui.GameView.OnMenuClicked -= GoToMenu;
        //ui.GameView.OnFinishClicked -= FinishGame;
        ui.GameView.Aside.OnShowSetting -= ShowSettingMenu;
        ui.GameView.Aside.OnClickNextStep -= NextStep;
        ui.GameView.SettingMenu.OnHideSetting -= HideSettingMenu;
        ui.GameView.SettingMenu.OnSave -= SaveGame;
        ui.GameView.Aside.OnShowTown -= ShowTown;

        base.DisengageController();

    }

    /// <summary>
    /// Unity method called each frame as game object is enabled.
    /// </summary>
    //private void Update()
    //{
    //    // Increasing time value.
    //    //gameData.gameTime += Time.deltaTime;
    //    //// Displaying current game time.
    //    //ui.GameView.UpdateTime(gameData.gameTime);
    //}

    /// <summary>
    /// Handling UI Finish Button Click.
    /// </summary>
    private void FinishGame()
    {
        // Assigning random score.
        //gameData.gameScore = Mathf.CeilToInt(gameData.gameTime * Random.Range(0.0f, 10.0f));
        //// Saving GameData in DataStorage.
        //DataStorage.Instance.SaveData(Keys.GAME_DATA_KEY, gameData);

        // Chaning controller to Game Over Controller
        root.ChangeController(RootController.ControllerTypeEnum.GameOver);
    }

    private void HideSettingMenu()
    {
        // root.ChangeController(RootController.ControllerTypeEnum.Menu);
        ui.GameView.SettingMenu.Hide();
    }
    private void ShowSettingMenu()
    {
        // root.ChangeController(RootController.ControllerTypeEnum.Menu);
        ui.GameView.SettingMenu.Show();
    }
    private void NextStep()
    {
        GameManager.Instance.ChangeState(GameState.StepNextPlayer);
    }
    private void ShowTown()
    {
        ui.GameView.Town.Show();
    }

    private void SaveGame() {
        GameManager.Instance.ChangeState(GameState.SaveGame);
    }
}