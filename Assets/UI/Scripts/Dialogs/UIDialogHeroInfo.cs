using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;

public class UIDialogHeroInfo : UIDialogBaseWindow
{
    [SerializeField] private VisualTreeAsset _templateHeroCreature;
    [SerializeField] private VisualTreeAsset _templateSecondarySkill;
    [SerializeField] private VisualTreeAsset _templateArtifact;
    public static event Action OnMoveCreature;
    private Button _buttonOk;
    private Button _buttonCancel;
    protected TaskCompletionSource<DataResultDialogHeroInfo> _processCompletionSource;
    protected DataResultDialogHeroInfo _dataResultDialog;
    private EntityHero _hero;
    private VisualElement _creaturesBlok;
    private VisualElement _avaHero;
    private VisualElement _secondSkillBlok;
    private VisualElement _listArtifacts;
    private Label _attack;
    private Label _defense;
    private Label _knowledge;
    private Label _power;
    private Label _experience;
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

        _secondSkillBlok = root.Q<VisualElement>("SecondSkills");
        _attack = root.Q<Label>("Attack");
        _defense = root.Q<Label>("Defense");
        _knowledge = root.Q<Label>("Knowledge");
        _power = root.Q<Label>("Power");
        _experience = root.Q<Label>("Experience");

        _listArtifacts = root.Q<VisualElement>("ListArtifact");

        _buttonCancel = root.Q<VisualElement>("Cancel").Q<Button>("Btn");
        _buttonCancel.clickable.clicked += OnClickCancel;

        _buttonSplitCreature = root.Q<TemplateContainer>("SplitCreature").Q<Button>("Btn");
        _buttonSplitCreature.clickable.clicked += () =>
        {
            _isSplit = true;
        };
        _buttonSplitCreature.SetEnabled(false);

    }

    public async Task<DataResultDialogHeroInfo> ProcessAction(EntityHero hero)
    {
        base.Init();

        _dataResultDialog = new DataResultDialogHeroInfo();
        _processCompletionSource = new TaskCompletionSource<DataResultDialogHeroInfo>();

        _hero = hero;
        FillPrimarySkills();
        FillSecondarySkills();
        FillCreaturesBlok();
        FillArtifactsBlok();

        ScriptableEntityHero configDataHero = _hero.ConfigData;

        if (configDataHero.MenuSprite != null)
        {
            _avaHero.style.backgroundImage = new StyleBackground(configDataHero.MenuSprite);
        }
        Title.style.display = DisplayStyle.None;

        _nameHero.text = _hero.Data.name;
        LocalizedString textLevel = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "level");
        _descriptionHero.text
            = textLevel.GetLocalizedString()
            + " " + _hero.Data.level
            + ", " + configDataHero.ClassHero.name;

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

    private void FillPrimarySkills()
    {
        _attack.text = _hero.Data.PSkills.GetValueOrDefault(TypePrimarySkill.Attack).ToString();
        _defense.text = _hero.Data.PSkills.GetValueOrDefault(TypePrimarySkill.Defense).ToString();
        _knowledge.text = _hero.Data.PSkills.GetValueOrDefault(TypePrimarySkill.Knowledge).ToString();
        _power.text = _hero.Data.PSkills.GetValueOrDefault(TypePrimarySkill.Power).ToString();
        _experience.text = _hero.Data.PSkills.GetValueOrDefault(TypePrimarySkill.Experience).ToString();
    }

    private void FillSecondarySkills()
    {
        _secondSkillBlok.Clear();
        foreach (var skill in _hero.Data.SSkills)
        {
            var skillConfigData = ResourceSystem.Instance
                .GetAttributesByType<ScriptableAttributeSecondarySkill>(TypeAttribute.SecondarySkill)
                .Where(t => t.TypeTwoSkill == skill.Key).First();

            string type = "";
            switch (skill.Value)
            {
                case 0:
                    type = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "basic").GetLocalizedString();
                    break;
                case 1:
                    type = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "advanced").GetLocalizedString();
                    break;
                case 2:
                    type = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "expert").GetLocalizedString();
                    break;
            }
            var newBlok = _templateSecondarySkill.Instantiate();
            newBlok.AddToClassList("w-50");
            newBlok.Q<VisualElement>("Img").style.backgroundImage
                = new StyleBackground(skillConfigData.Levels[skill.Value].Sprite);
            newBlok.Q<Label>("Type").text = type;
            newBlok.Q<Label>("Title").text = skillConfigData.Text.title.GetLocalizedString();
            _secondSkillBlok.Add(newBlok);
            newBlok.RegisterCallback<ClickEvent>((ClickEvent evt) => ShowInfoSecondarySkill(skillConfigData, skill.Value));
        }
        for (int i = _hero.Data.SSkills.Count; i < 8; i++)
        {
            var newBlok = _templateSecondarySkill.Instantiate();
            newBlok.AddToClassList("w-50");
            newBlok.Q<Label>("Title").text
                = "";
            _secondSkillBlok.Add(newBlok);
        }
    }

    private async void ShowInfoSecondarySkill(ScriptableAttributeSecondarySkill configData, int level)
    {
        var dialogData = new DataDialogHelp()
        {
            Header = configData.Levels[level].Title.GetLocalizedString(),
            Description = configData.Levels[level].Description.GetLocalizedString(),
        };

        var dialogWindow = new DialogHelpProvider(dialogData);
        await dialogWindow.ShowAndHide();
    }
    private void FillArtifactsBlok()
    {
        _listArtifacts.Clear();

        foreach (var idObject in _hero.Data.artifacts)
        {
            var artifact = ResourceSystem.Instance
                .GetAttributesByType<ScriptableAttributeArtifact>(TypeAttribute.Artifact)
                .Find(t => t.idObject == idObject);
            if (artifact != null)
            {
                var box = _templateArtifact.Instantiate();
                box.Q<VisualElement>("img").style.backgroundImage
                    = new StyleBackground(artifact.MenuSprite);

                _listArtifacts.Add(box);
            }
        }

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
                    ChooseCreature(index, _hero.Data.Creatures);
                }
                else
                {
                    MoveCreature(index, _hero.Data.Creatures);
                };
            });

            _creaturesBlok.Add(itemHeroForce);
        }

    }

    private void ChooseCreature(int index, SerializableDictionary<int, EntityCreature> creatures)
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

    private async void MoveCreature(int index, SerializableDictionary<int, EntityCreature> creatures)
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
                    || creatures[index].IdEntity == _startCheckedCreatures[_startPositionChecked].IdEntity
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
                    OnMoveCreature?.Invoke();
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
                OnMoveCreature?.Invoke();
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

