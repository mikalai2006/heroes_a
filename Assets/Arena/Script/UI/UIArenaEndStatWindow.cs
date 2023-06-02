using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;

public class UIArenaEndStatWindow : UIDialogBaseWindow
{
    [SerializeField] private VisualTreeAsset _templateCreatureInfoSmall;
    private readonly string _nameButtonClose = "Cancel";
    private Button _buttonClose;
    private VisualElement _AttackedDeadCreature;
    private VisualElement _DefendedDeadCreature;
    public static event Action OnCloseStat;
    protected TaskCompletionSource<ArenaStatResult> _processCompletionSource;
    protected ArenaStatResult _dataResultDialog;
    private ArenaStatData _statData;

    public override void Start()
    {
        base.Start();

        Title.style.display = DisplayStyle.None;
        Panel.AddToClassList("w-75");

        _buttonClose = DialogApp.rootVisualElement.Q<TemplateContainer>(_nameButtonClose).Q<Button>("Btn");
        _buttonClose.clickable.clicked += OnClickClose;

        _DefendedDeadCreature = root.Q<VisualElement>("DefendedDeadCreature");
        _DefendedDeadCreature.Clear();

        _AttackedDeadCreature = root.Q<VisualElement>("AttackedDeadCreature");
        _AttackedDeadCreature.Clear();
    }

    private async void OnClickClose()
    {
        await AudioManager.Instance.Click();
        _dataResultDialog.isOk = false;
        _processCompletionSource.SetResult(_dataResultDialog);
        OnCloseStat?.Invoke();
    }

    public async Task<ArenaStatResult> ProcessAction(ArenaStatData stat)
    {
        base.Init();
        _statData = stat;

        _dataResultDialog = new ArenaStatResult();
        _processCompletionSource = new TaskCompletionSource<ArenaStatResult>();

        DrawStat();

        return await _processCompletionSource.Task;
    }

    private void DrawStat()
    {
        DrawDeadedCreatures(_statData.arenaManager.ArenaStat.DeadedCreaturesAttacked, _AttackedDeadCreature);
        DrawDeadedCreatures(_statData.arenaManager.ArenaStat.DeadedCreaturesDefendied, _DefendedDeadCreature);

        var textMoveVictorious = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "victorious").GetLocalizedString();
        var textDefendied = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "defeated").GetLocalizedString();

        LocalizedString titleStat = new();
        LocalizedString descriptionStat = new();
        switch (_statData.arenaManager.heroLeft.typearenaHeroStatus)
        {
            case TypearenaHeroStatus.Victorious:
                titleStat = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "yourwin_t");
                descriptionStat = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "yourwin_d");
                break;
            case TypearenaHeroStatus.Defendied:
                titleStat = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "yourdefendied_t");
                descriptionStat = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "yourdefendied_d");
                break;
            case TypearenaHeroStatus.Runned:
                titleStat = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "hero_run_t");
                descriptionStat = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "hero_run_d");
                break;
            case TypearenaHeroStatus.PayOffed:
                titleStat = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "hero_payoff_t");
                descriptionStat = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "hero_payoff_d");
                break;
        }

        var hero = ((EntityHero)_statData.arenaManager.heroLeft.Entity);
        var dataSmart = new Dictionary<string, string> {
            { "value", _statData.arenaManager.ArenaStat.totalExperienceHero.ToString() },
            { "name", hero.Data.name },
            { "gender", hero.ConfigData.TypeGender.ToString() }
            };
        var arguments = new[] { dataSmart };
        var titleSmart = Helpers.GetLocalizedPluralString(
            titleStat,
            arguments,
            dataSmart
            );
        var descriptionSmart = Helpers.GetLocalizedPluralString(
            descriptionStat,
            arguments,
            dataSmart
            );

        root.Q<Label>("StatResult").text
            = string.Format("{0}\r\n\r\n{1}", titleSmart, descriptionSmart);

        root.Q<Label>("Status1").text = textMoveVictorious;
        root.Q<Label>("LeftName").text = hero.Data.name;
        root.Q<VisualElement>("LeftAva").style.backgroundImage
                = new StyleBackground(hero.ConfigData.MenuSprite);

        var enemy = ((EntityHero)_statData.arenaManager.heroRight.Entity);
        root.Q<Label>("Status2").text = textDefendied;
        if (enemy != null)
        {
            root.Q<Label>("RightName").text = enemy.Data.name;
            root.Q<VisualElement>("RightAva").style.backgroundImage
                    = new StyleBackground(enemy.ConfigData.MenuSprite);
        }
        else
        {
            var creature = ((EntityCreature)_statData.arenaManager.heroRight.Data.ArenaCreatures.First().Value.Entity);
            root.Q<Label>("RightName").text = creature.ConfigAttribute.Text.title.GetLocalizedString();
            root.Q<VisualElement>("RightAva").style.backgroundImage
                    = new StyleBackground(creature.ConfigAttribute.MenuSprite);
        }
    }

    private void DrawDeadedCreatures(Dictionary<ArenaEntityBase, int> deadedCreatures, VisualElement parentElement)
    {
        if (deadedCreatures.Count > 0)
        {
            foreach (var creatureItem in deadedCreatures)
            {
                EntityCreature creature = (EntityCreature)creatureItem.Key.Entity;

                VisualElement creatureElement = _templateCreatureInfoSmall.Instantiate();
                creatureElement.AddToClassList("pr-1");
                creatureElement.Q<VisualElement>("Img").style.backgroundImage
                    = new StyleBackground(creature.ConfigAttribute.MiniSprite);
                creatureElement.Q<Label>("Value").text = string.Format("{0}", creatureItem.Value);
                parentElement.Add(creatureElement);
            }
        }
        else
        {
            var noText = new Label();
            noText.AddToClassList("text-lg");
            noText.text = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "no").GetLocalizedString();
            parentElement.Add(noText);
        }
    }
}

