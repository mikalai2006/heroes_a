using System;
using System.Threading.Tasks;

using UnityEngine.UIElements;

public class UIArenaFullStatWindow : UIDialogBaseWindow
{
    private readonly string _nameButtonClose = "ButtonClose";
    private Button _buttonClose;
    private Label _textBlok;
    public static event Action OnCloseStat;
    protected TaskCompletionSource<ArenaStatResult> _processCompletionSource;
    protected ArenaStatResult _dataResultDialog;
    private ArenaStatData _statData;

    public override void Start()
    {
        base.Start();

        Title.style.display = DisplayStyle.None;
        Panel.AddToClassList("w-50");

        _buttonClose = root.Q<VisualElement>(_nameButtonClose).Q<Button>("Btn");
        _buttonClose.clickable.clicked += OnClickClose;

        _textBlok = root.Q<Label>("FullStat");
    }

    private async void OnClickClose()
    {
        await AudioManager.Instance.Click();
        _dataResultDialog.isOk = false;
        _processCompletionSource.SetResult(_dataResultDialog);
        OnCloseStat?.Invoke();
    }

    public async Task<ArenaStatResult> ProcessAction(ArenaStatData stat)
    {
        base.Init();
        _statData = stat;

        _dataResultDialog = new ArenaStatResult();
        _processCompletionSource = new TaskCompletionSource<ArenaStatResult>();

        DrawStat();

        return await _processCompletionSource.Task;
    }

    private void DrawStat()
    {
        _textBlok.text = _statData.arenaManager.ArenaStat.FullText;
    }
}

