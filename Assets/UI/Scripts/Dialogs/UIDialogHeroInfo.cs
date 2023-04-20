using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;

public class UIDialogHeroInfo : UIDialogBaseWindow
{
    [SerializeField] private VisualTreeAsset _templateHeroCreature;
    [SerializeField] private VisualTreeAsset _templateItem;
    private Button _buttonOk;
    private Button _buttonCancel;
    protected TaskCompletionSource<DataResultDialogHeroInfo> _processCompletionSource;
    protected DataResultDialogHeroInfo _dataResultDialog;
    private EntityHero _hero;
    private VisualElement _creaturesBlok;
    private VisualElement _avaHero;
    private Label _nameHero;
    private Label _descriptionHero;
    private Button _buttonSplitCreature;
    private bool _isSplit;
    private SerializableDictionary<int, EntityCreature> _startCheckedCreatures
        = new SerializableDictionary<int, EntityCreature>();
    private int _startPositionChecked = -1;

    public override void Start()
    {
        base.Start();

        Panel.AddToClassList("w-full");
        Panel.AddToClassList("h-full");
        // _buttonOk = root.Q<VisualElement>("Ok").Q<Button>("Btn");
        // _buttonOk.clickable.clicked += OnClickOk;

        _avaHero = root.Q<VisualElement>("Ava");
        _nameHero = root.Q<Label>("Name");
        _descriptionHero = root.Q<Label>("Description");

        _creaturesBlok = root.Q<VisualElement>("Creatures");

        _buttonCancel = root.Q<VisualElement>("Cancel").Q<Button>("Btn");
        _buttonCancel.clickable.clicked += OnClickCancel;

        _buttonSplitCreature = root.Q<TemplateContainer>("SplitCreature").Q<Button>("Btn");
        _buttonSplitCreature.clickable.clicked += () =>
        {
            _isSplit = true;
        };
        _buttonSplitCreature.SetEnabled(false);

    }

    public async Task<DataResultDialogHeroInfo> ProcessAction(
        EntityHero hero
    )
    {
        base.Init();


        _dataResultDialog = new DataResultDialogHeroInfo();
        _processCompletionSource = new TaskCompletionSource<DataResultDialogHeroInfo>();

        _hero = hero;
        FillCreaturesBlok();

        _nameHero.text = _hero.Data.name;
        _descriptionHero.text = _hero.Data.name + " 11 уровень";
        if (_hero.ConfigData.MenuSprite != null)
        {
            _avaHero.style.backgroundImage = new StyleBackground(_hero.ConfigData.MenuSprite);
        }
        ScriptableEntityHero configDataHero
            = (ScriptableEntityHero)_hero.ScriptableData;
        Title.style.display = DisplayStyle.None;



        return await _processCompletionSource.Task;
    }

    // private void OnClickOk()
    // {
    //     _dataResultDialog.isOk = true;
    //     _processCompletionSource.SetResult(_dataResultDialog);

    // }
    private void OnClickCancel()
    {
        _dataResultDialog.isOk = false;
        _processCompletionSource.SetResult(_dataResultDialog);
    }

    private void FillCreaturesBlok()
    {
        _creaturesBlok.Clear();

        for (int i = 0; i < 7; i++)
        {
            var index = i;
            var itemHeroForce = _templateHeroCreature.Instantiate();
            itemHeroForce.name = "creature";
            itemHeroForce.AddToClassList("w-full");
            itemHeroForce.AddToClassList("h-14");

            SerializableDictionary<int, EntityCreature> creatures = new SerializableDictionary<int, EntityCreature>();
            if (_hero != null)
            {
                creatures = _hero.Data.Creatures;
            }

            EntityCreature creature;
            creatures.TryGetValue(i, out creature);

            if (creature != null)
            {
                itemHeroForce.Q<VisualElement>("img").style.backgroundImage
                    = new StyleBackground(creature.ScriptableData.MenuSprite);
                itemHeroForce.Q<Label>("ForceValue").text = creature.Data.value.ToString();
            }
            itemHeroForce.RegisterCallback<ClickEvent>((ClickEvent evt) =>
            {
                if (_startPositionChecked == -1)
                {
                    OnChooseCreature(index, _hero.Data.Creatures);
                }
                else
                {
                    OnMoveCreature(index, _hero.Data.Creatures);
                };
            });

            _creaturesBlok.Add(itemHeroForce);
        }

    }

    private void OnChooseCreature(int index, SerializableDictionary<int, EntityCreature> creatures)
    {
        if (creatures[index] == null) return;
        _buttonSplitCreature.SetEnabled(true);

        UQueryBuilder<VisualElement> builder = new UQueryBuilder<VisualElement>(_creaturesBlok);
        List<VisualElement> list = builder.Name("creature").ToList();
        for (int i = 0; i < list.Count; i++)
        {
            var item = list[i];
            item.Q<Button>("HeroInfoForce").AddToClassList("button_active");
            item.SetEnabled(true);
        }

        _startCheckedCreatures = creatures;
        _startPositionChecked = index;
    }

    private async void OnMoveCreature(int index, SerializableDictionary<int, EntityCreature> creatures)
    {
        if (creatures[index] == _startCheckedCreatures[_startPositionChecked])
        {
            // if (_isSplit)
            // {
            //     // Show dialog split creatures.
            // }
            // else
            // {
            // Show dialog info creature.
            var dialogWindow = new UIInfoCreatureOperation();
            var result = await dialogWindow.ShowAndHide();
            if (result.isOk)
            {

            }
            // }
        }
        else
        {
            if (
                _startCheckedCreatures.Where(t => t.Value != null).Count() == 1
                && _startCheckedCreatures != creatures
                && creatures[index] == null
                && !_isSplit)
            {
                var dialogData = new DataDialogHelp()
                {
                    Header = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "Help").GetLocalizedString(),
                    Description = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "hero_lastcreature").GetLocalizedString(),
                };
                var dialogWindow = new DialogHelpProvider(dialogData);
                await dialogWindow.ShowAndHide();
            }
            else if (
                _isSplit
                && (
                    creatures[index] == null
                    || creatures[index].IdObject == _startCheckedCreatures[_startPositionChecked].IdObject
                    )
                )
            {
                // Show dialog split creatures.
                var dialogWindow = new DialogSplitCreatureOperation(
                    _startCheckedCreatures[_startPositionChecked],
                    creatures[index]
                    );
                var result = await dialogWindow.ShowAndHide();
                if (result.isOk)
                {
                    Helpers.MoveUnitBetweenList(
                        ref _startCheckedCreatures,
                        _startPositionChecked,
                        ref creatures,
                        index,
                        result.value1,
                        result.value2
                        );
                }
            }
            else
            {
                Helpers.MoveUnitBetweenList(
                    ref _startCheckedCreatures,
                    _startPositionChecked,
                    ref creatures,
                    index
                    );
            }
        }

        UQueryBuilder<VisualElement> builderGuest
                = new UQueryBuilder<VisualElement>(root);
        List<VisualElement> listGuestCreature = builderGuest.Name("creature").ToList();
        foreach (var item in listGuestCreature)
        {
            item.Q<Button>("HeroInfoForce").RemoveFromClassList("button_active");
        }
        FillCreaturesBlok();
        _isSplit = false;
        _buttonSplitCreature.SetEnabled(false);
        _startPositionChecked = -1;
    }
}

