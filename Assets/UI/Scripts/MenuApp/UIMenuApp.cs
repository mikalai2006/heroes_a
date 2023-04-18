using UnityEngine;
using UnityEngine.UIElements;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class UIMenuApp : UILocaleBase
{
    [SerializeField] private UIDocument _root;
    public UIDocument Root => _root;
    [SerializeField] private UIAppMenuNewGame _appMenuNewGame;
    public UIAppMenuNewGame AppMenuNewGame => _appMenuNewGame;
    [SerializeField] private UIAppMenuVariants _appMenuVariantsDoc;
    public UIAppMenuVariants AppMenuVariants => _appMenuVariantsDoc;
    [SerializeField] private UIAppMenuMultipleOneDevice _dialogMultipleOneDeviceDoc;
    public UIAppMenuMultipleOneDevice DialogMultipleOneDeviceDoc => _dialogMultipleOneDeviceDoc;

    private GameObject _environment;

    public void Init(GameObject environment)
    {
        _environment = environment;

        LevelManager.Instance.Init();

        _dialogMultipleOneDeviceDoc.Init();
        _appMenuVariantsDoc.Init();
        _appMenuNewGame.Init();

        var newGameButton = Root.rootVisualElement.Q<Button>("newgame");
        newGameButton.clickable.clicked += () =>
        {
            // GameManager.Instance.ChangeState(GameState.NewGame);
            _appMenuVariantsDoc.Show();
        };

        var loadGameButton = Root.rootVisualElement.Q<Button>("loadgame");
        loadGameButton.clickable.clicked += async () =>
        {
            GameManager.Instance.ChangeState(GameState.LoadGame);
            await GameManager.Instance.AssetProvider.UnloadAsset(_environment);
        };

        var btnQuit = Root.rootVisualElement.Q<Button>("ButtonQuit");
        btnQuit.clickable.clicked += () =>
        {
            Application.Quit();
        };

        base.Localize(Root.rootVisualElement);
    }

    public async UniTask DestroyMenu()
    {
        await GameManager.Instance.AssetProvider.UnloadAsset(_environment);
    }

}

