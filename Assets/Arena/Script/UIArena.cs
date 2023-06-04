using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;


public class UIArena : UILocaleBase
{
    private ArenaManager _arenaManager;
    [SerializeField] private UIDocument _uiDoc;
    [SerializeField] private VisualTreeAsset _templateButton;
    [SerializeField] private VisualTreeAsset _templateShortCreatureInfo;
    [SerializeField] private VisualTreeAsset _templateQueueCreature;
    [SerializeField] private VisualTreeAsset _templateQueueRound;
    [SerializeField] private VisualTreeAsset _templateShortSpellInfo;
    [SerializeField] private UnityEngine.UI.Image _bgImage;

    public static event Action<bool, bool> OnNextCreature;

    public static event Action OnCancelSpell;
    public static event Action OnOpenSpellBook;
    public static event Action OnClickAttack;
    public static event Action OnUnloadArena;
    public static event Action OnLoadArena;
    public static event Action OnClickAutoBattle;

    private VisualElement _box;
    private VisualElement _leftSide;
    private VisualElement _rightSide;
    private VisualElement _helpLeftCreature;
    private VisualElement _helpRightCreature;
    private VisualElement _queueBlok;
    private Label _statBlok;
    private Button _btnDirAttack;
    private Button _btnSpellBook;
    private Button _btnWait;
    private Button _btnAuto;
    private Button _btnRun;
    private Button _btnSetting;
    private Button _btnDefense;
    private Button _btnQueue;
    private const string _arenaButtons = "ArenaButtons";
    private Camera _cameraMain;
    private AsyncOperationHandle<ScriptableEntityTown> _asset;
    private string textMoveCreature;
    private string textAttackedCreature;
    private GameObject grid;
    protected TaskCompletionSource<ResultDialogArenaData> _processCompletionSource;
    protected ResultDialogArenaData _dataResultDialog;
    protected DialogArenaData _dialogArenaData;

    private void Awake()
    {
        textMoveCreature = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "movecreature").GetLocalizedString();
        textAttackedCreature = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "attackedcreature").GetLocalizedString();

        _cameraMain = Camera.main;
        _cameraMain.gameObject.SetActive(false);

        _box = _uiDoc.rootVisualElement;

        _leftSide = _box.Q<VisualElement>("SideLeft");
        _rightSide = _box.Q<VisualElement>("SideRight");

        _helpRightCreature = _rightSide.Q<VisualElement>("Creature");
        _helpLeftCreature = _leftSide.Q<VisualElement>("Creature");
        _queueBlok = _box.Q<VisualElement>("QueueBlok");

        _btnWait = _box.Q<Button>("WaitButton");
        _btnWait.clickable.clicked += async () =>
        {
            await AudioManager.Instance.Click();
            OnClickWait();
        };

        _btnAuto = _box.Q<Button>("AutoButton");
        _btnAuto.clickable.clicked += async () =>
        {
            await AudioManager.Instance.Click();
            OnClickAuto();
        };

        _btnDefense = _box.Q<Button>("DefenseButton");
        _btnDefense.clickable.clicked += async () =>
        {
            await AudioManager.Instance.Click();
            OnClickDefense();
        };

        _btnRun = _box.Q<Button>("RunButton");
        _btnRun.clickable.clicked += async () =>
        {
            await AudioManager.Instance.Click();
            await OnClickRun();
        };

        _btnQueue = _box.Q<Button>("QueueButton");
        _btnQueue.clickable.clicked += async () =>
        {
            await AudioManager.Instance.Click();
            OnToggleQueue();
        };
        OnToggleQueue();

        _btnDirAttack = _box.Q<Button>("DirAttack");
        _btnDirAttack.clickable.clicked += async () =>
        {
            await AudioManager.Instance.Click();
            ClickDirAttack();
        };
        _btnDirAttack.SetEnabled(false);

        _btnSpellBook = _box.Q<Button>("SpellBookButton");
        _btnSpellBook.clickable.clicked += async () =>
        {
            await AudioManager.Instance.Click();
            OnOpenSpellBook?.Invoke();
        };

        _btnSetting = _box.Q<Button>("SettingButton");
        _btnSetting.clickable.clicked += async () =>
        {
            await AudioManager.Instance.Click();
            ShowSettings();
        };

        _statBlok = _box.Q<Label>("Stat");
        _statBlok.RegisterCallback<ClickEvent>(async (ClickEvent evt) =>
        {
            await AudioManager.Instance.Click();
            await ShowFullStat();
        }, TrickleDown.NoTrickleDown);

        ArenaManager.OnChangeNodesForAttack += ChangeStatusButtonAttack;
        ArenaCreature.OnChangeParamsCreature += ChangeParamsCreature;
        ArenaManager.OnAutoNextCreature += DrawInfo;
        ArenaQueue.OnNextStep += ChangeStatusButton;
        UIDialogSpellBook.OnClickSpell += ShowSpellInfo;
        ArenaManager.OnChooseCreatureForSpell += ShowSpellInfo;
        ArenaManager.OnHideSpellInfo += HideSpellInfo;
        ArenaManager.OnShowState += ShowStat;
        UIArenaEndStatWindow.OnCloseStat += UnloadArena;
        ArenaStat.OnAddStat += RefreshStatText;
        // ArenaManager.OnRunFromBattle += OnClickClose;
        // ArenaManager.OnEndBattle += OnClickClose;
    }

    private void OnDestroy()
    {
        ArenaManager.OnChangeNodesForAttack -= ChangeStatusButtonAttack;
        ArenaCreature.OnChangeParamsCreature -= ChangeParamsCreature;
        ArenaManager.OnAutoNextCreature -= DrawInfo;
        ArenaQueue.OnNextStep -= ChangeStatusButton;
        UIDialogSpellBook.OnClickSpell -= ShowSpellInfo;
        ArenaManager.OnChooseCreatureForSpell -= ShowSpellInfo;
        ArenaManager.OnHideSpellInfo -= HideSpellInfo;
        ArenaManager.OnShowState -= ShowStat;
        UIArenaEndStatWindow.OnCloseStat -= UnloadArena;
        ArenaStat.OnAddStat -= RefreshStatText;
        // ArenaManager.OnRunFromBattle -= OnClickClose;
        // ArenaManager.OnEndBattle -= OnClickClose;
    }

    public void Init(ArenaManager arenaManager)
    {
        _arenaManager = arenaManager;

        if (_arenaManager.ArenaTown != null)
        {
            _bgImage.sprite = _arenaManager.DialogArenaData.ArenaSetting.BgTownArena;
        }
        else
        {
            _bgImage.sprite = _arenaManager.DialogArenaData.ArenaSetting.BgSprites[UnityEngine.Random.Range(0, _arenaManager.DialogArenaData.ArenaSetting.BgSprites.Count)];
        }

        OnLoadArena?.Invoke();

        DrawHelpCreature();

        DrawQueue();

        base.Localize(_box);
    }

    public async Task<ResultDialogArenaData> ProcessAction(DialogArenaData dialogArenaData)
    {
        _dialogArenaData = dialogArenaData;
        _dataResultDialog = new ResultDialogArenaData();
        _processCompletionSource = new TaskCompletionSource<ResultDialogArenaData>();

        grid = GameObject.FindGameObjectWithTag("Map");
        if (grid != null) grid.SetActive(false);

        return await _processCompletionSource.Task;
    }

    private void RefreshStatText(string text)
    {
        _statBlok.text = text;
    }

    private void HideSpellInfo()
    {
        _box.Q<VisualElement>("SpellButtons").style.display = DisplayStyle.None;
    }

    private void ShowSpellInfo()
    {
        if (_arenaManager.ArenaQueue.ActiveHero.Entity.SpellBook.ChoosedSpell == null)
        {
            return;
        }

        _helpLeftCreature.style.display = DisplayStyle.None;
        _helpRightCreature.style.display = DisplayStyle.None;

        DrawShortSpellInfo();
        base.Localize(_box);
    }

    private void DrawShortSpellInfo()
    {
        var activeEntity = _arenaManager.ArenaQueue.activeEntity;
        if (activeEntity.arenaEntity.TypeArenaPlayer == TypeArenaPlayer.Left)
        {
            //_helpLeftCreature.style.display = DisplayStyle.Flex;
            DrawShortSpell(_leftSide.Q<VisualElement>("Anymore"), activeEntity.arenaEntity);
        }
        else
        {
            //_helpRightCreature.style.display = DisplayStyle.Flex;
            DrawShortSpell(_rightSide.Q<VisualElement>("Anymore"), activeEntity.arenaEntity);
        }
    }

    private void DrawShortSpell(VisualElement box, ArenaEntityBase arenaEntity)
    {
        box.Clear();
        var boxSpell = _templateShortSpellInfo.Instantiate();

        var spellBook = _arenaManager.ArenaQueue.ActiveHero.Entity.SpellBook;
        if (spellBook != null)
        {
            boxSpell.Q<VisualElement>("Ava").style.backgroundImage
                = new StyleBackground(spellBook.ChoosedSpell.MenuSprite);
            boxSpell.Q<Label>("Name").text = spellBook.ChoosedSpell.Text.title.GetLocalizedString();
            boxSpell.Q<VisualElement>("Cancel").Q<Button>("Btn").clickable.clicked += async () =>
            {
                await AudioManager.Instance.Click();
                spellBook.ChooseSpell(null);
                box.Remove(boxSpell);
                OnCancelSpell?.Invoke();
            };
            if (_arenaManager.clickedNode != null)
            {
                var attackedCreature = _arenaManager.clickedNode.OccupiedUnit != null
                    ? _arenaManager.clickedNode.OccupiedUnit.Entity.ScriptableDataAttribute
                    : null;
                if (attackedCreature != null)
                {
                    var boxInfoCreature = boxSpell.Q<VisualElement>("CreatureInfo");
                    boxInfoCreature.style.display = DisplayStyle.Flex;
                    boxInfoCreature.Q<VisualElement>("AvaCreature").style.backgroundImage
                        = new StyleBackground(attackedCreature.MenuSprite);

                    var dataPlural = new Dictionary<string, int> { { "value", 1 } };
                    var arguments = new[] { dataPlural };
                    var titlePlural = Helpers.GetLocalizedPluralString(
                        attackedCreature.Text.title,
                        arguments,
                        dataPlural
                        );
                    boxInfoCreature.Q<Label>("NameCreature").text = titlePlural;

                    CreateButtonApply(boxSpell);

                    boxSpell.Q<Label>("For").text
                        = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "for").GetLocalizedString();
                }
                else
                {
                    boxSpell.Q<Label>("For").style.display = DisplayStyle.None;
                    boxSpell.Q<VisualElement>("CreatureInfo").style.display = DisplayStyle.None;
                    CreateButtonApply(boxSpell);
                }
            }
            else
            {
                boxSpell.Q<Label>("For").text
                    = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "choosenodeforspell").GetLocalizedString();
                boxSpell.Q<VisualElement>("CreatureInfo").style.display = DisplayStyle.None;
                boxSpell.Q<VisualElement>("Apply").Q<Button>("Btn").style.display = DisplayStyle.None;
            }
            box.Add(boxSpell);
        }
    }

    private void CreateButtonApply(VisualElement boxSpell)
    {
        var btnApplySpell = boxSpell.Q<VisualElement>("Apply").Q<Button>("Btn");
        btnApplySpell.style.display = DisplayStyle.Flex;
        btnApplySpell.clickable.clicked += async () =>
        {
            HideSpellInfo();

            await AudioManager.Instance.Click();
            await _arenaManager.ClickButtonSpell();
        };
    }

    private void ChangeStatusButton()
    {
        if (
            _arenaManager.ArenaQueue.ActiveHero.Data.autoRun
            ||
            _arenaManager.ArenaQueue.ActiveHero.Data.playerType == PlayerType.Bot
            )
        {
            _btnSpellBook.SetEnabled(false);
            _btnWait.SetEnabled(false);
            _btnDirAttack.SetEnabled(false);
            // _btnRun.SetEnabled(false);
            _btnDefense.SetEnabled(false);
        }
        else
        {
            _btnDefense.SetEnabled(true);
            // Button spellbook.
            if (
                _arenaManager.ArenaQueue.ActiveHero.Entity != null
                &&
                _arenaManager.ArenaQueue.ActiveHero.Entity.SpellBook != null
                &&
                _arenaManager.ArenaQueue.ActiveHero.Entity.SpellBook.countCreatedSpell > 0
                )
            {
                _btnSpellBook.SetEnabled(true);
            }
            else
            {
                _btnSpellBook.SetEnabled(false);
            }

            // Button wait.
            if (
                _arenaManager.ArenaQueue.activeEntity.arenaEntity.Data.waitTick > 0
                )
            {
                _btnWait.SetEnabled(false);
            }
            else
            {
                _btnWait.SetEnabled(true);
            }
        }
    }

    private void OnToggleQueue()
    {
        _queueBlok.style.display = _queueBlok.style.display.value == DisplayStyle.Flex
            ? DisplayStyle.None
            : DisplayStyle.Flex;
    }

    private void DrawQueue()
    {
        _queueBlok.Clear();
        var roundElement = _templateQueueRound.Instantiate();
        int startRound = _arenaManager.ArenaQueue.activeEntity.round;
        roundElement.Q<Label>("Round").text = startRound.ToString();
        _queueBlok.Add(roundElement);
        var fullQueue = _arenaManager.ArenaQueue.GetQueue();
        var maxCountQueueItem = fullQueue.Count < LevelManager.Instance.ConfigGameSettings.arenaMaxCountQueue
            ? fullQueue.Count
            : LevelManager.Instance.ConfigGameSettings.arenaMaxCountQueue;
        foreach (var creature in _arenaManager.ArenaQueue.GetQueue().GetRange(0, maxCountQueueItem))
        {
            var entity = ((EntityCreature)creature.arenaEntity.Entity);

            var sprite = creature.arenaEntity is ArenaShootTown
                ? _arenaManager.ArenaTown.Town.ConfigData.MenuSprite
                : creature.arenaEntity.Entity.ScriptableDataAttribute.MenuSprite;
            var creatureElement = _templateQueueCreature.Instantiate();
            creatureElement.Q<VisualElement>("Img").style.backgroundImage
                = new StyleBackground(sprite);
            creatureElement.Q<VisualElement>("Img").style.width
                = new StyleLength(new Length(58, LengthUnit.Pixel));
            // creatureElement.Q<VisualElement>("Img").style.height
            //     = new StyleLength(new Length(64, LengthUnit.Pixel));
            creatureElement.Q<Label>("Value").text
                = creature.arenaEntity.Data.quantity.ToString(); // entity.Data.value.ToString();

            creatureElement.Q<VisualElement>("Overlay").style.backgroundColor
                = new StyleColor(creature.arenaEntity.Hero.Entity != null
                    ? creature.arenaEntity.Hero.Entity.Player.DataPlayer.color
                    : new Color(.5f, .5f, .5f));

            if (creature.arenaEntity == _arenaManager.ArenaQueue.activeEntity.arenaEntity)
            {
                creatureElement.Q<Button>().RemoveFromClassList("button_bordered");
                creatureElement.Q<Button>().AddToClassList("button_active");
            }

            creatureElement.RegisterCallback<ClickEvent>(async (ClickEvent evt) =>
            {
                await AudioManager.Instance.Click();
                creature.arenaEntity.ShowDialogInfo();
            });

            if (startRound != creature.round)
            {
                startRound = creature.round;
                var otherRoundElement = _templateQueueRound.Instantiate();
                otherRoundElement.Q<Label>("Round").text = startRound.ToString();
                _queueBlok.Add(otherRoundElement);
            }

            _queueBlok.Add(creatureElement);
        }

        base.Localize(_box);
    }

    private void ChangeParamsCreature()
    {
        DrawInfo();
    }

    private void DrawInfo()
    {
        _leftSide.Q<VisualElement>("Anymore").Clear();
        _rightSide.Q<VisualElement>("Anymore").Clear();

        DrawQueue();
        ChangeStatusButtonAttack();
        ChangeStatusButton();
        DrawHelpCreature();
        // ShowSpellInfo();
    }

    private void OnClickDefense()
    {
        OnNextCreature?.Invoke(false, true);
        DrawInfo();
    }

    private void OnClickWait()
    {
        OnNextCreature?.Invoke(true, false);
        DrawInfo();
    }

    private void OnClickAuto()
    {
        OnClickAutoBattle?.Invoke();
    }

    private void ChangeStatusButtonAttack()
    {
        if (
            _arenaManager.NodesForAttackActiveCreature.Count > 0
            && !_arenaManager.ArenaQueue.ActiveHero.Data.autoRun
            )
        {
            _btnDirAttack.SetEnabled(true);
        }
        else
        {
            _btnDirAttack.SetEnabled(false);
        }

        DrawHelpAttackedCreature();
    }

    private void ClickDirAttack()
    {
        OnClickAttack?.Invoke();
    }

    private void DrawHelpCreature()
    {
        var activeEntity = _arenaManager.ArenaQueue.activeEntity;
        if (activeEntity.arenaEntity == null) return;

        _helpLeftCreature.style.display = DisplayStyle.None;
        _helpRightCreature.style.display = DisplayStyle.None;

        VisualElement blokInfoCreature;
        if (activeEntity.arenaEntity.TypeArenaPlayer == TypeArenaPlayer.Left)
        {
            blokInfoCreature = _helpLeftCreature;
            _helpLeftCreature.style.display = DisplayStyle.Flex;
            DrawCreatureInfo(_helpLeftCreature, activeEntity.arenaEntity, textMoveCreature);
        }
        else
        {
            blokInfoCreature = _helpRightCreature;
            _helpRightCreature.style.display = DisplayStyle.Flex;
            DrawCreatureInfo(_helpRightCreature, activeEntity.arenaEntity, textMoveCreature);
        }
    }
    private void DrawHelpAttackedCreature()
    {
        // _helpHero.style.display = DisplayStyle.None;
        // _helpEnemy.style.display = DisplayStyle.None;

        VisualElement blokInfoCreature;
        var attackedEntity = _arenaManager.AttackedCreature;
        if (attackedEntity != null)
        {

            if (attackedEntity.TypeArenaPlayer == TypeArenaPlayer.Left)
            {
                blokInfoCreature = _helpLeftCreature;
                _helpLeftCreature.style.display = DisplayStyle.Flex;
                DrawCreatureInfo(_helpLeftCreature, attackedEntity, textAttackedCreature);
            }
            else
            {
                blokInfoCreature = _helpRightCreature;
                _helpRightCreature.style.display = DisplayStyle.Flex;
                DrawCreatureInfo(_helpRightCreature, attackedEntity, textAttackedCreature);
            }
        }
        else
        {
            var activeEntity = _arenaManager.ArenaQueue.activeEntity;
            if (activeEntity.arenaEntity.TypeArenaPlayer == TypeArenaPlayer.Left)
            {
                _helpRightCreature.style.display = DisplayStyle.None;
            }
            else
            {
                _helpLeftCreature.style.display = DisplayStyle.None;
            }
        }
    }

    private void DrawCreatureInfo(VisualElement blokInfoCreature, ArenaEntityBase activeEntity, string text)
    {
        VisualElement infoBlok = blokInfoCreature.Q<VisualElement>("Info");
        infoBlok.Clear();

        VisualElement blokParamsCreature = _templateShortCreatureInfo.Instantiate();
        infoBlok.Add(blokParamsCreature);

        infoBlok.Q<Label>().text = text;

        ScriptableAttributeCreature creatureData = ((EntityCreature)activeEntity.Entity).ConfigAttribute;
        // var parameters = creatureData.CreatureParams;
        var sprite = activeEntity is ArenaShootTown
            ? _arenaManager.ArenaTown.Town.ConfigData.MenuSprite
            : creatureData.MenuSprite;

        infoBlok.Q<VisualElement>("Ava").style.backgroundImage
            = new StyleBackground(sprite);

        string titlePlural;
        if (activeEntity is ArenaShootTown)
        {
            titlePlural = _arenaManager.ArenaTown.Town.ConfigData.Text.title.GetLocalizedString();
            infoBlok.Q<VisualElement>("RowQuantity").style.display = DisplayStyle.None;
            infoBlok.Q<VisualElement>("RowHP").style.display = DisplayStyle.None;
            infoBlok.Q<VisualElement>("RowAttack").style.display = DisplayStyle.None;
            infoBlok.Q<VisualElement>("RowDefense").style.display = DisplayStyle.None;

            infoBlok.Q<Label>("Damage").text = string.Format(
                "{0}-{1}",
                activeEntity.Data.damageMin + activeEntity.Data.DamageModificators.Values.Sum(),
                activeEntity.Data.damageMax + activeEntity.Data.DamageModificators.Values.Sum()
            );
        }
        else
        {
            var dataPlural = new Dictionary<string, int> { { "value", 1 } };
            var arguments = new[] { dataPlural };
            titlePlural = Helpers.GetLocalizedPluralString(
                creatureData.Text.title,
                arguments,
                dataPlural
                );

            infoBlok.Q<Label>("Attack").text = string.Format(
                "<size=80%>{0}</size> (<color=#FFFFAB>{1}</color>)",
                activeEntity.Data.attack,
                activeEntity.Data.attack + activeEntity.Data.AttackModificators.Values.Sum()
            );
            infoBlok.Q<Label>("Defense").text = string.Format(
                "<size=80%>{0}</size> (<color=#FFFFAB>{1}</color>)",
                activeEntity.Data.defense,
                activeEntity.Data.defense + activeEntity.Data.DefenseModificators.Values.Sum()
            );
            int currentHP = activeEntity.Data.totalHP - (activeEntity.Data.HP * (activeEntity.Data.quantity - 1));
            infoBlok.Q<Label>("HP").text = string.Format(
                "<size=80%>{0}</size> (<color=#FFFFAB><b>{1}</b></color>)",
                creatureData.CreatureParams.HP,
                currentHP == 0 ? activeEntity.Data.HP : currentHP
            );
            infoBlok.Q<Label>("Quantity").text = string.Format("<color=#FFFFAB>{0}</color>", activeEntity.Data.quantity);

            string templateDamage = "<color=#FFFFAB>{0}-{1}</color>";
            var damageMin = activeEntity.Data.damageMin + activeEntity.Data.DamageModificators.Values.Sum();
            var damageMax = activeEntity.Data.damageMax + activeEntity.Data.DamageModificators.Values.Sum();
            if (damageMin == damageMax)
            {
                templateDamage = "<color=#FFFFAB>{0}</color>";
            }
            infoBlok.Q<Label>("Damage").text = string.Format(
                templateDamage,
                damageMin,
                damageMax
            );
        }

        infoBlok.Q<Label>("Name").text = titlePlural;

        base.Localize(_box);
    }

    public async void ShowStat()
    {
        var loaderStat = new UIArenaEndStatOperation(new ArenaStatData()
        {
            arenaManager = _arenaManager,
        });
        await loaderStat.ShowAndHide();
    }

    public async void ShowSettings()
    {
        _arenaManager.DisableInputSystem();
        var loaderStat = new UIArenaSettingOperation();
        await loaderStat.ShowAndHide(new ArenaSettingData()
        {
            arenaManager = _arenaManager
        });
        _arenaManager.EnableInputSystem();
    }

    private async UniTask OnClickClose()
    {
        // Release asset prefab town.
        // if (_asset.IsValid())
        // {
        //     Addressables.ReleaseInstance(_asset);
        // }
        await _arenaManager.CalculateStat();
        // UnloadArena();
    }

    private async UniTask ShowFullStat()
    {
        _arenaManager.DisableInputSystem();
        var loaderStat = new UIArenaFullStatOperation(new ArenaStatData()
        {
            arenaManager = _arenaManager
        });
        await loaderStat.ShowAndHide();
        _arenaManager.EnableInputSystem();
    }

    private async UniTask OnClickRun()
    {
        _arenaManager.DisableInputSystem();
        var dialogData = new DataDialogHelp()
        {
            Header = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "askrun_t").GetLocalizedString(),
            Description = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "askrun_d").GetLocalizedString(),
            showCancelButton = true,
        };

        var dialogWindow = new DialogHelpProvider(dialogData);
        var result = await dialogWindow.ShowAndHide();
        if (result.isOk)
        {
            _arenaManager.heroLeft.typearenaHeroStatus = TypearenaHeroStatus.Runned;
            _arenaManager.heroRight.typearenaHeroStatus = TypearenaHeroStatus.Victorious;
            await OnClickClose();
        }
        _arenaManager.EnableInputSystem();
    }

    private void UnloadArena()
    {
        _cameraMain.gameObject.SetActive(true);
        if (grid != null) grid.SetActive(true);

        _dataResultDialog.isEnd = true;
        _processCompletionSource.SetResult(_dataResultDialog);

        OnUnloadArena?.Invoke();
    }
}

