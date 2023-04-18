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
    [SerializeField] private VisualTreeAsset _templateBuildDialog;
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
    private Label _requireBuildBlok;
    private VisualElement _boxSpriteObject;
    private readonly string _nameGeneralBlok = "GeneralBlok";
    private VisualElement _generalBlok;
    private TaskCompletionSource<DataResultBuildDialog> _processCompletionSource;

    public UnityEvent processAction;

    private DataDialogBuild _buildDialog;
    private DataResultBuildDialog _dataResultDialog;
    private EntityTown _activeTown;
    private Player _activePlayer;
    private ScriptableEntityTown _scriptObjectTown;

    private void Awake()
    {

        DialogApp.rootVisualElement.Q<VisualElement>("Panel").AddToClassList("w-75");
        _headerLabel = DialogApp.rootVisualElement.Q<Label>(_nameHeaderLabel);
        _generalBlok = DialogApp.rootVisualElement.Q<VisualElement>(_nameGeneralBlok);
        VisualElement dialog = _templateBuildDialog.Instantiate();
        dialog.style.flexGrow = 1;
        _generalBlok.Clear();
        _generalBlok.Add(dialog);

        _activePlayer = LevelManager.Instance.ActivePlayer;
        _activeTown = _activePlayer.ActiveTown;

        _scriptObjectTown = (ScriptableEntityTown)_activeTown.ScriptableData;
        // _scriptObjectBuildTown = ResourceSystem.Instance.GetBuildTowns().Where(t => t.TypeFaction == _scriptObjectTown.TypeFaction).First();

        _buttonCancel = DialogApp.rootVisualElement.Q<Button>(_nameButtonCancel);
        _buttonCancel.clickable.clicked += OnClickCancel;
        _buttonOk = DialogApp.rootVisualElement.Q<Button>(_nameButtonOk);
        _buttonOk.clickable.clicked += OnClickOk;

        // _headerLabel = DialogApp.rootVisualElement.Q<Label>(_nameHeaderLabel);
        _descriptionLabel = DialogApp.rootVisualElement.Q<Label>(_nameDescriptionLabel);

        _requireResourceBlok = DialogApp.rootVisualElement.Q<VisualElement>(_nameRequireResource);
        _requireBuildBlok = DialogApp.rootVisualElement.Q<Label>(_nameRequireBuild);
        _boxSpriteObject = DialogApp.rootVisualElement.Q<VisualElement>("SpriteObject");

    }

    public async Task<DataResultBuildDialog> ProcessAction(DataDialogBuild buildDialogData)
    {
        _buildDialog = buildDialogData;
        _dataResultDialog = new DataResultBuildDialog()
        {
            // build = build
        };

        UQueryBuilder<VisualElement> builder = new UQueryBuilder<VisualElement>(DialogApp.rootVisualElement);
        List<VisualElement> list = builder.Name(_nameOverlay).ToList();
        Color color = _activePlayer.DataPlayer.color;
        color.a = .6f;
        foreach (var overlay in list)
        {
            overlay.style.backgroundColor = color;
        }

        _headerLabel.text = _buildDialog.title; // titlePrefix.GetLocalizedString() + ": " + t.Text.title;
        _descriptionLabel.text = _buildDialog.description; // t.Text.description;
        _requireBuildBlok.text = _buildDialog.textRequireBuild;

        if (_buildDialog.MenuSprite != null)
        {
            _boxSpriteObject.style.backgroundImage = new StyleBackground(_buildDialog.MenuSprite);
            _boxSpriteObject.style.width = new StyleLength(new Length(_buildDialog.MenuSprite.bounds.size.x * _buildDialog.MenuSprite.pixelsPerUnit * 1.5f, LengthUnit.Pixel));
            _boxSpriteObject.style.height = new StyleLength(new Length(_buildDialog.MenuSprite.bounds.size.y * _buildDialog.MenuSprite.pixelsPerUnit * 1.5f, LengthUnit.Pixel));
        }

        for (int i = 0; i < _buildDialog.CostResource.Count; i++)
        {
            VisualElement item = _templateItem.Instantiate();
            var _spriteElement = item.Q<VisualElement>(_nameSpriteElement);
            var _valueLabel = item.Q<Label>(_nameValueLabel);
            var sprite = _buildDialog.CostResource[i].Resource.MenuSprite;

            _spriteElement.style.backgroundImage = new StyleBackground(sprite);
            _spriteElement.style.width = new StyleLength(new Length(sprite.bounds.size.x * sprite.pixelsPerUnit, LengthUnit.Pixel));
            _spriteElement.style.height = new StyleLength(new Length(sprite.bounds.size.y * sprite.pixelsPerUnit, LengthUnit.Pixel));

            var val = _buildDialog.CostResource[i].Count;
            if (val != 0)
            {
                _valueLabel.text = _buildDialog.CostResource[i].Count.ToString();
            }

            _requireResourceBlok.Add(item);
        }

        if (_buildDialog.isNotBuild)
        {
            _buttonOk.SetEnabled(false);
        }

        _processCompletionSource = new TaskCompletionSource<DataResultBuildDialog>();

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

