using System;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// UI root class for Menu controller.
/// </summary>
public class UIMenu : UIRoot
{
    // Reference to menu view class.
    [SerializeField] private UIMenuView menuView;
    public UIMenuView MenuView => menuView;

    
    public override void ShowRoot()
    {
        base.ShowRoot();

        menuView.ShowView();


    }

    public override void HideRoot()
    {
        menuView.HideView();

        base.HideRoot();
    }
}
