using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Cysharp.Threading.Tasks;

using Loader;

using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.UIElements;


public class UIArena : UILocaleBase
{
    public ArenaManager arenaManager;
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

    private VisualElement _box;
    private VisualElement _leftSide;
    private VisualElement _rightSide;
    private VisualElement _helpLeftCreature;
    private VisualElement _helpRightCreature;
    private VisualElement _queueBlok;
    private Button _btnDirAttack;
    private Button _btnSpellBook;
    private Button _btnQueue;
    private const string _arenaButtons = "ArenaButtons";
    private Camera _cameraMain;
    private AsyncOperationHandle<ScriptableEntityTown> _asset;
    private string textMoveCreature;
    private string textAttackedCreature;
    private GameObject grid;
    protected TaskCompletionSource<ResultDialogArenaData> _processCompletionSource;
    protected ResultDialogArenaData _dataResultDialog;

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

        var btnWait = _box.Q<Button>("WaitButton");
        btnWait.clickable.clicked += async () =>
        {
            await AudioManager.Instance.Click();
            OnClickWait();
        };

        var btnDefense = _box.Q<Button>("DefenseButton");
        btnDefense.clickable.clicked += async () =>
        {
            await AudioManager.Instance.Click();
            OnClickDefense();
        };

        var btnRunCreature = _box.Q<Button>("RunButton");
        btnRunCreature.clickable.clicked += async () =>
        {
            await AudioManager.Instance.Click();
            OnClickClose();
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

        ArenaManager.OnChangeNodesForAttack += ChangeStatusButtonAttack;
        ArenaCreature.OnChangeParamsCreature += ChangeParamsCreature;
        ArenaManager.OnAutoNextCreature += DrawInfo;
        ArenaQueue.OnNextStep += ChangeStatusButton;
        UIDialogSpellBook.OnClickSpell += ShowSpellInfo;
        ArenaManager.OnChooseCreatureForSpell += ShowSpellInfo;
        ArenaManager.OnHideSpellInfo += HideSpellInfo;
        ArenaManager.OnShowState += ShowStat;
        UIArenaEndStatWindow.OnCloseStat += UnloadArena;
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
        // ArenaManager.OnRunFromBattle -= OnClickClose;
        // ArenaManager.OnEndBattle -= OnClickClose;
    }

    public void Init()
    {
        _bgImage.sprite = arenaManager.DialogArenaData.ArenaSetting.BgSprites[UnityEngine.Random.Range(0, arenaManager.DialogArenaData.ArenaSetting.BgSprites.Count)];

        OnLoadArena?.Invoke();

        DrawHelpCreature();

        DrawQueue();

        base.Localize(_box);
    }

    public async Task<ResultDialogArenaData> ProcessAction()
    {
        _dataResultDialog = new ResultDialogArenaData();
        _processCompletionSource = new TaskCompletionSource<ResultDialogArenaData>();

        grid = GameObject.FindGameObjectWithTag("Map");
        if (grid != null) grid.SetActive(false);

        return await _processCompletionSource.Task;
    }

    private void HideSpellInfo()
    {
        _box.Q<VisualElement>("SpellButtons").style.display = DisplayStyle.None;
    }

    private void ShowSpellInfo()
    {
        if (arenaManager.ArenaQueue.ActiveHero.SpellBook.ChoosedSpell == null)
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
        var activeEntity = arenaManager.ArenaQueue.activeEntity;
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

        var spellBook = arenaManager.ArenaQueue.ActiveHero.SpellBook;
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
            if (arenaManager.clickedNode != null)
            {
                var attackedCreature = arenaManager.clickedNode.OccupiedUnit != null
                    ? arenaManager.clickedNode.OccupiedUnit.Entity.ScriptableDataAttribute
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
            await arenaManager.ClickButtonSpell();
        };
    }

    private void ChangeStatusButton()
    {
        if (
            arenaManager.ArenaQueue.ActiveHero != null
            &&
            arenaManager.ArenaQueue.ActiveHero.SpellBook != null
            &&
            arenaManager.ArenaQueue.ActiveHero.SpellBook.countCreatedSpell > 0
            )
        {
            _btnSpellBook.SetEnabled(true);
        }
        else
        {
            _btnSpellBook.SetEnabled(false);
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
        int startRound = arenaManager.ArenaQueue.activeEntity.round;
        roundElement.Q<Label>("Round").text = startRound.ToString();
        _queueBlok.Add(roundElement);

        foreach (var creature in arenaManager.ArenaQueue.ListEntities)
        {
            var entity = ((EntityCreature)creature.arenaEntity.Entity);

            var sprite = creature.arenaEntity.Data.typeAttack == TypeAttack.AttackShootTown
                ? arenaManager.town.Town.ConfigData.MenuSprite
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
                = new StyleColor(creature.arenaEntity.Entity.Player != null
                    ? creature.arenaEntity.Entity.Player.DataPlayer.color
                    : new Color(.5f, .5f, .5f));

            if (creature.arenaEntity == arenaManager.ArenaQueue.activeEntity.arenaEntity)
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

    private void ChangeStatusButtonAttack()
    {
        if (arenaManager.NodesForAttackActiveCreature.Count > 0)
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
        var activeEntity = arenaManager.ArenaQueue.activeEntity;
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
        var attackedEntity = arenaManager.AttackedCreature;
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
            var activeEntity = arenaManager.ArenaQueue.activeEntity;
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

        ScriptableAttributeCreature creatureData = (ScriptableAttributeCreature)activeEntity.Entity.ScriptableDataAttribute;
        infoBlok.Q<VisualElement>("Ava").style.backgroundImage
            = new StyleBackground(creatureData.MenuSprite);

        var dataPlural = new Dictionary<string, int> { { "value", 1 } };
        var arguments = new[] { dataPlural };
        var titlePlural = Helpers.GetLocalizedPluralString(
            creatureData.Text.title,
            arguments,
            dataPlural
            );
        infoBlok.Q<Label>("Name").text = titlePlural;

        infoBlok.Q<Label>("Attack").text = string.Format("{0}", creatureData.CreatureParams.Attack);
        infoBlok.Q<Label>("Defense").text = string.Format("{0}", creatureData.CreatureParams.Defense);
        infoBlok.Q<Label>("Damage").text = string.Format("{0}-{1}", creatureData.CreatureParams.DamageMin, creatureData.CreatureParams.DamageMax);
        infoBlok.Q<Label>("Hp").text = string.Format("{0}", creatureData.CreatureParams.HP);

        base.Localize(_box);
    }

    public async void ShowStat()
    {
        var loaderStat = new UIArenaEndStatOperation(new ArenaStat()
        {

        });
        await loaderStat.ShowAndHide();
    }

    private async void OnClickClose()
    {
        // Release asset prefab town.
        // if (_asset.IsValid())
        // {
        //     Addressables.ReleaseInstance(_asset);
        // }
        await arenaManager.CalculateStat();
        // UnloadArena();
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

