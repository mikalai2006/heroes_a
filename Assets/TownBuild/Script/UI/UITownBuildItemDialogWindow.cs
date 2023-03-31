using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.Localization;
using System.Linq;

public class UITownBuildItemDialogWindow : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDoc;
    public UIDocument DialogApp => _uiDoc;
    [SerializeField] private VisualTreeAsset _templateItem;
    private readonly string _nameButtonOk = "ButtonOk";
    private readonly string _nameButtonCancel = "ButtonCancel";
    private readonly string _nameOverlay = "Overlay";
    private readonly string _nameRequireResource = "ListRequireResource";
    private readonly string _nameRequireBuild = "ListRequireBuild";
    private readonly string _nameHeaderLabel = "HeaderDialog";
    private readonly string _nameDescriptionLabel = "DescriptionDialog";
    private readonly string _nameSpriteElement = "Sprite";
    private readonly string _nameValueLabel = "Value";

    private Button _buttonCancel;
    private Button _buttonOk;
    private Label _headerLabel;
    private Label _descriptionLabel;
    private VisualElement _requireResourceBlok;
    private VisualElement _requireBuildBlok;
    private VisualElement _boxSpriteObject;
    private TaskCompletionSource<DataResultDialog> _processCompletionSource;

    public UnityEvent processAction;

    private DataDialog _dataDialog;
    private DataResultDialog _dataResultDialog;

    private void Awake()
    {
        _buttonCancel = DialogApp.rootVisualElement.Q<Button>(_nameButtonCancel);
        _buttonCancel.clickable.clicked += OnClickCancel;
        _buttonOk = DialogApp.rootVisualElement.Q<Button>(_nameButtonOk);
        _buttonOk.clickable.clicked += OnClickOk;

        _headerLabel = DialogApp.rootVisualElement.Q<Label>(_nameHeaderLabel);
        _descriptionLabel = DialogApp.rootVisualElement.Q<Label>(_nameDescriptionLabel);

        _requireResourceBlok = DialogApp.rootVisualElement.Q<VisualElement>(_nameRequireResource);
        _requireBuildBlok = DialogApp.rootVisualElement.Q<VisualElement>(_nameRequireBuild);
        _boxSpriteObject = DialogApp.rootVisualElement.Q<VisualElement>("SpriteObject");

    }

    public async Task<DataResultDialog> ProcessAction(DataDialog dataDialog)
    {
        _dataDialog = dataDialog;
        _dataResultDialog = new DataResultDialog();

        Player player = LevelManager.Instance.ActivePlayer;
        BaseTown town = player.ActiveTown;
        ScriptableTown scriptDataTown = (ScriptableTown)player.ActiveTown.ScriptableData;
        var startDataActiveTown = ResourceSystem.Instance.GetBuildTowns().Where(t => t.TypeFaction == scriptDataTown.TypeFaction).First();

        UQueryBuilder<VisualElement> builder = new UQueryBuilder<VisualElement>(DialogApp.rootVisualElement);
        List<VisualElement> list = builder.Name(_nameOverlay).ToList();
        Color color = player.DataPlayer.color;
        color.a = .6f;
        foreach (var overlay in list)
        {
            overlay.style.backgroundColor = color;
        }

        _headerLabel.text = _dataDialog.Header;
        _descriptionLabel.text = _dataDialog.Description;
        if (_dataDialog.Sprite != null)
        {
            _boxSpriteObject.style.backgroundImage = new StyleBackground(_dataDialog.Sprite);
            _boxSpriteObject.style.width = new StyleLength(new Length(_dataDialog.Sprite.bounds.size.x * _dataDialog.Sprite.pixelsPerUnit * 1.5f, LengthUnit.Pixel));
            _boxSpriteObject.style.height = new StyleLength(new Length(_dataDialog.Sprite.bounds.size.y * _dataDialog.Sprite.pixelsPerUnit * 1.5f, LengthUnit.Pixel));
        }
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

            _requireResourceBlok.Add(item);
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

