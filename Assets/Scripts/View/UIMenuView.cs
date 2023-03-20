using System;
//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Events;
//using UnityEngine.UIElements;

public class UIMenuView : UIView
{
    [SerializeField] private UIMenuAppView menuApp;
    public UIMenuAppView MenuApp => menuApp;

    [SerializeField] private UIMenuNewGameView newGame;
    public UIMenuNewGameView NewGame => newGame;

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
