using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.Localization;
using System;

public class UIDialogDwellingWindow : UIDialogBaseWindow
{
    [SerializeField] private VisualTreeAsset _templateButton;
    [SerializeField] private VisualTreeAsset _templateItem;
    private readonly string _nameCostBlok = "CostBlok";
    private readonly string _nameTotalCostBlok = "TotalCostBlok";
    private readonly string _nameButtonsBlok = "ButtonsBlok";
    private readonly string _nameClassBorder = "border-color";
    private readonly string _nameClassBorderActive = "button_active";
    public static event Action OnBuyCreature;

    private Button _buttonOk;
    private Button _buttonCancel;
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
    private ScriptableAttributeCreature _creature;
    private Dictionary<Label, int> _labelsTotalCost = new Dictionary<Label, int>();
    private List<CostEntity> _costEntities = new List<CostEntity>();


    public override void Start()
    {
        base.Start();

        Panel.AddToClassList("w-75");
        Panel.AddToClassList("h-full");

        var blokButtons = root.Q<VisualElement>(_nameButtonsBlok);
        blokButtons.Clear();

        _buttonOk = _templateButton.Instantiate().Q<Button>("Btn");
        _buttonOk.clickable.clicked += OnClickOk;
        blokButtons.Add(_buttonOk);

        _buttonCancel = _templateButton.Instantiate().Q<Button>("Btn");
        blokButtons.Add(_buttonCancel);
        _buttonCancel.clickable.clicked += OnClickCancel;
        LocalizedString textCancel = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "cancel");
        _buttonCancel.text = textCancel.GetLocalizedString();

        _costBlok = root.Q<VisualElement>(_nameCostBlok);
        _costBlok.Clear();
        _totalCostBlok = root.Q<VisualElement>(_nameTotalCostBlok);
        _totalCostBlok.Clear();

        _hireCountLabel = root.Q<Label>("HireCount");
        _availableCountLabel = root.Q<Label>("AvailableCount");

        _level1 = root.Q<VisualElement>("Level1");
        _level1.RegisterCallback<ClickEvent>((ClickEvent evt) =>
        {
            ChooseLevel(1);
        });
        _level2 = root.Q<VisualElement>("Level2");
        _level2.RegisterCallback<ClickEvent>((ClickEvent evt) =>
        {
            ChooseLevel(2);
        });

        _sliderValue = root.Q<SliderInt>("SliderValue");
        _sliderValue.RegisterValueChangedCallback((changeEvent) =>
        {
            ChangeValue(changeEvent);
        });

        // base.Localize(root);
    }

    private void UpdateTotalCost()
    {
        // foreach (KeyValuePair<Label, int> label in _labelsTotalCost)
        // {
        //     label.Key.text = (_hireCount * label.Value).ToString();
        // }
        CreateBlokTotal();
        if (_hireCount > 0 && _activePlayer.IsExistsResource(_costEntities))
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
        _costEntities.Clear();

        foreach (var res in _creature.CreatureParams.Cost)
        {
            _costEntities.Add(new CostEntity()
            {
                Count = _hireCount * res.Count,
                Resource = res.Resource
            });
            var itemResource = _templateItem.Instantiate();
            // itemResource.style.flexGrow = 1;
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

    public async Task<DataResultDialogDwelling> ProcessAction(EntityDwelling dwelling)
    {
        base.Init();

        _dwelling = dwelling;
        ScriptableEntityDwelling configData
            = (ScriptableEntityDwelling)_dwelling.ScriptableData;
        _creature = configData.Creature[_dwelling.Data.level];
        _dataResultDialog = new DataResultDialogDwelling();

        var nameCreature = configData.Creature[_dwelling.Data.level].Text.title.IsEmpty ?
            "" : configData.Creature[_dwelling.Data.level].Text.title.GetLocalizedString();

        _availableCountLabel.text = _dwelling.Data.value.ToString();
        _sliderValue.highValue = _dwelling.Data.value;
        _sliderValue.lowValue = _sliderValue.value = 0;

        var title = _creature.Text.title.GetLocalizedString();
        LocalizedString textHire = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "hire");
        Title.text = textHire.GetLocalizedString() + "<color=#FFFFAB>" + title + "</color>";

        root.Q<Label>("TitleHire").text = textHire.GetLocalizedString();
        _buttonOk.text = textHire.GetLocalizedString();

        var _spriteLevel1 = root.Q<VisualElement>("Level1");
        _spriteLevel1.style.backgroundImage = new StyleBackground(_creature.MenuSprite);
        var _spriteLevel2 = root.Q<VisualElement>("Level2");
        _spriteLevel2.style.backgroundImage = new StyleBackground(_creature.MenuSprite);


        foreach (var res in _creature.CreatureParams.Cost)
        {
            var itemResource = _templateItem.Instantiate();
            // itemResource.style.flexGrow = 1;
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

        _processCompletionSource = new TaskCompletionSource<DataResultDialogDwelling>();

        return await _processCompletionSource.Task;
    }

    private async void OnClickOk()
    {
        if (_hireCount > 0)
        {
            foreach (var res in _costEntities)
            {
                Debug.Log($"res {res.Resource.TypeResource}->{res.Count}");
            }
            if (!_activePlayer.IsExistsResource(_costEntities))
            {
                // Show dialog no resources.
                Debug.Log("Show dialog no resources.");
            }
            else
            {

                var indexVacantCreature = -1;
                for (var i = 0; i < _activePlayer.ActiveHero.Data.Creatures.Count; i++)
                {
                    if (
                        _activePlayer.ActiveHero.Data.Creatures.GetValueOrDefault(i) == null
                        || _activePlayer.ActiveHero.Data.Creatures.GetValueOrDefault(i).IdObject
                            == _dwelling.ConfigData.Creature[_dwelling.Data.level].idObject)
                    {
                        indexVacantCreature = i;
                        break;
                    }
                }
                if (indexVacantCreature == -1)
                {
                    // Show dialog - not place.
                    var dialogData = new DataDialogHelp()
                    {
                        Header = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "Help").GetLocalizedString(),
                        Description = new LocalizedString(Constants.LanguageTable.LANG_TABLE_ADVENTURE, "novacantplace").GetLocalizedString(),
                    };

                    var dialogWindow = new DialogHelpProvider(dialogData);
                    await dialogWindow.ShowAndHide();
                }
                else
                {
                    var newCreature = _dwelling.BuyCreatures(_dwelling.Data.level, _hireCount);
                    _activePlayer.ActiveHero.Data.Creatures[indexVacantCreature] = newCreature;
                    OnBuyCreature?.Invoke();
                }
            }
        }

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

