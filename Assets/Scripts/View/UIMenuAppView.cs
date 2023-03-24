//using Cysharp.Threading.Tasks;
//using Loading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;


public class UIMenuAppView : UIView
{
    [SerializeField] private UIDocument _uiDoc;
    public UIDocument MenuApp => _uiDoc;

    private VisualElement _box;

    // Event called when Play Button is clicked.
    public UnityAction OnShowNewGame;

    // Event called when Play Button is clicked.
    public UnityAction OnQuit;

    private VisualElement progressBar;
    private Label progressBarText;
    private VisualElement progressBarSection;
    private VisualElement buttonsSection;

    private string NameProgressBarSection = "ProgressBarSection";
    private string NameProgressBarText = "ProgressBarText";
    private string NameProgressBar = "ProgressBar";

    public void Init()
    {
        try
        {
            progressBarSection = MenuApp.rootVisualElement.Q<VisualElement>(NameProgressBarSection);
            progressBar = MenuApp.rootVisualElement.Q<VisualElement>(NameProgressBar);
            progressBarText = MenuApp.rootVisualElement.Q<Label>(NameProgressBarText);

            buttonsSection = MenuApp.rootVisualElement.Q<VisualElement>("ButtonsSection");

            var newGameButton = MenuApp.rootVisualElement.Q<Button>("newgame");
            newGameButton.clickable.clicked += () =>
            {
                // GameManager.Instance.ChangeState(GameState.NewGame);
                OnShowNewGame?.Invoke();
            };

            var loadGameButton = MenuApp.rootVisualElement.Q<Button>("loadgame");
            loadGameButton.clickable.clicked += () =>
            {
                GameManager.Instance.ChangeState(GameState.LoadGame);
            };

            var btnQuit = MenuApp.rootVisualElement.Q<Button>("ButtonQuit");
            btnQuit.clickable.clicked += () =>
            {
                OnQuit?.Invoke();
            };

            progressBarSection.visible = false;


        } catch (Exception e) {
            Debug.LogWarning("Menu Ne Game error: \n" + e);
        }
        
    }

    private void ResetFill() { }
    private void OnProgress(float progress) { }


    public void InitNewGame()
    {
        buttonsSection.visible = false;
        progressBarSection.visible = true;
    }

    public void SetProgressText(string text) {
        progressBarText.text = text;
    }

    public void SetProgressValue(float value)
    {
        //Debug.Log($"Value=[{progressBarText.text}]{value}");
        progressBar.style.width =  new StyleLength(new Length(value, LengthUnit.Percent)); ;
    }
}

