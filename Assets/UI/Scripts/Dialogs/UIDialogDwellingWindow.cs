using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

public class UIDialogDwellingWindow : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDoc;
    public UIDocument DialogApp => _uiDoc;
    [SerializeField] private VisualTreeAsset _templateDwellingBlok;
    [SerializeField] private VisualTreeAsset _templateButton;
    [SerializeField] private VisualTreeAsset _templateItem;
    private readonly string _nameGeneralBlok = "GeneralBlok";
    private readonly string _nameCostBlok = "CostBlok";
    private readonly string _nameTotalCostBlok = "TotalCostBlok";
    private readonly string _nameButtonsBlok = "ButtonsBlok";
    private readonly string _nameHeaderLabel = "HeaderDialog";
    private readonly string _nameOverlay = "Overlay";
    private readonly string _nameClassBorder = "border-color";
    private readonly string _nameClassBorderActive = "button_active";


    private Button _buttonOk;
    private Button _buttonCancel;
    private Label _headerLabel;

    private VisualElement _generalBlok;
    private VisualElement _costBlok;
    private VisualElement _totalCostBlok;
    private VisualElement _level1;
    private VisualElement _level2;
    private Label _hireCountLabel;
    private Label _availableCountLabel;
    private SliderInt _sliderValue;
    private int _hireCount;

    private TaskCompletionSource<DataResultDialogDwelling> _processCompletionSource;

    public UnityEvent processAction;

    private EntityDwelling _dwelling;
    private DataResultDialogDwelling _dataResultDialog;
    private ScriptableEntityCreature _creature;
    private Dictionary<Label, int> _labelsTotalCost = new Dictionary<Label, int>();


    private void Start()
    {
        _headerLabel = DialogApp.rootVisualElement.Q<Label>(_nameHeaderLabel);
        _generalBlok = DialogApp.rootVisualElement.Q<VisualElement>(_nameGeneralBlok);

        var panel = DialogApp.rootVisualElement.Q<VisualElement>("Panel");
        panel.AddToClassList("w-75");
        panel.AddToClassList("h-full");

        var panelBlok = DialogApp.rootVisualElement.Q<VisualElement>("PanelBlok");
        panelBlok.style.flexGrow = 1;

        VisualElement docDialogDwelling = _templateDwellingBlok.Instantiate();
        docDialogDwelling.style.flexGrow = 1;
        _generalBlok.Clear();
        _generalBlok.Add(docDialogDwelling);

        var blokButtons = _generalBlok.Q<VisualElement>(_nameButtonsBlok);
        blokButtons.Clear();
        _buttonOk = _templateButton.Instantiate().Q<Button>("Btn");
        _buttonOk.clickable.clicked += OnClickOk;
        blokButtons.Add(_buttonOk);

        _buttonCancel = _templateButton.Instantiate().Q<Button>("Btn");
        blokButtons.Add(_buttonCancel);
        _buttonCancel.clickable.clicked += OnClickCancel;
        LocalizedString textCancel = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "cancel");
        _buttonCancel.text = textCancel.GetLocalizedString();

        _costBlok = _generalBlok.Q<VisualElement>(_nameCostBlok);
        _costBlok.Clear();
        _totalCostBlok = _generalBlok.Q<VisualElement>(_nameTotalCostBlok);
        _totalCostBlok.Clear();

        LocalizedString textCost = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "cost");
        LocalizedString textAvailable = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "available");
        LocalizedString textTotalCost = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "totalcost");
        _generalBlok.Q<Label>("TitleAvailable").text = textAvailable.GetLocalizedString();
        _generalBlok.Q<Label>("TitleCostBlok").text = textCost.GetLocalizedString();
        _generalBlok.Q<Label>("TitleTotalCostBlok").text = textTotalCost.GetLocalizedString();

        _hireCountLabel = _generalBlok.Q<Label>("HireCount");
        _availableCountLabel = _generalBlok.Q<Label>("AvailableCount");

        _level1 = _generalBlok.Q<VisualElement>("Level1");
        _level1.RegisterCallback<ClickEvent>((ClickEvent evt) =>
        {
            ChooseLevel(1);
        });
        _level2 = _generalBlok.Q<VisualElement>("Level2");
        _level2.RegisterCallback<ClickEvent>((ClickEvent evt) =>
        {
            ChooseLevel(2);
        });

        _sliderValue = _generalBlok.Q<SliderInt>("SliderValue");
        _sliderValue.RegisterValueChangedCallback((changeEvent) =>
        {
            ChangeValue(changeEvent);
        });
        // _descriptionLabel = DialogApp.rootVisualElement.Q<Label>(_nameDescriptionLabel);
        // _boxVariantsElement = DialogApp.rootVisualElement.Q<VisualElement>(_nameBoxVariants);
        // _boxSpriteObject = DialogApp.rootVisualElement.Q<VisualElement>(_nameSpriteObject);

    }

    private void UpdateTotalCost()
    {
        foreach (KeyValuePair<Label, int> label in _labelsTotalCost)
        {
            label.Key.text = (_hireCount * label.Value).ToString();
        }
        if (_hireCount > 0)
        {
            _buttonOk.SetEnabled(true);
        }
        else
        {
            _buttonOk.SetEnabled(false);
        }
    }

    private void CreateBlokTotal()
    {
        _totalCostBlok.Clear();

        foreach (var res in _creature.CreatureParams.Cost)
        {
            var itemResource = _templateItem.Instantiate();
            itemResource.style.flexGrow = 1;
            var label = itemResource.Q<Label>("Value");
            _labelsTotalCost.Add(label, res.Count);
            label.text = (_hireCount * res.Count).ToString();
            var sprite = res.Resource.MenuSprite;

            var _spriteElement = itemResource.Q<VisualElement>("Sprite");
            _spriteElement.style.backgroundImage = new StyleBackground(sprite);
            _spriteElement.style.width = new StyleLength(
                new Length(sprite.bounds.size.x * sprite.pixelsPerUnit, LengthUnit.Pixel)
            );
            _spriteElement.style.height = new StyleLength(
                new Length(sprite.bounds.size.y * sprite.pixelsPerUnit, LengthUnit.Pixel)
            );
            _totalCostBlok.Add(itemResource);
        }
    }

    private void ChangeValue(ChangeEvent<int> changeEvent)
    {
        _hireCount = changeEvent.newValue;
        _hireCountLabel.text = _hireCount.ToString();
        _availableCountLabel.text = (_dwelling.Data.value - _hireCount).ToString();
        UpdateTotalCost();
    }

    private void ChooseLevel(int level)
    {
        switch (level)
        {
            case 1:
                _level2.RemoveFromClassList(_nameClassBorderActive);
                _level2.AddToClassList(_nameClassBorder);
                _level1.RemoveFromClassList(_nameClassBorder);
                _level1.AddToClassList(_nameClassBorderActive);
                break;
            case 2:
                _level1.RemoveFromClassList(_nameClassBorderActive);
                _level1.AddToClassList(_nameClassBorder);
                _level2.RemoveFromClassList(_nameClassBorder);
                _level2.AddToClassList(_nameClassBorderActive);
                break;
        }
    }

    public async Task<DataResultDialogDwelling> ProcessAction(
        EntityDwelling dwelling
    )
    {
        _dwelling = dwelling;
        ScriptableEntityDwelling configData
            = (ScriptableEntityDwelling)_dwelling.ScriptableData;
        _creature = configData.Creature[_dwelling.Data.level];
        _dataResultDialog = new DataResultDialogDwelling();

        var nameCreature = configData.Creature[_dwelling.Data.level].title.IsEmpty ?
            "" : configData.Creature[_dwelling.Data.level].title.GetLocalizedString();

        _availableCountLabel.text = _dwelling.Data.value.ToString();
        _sliderValue.highValue = _dwelling.Data.value;
        _sliderValue.lowValue = _sliderValue.value = 0;

        var title = _creature.title.GetLocalizedString();
        LocalizedString textHire = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "hire");
        _headerLabel.text = textHire.GetLocalizedString() + "<color=#FFFFAB>" + title + "</color>";

        _generalBlok.Q<Label>("TitleHire").text = textHire.GetLocalizedString();
        _buttonOk.text = textHire.GetLocalizedString();

        var _spriteLevel1 = _generalBlok.Q<VisualElement>("Level1");
        _spriteLevel1.style.backgroundImage = new StyleBackground(_creature.MenuSprite);
        var _spriteLevel2 = _generalBlok.Q<VisualElement>("Level2");
        _spriteLevel2.style.backgroundImage = new StyleBackground(_creature.MenuSprite);


        foreach (var res in _creature.CreatureParams.Cost)
        {
            var itemResource = _templateItem.Instantiate();
            itemResource.style.flexGrow = 1;
            itemResource.Q<Label>("Value").text = res.Count.ToString();
            var sprite = res.Resource.MenuSprite;

            var _spriteElement = itemResource.Q<VisualElement>("Sprite");
            _spriteElement.style.backgroundImage = new StyleBackground(sprite);
            _spriteElement.style.width = new StyleLength(
                new Length(sprite.bounds.size.x * sprite.pixelsPerUnit, LengthUnit.Pixel)
            );
            _spriteElement.style.height = new StyleLength(
                new Length(sprite.bounds.size.y * sprite.pixelsPerUnit, LengthUnit.Pixel)
            );
            _costBlok.Add(itemResource);
        }

        CreateBlokTotal();
        // if (_dataDialog.Sprite != null)
        // {
        //     _boxSpriteObject.style.backgroundImage = new StyleBackground(_dataDialog.Sprite);
        //     _boxSpriteObject.style.width = new StyleLength(new Length(
        //         _dataDialog.Sprite.bounds.size.x * _dataDialog.Sprite.pixelsPerUnit,
        //         LengthUnit.Pixel
        //     ));
        //     _boxSpriteObject.style.height = new StyleLength(new Length(
        //         _dataDialog.Sprite.bounds.size.y * _dataDialog.Sprite.pixelsPerUnit,
        //         LengthUnit.Pixel
        //     ));
        // }

        // for (int i = 0; i < _dataDialog.Groups.Count; i++)
        // {
        //     for (int j = 0; j < _dataDialog.Groups[i].Values.Count; j++)
        //     {
        //         VisualElement item = _templateDwellingBlok.Instantiate();
        //         if (_dataDialog.TypeWorkAttribute == TypeWorkAttribute.One)
        //         {
        //             item = _templateButton.Instantiate();
        //         }
        //         var _spriteElement = item.Q<VisualElement>(_nameSpriteElement);
        //         var _valueLabel = item.Q<Label>(_nameValueLabel);
        //         var sprite = _dataDialog.Groups[i].Values[j].Sprite;

        //         _spriteElement.style.backgroundImage = new StyleBackground(sprite);
        //         _spriteElement.style.width = new StyleLength(
        //             new Length(sprite.bounds.size.x * sprite.pixelsPerUnit, LengthUnit.Pixel)
        //         );
        //         _spriteElement.style.height = new StyleLength(
        //             new Length(sprite.bounds.size.y * sprite.pixelsPerUnit, LengthUnit.Pixel)
        //         );

        //         var val = _dataDialog.Groups[i].Values[j].Value;
        //         _valueLabel.text = val != 0 ? val.ToString() : "";

        //         _boxVariantsElement.Add(item);
        //     }
        // }

        Player player = LevelManager.Instance.ActivePlayer;

        UQueryBuilder<VisualElement> builder
            = new UQueryBuilder<VisualElement>(DialogApp.rootVisualElement);
        List<VisualElement> list = builder.Name(_nameOverlay).ToList();
        foreach (var overlay in list)
        {
            overlay.style.backgroundColor = player.DataPlayer.color;
        }

        _processCompletionSource = new TaskCompletionSource<DataResultDialogDwelling>();

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

