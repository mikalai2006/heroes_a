using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;

public class UIDialogSplitCreatureWindow : UIDialogBaseWindow
{
    [SerializeField] private VisualTreeAsset _templateItem;
    private Button _buttonOk;
    private Button _buttonCancel;
    private Label _value1Label;
    private Label _value2Label;
    private int value1;
    private int value2;
    private SliderInt _slider;
    private float sliderValue;
    protected TaskCompletionSource<DataResultDialogSplitCreature> _processCompletionSource;
    protected DataResultDialogSplitCreature _dataResultDialog;
    private EntityCreature _startCreature;
    private EntityCreature _endCreature;
    private int _max;

    public override void Start()
    {
        base.Start();

        _buttonOk = root.Q<VisualElement>("Ok").Q<Button>("Btn");
        _buttonOk.clickable.clicked += OnClickOk;

        _buttonCancel = root.Q<VisualElement>("Cancel").Q<Button>("Btn");
        _buttonCancel.clickable.clicked += OnClickCancel;

        _value1Label = root.Q<Label>("Value1");
        _value2Label = root.Q<Label>("Value2");

        _slider = root.Q<SliderInt>("Slider");
        _slider.RegisterValueChangedCallback((changeEvent) =>
        {
            ChangeValue(changeEvent.newValue);
        });
    }

    private void ChangeValue(int newValue)
    {
        sliderValue = newValue;
        value1 = (int)(_max - sliderValue);
        value2 = (int)(sliderValue);
        _value1Label.text = value1.ToString();
        _value2Label.text = (sliderValue).ToString();
    }

    public async Task<DataResultDialogSplitCreature> ProcessAction(
        EntityCreature startCreature,
        EntityCreature endCreature
    )
    {
        base.Init();


        _dataResultDialog = new DataResultDialogSplitCreature();
        _processCompletionSource = new TaskCompletionSource<DataResultDialogSplitCreature>();

        _startCreature = startCreature;
        _endCreature = endCreature;
        ScriptableEntityCreature configDataValue1
            = (ScriptableEntityCreature)_startCreature.ScriptableData;

        LocalizedString textSplit = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "split");
        var title = textSplit.GetLocalizedString() + ": " + _startCreature.ConfigData.title.GetLocalizedString();
        Title.text = title;

        if (endCreature != null)
        {
            ScriptableEntityCreature configDataValue2
                = (ScriptableEntityCreature)_endCreature.ScriptableData;
        }

        var _spriteLevel1 = root.Q<VisualElement>("Creature1");
        _spriteLevel1.style.backgroundImage = new StyleBackground(configDataValue1.MenuSprite);
        var _spriteLevel2 = root.Q<VisualElement>("Creature2");
        _spriteLevel2.style.backgroundImage = new StyleBackground(configDataValue1.MenuSprite);

        _max = _startCreature.Data.value + (endCreature != null ? _endCreature.Data.value : 0);
        _slider.highValue = _max;
        _slider.lowValue = 0;
        _slider.value = _max - _startCreature.Data.value;

        ChangeValue(_slider.value);

        return await _processCompletionSource.Task;
    }

    private void OnClickOk()
    {
        _dataResultDialog.isOk = true;
        _dataResultDialog.value1 = value1;
        _dataResultDialog.value2 = value2;
        _processCompletionSource.SetResult(_dataResultDialog);

    }
    private void OnClickCancel()
    {
        _dataResultDialog.isOk = false;
        _processCompletionSource.SetResult(_dataResultDialog);
    }
}

