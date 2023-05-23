using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;
using System.Collections.Generic;

public class UIDialogHelpWindow : UILocaleBase
{
    [SerializeField] private UIDocument _uiDoc;
    public UIDocument DialogApp => _uiDoc;
    [SerializeField] private VisualTreeAsset _templateHelp;
    private readonly string _nameButtonOk = "ButtonOk";
    private readonly string _nameSpriteElement = "Sprite";
    private readonly string _nameDescriptionLabel = "DescriptionDialog";
    private readonly string _nameHeaderLabel = "HeaderDialog";
    private readonly string _nameGeneralBlok = "GeneralBlok";
    private readonly string _nameOverlay = "Overlay";

    private VisualElement _generalBlok;
    private Button _buttonOk;
    private Label _headerLabel;
    private Label _descriptionLabel;
    private VisualElement _boxSpriteObject;

    private TaskCompletionSource<DataResultDialog> _processCompletionSource;

    public UnityEvent processAction;

    private DataDialogHelp _dataDialog;
    private DataResultDialog _dataResultDialog;

    private VisualElement _root;

    private void Start()
    {
        _root = DialogApp.rootVisualElement;

        _headerLabel = _root.Q<Label>(_nameHeaderLabel);
        _generalBlok = _root.Q<VisualElement>(_nameGeneralBlok);

        var panel = _root.Q<VisualElement>("Panel");
        panel.AddToClassList("w-50");

        var panelBlok = _root.Q<VisualElement>("PanelBlok");
        panelBlok.style.flexGrow = 1;

        VisualElement docDialogBlok = _templateHelp.Instantiate();
        docDialogBlok.style.flexGrow = 1;
        _generalBlok.Clear();
        _generalBlok.Add(docDialogBlok);

        _buttonOk = _root.Q<Button>(_nameButtonOk);
        _buttonOk.clickable.clicked += OnClickOk;
        _headerLabel = _root.Q<Label>(_nameHeaderLabel);
        _descriptionLabel = _root.Q<Label>(_nameDescriptionLabel);
        _boxSpriteObject = _root.Q<VisualElement>(_nameSpriteElement);

        base.Localize(_root);
    }

    public async Task<DataResultDialog> ProcessAction(DataDialogHelp dataDialog)
    {
        _dataDialog = dataDialog;
        _dataResultDialog = new DataResultDialog();
        _headerLabel.text = _dataDialog.Header;
        _descriptionLabel.text = _dataDialog.Description;
        if (_dataDialog.Sprite != null)
        {
            _boxSpriteObject.style.backgroundImage = new StyleBackground(_dataDialog.Sprite);
            _boxSpriteObject.style.width = new StyleLength(new Length(
                _dataDialog.Sprite.bounds.size.x * _dataDialog.Sprite.pixelsPerUnit,
                LengthUnit.Pixel
            ));
            _boxSpriteObject.style.height = new StyleLength(new Length(
                _dataDialog.Sprite.bounds.size.y * _dataDialog.Sprite.pixelsPerUnit,
                LengthUnit.Pixel
            ));
        }

        Player player = LevelManager.Instance.ActivePlayer;

        UQueryBuilder<VisualElement> builder
            = new UQueryBuilder<VisualElement>(DialogApp.rootVisualElement);
        List<VisualElement> list = builder.Name(_nameOverlay).ToList();
        foreach (var overlay in list)
        {
            overlay.style.backgroundColor = player != null
                ? player.DataPlayer.color
                : LevelManager.Instance.ConfigGameSettings.neutralColor;
        }

        _processCompletionSource = new TaskCompletionSource<DataResultDialog>();

        return await _processCompletionSource.Task;
    }

    private async void OnClickOk()
    {
        await AudioManager.Instance.Click();
        _dataResultDialog.isOk = true;
        _processCompletionSource.SetResult(_dataResultDialog);

        processAction?.Invoke();
    }
}

