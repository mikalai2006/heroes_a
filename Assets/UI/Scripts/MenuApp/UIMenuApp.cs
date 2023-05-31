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

        PlayerType PlayerType = GameSetting.typeTestLeftPlayer;
        PlayerType PlayerType2 = GameSetting.typeTestRightPlayer;
        var testArenaButton = Root.rootVisualElement.Q<Button>("TestArena");
        testArenaButton.clickable.clicked += async () =>
        {
            await AudioManager.Instance.Click();
            await DestroyMenu();

            UnitManager.Entities.Clear();

            // Create test hero.
            var testHero = new EntityHero(TypeFaction.Castle, GameSetting.ArenaTestHeroes[0]);
            Player player = new Player();
            PlayerData dataPlayer = new PlayerData()
            {
                id = 0,
                color = GameSetting.colors[0],
                playerType = PlayerType,
                command = 0
            };
            StartSetting startSetting = new StartSetting();
            startSetting.TypePlayerItem = new()
            {
                title = "Test Player",
                TypePlayer = PlayerType
            };
            dataPlayer.playerType = startSetting.TypePlayerItem.TypePlayer;
            player.New(dataPlayer);
            testHero.SetPlayer(player);

            // Create second test hero.
            var testEnemy = new EntityHero(TypeFaction.Castle, GameSetting.ArenaTestHeroes[1]);
            Player playerSecond = new Player();
            PlayerData dataPlayerSecond = new PlayerData()
            {
                id = 1,
                color = GameSetting.colors[1],
                playerType = PlayerType2,
                command = 1
            };
            StartSetting startSettingSecond = new StartSetting();
            startSettingSecond.TypePlayerItem = new()
            {
                title = "Test Player",
                TypePlayer = PlayerType2
            };
            dataPlayerSecond.playerType = startSettingSecond.TypePlayerItem.TypePlayer;
            playerSecond.New(dataPlayerSecond);
            testEnemy.SetPlayer(playerSecond);

            var loadingOperations = new ArenaLoadOperation(new DialogArenaData()
            {
                heroAttacking = testHero,
                town = null,
                heroDefending = testEnemy,
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

            // Create test hero.
            var testHero = new EntityHero(TypeFaction.Castle, GameSetting.ArenaTestHeroes[0]);
            UnitManager.Entities.Add(testHero.Id, testHero);
            Player player = new Player();
            PlayerData dataPlayer = new PlayerData()
            {
                id = 0,
                color = GameSetting.colors[0],
                playerType = PlayerType,
                command = 0
            };
            StartSetting startSetting = new StartSetting();
            startSetting.TypePlayerItem = new()
            {
                title = "Test Player",
                TypePlayer = PlayerType
            };
            dataPlayer.playerType = startSetting.TypePlayerItem.TypePlayer;
            player.New(dataPlayer);
            testHero.SetPlayer(player);

            // create test second hero.
            var testEnemy = new EntityHero(TypeFaction.Castle, GameSetting.ArenaTestHeroes[1]);
            UnitManager.Entities.Add(testEnemy.Id, testEnemy);
            Player playerSecond = new Player();
            PlayerData dataPlayerSecond = new PlayerData()
            {
                id = 1,
                color = GameSetting.colors[1],
                playerType = PlayerType2,
                command = 1
            };
            StartSetting startSettingSecond = new StartSetting();
            startSettingSecond.TypePlayerItem = new()
            {
                title = "Test Player",
                TypePlayer = PlayerType2
            };
            dataPlayerSecond.playerType = startSettingSecond.TypePlayerItem.TypePlayer;
            playerSecond.New(dataPlayerSecond);
            testEnemy.SetPlayer(playerSecond);

            var configTown = GameSetting.ArenaTestTowns[Random.Range(0, GameSetting.ArenaTestTowns.Count)];
            var town = new EntityTown(configTown.TypeGround, configTown);
            town.Data.level = 2;
            town.Data.HeroinTown = testEnemy.Id;
            var loadingOperations = new ArenaLoadOperation(new DialogArenaData()
            {
                heroAttacking = testHero,
                town = town,
                ArenaSetting = GameSetting.ArenaSettings.Find(t => t.NativeGround.typeGround == configTown.TypeGround)
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

