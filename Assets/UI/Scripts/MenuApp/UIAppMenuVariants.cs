using UnityEngine;
using UnityEngine.UIElements;

public class UIAppMenuVariants : UILocaleBase
{
    [SerializeField] private UIDocument _rootDoc;
    public UIDocument RootDoc => _rootDoc;
    private VisualElement _root;
    [SerializeField] private UIMenuApp _parent;
    public UIMenuApp Parent => _parent;

    public void Init()
    {
        _root = _rootDoc.rootVisualElement;
        _root.Q<Button>("back").clickable.clicked += () =>
        {
            Hide();
        };

        _root.Q<Button>("singlegame").clickable.clicked += () =>
        {
            LevelManager.Instance.Level.Settings.countPlayer = 1;
            LevelManager.Instance.Level.Settings.countBot = 1;
            LevelManager.Instance.Level.Settings.TypeGame = TypeGame.Single;
            LevelManager.Instance.Init();
            _parent.AppMenuNewGame.Show();
            // _parent.AppMenuNewGame.DrawAdvancedOptions();
            Hide();
        };
        var btnMultipleGame = _root.Q<Button>("multiplegame");
        btnMultipleGame.clickable.clicked += () =>
        {
            LevelManager.Instance.Level.Settings.TypeGame = TypeGame.MultipleOneDevice;
            LevelManager.Instance.Init();
            // _parent.AppMenuNewGame.Show();
            _parent.DialogMultipleOneDeviceDoc.Show();
            // _parent.AppMenuNewGame.DrawAdvancedOptions();
            // Hide();
        };
        // btnMultipleGame.SetEnabled(false); //TODO

        Hide();
        base.Localize(_root);
    }

    public void Hide()
    {
        _root.style.display = DisplayStyle.None;
    }

    public void Show()
    {
        _root.style.display = DisplayStyle.Flex;
    }
}

