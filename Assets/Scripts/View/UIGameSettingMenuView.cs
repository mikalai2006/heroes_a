using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;


public class UIGameSettingMenuView : UIView
{
    public UnityAction OnHideSetting;
    public UnityAction OnSave;

    [SerializeField] private UIDocument _menuDoc;

    private VisualElement _box;

    public void Init()
    {
        try
        {
            _box = _menuDoc.rootVisualElement.Q<VisualElement>("SettingMenu");

            var btnSettings = _box.Q<Button>("ButtonSetting");
            btnSettings.clickable.clicked += () =>
            {
                // GameManager.Instance.ChangeState(GameState.StartApp);
            };

            var btnClose = _box.Q<Button>("ButtonClose");
            btnClose.clickable.clicked += Hide;

            var btnSave = _box.Q<Button>("ButtonSave");
            btnSave.clickable.clicked += () =>
            {
                OnSave?.Invoke();
            };

            Hide();
        }
        catch(Exception e) {
            Debug.LogWarning("Setting Menu error: \n" + e);
        } 
    }

    public void Show()
    {
        _box.style.display = DisplayStyle.Flex;
    }

    public void Hide()
    {
        _box.style.display = DisplayStyle.None;
    }
}

