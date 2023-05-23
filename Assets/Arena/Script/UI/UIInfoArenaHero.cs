using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;

public class UIInfoArenaHero : UIDialogBaseWindow
{
    [SerializeField] private VisualTreeAsset _templateSecondarySkill;
    // public static event Action OnMoveCreature;
    private Button _buttonCancel;
    protected TaskCompletionSource<DataResultDialogHeroInfo> _processCompletionSource;
    protected DataResultDialogHeroInfo _dataResultDialog;
    private EntityHero _hero;
    private VisualElement _avaHero;
    private VisualElement _secondSkillBlok;
    private Button _spellBook;
    private Label _attack;
    private Label _defense;
    private Label _knowledge;
    private Label _power;
    private Label _mana;
    private Label _experience;
    private Label _nameHero;
    private Label _descriptionHero;


    public override void Start()
    {
        base.Start();

        Panel.AddToClassList("w-50");
        Panel.AddToClassList("h-full");

        _avaHero = root.Q<VisualElement>("Ava");
        _nameHero = root.Q<Label>("Name");
        _descriptionHero = root.Q<Label>("Description");

        _secondSkillBlok = root.Q<VisualElement>("SecondSkills");
        _attack = root.Q<Label>("Attack");
        _defense = root.Q<Label>("Defense");
        _knowledge = root.Q<Label>("Knowledge");
        _power = root.Q<Label>("Power");
        _mana = root.Q<Label>("Mana");
        _experience = root.Q<Label>("Experience");

        _buttonCancel = root.Q<VisualElement>("Cancel").Q<Button>("Btn");
        _buttonCancel.clickable.clicked += OnClickCancel;

    }

    public async Task<DataResultDialogHeroInfo> ProcessAction(EntityHero hero)
    {
        base.Init();

        _dataResultDialog = new DataResultDialogHeroInfo();
        _processCompletionSource = new TaskCompletionSource<DataResultDialogHeroInfo>();

        _hero = hero;

        _mana.text = string.Format("{0}/{1}", _hero.Data.mana, _hero.GetMana());

        FillPrimarySkills();
        FillSecondarySkills();

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
            + ", " + configDataHero.ClassHero.Text.title.GetLocalizedString();

        return await _processCompletionSource.Task;
    }

    private async void OnClickCancel()
    {
        await AudioManager.Instance.Click();
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
            switch (skill.Value.level)
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
                = new StyleBackground(skillConfigData.Levels[skill.Value.level].Sprite);
            newBlok.Q<Label>("Type").text = type;
            newBlok.Q<Label>("Title").text = skillConfigData.Text.title.GetLocalizedString();
            _secondSkillBlok.Add(newBlok);
            newBlok.RegisterCallback<ClickEvent>(async (ClickEvent evt) =>
            {
                await AudioManager.Instance.Click();
                ShowInfoSecondarySkill(skillConfigData, skill.Value.level);
            });
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
}

