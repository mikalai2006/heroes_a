using System.Threading.Tasks;

using UnityEngine.UIElements;

public class UIInfoCreatureWindow : UIDialogBaseWindow
{
    private readonly string _nameButtonClose = "ButtonClose";
    private Button _buttonClose;
    protected TaskCompletionSource<DataResultBuildDialog> _processCompletionSource;
    protected DataResultBuildDialog _dataResultDialog;

    public override void Start()
    {
        base.Start();

        _buttonClose = DialogApp.rootVisualElement.Q<TemplateContainer>(_nameButtonClose).Q<Button>("Btn");
        _buttonClose.clickable.clicked += OnClickClose;

        // _buttonPrice = DialogApp.rootVisualElement.Q<VisualElement>(_nameButtonPrice).Q<Button>("Btn");
        // _buttonPrice.clickable.clicked += OnClickBuy;
    }

    private void OnClickClose()
    {
        _dataResultDialog.isOk = false;
        _processCompletionSource.SetResult(_dataResultDialog);
    }

    public async Task<DataResultBuildDialog> ProcessAction()
    {
        base.Init();

        _dataResultDialog = new DataResultBuildDialog();
        _processCompletionSource = new TaskCompletionSource<DataResultBuildDialog>();

        return await _processCompletionSource.Task;
    }
}

