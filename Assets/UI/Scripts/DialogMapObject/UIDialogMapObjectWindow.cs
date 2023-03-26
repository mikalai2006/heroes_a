using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;
using System.Collections.Generic;

public class UIDialogMapObjectWindow : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDoc;
    public UIDocument DialogApp => _uiDoc;
    [SerializeField] private VisualTreeAsset _templateItem;
    private readonly string _nameButtonOk = "ButtonOk";
    private readonly string _nameButtonCancel = "ButtonCancel";
    private readonly string _nameSpriteElement = "Sprite";
    private readonly string _nameDescriptionLabel = "DescriptionDialog";
    private readonly string _nameHeaderLabel = "HeaderDialog";
    private readonly string _nameValueLabel = "Value";
    private readonly string _nameBoxVariants = "BoxVariants";
    private readonly string _nameOverlay = "Overlay";

    private Button _buttonOk;
    private Button _buttonCancel;
    // private VisualElement _spriteElement;
    private Label _headerLabel;
    private Label _descriptionLabel;
    // private Label _valueLabel;
    private VisualElement _boxVariantsElement;

    private TaskCompletionSource<DataResultDialog> _processCompletionSource;

    public UnityEvent processAction;

    private DataDialog _dataDialog;
    private DataResultDialog _dataResultDialog;

    private void Awake()
    {
        _buttonOk = DialogApp.rootVisualElement.Q<Button>(_nameButtonOk);
        _buttonOk.clickable.clicked += OnClickOk;
        _buttonCancel = DialogApp.rootVisualElement.Q<Button>(_nameButtonCancel);
        _headerLabel = DialogApp.rootVisualElement.Q<Label>(_nameHeaderLabel);
        _descriptionLabel = DialogApp.rootVisualElement.Q<Label>(_nameDescriptionLabel);
        _buttonCancel.clickable.clicked += OnClickCancel;
        _boxVariantsElement = DialogApp.rootVisualElement.Q<VisualElement>(_nameBoxVariants);
    }

    public async Task<DataResultDialog> ProcessAction(DataDialog dataDialog)
    {
        _dataDialog = dataDialog;
        _dataResultDialog = new DataResultDialog();

        _headerLabel.text = _dataDialog.Header;
        _descriptionLabel.text = _dataDialog.Description;

        for (int i = 0; i < _dataDialog.Value.Count; i++)
        {
            VisualElement item = _templateItem.Instantiate();
            var _spriteElement = item.Q<VisualElement>(_nameSpriteElement);
            var _valueLabel = item.Q<Label>(_nameValueLabel);
            var sprite = _dataDialog.Value[i].Sprite;

            _spriteElement.style.backgroundImage = new StyleBackground(sprite);
            _spriteElement.style.width = new StyleLength(new Length(sprite.bounds.size.x * sprite.pixelsPerUnit, LengthUnit.Pixel));
            _spriteElement.style.height = new StyleLength(new Length(sprite.bounds.size.y * sprite.pixelsPerUnit, LengthUnit.Pixel));

            var val = _dataDialog.Value[i].Value;
            if (val != 0)
            {
                _valueLabel.text = _dataDialog.Value[i].Value.ToString();
            }

            _boxVariantsElement.Add(item);
        }

        Player player = LevelManager.Instance.ActivePlayer;

        UQueryBuilder<VisualElement> builder = new UQueryBuilder<VisualElement>(DialogApp.rootVisualElement);
        List<VisualElement> list = builder.Name(_nameOverlay).ToList();
        foreach (var overlay in list)
        {
            overlay.style.backgroundColor = player.DataPlayer.color;
        }

        _processCompletionSource = new TaskCompletionSource<DataResultDialog>();

        return await _processCompletionSource.Task;
    }

    private void OnClickOk()
    {
        _dataResultDialog.isOk = true;
        _processCompletionSource.SetResult(_dataResultDialog);

        processAction?.Invoke();
    }
    private void OnClickCancel()
    {
        _dataResultDialog.isOk = false;
        _processCompletionSource.SetResult(_dataResultDialog);

        processAction?.Invoke();
    }
}

