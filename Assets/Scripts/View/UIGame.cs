using UnityEngine;

/// <summary>
/// UI root for Game controller.
/// </summary>
public class UIGame : UIRoot
{
    // Reference to game view class.
    [SerializeField]
    private UIGameView gameView;
    public UIGameView GameView => gameView;

    public override void ShowRoot()
    {
        base.ShowRoot();

        gameView.ShowView();
    }

    public override void HideRoot()
    {
        gameView.HideView();

        base.HideRoot();
    }
}
