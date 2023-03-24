using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;


public class UIMenuNewGameView : UIView
{
    [SerializeField] private UIDocument _menuDoc;
    [SerializeField] private VisualTreeAsset _templateButton;
    [SerializeField] private VisualTreeAsset _templateColumn;
    public UIDocument MenuNewGame => _menuDoc;

    [SerializeField] public LevelManager _levelManager;
    public LevelManager LevelManager => _levelManager;

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
    private string nameTabContentAdvance = "TabContentAdvance";
    private VisualElement _tabContentAdvance;


    // Event called when Play Button is clicked.
    public UnityAction OnNewGame;
    public UnityAction OnCreateListGameMode;
    //public static event Action<string> OnChangeGameMode;



    public void Init()
    {
        try
        {
            _box = _menuDoc.rootVisualElement.Q<VisualElement>("NewGameBox");

            _tabContentAdvance = _box.Q<VisualElement>(nameTabContentAdvance);
            _btnTabAdvance = _box.Q<Button>(nameTabAdvance);
            _btnTabAdvance.clickable.clicked += () =>
            {
                _tabContentGeneral.style.display = DisplayStyle.None;
                _btnTabGeneral.RemoveFromClassList(nameTabActive);
                _tabContentAdvance.style.display = DisplayStyle.Flex;
                _btnTabAdvance.AddToClassList(nameTabActive);

            };

            _tabContentGeneral = _box.Q<VisualElement>(nameTabContentGeneral);
            _btnTabGeneral = _box.Q<Button>(nameTabGeneral);
            _btnTabGeneral.clickable.clicked += () =>
            {
                _tabContentGeneral.style.display = DisplayStyle.Flex;
                _btnTabGeneral.AddToClassList(nameTabActive);
                _tabContentAdvance.style.display = DisplayStyle.None;
                _btnTabAdvance.RemoveFromClassList(nameTabActive);
            };

            _btnNewGame = _box.Q<Button>("ButtonNewGame");
            _btnNewGame.clickable.clicked += () =>
            {
                //GameManager.Instance.ChangeState(GameState.StartApp);
                ClickNewGame();
            };

            var btnClose = _menuDoc.rootVisualElement.Q<Button>("ButtonClose");
            btnClose.clickable.clicked += () =>
            {
                // UIManager.Instance.HideNewGame();
                Hide();
            };

            RefreshOptions();

            Hide();

        }
        catch (Exception e)
        {
            Debug.LogWarning("Menu Ne Game error: \n" + e);
        }

    }

    private void RefreshOptions()
    {
        Debug.Log("Refresh");
        _boxOptions = _box.Q<ScrollView>("BoxOptions");
        _boxOptions.Clear();

        OnCreateListGameMode?.Invoke();
        //OnChangeGameMode?.Invoke(_currentMode);
        AddOptionCountPlay();
        AddOptionCountBot();
        //AddOptionSizeArea();


        if (
            (LevelManager.Instance.countPlayer < 2 && LevelManager.Instance.countBot == 0)
            ||
            (LevelManager.Instance.GameModeData.title == "")
            )
        {
            _btnNewGame.SetEnabled(false);
        }
        else
        {
            _btnNewGame.SetEnabled(true);
        }
    }

    //private void ClickChangeModeGame(string mode)
    //{
    //    OnChangeGameMode?.Invoke(mode);
    //}

    private void AddOptionSizeArea()
    {
        var newCol = _templateColumn.Instantiate();
        newCol.Q<Label>(nameNameOption).text = "#sizearea";

        var NewColBoxBtn = newCol.Q<VisualElement>(nameBoxOption);
        NewColBoxBtn.Clear();

        var newSlider = new Slider();
        newSlider.value = LevelManager.Instance.GameModeData.koofSizeArea;
        newSlider.highValue = 1f;
        newSlider.lowValue = 0f;
        newSlider.style.flexGrow = 1;


        newSlider.RegisterValueChangedCallback(x =>
        {
            LevelManager.Instance.GameModeData.koofSizeArea = x.newValue;
        });
        NewColBoxBtn.Add(newSlider);

        _boxOptions.Add(newCol);
    }

    public void CreateListGameMode(ScriptableGameMode[] listMode)
    {
        var newCol = _templateColumn.Instantiate();
        newCol.Q<Label>(nameNameOption).text = "#size map";

        var NewColBoxBtn = newCol.Q<VisualElement>(nameBoxOption);
        NewColBoxBtn.Clear();

        for (int i = 0; i < listMode.Length; i++)
        {
            ScriptableGameMode currentMode = listMode[i];
            var newButtonBox = _templateButton.Instantiate();
            var btn = newButtonBox.Q<Button>("Btn");
            btn.text = currentMode.GameModeData.title;

            if (currentMode.GameModeData.title == LevelManager.Instance.GameModeData.title)
            {
                btn.AddToClassList("button_checked");
                btn.RemoveFromClassList("button_bg");
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
            }
            else
            {
                newBtn.clickable.clicked += () =>
                {
                    Debug.Log($"Click {j} bot button!");
                    LevelManager.Instance.countBot = j;
                    RefreshOptions();
                };
            }
            NewColBoxBtn.Add(newButtonBox);
        }


        _boxOptions.Add(newCol);
    }

    public void Hide()
    {
        MenuNewGame.rootVisualElement.style.display = DisplayStyle.None;
    }
    public void Show()
    {
        MenuNewGame.rootVisualElement.style.display = DisplayStyle.Flex;
    }
    private void ClickNewGame()
    {
        OnNewGame?.Invoke();
        Hide();
    }

}

