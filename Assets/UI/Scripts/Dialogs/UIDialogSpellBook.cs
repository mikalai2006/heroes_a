using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.Localization;
using System;

public class UIDialogSpellBook : UIDialogBaseWindow
{
    [SerializeField] private VisualTreeAsset _templateSpellItem;
    [SerializeField] private VisualTreeAsset _templateButton;
    public static event Action OnClickSpell;

    private Button _buttonOk;
    private Button _buttonAir;
    private Button _buttonWater;
    private Button _buttonFire;
    private Button _buttonEarth;
    private Button _buttonAll;
    private Button _buttonCombat;
    private Button _buttonAdv;
    private Button _buttonPrev;
    private Button _buttonNext;
    private VisualElement _spellList1;
    private Label _mana;
    private VisualElement _spellList2;
    private TaskCompletionSource<DataResultDialogSpellBook> _processCompletionSource;

    // public UnityEvent processAction;

    private EntityHero _hero;
    private DataResultDialogSpellBook _dataResultDialog;

    public override void Start()
    {
        base.Start();

        Panel.AddToClassList("w-full");
        Panel.AddToClassList("h-full");
        Title.style.display = DisplayStyle.None;

        _buttonPrev = root.Q<Button>("Prev");
        _buttonPrev.clickable.clicked += async () =>
        {
            await AudioManager.Instance.Click();
            ChangePage(-1);
        };
        _buttonNext = root.Q<Button>("Next");
        _buttonNext.clickable.clicked += async () =>
        {
            await AudioManager.Instance.Click();
            ChangePage(1);
        };

        _buttonOk = root.Q<Button>("Close");
        _buttonOk.clickable.clicked += async () =>
        {
            await AudioManager.Instance.Click();
            OnClickClose();
        };

        _spellList1 = root.Q<VisualElement>("SpellList1");
        _spellList2 = root.Q<VisualElement>("SpellList2");

        _buttonAir = root.Q<Button>("Air");
        _buttonAir.clickable.clicked += async () =>
        {
            await AudioManager.Instance.Click();
            ClickRightButtons();
            _hero.SpellBook.SetSchoolMagic(TypeSchoolMagic.SchoolofAirMagic);
            _buttonAir.style.marginLeft = new StyleLength(new Length(0, LengthUnit.Pixel));
            // Draw spells for current type.
            DrawSpells();
        };
        _buttonFire = root.Q<Button>("Fire");
        _buttonFire.clickable.clicked += async () =>
        {
            await AudioManager.Instance.Click();
            ClickRightButtons();
            _hero.SpellBook.SetSchoolMagic(TypeSchoolMagic.SchoolofFireMagic);
            _buttonFire.style.marginLeft = new StyleLength(new Length(0, LengthUnit.Pixel));
            // Draw spells for current type.
            DrawSpells();
        };
        _buttonWater = root.Q<Button>("Water");
        _buttonWater.clickable.clicked += async () =>
        {
            await AudioManager.Instance.Click();
            ClickRightButtons();
            _hero.SpellBook.SetSchoolMagic(TypeSchoolMagic.SchoolofWaterMagic);
            _buttonWater.style.marginLeft = new StyleLength(new Length(0, LengthUnit.Pixel));
            // Draw spells for current type.
            DrawSpells();
        };
        _buttonEarth = root.Q<Button>("Earth");
        _buttonEarth.clickable.clicked += async () =>
        {
            await AudioManager.Instance.Click();
            ClickRightButtons();
            _hero.SpellBook.SetSchoolMagic(TypeSchoolMagic.SchoolofEarthMagic);
            _buttonEarth.style.marginLeft = new StyleLength(new Length(0, LengthUnit.Pixel));
            // Draw spells for current type.
            DrawSpells();
        };
        _buttonAll = root.Q<Button>("All");
        _buttonAll.clickable.clicked += async () =>
        {
            await AudioManager.Instance.Click();
            ClickRightButtons();
            _hero.SpellBook.SetSchoolMagic(TypeSchoolMagic.AllSchools);
            _buttonAll.style.marginLeft = new StyleLength(new Length(0, LengthUnit.Pixel));
            // Draw spells for current type.
            DrawSpells();
        };
        _buttonCombat = root.Q<Button>("Combat");
        _buttonCombat.clickable.clicked += async () =>
        {
            await AudioManager.Instance.Click();
            _hero.SpellBook.SetTypeSpell(TypeSpell.Combat);
            DrawSpells();
        };
        _buttonAdv = root.Q<Button>("Adv");
        _buttonAdv.clickable.clicked += async () =>
        {
            await AudioManager.Instance.Click();
            _hero.SpellBook.SetTypeSpell(TypeSpell.Adventure);
            DrawSpells();
        };

        _mana = root.Q<Label>("ManaValue");

        // base.Localize(root);
    }

    private void ChangePage(int page)
    {
        _hero.SpellBook.ChangePage(page);
        DrawSpells();
    }

    private void ClickRightButtons()
    {
        UQueryBuilder<Button> builder = new UQueryBuilder<Button>(root);
        List<Button> list = builder.Class("button_right").ToList();

        foreach (var btn in list)
        {
            btn.style.marginLeft = StyleKeyword.Null;
        }

    }

    private void DrawSpells()
    {
        _mana.text = _hero.Data.mana.ToString();

        var spellBook = _hero.SpellBook;
        Debug.Log(spellBook.ToString());
        _dataResultDialog = new DataResultDialogSpellBook()
        {
        };

        // Clear spells bloks.
        _spellList1.Clear();
        _spellList2.Clear();

        // Set status button prev.
        if (spellBook.Pagination.page == 0)
        {
            _buttonPrev.style.display = DisplayStyle.None;
        }
        else
        {
            _buttonPrev.style.display = DisplayStyle.Flex;
        }

        // Set status button next.
        if (spellBook.Pagination.end >= spellBook.Pagination.total)
        {
            _buttonNext.style.display = DisplayStyle.None;
        }
        else
        {
            _buttonNext.style.display = DisplayStyle.Flex;
        }

        var _boxForSpell = _spellList1;
        int index = 0;

        // Draw school magic banner.
        if (spellBook.ActiveSchoolMagic != TypeSchoolMagic.AllSchools && spellBook.Pagination.page == 0)
        {
            var activeSchool = ResourceSystem.Instance
                .GetAttributesByType<ScriptableAttributeSchoolMagic>(TypeAttribute.SchoolMagic)
                .Find(t => t.typeSchoolMagic == spellBook.ActiveSchoolMagic);
            if (activeSchool != null)
            {
                var bannerSchool = new VisualElement();
                bannerSchool.AddToClassList("w-67");
                bannerSchool.AddToClassList("h-33");
                bannerSchool.style.backgroundImage
                    = new StyleBackground(activeSchool.MenuSprite);

                _boxForSpell.Add(bannerSchool);
            }
            index = 2;
        }

        for (int i = spellBook.Pagination.start; i < spellBook.Pagination.end; i++)
        {
            var spellData = spellBook.ActiveSpells[i];
            var schoolSpell = spellData.SchoolMagic;

            var levelSSkill = _hero.GetLevelSSkil(spellData.SchoolMagic.BaseSecondarySkill.TypeTwoSkill);
            var levelSpell = -1;
            if (_hero.Data.SSkills.ContainsKey(schoolSpell.BaseSecondarySkill.TypeTwoSkill))
            {
                levelSpell = _hero.Data.SSkills[schoolSpell.BaseSecondarySkill.TypeTwoSkill].level;
            }

            var newNodeElementSpell = new Button();
            newNodeElementSpell.AddToClassList("button");
            newNodeElementSpell.AddToClassList("border-0");
            newNodeElementSpell.AddToClassList("m-0");
            newNodeElementSpell.AddToClassList("bg-transparent");
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
                = string.Format(
                    "{0}: {1}",
                    manaText.GetLocalizedString(),
                    spellData.LevelData[levelSSkill + 1].cost
                );

            newNodeElementSpell.clickable.clicked += async () =>
            {
                await AudioManager.Instance.Click();
                spellBook.ChooseSpell(spellData);
                OnClickSpell();
                OnClickClose();
            };

            _boxForSpell.Add(newNodeElementSpell);
            index++;
            if (index % 9 == 0)
            {
                _boxForSpell = _spellList2;
            }
        }
    }

    public async Task<DataResultDialogSpellBook> ProcessAction(EntityHero hero)
    {
        base.Init();

        _hero = hero;


        DrawSpells();

        _processCompletionSource = new TaskCompletionSource<DataResultDialogSpellBook>();

        return await _processCompletionSource.Task;
    }

    private void OnClickClose()
    {
        _dataResultDialog.isOk = true;
        _processCompletionSource.SetResult(_dataResultDialog);

        // processAction?.Invoke();
    }
}

