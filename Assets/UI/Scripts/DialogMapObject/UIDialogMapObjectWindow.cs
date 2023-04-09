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
    [SerializeField] private VisualTreeAsset _templateButtonCheckItem;
    private readonly string _nameButtonOk = "ButtonOk";
    private readonly string _nameButtonCancel = "ButtonCancel";
    private readonly string _nameSpriteElement = "Sprite";
    private readonly string _nameDescriptionLabel = "DescriptionDialog";
    private readonly string _nameHeaderLabel = "HeaderDialog";
    private readonly string _nameValueLabel = "Value";
    private readonly string _nameBoxVariants = "BoxVariants";
    private readonly string _nameOverlay = "Overlay";
    private readonly string _nameSpriteObject = "SpriteObject";

    private Button _buttonOk;
    private Button _buttonCancel;
    // private VisualElement _spriteElement;
    private Label _headerLabel;
    private Label _descriptionLabel;
    // private Label _valueLabel;
    private VisualElement _boxVariantsElement;
    private VisualElement _boxSpriteObject;

    private TaskCompletionSource<DataResultDialog> _processCompletionSource;

    public UnityEvent processAction;

    private DataDialogMapObject _dataDialog;
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
        _boxSpriteObject = DialogApp.rootVisualElement.Q<VisualElement>(_nameSpriteObject);

    }

    public async Task<DataResultDialog> ProcessAction(DataDialogMapObject dataDialog)
    {
        _dataDialog = dataDialog;
        _dataResultDialog = new DataResultDialog();

        if (_dataDialog.TypeCheck == TypeCheck.OnlyOk)
        {
            _buttonCancel.style.display = DisplayStyle.None;
        }

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

        for (int i = 0; i < _dataDialog.Groups.Count; i++)
        {
            for (int j = 0; j < _dataDialog.Groups[i].Values.Count; j++)
            {
                VisualElement item = _templateItem.Instantiate();
                if (_dataDialog.TypeWorkAttribute == TypeWorkAttribute.One)
                {
                    item = _templateButtonCheckItem.Instantiate();
                }
                var _spriteElement = item.Q<VisualElement>(_nameSpriteElement);
                var _valueLabel = item.Q<Label>(_nameValueLabel);
                var sprite = _dataDialog.Groups[i].Values[j].Sprite;

                _spriteElement.style.backgroundImage = new StyleBackground(sprite);
                _spriteElement.style.width = new StyleLength(
                    new Length(sprite.bounds.size.x * sprite.pixelsPerUnit, LengthUnit.Pixel)
                );
                _spriteElement.style.height = new StyleLength(
                    new Length(sprite.bounds.size.y * sprite.pixelsPerUnit, LengthUnit.Pixel)
                );

                var val = _dataDialog.Groups[i].Values[j].Value;
                _valueLabel.text = val != 0 ? val.ToString() : "";

                _boxVariantsElement.Add(item);
            }
        }

        Player player = LevelManager.Instance.ActivePlayer;

        UQueryBuilder<VisualElement> builder
            = new UQueryBuilder<VisualElement>(DialogApp.rootVisualElement);
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

