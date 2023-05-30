using UnityEngine;
using UnityEngine.UIElements;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Loader;

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
    private SOGameSetting GameSetting => LevelManager.Instance.ConfigGameSettings;

    public void Init(GameObject environment)
    {
        _environment = environment;

        LevelManager.Instance.Init();

        _dialogMultipleOneDeviceDoc.Init();
        _appMenuVariantsDoc.Init();
        _appMenuNewGame.Init();

        var newGameButton = Root.rootVisualElement.Q<Button>("newgame");
        newGameButton.clickable.clicked += async () =>
        {
            await AudioManager.Instance.Click();
            _appMenuVariantsDoc.Show();
        };

        var loadGameButton = Root.rootVisualElement.Q<Button>("loadgame");
        loadGameButton.clickable.clicked += async () =>
        {
            await AudioManager.Instance.Click();
            GameManager.Instance.ChangeState(GameState.LoadGame);
            await GameManager.Instance.AssetProvider.UnloadAsset(_environment);
        };

        var testArenaButton = Root.rootVisualElement.Q<Button>("TestArena");
        testArenaButton.clickable.clicked += async () =>
        {
            await AudioManager.Instance.Click();
            await DestroyMenu();

            UnitManager.Entities.Clear();
            var testHero = new EntityHero(TypeFaction.Castle, GameSetting.ArenaTestHeroes[0]);
            var testEnemy = new EntityHero(TypeFaction.Castle, GameSetting.ArenaTestHeroes[1]);
            var loadingOperations = new ArenaLoadOperation(new DialogArenaData()
            {
                hero = testHero,
                town = null,
                enemy = testEnemy,
                ArenaSetting = GameSetting.ArenaSettings[Random.Range(0, GameSetting.ArenaSettings.Count)]
            });
            var result = await loadingOperations.ShowHide();

            var loaderMenu = new Queue<ILoadingOperation>();
            loaderMenu.Enqueue(new MenuAppOperation());
            await GameManager.Instance.LoadingScreenProvider.LoadAndDestroy(loaderMenu);
        };

        var testArenaTownButton = Root.rootVisualElement.Q<Button>("TestArenaTown");
        testArenaTownButton.clickable.clicked += async () =>
        {
            await AudioManager.Instance.Click();
            await DestroyMenu();

            UnitManager.Entities.Clear();
            var testHero = new EntityHero(TypeFaction.Castle, GameSetting.ArenaTestHeroes[0]);
            UnitManager.Entities.Add(testHero.Id, testHero);
            var testEnemy = new EntityHero(TypeFaction.Castle, GameSetting.ArenaTestHeroes[1]);
            UnitManager.Entities.Add(testEnemy.Id, testEnemy);
            var configTown = ResourceSystem.Instance
                .GetEntityByType<ScriptableEntityTown>(TypeEntity.Town)
                .Find(t => t.TypeFaction == TypeFaction.Castle);
            var town = new EntityTown(TypeGround.Grass, configTown);
            town.Data.level = 2;
            town.Data.HeroinTown = testEnemy.Id;
            var loadingOperations = new ArenaLoadOperation(new DialogArenaData()
            {
                hero = testHero,
                town = town,
                ArenaSetting = GameSetting.ArenaSettings[Random.Range(0, GameSetting.ArenaSettings.Count)]
            });
            var result = await loadingOperations.ShowHide();

            var loaderMenu = new Queue<ILoadingOperation>();
            loaderMenu.Enqueue(new MenuAppOperation());
            await GameManager.Instance.LoadingScreenProvider.LoadAndDestroy(loaderMenu);
        };

        var btnQuit = Root.rootVisualElement.Q<Button>("ButtonQuit");
        btnQuit.clickable.clicked += async () =>
        {
            await AudioManager.Instance.Click();
            Application.Quit();
        };

        base.Localize(Root.rootVisualElement);
    }

    public async UniTask DestroyMenu()
    {
        await GameManager.Instance.AssetProvider.UnloadAsset(_environment);
    }

}

