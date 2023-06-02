using System;
using System.Threading.Tasks;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.UIElements;


public class UIArenaSettingWindow : UIDialogBaseWindow
{
    [SerializeField] private VisualTreeAsset _templateButtonToogle;
    [SerializeField] public UnityAction OnSave;

    private DataResultGameMenu _dataResultGameMenu;
    private TaskCompletionSource<DataResultGameMenu> _processCompletionSource;
    private ArenaSettingData _arenaSettingData;

    public override void Start()
    {
        base.Start();

        Title.text = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "settingbattle").GetLocalizedString();

        var left = root.Q<VisualElement>("Left");

        var row = new VisualElement();
        row.style.flexDirection = FlexDirection.Row;
        var btnGrid = _templateButtonToogle.Instantiate();
        btnGrid.Q<Button>().clickable.clicked += async () =>
        {
            await AudioManager.Instance.Click();
            OnToogleGrid(btnGrid);
        };
        SetToogle(btnGrid, GameSetting.showGrid);
        row.Add(btnGrid);
        row.Add(new Label() { text = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "showgrid").GetLocalizedString() });
        left.Add(row);

        var row2 = new VisualElement();
        row2.style.flexDirection = FlexDirection.Row;
        row2.AddToClassList("pt-1");
        var btnShadow = _templateButtonToogle.Instantiate();
        btnShadow.Q<Button>().clickable.clicked += async () =>
        {
            await AudioManager.Instance.Click();
            await OnToogleShadow(btnShadow);
        };
        SetToogle(btnShadow, GameSetting.showShadowGrid);
        row2.Add(btnShadow);
        row2.Add(new Label() { text = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "showshadow").GetLocalizedString() });
        left.Add(row2);

        var row3 = new VisualElement();
        row3.style.flexDirection = FlexDirection.Row;
        row3.AddToClassList("pt-1");
        var btnShadowCursor = _templateButtonToogle.Instantiate();
        btnShadowCursor.Q<Button>().clickable.clicked += async () =>
        {
            await AudioManager.Instance.Click();
            OnToogleShadowCursor(btnShadowCursor);
        };
        SetToogle(btnShadowCursor, GameSetting.showShadowCursor);
        row3.Add(btnShadowCursor);
        row3.Add(new Label() { text = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "showshadowcursor").GetLocalizedString() });
        left.Add(row3);

        root.Q<Button>("ButtonClose").clickable.clicked += async () =>
        {
            await AudioManager.Instance.Click();
            OnClickClose();
        };

    }

    private void OnToogleShadowCursor(VisualElement btn)
    {
        GameSetting.showShadowCursor = !GameSetting.showShadowCursor;
        SetToogle(btn, GameSetting.showShadowCursor);
    }

    private async UniTask OnToogleShadow(VisualElement btn)
    {
        GameSetting.showShadowGrid = !GameSetting.showShadowGrid;
        SetToogle(btn, GameSetting.showShadowGrid);
        await _arenaSettingData.arenaManager.CreateMoveArea();
    }

    private void OnToogleGrid(VisualElement btn)
    {
        GameSetting.showGrid = !GameSetting.showGrid;
        SetToogle(btn, GameSetting.showGrid);
        _arenaSettingData.arenaManager.CreateGrid();
    }

    private void OnClickClose()
    {
        _dataResultGameMenu.isOk = false;
        _processCompletionSource.SetResult(_dataResultGameMenu);

    }

    public async Task<DataResultGameMenu> ProcessAction(ArenaSettingData arenaSettingData)
    {
        base.Init();

        _arenaSettingData = arenaSettingData;

        _dataResultGameMenu = new DataResultGameMenu();

        _processCompletionSource = new TaskCompletionSource<DataResultGameMenu>();

        base.Localize(root);

        return await _processCompletionSource.Task;
    }

    private void SetToogle(VisualElement btn, bool status)
    {
        if (status)
        {
            btn.Q<VisualElement>("No").style.display = DisplayStyle.None;
            btn.Q<VisualElement>("Yes").style.display = DisplayStyle.Flex;
        }
        else
        {
            btn.Q<VisualElement>("No").style.display = DisplayStyle.Flex;
            btn.Q<VisualElement>("Yes").style.display = DisplayStyle.None;
        }
    }

}

