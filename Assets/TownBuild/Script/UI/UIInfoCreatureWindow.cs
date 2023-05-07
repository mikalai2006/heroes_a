using System.Threading.Tasks;

using UnityEngine.UIElements;

public class UIInfoCreatureWindow : UIDialogBaseWindow
{
    private readonly string _nameButtonClose = "ButtonClose";
    private Button _buttonClose;
    protected TaskCompletionSource<DataResultBuildDialog> _processCompletionSource;
    protected DataResultBuildDialog _dataResultDialog;
    private EntityCreature _entityCreature;

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

    public async Task<DataResultBuildDialog> ProcessAction(EntityCreature entityCreature)
    {
        base.Init();
        _entityCreature = entityCreature;

        _dataResultDialog = new DataResultBuildDialog();
        _processCompletionSource = new TaskCompletionSource<DataResultBuildDialog>();

        DrawCreatureInfo();

        return await _processCompletionSource.Task;
    }

    private void DrawCreatureInfo()
    {
        var parameters = _entityCreature.ConfigAttribute.CreatureParams;
        root.Q<Label>("AttackValue").text = parameters.Attack.ToString();
        root.Q<Label>("DefenseValue").text = parameters.Defense.ToString();
        if (parameters.Shoots != 0)
        {
            root.Q<Label>("AmmountValue").text = parameters.Shoots.ToString();
        }
        root.Q<Label>("DamageValue").text = string.Format("{0}-{1}", parameters.DamageMin, parameters.DamageMax);
        root.Q<Label>("HPValue").text = parameters.HP.ToString();
        root.Q<Label>("SpeedValue").text = parameters.Speed.ToString();
    }
}

