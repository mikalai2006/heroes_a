using System;

using UnityEngine;

public class UIMenuView : UIView
{
    [SerializeField] private readonly UIMenuAppView _menuApp;
    public UIMenuAppView MenuApp => _menuApp;

    [SerializeField] private readonly UIMenuNewGameView _newGame;
    public UIMenuNewGameView NewGame => _newGame;

    public void Init()
    {
        try
        {
            MenuApp.Init();

            NewGame.Init();
        }
        catch (Exception e)
        {
            Debug.LogWarning("Not found button of startMenu. \n" + e);
        }

    }

}
