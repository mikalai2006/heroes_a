using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.Localization;

public class UIDialogSpellBook : UIDialogBaseWindow
{
    [SerializeField] private VisualTreeAsset _templateSpellItem;
    [SerializeField] private VisualTreeAsset _templateButton;

    private Button _buttonOk;
    private VisualElement _spellList1;
    private VisualElement _spellList2;

    private TaskCompletionSource<DataResultDialogSpellBook> _processCompletionSource;

    public UnityEvent processAction;

    private EntityHero _hero;
    private DataResultDialogSpellBook _dataResultDialog;

    public override void Start()
    {
        base.Start();

        Panel.AddToClassList("w-full");
        Panel.AddToClassList("h-full");
        Title.style.display = DisplayStyle.None;

        _buttonOk = root.Q<Button>("Close");
        _buttonOk.clickable.clicked += OnClickClose;

        _spellList1 = root.Q<VisualElement>("SpellList1");
        _spellList1.Clear();
        // _spellList2 = root.Q<VisualElement>("SpellList2");

        // base.Localize(root);
    }

    public async Task<DataResultDialogSpellBook> ProcessAction(EntityHero hero)
    {
        base.Init();

        _hero = hero;
        _dataResultDialog = new DataResultDialogSpellBook()
        {
        };

        foreach (var spellData in _hero.Data.SpellBook.Data.Spells)
        {
            var schoolSpell = spellData.SchoolMagic;
            var levelSpell = -1;
            if (_hero.Data.SSkills.ContainsKey(schoolSpell.BaseSecondarySkill.TypeTwoSkill))
            {
                levelSpell = _hero.Data.SSkills[schoolSpell.BaseSecondarySkill.TypeTwoSkill];
            }

            var newNodeElementSpell = new VisualElement();
            newNodeElementSpell.AddToClassList("w-33");
            newNodeElementSpell.AddToClassList("h-33");

            var newBlokSpell = _templateSpellItem.Instantiate();
            newBlokSpell.style.flexGrow = 1;
            newNodeElementSpell.Add(newBlokSpell);
            newBlokSpell.Q<VisualElement>("Lavra").style.backgroundImage
                = new StyleBackground(schoolSpell.Sp[levelSpell + 1]);
            newBlokSpell.Q<VisualElement>("Img").style.backgroundImage
                = new StyleBackground(spellData.MenuSprite);
            newBlokSpell.Q<Label>("Name").text = spellData.Text.title.GetLocalizedString();

            var dataSmart = new Dictionary<string, int> {
            { "level", spellData.level },
            };
            var arguments = new[] { dataSmart };
            var titleLevel = Helpers.GetLocalizedPluralString(
                new LocalizedString(Constants.LanguageTable.LANG_TABLE_SPELLBOOK, "levelshort"),
                arguments,
                dataSmart
                );
            dataSmart = new Dictionary<string, int> {
            { "level", levelSpell },
            };
            arguments = new[] { dataSmart };
            var levelSchool = Helpers.GetLocalizedPluralString(
                new LocalizedString(Constants.LanguageTable.LANG_TABLE_SPELLBOOK, "levelsskilshort"),
                arguments,
                dataSmart
                );
            newBlokSpell.Q<Label>("Level").text = string.Format("{0}{1}{2}", spellData.level, titleLevel, levelSchool);

            var manaText = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "mana");
            newBlokSpell.Q<Label>("Mana").text
                = string.Format("{0}: {1}", manaText.GetLocalizedString(), spellData.LevelData[0].cost);
            _spellList1.Add(newNodeElementSpell);
        }

        _processCompletionSource = new TaskCompletionSource<DataResultDialogSpellBook>();

        return await _processCompletionSource.Task;
    }

    private void OnClickClose()
    {
        _dataResultDialog.isOk = true;
        _processCompletionSource.SetResult(_dataResultDialog);

        processAction?.Invoke();
    }
}

