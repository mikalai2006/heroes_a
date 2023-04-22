using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;

public class UIDialogMapObjectWindow : UIDialogBaseWindow
{
    [SerializeField] private VisualTreeAsset _templateCostItem;
    [SerializeField] private VisualTreeAsset _templateButton;
    [SerializeField] private VisualTreeAsset _templateButtonCheckItem;
    private readonly string _nameSpriteElement = "Sprite";
    private readonly string _nameValueLabel = "Value";
    private readonly string _nameBoxVariants = "BoxVariants";
    private readonly string _nameSpriteObject = "SpriteObject";

    private Button _buttonOk;
    private Button _buttonCancel;
    private VisualElement _boxVariantsElement;
    private VisualElement _boxSpriteObject;

    private TaskCompletionSource<DataResultDialog> _processCompletionSource;

    public UnityEvent processAction;

    private DataDialogMapObject _dataDialog;
    private DataResultDialog _dataResultDialog;

    public override void Start()
    {
        base.Start();

        _buttonOk = root.Q<VisualElement>("Ok").Q<Button>("Btn");
        _buttonOk.clickable.clicked += OnClickOk;

        _buttonCancel = root.Q<VisualElement>("Cancel").Q<Button>("Btn");
        _buttonCancel.clickable.clicked += OnClickCancel;

        _boxVariantsElement = root.Q<VisualElement>(_nameBoxVariants);
        _boxSpriteObject = root.Q<VisualElement>(_nameSpriteObject);

        // base.Localize(root);
    }

    public async Task<DataResultDialog> ProcessAction(DataDialogMapObject dataDialog)
    {
        base.Init();

        _dataDialog = dataDialog;
        _dataResultDialog = new DataResultDialog();

        if (_dataDialog.TypeCheck == TypeCheck.OnlyOk)
        {
            _buttonCancel.style.display = DisplayStyle.None;
        }

        Title.text = _dataDialog.Header;
        Panel.Q<Label>("Description").text = _dataDialog.Description;
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
                VisualElement item = _templateCostItem.Instantiate();
                if (_dataDialog.TypeWorkAttribute == TypeWorkAttribute.One)
                {
                    var index = i;
                    item = _templateButtonCheckItem.Instantiate();
                    item.RegisterCallback<ClickEvent>((ClickEvent evt) =>
                    {
                        Debug.Log($"Var{i}-{index}");
                        _dataResultDialog.keyVariant = index;
                    });
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

