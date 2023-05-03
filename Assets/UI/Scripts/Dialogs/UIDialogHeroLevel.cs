using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.UIElements;

public class UIDialogHeroLevel : UIDialogBaseWindow
{
    [SerializeField] private VisualTreeAsset _templateSecondarySkill;
    public static event Action OnMoveCreature;
    private Button _buttonOk;
    protected TaskCompletionSource<DataResultDialogLevelHero> _processCompletionSource;
    protected DataResultDialogLevelHero _dataResultDialog;
    private DataDialogLevelHero _data;
    private VisualElement _avaHero;
    private VisualElement _secondSkillBlok;

    public override void Start()
    {
        base.Start();

        // Panel.AddToClassList("w-full");
        Panel.AddToClassList("h-full");
        Title.style.display = DisplayStyle.None;

        _avaHero = root.Q<VisualElement>("Ava");
        _secondSkillBlok = root.Q<VisualElement>("SecondSkills");
        _buttonOk = root.Q<VisualElement>("Ok").Q<Button>("Btn");
        _buttonOk.clickable.clicked += OnClickOk;
        _buttonOk.SetEnabled(false);

    }

    public async Task<DataResultDialogLevelHero> ProcessAction(DataDialogLevelHero data)
    {
        base.Init();

        _dataResultDialog = new DataResultDialogLevelHero();
        _processCompletionSource = new TaskCompletionSource<DataResultDialogLevelHero>();

        _data = data;
        FillSecondarySkills();

        if (_data.sprite != null)
        {
            _avaHero.style.backgroundImage = new StyleBackground(_data.sprite);
        }

        // _nameHero.text = _hero.Data.name;
        // LocalizedString textLevel = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "level");
        // _descriptionHero.text
        //     = textLevel.GetLocalizedString()
        //     + " " + _hero.Data.level
        //     + ", " + configDataHero.ClassHero.name;
        var dataPlural = new Dictionary<string, string> {
            { "name", Helpers.GetColorString(_data.name) },
            { "gender", _data.gender },
            };
        var arguments = new[] { dataPlural };
        var titlePlural = Helpers.GetLocalizedPluralString(
            new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "nextlevel_run"),
            arguments,
            dataPlural
            );
        root.Q<Label>("TitleLevel").text = titlePlural;
        LocalizedString textCurrentLevel = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "nextlevel_current")
        {
            { "name", new StringVariable { Value = Helpers.GetColorString(_data.name) } },
            { "level", new StringVariable { Value = Helpers.GetColorString(_data.level) } },
        };
        root.Q<Label>("TextCurrentLevel").text = textCurrentLevel.GetLocalizedString();

        var firstSkill = _data.SecondarySkills.ElementAt(0);
        var skillConfigData1 = ResourceSystem.Instance
            .GetAttributesByType<ScriptableAttributeSecondarySkill>(TypeAttribute.SecondarySkill)
            .Where(t => t.TypeTwoSkill == firstSkill.Key).First();
        var secondSkill = _data.SecondarySkills.ElementAt(1);
        var skillConfigData2 = ResourceSystem.Instance
            .GetAttributesByType<ScriptableAttributeSecondarySkill>(TypeAttribute.SecondarySkill)
            .Where(t => t.TypeTwoSkill == secondSkill.Key).First();

        dataPlural = new Dictionary<string, string> {
            { "skill1", Helpers.GetColorString(skillConfigData1.Levels[firstSkill.Value].Title.GetLocalizedString()) },
            { "skill2", Helpers.GetColorString(skillConfigData2.Levels[secondSkill.Value].Title.GetLocalizedString()) },
            };
        arguments = new[] { dataPlural };
        titlePlural = Helpers.GetLocalizedPluralString(
            new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "nextlevel_sskils"),
            arguments,
            dataPlural
            );
        root.Q<Label>("TextMybeLernen").text = titlePlural;

        root.Q<VisualElement>("ImgPrimarySkill").style.backgroundImage
            = new StyleBackground(_data.PrimarySkill.MenuSprite);
        root.Q<Label>("NamePrimarySkill").text = _data.PrimarySkill.Text.title.GetLocalizedString();

        return await _processCompletionSource.Task;
    }

    private void OnClickOk()
    {
        _dataResultDialog.isOk = true;
        _processCompletionSource.SetResult(_dataResultDialog);
    }

    private void FillSecondarySkills()
    {
        _secondSkillBlok.Clear();
        foreach (var skill in _data.SecondarySkills)
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

            var blok = _templateSecondarySkill.Instantiate();
            var btnInBlok = blok.Q<Button>("Btn");
            btnInBlok.RegisterCallback<ClickEvent>((ClickEvent evt) =>
            {
                _dataResultDialog.typeSecondarySkill = skill.Key;
                ResetClassButton();
                btnInBlok.AddToClassList("button_checked");
                btnInBlok.AddToClassList("border-color");
                btnInBlok.RemoveFromClassList("button_bordered");
                _buttonOk.SetEnabled(true);
            });
            btnInBlok.AddToClassList("w-33");
            btnInBlok.Q<VisualElement>("Img").style.backgroundImage
                = new StyleBackground(skillConfigData.Levels[skill.Value].Sprite);
            btnInBlok.Q<Label>("Type").text = type;
            btnInBlok.Q<Label>("Title").text = skillConfigData.Text.title.GetLocalizedString();
            _secondSkillBlok.Add(btnInBlok);
            // newBlok.RegisterCallback<ClickEvent>((ClickEvent evt) => ShowInfoSecondarySkill(skillConfigData, skill.Value));
        }
    }
    private void ResetClassButton()
    {
        UQueryBuilder<Button> builder = new UQueryBuilder<Button>(_secondSkillBlok);
        List<Button> list = builder.Name("Btn").ToList();

        foreach (var btn in list)
        {
            btn.RemoveFromClassList("button_checked");
            btn.RemoveFromClassList("border-color");
            btn.AddToClassList("button_bordered");
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

