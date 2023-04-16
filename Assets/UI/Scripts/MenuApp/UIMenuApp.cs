using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.AddressableAssets;

public class UIMenuApp : UILocaleBase
{
    [SerializeField] private UIDocument _menuAppDoc;
    public UIDocument MenuApp => _menuAppDoc;

    // [SerializeField] private MapManager mapGenerator;

    // public UnityAction OnQuit;
    private VisualElement buttonsSection;

    [SerializeField] private UIDocument _menuNewGameDoc;
    [SerializeField] private VisualTreeAsset _templateButton;
    [SerializeField] private VisualTreeAsset _templateButtonWithImg;
    [SerializeField] private VisualTreeAsset _templateColumn;
    public UIDocument MenuNewGame => _menuNewGameDoc;

    // [SerializeField] public LevelManager _levelManager;
    // public LevelManager LevelManager => _levelManager;

    private VisualElement _box;
    private VisualElement _boxOptions;
    private Button _btnNewGame;

    private string _currentMode;

    private string nameNameOption = "NameOption";
    private string nameBoxOption = "BoxOption";

    private string nameTabActive = "tab_active";
    private string nameTabGeneral = "General";
    private Button _btnTabGeneral;
    private string nameTabContentGeneral = "TabContentGeneral";
    private VisualElement _tabContentGeneral;

    private string nameTabAdvance = "Advance";
    private Button _btnTabAdvance;
    private string nameBoxContentSetting = "BoxSetting";
    private VisualElement _boxContentSetting;
    private VisualElement _boxContentGeneral;
    private VisualElement _buttonRandomSetting;
    private VisualElement _complexityBox;
    private List<ScriptableGameMode> _listGameMode;

    private GameObject _environment;

    public void Init(GameObject environment)
    {
        _environment = environment;

        _listGameMode = ResourceSystem.Instance.GetGameMode();

        buttonsSection = MenuApp.rootVisualElement.Q<VisualElement>("ButtonsSection");

        var newGameButton = MenuApp.rootVisualElement.Q<Button>("newgame");
        newGameButton.clickable.clicked += () =>
        {
            // GameManager.Instance.ChangeState(GameState.NewGame);
            ShowNewGameWindow();
        };

        var loadGameButton = MenuApp.rootVisualElement.Q<Button>("loadgame");
        loadGameButton.clickable.clicked += async () =>
        {
            GameManager.Instance.ChangeState(GameState.LoadGame);
            HideNewGameWindow();
            await GameManager.Instance.AssetProvider.UnloadAsset(_environment);
        };

        var btnQuit = MenuApp.rootVisualElement.Q<Button>("ButtonQuit");
        btnQuit.clickable.clicked += () =>
        {
            Application.Quit();
        };

        // new game window
        _box = _menuNewGameDoc.rootVisualElement.Q<VisualElement>("NewGameBox");
        _complexityBox = _box.Q<VisualElement>("Complexity");
        _boxContentSetting = _box.Q<VisualElement>(nameBoxContentSetting);
        _boxContentGeneral = _box.Q<VisualElement>("BoxGeneral");
        _buttonRandomSetting = _box.Q<VisualElement>("RandomOptions");
        _buttonRandomSetting.Q<Button>().clickable.clicked += () =>
        {
            ShowHideRandomOptions();
        };

        ShowHideRandomOptions();
        // _tabContentAdvance = _box.Q<VisualElement>(nameTabContentAdvance);
        // _btnTabAdvance = _box.Q<Button>(nameTabAdvance);
        // _btnTabAdvance.clickable.clicked += () =>
        // {
        //     _tabContentGeneral.style.display = DisplayStyle.None;
        //     _btnTabGeneral.RemoveFromClassList(nameTabActive);
        //     _tabContentAdvance.style.display = DisplayStyle.Flex;
        //     _btnTabAdvance.AddToClassList(nameTabActive);

        // };

        // _tabContentGeneral = _box.Q<VisualElement>(nameTabContentGeneral);
        // _btnTabGeneral = _box.Q<Button>(nameTabGeneral);
        // _btnTabGeneral.clickable.clicked += () =>
        // {
        //     _tabContentGeneral.style.display = DisplayStyle.Flex;
        //     _btnTabGeneral.AddToClassList(nameTabActive);
        //     _tabContentAdvance.style.display = DisplayStyle.None;
        //     _btnTabAdvance.RemoveFromClassList(nameTabActive);
        // };

        _btnNewGame = _box.Q<VisualElement>("ButtonNewGame").Q<Button>("Btn");
        _btnNewGame.clickable.clicked += async () =>
        {
            GameManager.Instance.ChangeState(GameState.NewGame);
            HideNewGameWindow();
            await GameManager.Instance.AssetProvider.UnloadAsset(_environment);
        };

        var btnClose = _menuNewGameDoc.rootVisualElement.Q<VisualElement>("ButtonClose").Q<Button>("Btn");
        btnClose.clickable.clicked += () =>
        {
            HideNewGameWindow();
        };
        // MenuApp.rootVisualElement.Add(_menuNewGameDoc.rootVisualElement);

        HideNewGameWindow();
        RefreshOptions();

        base.Localize(MenuApp.rootVisualElement);

    }

    private void ShowHideRandomOptions()
    {
        if (_boxContentSetting.style.display == DisplayStyle.None)
        {
            _boxContentSetting.style.display = DisplayStyle.Flex;
        }
        else
        {
            _boxContentSetting.style.display = DisplayStyle.None;
        }
    }

    private void RefreshOptions()
    {
        _boxOptions = _box.Q<ScrollView>("BoxOptions");
        _boxOptions.Clear();

        CreateListGameMode();
        AddOptionCountPlay();
        AddOptionCountBot();
        AddCompexity();

        if (
            (LevelManager.Instance.countPlayer < 2 && LevelManager.Instance.countBot == 0)
            ||
            (LevelManager.Instance.GameModeData.title == "")
            ||
            (LevelManager.Instance.DataGameSetting.Compexity < 99)
            )
        {
            _btnNewGame.SetEnabled(false);
        }
        else
        {
            _btnNewGame.SetEnabled(true);
        }
        base.Localize(_box);
    }

    //private void AddOptionSizeArea()
    //{
    //    var newCol = _templateColumn.Instantiate();
    //    newCol.Q<Label>(nameNameOption).text = "#sizearea";

    //    var NewColBoxBtn = newCol.Q<VisualElement>(nameBoxOption);
    //    NewColBoxBtn.Clear();

    //    var newSlider = new Slider();
    //    newSlider.value = LevelManager.Instance.gameModeData.koofSizeArea;
    //    newSlider.highValue = 1f;
    //    newSlider.lowValue = 0f;
    //    newSlider.style.flexGrow = 1;


    //    newSlider.RegisterValueChangedCallback(x =>
    //    {
    //        LevelManager.Instance.gameModeData.koofSizeArea = x.newValue;
    //    });
    //    NewColBoxBtn.Add(newSlider);

    //    _boxOptions.Add(newCol);
    //}
    public void AddCompexity()
    {
        var _complexityList = _complexityBox.Q<VisualElement>("ComplexityList");
        var _complexityValue = _complexityBox.Q<Label>("ComplexityValue");
        _complexityList.Clear();
        var allComplexity = ResourceSystem.Instance
            .GetAllAssetsByLabel<ScriptableGameSetting>(Constants.Labels.LABEL_GAMESETTING);

        foreach (var item in allComplexity[0].Complexities)
        {
            var btnTemplate = _templateButtonWithImg.Instantiate();
            var btn = btnTemplate.Q<Button>();
            btn.Q<VisualElement>("img").style.backgroundImage
                = new StyleBackground(item.sprite);
            btn.AddToClassList("w-125");
            btn.style.height = new StyleLength(new Length(70, LengthUnit.Pixel));

            if (LevelManager.Instance.DataGameSetting.Compexity == item.value)
            {
                btn.AddToClassList("button_checked");
                btn.RemoveFromClassList("button_bg");
                btn.RemoveFromClassList("button_bordered");
                _complexityValue.text = item.value + "%";
            }
            else
            {
                btn.clickable.clicked += () =>
                {
                    LevelManager.Instance.DataGameSetting.Compexity = item.value;

                    RefreshOptions();
                };
            }

            _complexityList.Add(btn);
        }


    }

    public void CreateListGameMode()
    {
        var boxActiveMode = _box.Q<Label>("ActiveSizeMap");
        var newCol = _templateColumn.Instantiate();
        newCol.Q<Label>(nameNameOption).text = "#sizemap";

        var NewColBoxBtn = newCol.Q<VisualElement>(nameBoxOption);
        NewColBoxBtn.Clear();

        foreach (ScriptableGameMode mode in _listGameMode)
        {
            ScriptableGameMode currentMode = mode;
            var newButtonBox = _templateButton.Instantiate();
            var btn = newButtonBox.Q<Button>("Btn");
            btn.text = currentMode.GameModeData.title;

            if (currentMode.GameModeData.title == LevelManager.Instance.GameModeData.title)
            {
                btn.AddToClassList("button_checked");
                btn.RemoveFromClassList("button_bg");
                btn.RemoveFromClassList("button_bordered");

                boxActiveMode.text = currentMode.GameModeData.title;
            }
            else
            {
                btn.clickable.clicked += () =>
                {
                    LevelManager.Instance.GameModeData = currentMode.GameModeData;
                    RefreshOptions();
                };
            }
            NewColBoxBtn.Add(newButtonBox);
        }

        _boxOptions.Add(newCol);
    }

    private void OnDestroy()
    {
        foreach (var item in _listGameMode)
        {
            //Addressables.Release(item);
            Addressables.ReleaseInstance(this.gameObject);
        }
    }

    private void AddOptionCountPlay()
    {
        var newCol = _templateColumn.Instantiate();
        newCol.Q<Label>(nameNameOption).text = "#countplayer";

        var NewColBoxBtn = newCol.Q<VisualElement>(nameBoxOption);
        NewColBoxBtn.Clear();

        for (int i = 1; i <= LevelManager.Instance.maxPlayer; i++)
        {
            var newButtonBox = _templateButton.Instantiate();
            var newBtn = newButtonBox.Q<Button>("Btn");
            newBtn.text = i.ToString();
            newBtn.name = i.ToString();
            var j = i;

            if (LevelManager.Instance.countPlayer == i)
            {
                newBtn.AddToClassList("button_checked");
                newBtn.RemoveFromClassList("button_bg");
                newBtn.RemoveFromClassList("button_bordered");
            }
            else
            {
                newBtn.clickable.clicked += () =>
                {
                    Debug.Log($"Click {j} button!");
                    LevelManager.Instance.countPlayer = j;
                    RefreshOptions();
                };
            }
            NewColBoxBtn.Add(newButtonBox);
        }

        _boxOptions.Add(newCol);
    }
    private void AddOptionCountBot()
    {
        var newCol = _templateColumn.Instantiate();
        newCol.Q<Label>(nameNameOption).text = "#countbot";

        var NewColBoxBtn = newCol.Q<VisualElement>(nameBoxOption);
        NewColBoxBtn.Clear();

        int maxCountBot = LevelManager.Instance.maxPlayer - LevelManager.Instance.countPlayer;

        if (LevelManager.Instance.countBot > maxCountBot)
        {
            LevelManager.Instance.countBot = maxCountBot;
        }

        for (int i = 0; i <= maxCountBot; i++)
        {
            var newButtonBox = _templateButton.Instantiate();
            var newBtn = newButtonBox.Q<Button>("Btn");
            newBtn.text = i.ToString();
            newBtn.name = i.ToString();
            var j = i;

            if (LevelManager.Instance.countBot == i)
            {
                newBtn.AddToClassList("button_checked");
                newBtn.RemoveFromClassList("button_bg");
                newBtn.RemoveFromClassList("button_bordered");
            }
            else
            {
                newBtn.clickable.clicked += () =>
                {
                    LevelManager.Instance.countBot = j;
                    RefreshOptions();
                };
            }
            NewColBoxBtn.Add(newButtonBox);
        }


        _boxOptions.Add(newCol);
    }

    private void ShowNewGameWindow()
    {
        _menuNewGameDoc.rootVisualElement.style.display = DisplayStyle.Flex;
    }
    public void HideNewGameWindow()
    {
        _menuNewGameDoc.rootVisualElement.style.display = DisplayStyle.None;
    }
}

