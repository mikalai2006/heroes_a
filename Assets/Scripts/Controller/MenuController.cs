using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Controller responsible for menu phase.
/// </summary>
public class MenuController : SubController<UIMenu>
{
    private ScriptableGameMode gameModeData;
    private ScriptableGameMode[] listMode;

    public override void EngageController()
    {
        // Attaching UI events.
        ui.MenuView.NewGame.OnNewGame += OnNewGameStart;
        ui.MenuView.MenuApp.OnQuit += QuitGame;
        ui.MenuView.MenuApp.OnShowNewGame += ShowDialogNewGame;
        ui.MenuView.NewGame.OnCreateListGameMode += CreateListGameMode;
        //UIMenuNewGameView.OnChangeGameMode += ChangeGameMode;


        base.EngageController();
        LoadGameMode();
        ui.MenuView.Init();
    }

    public override void DisengageController()
    {
        base.DisengageController();

        // Detaching UI events.
        ui.MenuView.NewGame.OnNewGame -= OnNewGameStart;
        ui.MenuView.MenuApp.OnQuit -= QuitGame;
        ui.MenuView.MenuApp.OnShowNewGame -= ShowDialogNewGame;
        ui.MenuView.NewGame.OnCreateListGameMode -= CreateListGameMode;
        //UIMenuNewGameView.OnChangeGameMode -= ChangeGameMode;
    }

    private void OnNewGameStart()
    {
        ui.MenuView.MenuApp.InitNewGame();
        StartCoroutine(OnNewGame());
    }

    private IEnumerator OnNewGame()
    {
        LevelManager.Instance.NewLevel();
        yield return StartCoroutine(GameManager.Instance.mapManager.NewMap());
        // Changing controller to Game Controller.
        //GameManager.Instance.ChangeState(GameState.NewGame);
        root.ChangeController(RootController.ControllerTypeEnum.Game);
        yield return null;
    }
    private void QuitGame()
    {
        // Closing the game.
        Application.Quit();
        Debug.Log("Quit");
    }
    private void ShowDialogNewGame()
    {
        ui.MenuView.NewGame.Show();
    }
    //private void StartGame()
    //{
    //    // Changing controller to Game Controller.
    //    root.ChangeController(RootController.ControllerTypeEnum.Game);
    //}

    private void LoadGameMode() {
        listMode = Resources.LoadAll<ScriptableGameMode>("GameMode");
    }

    private void CreateListGameMode()
    {
        ui.MenuView.NewGame.CreateListGameMode(listMode);
    }
    //private void ChangeGameMode(string mode)
    //{
    //    for(int i = 0; i < listMode.Length; i++)
    //    {
    //        if (listMode[i].name == mode)
    //        {
    //            //return allModes[i];
    //            //gameModeData = listMode[i];
    //            LevelManager.Instance.gameModeData = listMode[i].GameModeData;
    //            Debug.Log($"Change mode is {mode}");
    //        }
    //    }
    //    //return null;
    //}

}