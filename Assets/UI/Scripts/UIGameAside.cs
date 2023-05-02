using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Cysharp.Threading.Tasks;

using Loader;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.UIElements;


public class UIGameAside : UILocaleBase
{
    [SerializeField] public UnityAction OnShowSetting;
    [SerializeField] public UnityAction OnClickMoveHero;
    [SerializeField] public UnityAction OnClickNextStep;
    //public UnityAction OnClickButtonHero;
    //public UnityAction OnClickButtonTown;
    [SerializeField] public UnityAction OnShowTown;

    [SerializeField] private UIDocument _aside;

    //[Header("General settings")]
    //[Space(10)]
    private VisualElement aside;
    private VisualElement _footer;
    private VisualElement _mapButtons;
    [SerializeField] private VisualTreeAsset _templateButtonHero;
    [SerializeField] private VisualTreeAsset _templateButtonTown;
    [SerializeField] private VisualTreeAsset _templateHeroInfo;
    [SerializeField] private VisualTreeAsset _templateTownInfoCreature;
    [SerializeField] private VisualTreeAsset _templateTownInfo;
    [SerializeField] private VisualTreeAsset _templateHeroForce;

    private Player player;
    private VisualElement boxinfo;

    private string NameWrapper = "wrapper";
    private string NameAsideBoxInfo = "AsideBoxInfo";
    private string NameBtnGoHero = "ButtonGoHero";
    private string NameBtnGameMenu = "ButtonSettingMenu";
    private string NameBtnNextStep = "ButtonNextStep";
    private string NameFooter = "Footer";
    private string NameHeroBox = "herobox";
    private string NameTownBox = "townbox";
    private string NameHeroButton = "aside_hero_button";
    private string NameTownButton = "aside_town_button";
    private string NameAllAsideButton = "ButtonAside";
    private string NameSelectedButton = "button_active";
    private string NameBorderedButton = "button_bordered";
    private string NameHit = "hit";
    private string NameMana = "mana";
    private string NameOverlay = "Overlay";
    private Label _timeBlok;

    private VisualElement _herobox;

    private VisualElement _townbox;

    const int countTown = 3;

    private SceneInstance _scene;

    private void Start()
    {
        GameManager.OnAfterStateChanged += OnAfterStateChanged;
        EntityHero.onChangeParamsActiveHero += ChangeParamsActiveHero;
        UITownInfo.onMoveHero += DrawHeroBox;
        UITown.OnExitFromTown += DrawAside;
        UITown.OnInputToTown += HideMapButtons;
        UIDialogDwellingWindow.OnBuyCreature += ChangeHeroInfo;
        UIDialogHeroInfo.OnMoveCreature += ChangeHeroInfo;
    }

    private void OnDestroy()
    {
        GameManager.OnAfterStateChanged -= OnAfterStateChanged;
        EntityHero.onChangeParamsActiveHero -= ChangeParamsActiveHero;
        UITownInfo.onMoveHero -= DrawHeroBox;
        UITown.OnExitFromTown -= DrawAside;
        UITown.OnInputToTown -= HideMapButtons;
        UIDialogDwellingWindow.OnBuyCreature -= ChangeHeroInfo;
        UIDialogHeroInfo.OnMoveCreature -= ChangeHeroInfo;
    }

    private void OnAfterStateChanged(GameState state)
    {
        player = LevelManager.Instance.ActivePlayer;
        // if (player.DataPlayer.command == ) return;

        switch (state)
        {
            case GameState.NextPlayer:
            case GameState.StartGame:
                DrawAside();
                ChangeHeroInfo();
                break;
            // case GameState.StartMoveHero:
            //     SetDisableAllButton();
            //     break;
            case GameState.StopMoveHero:
                SetEnableAllButton();
                break;
            case GameState.CreatePathHero:
                OnToogleEnableBtnGoHero();
                break;
            case GameState.ChangeResources:
                OnRedrawResource();
                break;
            case GameState.ChangeHeroParams:
                // DrawAside();
                ChangeHeroInfo();
                break;
        }
    }

    private void ChangeParamsActiveHero(EntityHero hero)
    {
        VisualElement heroHit = _herobox.Q<VisualElement>(hero.Id);
        if (heroHit != null)
        {
            heroHit.style.height = new StyleLength(new Length(hero.Data.hit, LengthUnit.Percent));
        }
    }

    private async UniTask<DataResultGameMenu> ShowGameMenu()
    {
        //UIManager.Instance.ShowSettingMenu();
        OnShowSetting?.Invoke();

        var dialogWindow = new GameMenuProvider();
        return await dialogWindow.ShowAndHide();

    }

    private async void ShowTown()
    {
        OnShowTown?.Invoke();
        var loadingOperations = new Queue<ILoadingOperation>();
        loadingOperations.Enqueue(new TownLoadOperation(player.ActiveTown));
        await GameManager.Instance.LoadingScreenProvider.LoadAndDestroy(loadingOperations);
    }

    public void Init(SceneInstance scene)
    {
        try
        {
            _scene = scene;
            aside = _aside.rootVisualElement.Q<VisualElement>(NameWrapper);
            _mapButtons = _aside.rootVisualElement.Q<VisualElement>("MapButtons");
            _timeBlok = _aside.rootVisualElement.Q<Label>("Time");

            _herobox = aside.Q<VisualElement>(NameHeroBox);
            _townbox = aside.Q<VisualElement>(NameTownBox);

            var btnGameMenu = _aside.rootVisualElement.Q<Button>(NameBtnGameMenu);
            btnGameMenu.clickable.clicked += async () =>
            {
                DataResultGameMenu result = await ShowGameMenu();
                if (result.isOk)
                {
                    var loadingOperations = new Queue<ILoadingOperation>();
                    await GameManager.Instance.AssetProvider.UnloadAdditiveScene(_scene);
                    loadingOperations.Enqueue(new MenuAppOperation());
                    await GameManager.Instance.LoadingScreenProvider.LoadAndDestroy(loadingOperations);
                }
            };

            var btnGoHero = _aside.rootVisualElement.Q<Button>(NameBtnGoHero);
            btnGoHero.RegisterCallback<ClickEvent>(OnMoveHero, TrickleDown.NoTrickleDown);

            var btnNextStep = _aside.rootVisualElement.Q<Button>(NameBtnNextStep);
            btnNextStep.clickable.clicked += () =>
            {
                GameManager.Instance.ChangeState(GameState.NextPlayer);
                OnClickNextStep?.Invoke();
            };

            _footer = _aside.rootVisualElement.Q<VisualElement>(NameFooter);

            boxinfo = _aside.rootVisualElement.Q<VisualElement>(NameAsideBoxInfo);
        }
        catch (Exception e)
        {
            Debug.LogWarning("Aside UI error: \n" + e);
        }

        base.Localize(aside);
    }

    private void HideMapButtons()
    {
        _mapButtons.style.display = DisplayStyle.None;
    }
    private void ShowMapButtons()
    {
        _mapButtons.style.display = DisplayStyle.Flex;
        DrawTownInfo();
    }
    private void DrawTimeBlok()
    {
        _timeBlok.Clear();

        var date = LevelManager.Instance.GameDate;
        LocalizedString monthText = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "month_short");
        LocalizedString weekText = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "week_short");
        LocalizedString dayText = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "day");

        _timeBlok.text = string.Format(
            "{0}: {1}, {2}: {3}, {4}: {5}({6})",
            monthText.GetLocalizedString(),
            date.month + 1,
            weekText.GetLocalizedString(),
            date.week + 1,
            dayText.GetLocalizedString(),
            date.day + 1,
            date.countDay
            );

    }

    private void DrawTownInfo()
    {
        var settings = LevelManager.Instance.ConfigGameSettings;
        // player.SetActiveHero(null);

        EntityTown activeTown = player.ActiveTown;
        if (activeTown == null) return;

        VisualElement townInfo = _templateTownInfo.Instantiate();
        townInfo.style.flexGrow = 1;

        var spriteTownEl = townInfo.Q<VisualElement>("TownIcon");
        var activeSprite = activeTown.ConfigData.LevelSprites.ElementAtOrDefault(activeTown.Data.level + 1);
        spriteTownEl.style.backgroundImage = new StyleBackground(activeSprite);

        var nameTownEl = townInfo.Q<Label>("TownName");
        nameTownEl.text = activeTown.Data.name;

        var _townBuildStatus = townInfo.Q<VisualElement>("TownBuildStatus");
        _townBuildStatus.Clear();
        // Draw status hall.
        var hallLevel = activeTown.Data.Generals
            .Where(t => t.Value.ConfigData.TypeBuild == TypeBuild.Town)
            .FirstOrDefault();
        var boxStatusHall = new VisualElement();
        boxStatusHall.style.backgroundImage
            = new StyleBackground(settings.SpriteHall[hallLevel.Value.level]);
        boxStatusHall.AddToClassList("h-full");
        boxStatusHall.AddToClassList("w-33");
        boxStatusHall.AddToClassList("p-px");
        _townBuildStatus.Add(boxStatusHall);

        // Draw status castle;
        var castleLevel = activeTown.Data.Generals
            .Where(t => t.Value.ConfigData.TypeBuild == TypeBuild.Castle)
            .FirstOrDefault();
        var boxStatusCastle = new VisualElement();
        var castleIndex = castleLevel.Value == null ? 0 : castleLevel.Value.level + 1;
        boxStatusCastle.style.backgroundImage
            = new StyleBackground(settings.SpriteCastle[castleIndex]);
        boxStatusCastle.AddToClassList("h-full");
        boxStatusCastle.AddToClassList("w-33");
        boxStatusHall.AddToClassList("p-px");
        _townBuildStatus.Add(boxStatusCastle);

        var listTownDwellingEl = townInfo.Q<VisualElement>("DwellingList");
        listTownDwellingEl.Clear();

        // Get hero by id.
        EntityHero hero = activeTown.Data.HeroinTown != null && activeTown.Data.HeroinTown != ""
            ? (EntityHero)UnitManager.Entities[activeTown.Data.HeroinTown]
            : null;
        SerializableDictionary<int, EntityCreature> creatures = new SerializableDictionary<int, EntityCreature>();
        if (hero != null)
        {
            creatures = hero.Data.Creatures;
        }
        else
        {
            creatures = activeTown.Data.Creatures;
        }
        foreach (var creature in creatures)
        {
            var btnCreature = _templateTownInfoCreature.Instantiate();
            btnCreature.AddToClassList("w-25");
            btnCreature.AddToClassList("h-50");
            if (creature.Value != null)
            {
                btnCreature.Q<VisualElement>("Img").style.backgroundImage
                    = new StyleBackground(creature.Value.ConfigAttribute?.MenuSprite);
            }
            btnCreature.Q<Label>("Value").text = creature.Value != null ? creature.Value.Data.value.ToString() : "";
            listTownDwellingEl.Add(btnCreature);
        }
        // for (int i = activeTown.Data.Armys.Count; i < 7; i++)
        // {
        //     var btnCreature = _templateTownInfoCreature.Instantiate();
        //     btnCreature.AddToClassList("w-25");
        //     btnCreature.AddToClassList("h-50");
        //     btnCreature.Q<Label>("Value").text = "";
        //     listTownDwellingEl.Add(btnCreature);
        // }

        // foreach (var build in activeTown.Data.Armys
        //     .OrderBy(t => ((ScriptableBuildingArmy)((BuildArmy)t.Value).ConfigData).Creatures[t.Value.level].CreatureParams.Level))
        // {
        //     var btnCreature = _templateTownInfoCreature.Instantiate();
        //     btnCreature.AddToClassList("w-25");
        //     btnCreature.AddToClassList("h-50");
        //     btnCreature.Q<VisualElement>("Img").style.backgroundImage
        //         = new StyleBackground(((ScriptableBuildingArmy)((BuildArmy)build.Value).ConfigData).Creatures[build.Value.level].MenuSprite);
        //     btnCreature.Q<Label>("Value").text = "+" + (build.Value.Data.quantity.ToString());
        //     listTownDwellingEl.Add(btnCreature);
        // }
        // for (int i = activeTown.Data.Armys.Count; i < 7; i++)
        // {
        //     var btnCreature = _templateTownInfoCreature.Instantiate();
        //     btnCreature.AddToClassList("w-25");
        //     btnCreature.AddToClassList("h-50");
        //     btnCreature.Q<Label>("Value").text = "";
        //     listTownDwellingEl.Add(btnCreature);
        // }

        boxinfo.Clear();
        boxinfo.Add(townInfo);
    }

    private void DrawTownBox()
    {
        _townbox.Clear();

        for (int i = 0; i < countTown; i++)
        {
            var newButtonTown = _templateButtonTown.Instantiate();

            newButtonTown.style.flexGrow = 1;
            newButtonTown.AddToClassList(NameTownButton);

            if (i < player.DataPlayer.PlayerDataReferences.ListTown.Count)
            {
                EntityTown town = (EntityTown)player.DataPlayer.PlayerDataReferences.ListTown[i];

                var activeSprite = town.ConfigData.LevelSprites.ElementAtOrDefault(town.Data.level + 1);

                newButtonTown.Q<VisualElement>("image").style.backgroundImage =
                    new StyleBackground(activeSprite);

                var time = Time.realtimeSinceStartup;
                newButtonTown.Q<Button>(NameAllAsideButton).RegisterCallback<ClickEvent>((ClickEvent evt) =>
                {
                    if (Time.realtimeSinceStartup - time < LevelManager.Instance.ConfigGameSettings.deltaDoubleClick)
                    {
                        ShowTown();
                    }
                    else
                    {
                        town.SetTownAsActive();
                        time = Time.realtimeSinceStartup;
                    }
                    OnResetFocusButton();
                    DrawTownInfo();
                    newButtonTown.Q<Button>(NameAllAsideButton).RemoveFromClassList("button_bordered");
                    newButtonTown.Q<Button>(NameAllAsideButton).AddToClassList(NameSelectedButton);

                }, TrickleDown.NoTrickleDown);

                if (town.Data.countBuild == LevelManager.Instance.ConfigGameSettings.countBuildPerDay)
                {
                    newButtonTown.Q<VisualElement>("Nobuild").style.display
                        = DisplayStyle.Flex;
                }
            }
            else
            {
                newButtonTown.SetEnabled(false);
            }


            _townbox.Add(newButtonTown);
        }
    }

    private void DrawHeroBox()
    {
        _herobox.Clear();

        var listHeroOnMap = player.DataPlayer.PlayerDataReferences.ListHero
            .Where(h => h.Data.State.HasFlag(StateHero.OnMap)).ToList();
        for (int i = 0; i < 5; i++)
        {
            var newButtonHero = _templateButtonHero.Instantiate();
            newButtonHero.AddToClassList(NameHeroButton);

            if (i < listHeroOnMap.Count)
            {
                EntityHero hero = listHeroOnMap[i];
                MapEntityHero heroGameObject = (MapEntityHero)hero.MapObject.MapObjectGameObject;
                var hit = newButtonHero.Q<VisualElement>(NameHit);
                hit.name = hero.Id;

                hit.style.height = new StyleLength(new Length(hero.Data.hit, LengthUnit.Percent));

                var mana = newButtonHero.Q<VisualElement>(NameMana);
                mana.style.height = new StyleLength(new Length(hero.Data.mana, LengthUnit.Percent));

                newButtonHero.Q<VisualElement>("image").style.backgroundImage =
                    new StyleBackground(hero.ConfigData.MenuSprite);

                var time = Time.realtimeSinceStartup;
                newButtonHero.Q<Button>(NameAllAsideButton).clickable.clicked += async () =>
                {
                    if (Time.realtimeSinceStartup - time < LevelManager.Instance.ConfigGameSettings.deltaDoubleClick)
                    {
                        var dialogHeroInfo = new DialogHeroInfoOperation(hero);
                        var result = await dialogHeroInfo.ShowAndHide();
                        if (result.isOk)
                        {

                        }
                    }
                    else
                    {
                        OnSetActiveHero(hero);
                        time = Time.realtimeSinceStartup;
                    }
                    OnResetFocusButton();

                    newButtonHero.Q<Button>(NameAllAsideButton).AddToClassList(NameSelectedButton);
                    newButtonHero.Q<Button>(NameAllAsideButton).RemoveFromClassList(NameBorderedButton);
                };

                if (hero == player.ActiveHero)
                {
                    newButtonHero.Q<Button>(NameAllAsideButton).AddToClassList(NameSelectedButton);
                    newButtonHero.Q<Button>(NameAllAsideButton).RemoveFromClassList(NameBorderedButton);
                }
            }
            else
            {
                newButtonHero.SetEnabled(false);
            }

            _herobox.Add(newButtonHero);
        }
    }

    private async void OnMoveHero(ClickEvent evt)
    {
        //GameManager.Instance.ChangeState(GameState.StartMoveHero);
        SetDisableAllButton();
        await LevelManager.Instance.ActivePlayer.ActiveHero.StartMove();
    }

    private void DrawAside()
    {
        DrawTimeBlok();

        ShowMapButtons();

        aside.style.display = DisplayStyle.Flex;
        _footer.style.display = DisplayStyle.Flex;

        OnResetFocusButton();

        DrawHeroBox();

        DrawTownBox();

        OnCalculateArrow();
        // OnToogleEnableBtnGoHero();
        OnRedrawResource();

        Color color = player.DataPlayer.color;
        color.a = LevelManager.Instance.ConfigGameSettings.alphaOverlay;

        // Queries overlay.
        UQueryBuilder<VisualElement> builder = new UQueryBuilder<VisualElement>(_aside.rootVisualElement);
        List<VisualElement> list = builder.Name(NameOverlay).ToList();
        foreach (var overlay in list)
        {
            overlay.style.backgroundColor = color;
        }

        SetEnableAllButton();
    }

    private void SetEnableAllButton()
    {
        UQueryBuilder<Button> btns2 = new UQueryBuilder<Button>(_aside.rootVisualElement);
        List<Button> listBtn2 = btns2.Class("button").ToList();
        foreach (var btn in listBtn2)
        {
            btn.SetEnabled(true);
        }
        // OnToogleEnableBtnGoHero();
    }

    private void SetDisableAllButton()
    {
        UQueryBuilder<Button> btns = new UQueryBuilder<Button>(_aside.rootVisualElement);
        List<Button> listBtn = btns.Class("button").ToList();
        foreach (var btn in listBtn)
        {
            btn.SetEnabled(false);
        }
    }

    private void OnToogleEnableBtnGoHero()
    {
        Button btn = _aside.rootVisualElement.Q<Button>(NameBtnGoHero);
        EntityHero activeHero = player.ActiveHero;
        if (activeHero != null)
        {
            btn.SetEnabled(activeHero.IsExistPath);
        }
        else
        {
            btn.SetEnabled(false);
        }
    }

    private void OnSetActiveHero(EntityHero hero)
    {

        EntityHero activeHero = player.ActiveHero;

        // OnResetFocusButton();

        // if (activeHero == hero)
        // {
        //     if (boxinfo.style.display == DisplayStyle.None)
        //     {
        //         // ChangeHeroInfo();
        //         boxinfo.style.display = DisplayStyle.Flex;
        //     }
        //     else
        //     {
        //         boxinfo.style.display = DisplayStyle.None;
        //     }
        // }
        // else
        // {
        //     boxinfo.style.display = DisplayStyle.None;

        //     //activeHero = hero;
        //     // player.SetActiveHero(hero);
        // }


        hero.SetHeroAsActive();
        ChangeHeroInfo();
        // OnToogleEnableBtnGoHero();
    }

    private void ChangeHeroInfo()
    {
        EntityHero activeHero = player.ActiveHero;
        if (activeHero == null) return;

        VisualElement HeroInfo = _templateHeroInfo.Instantiate();
        HeroInfo.style.flexGrow = 1;

        VisualElement heroImg = HeroInfo.Q<VisualElement>("HeroIcon");
        heroImg.style.backgroundImage = new StyleBackground(activeHero.ScriptableData.MenuSprite);
        Label name = HeroInfo.Q<Label>("HeroName");
        name.text = activeHero.Data.name;
        HeroInfo.Q<Label>("Attack").text = activeHero.Data.PSkills.GetValueOrDefault(TypePrimarySkill.Attack).ToString();
        HeroInfo.Q<Label>("Defense").text = activeHero.Data.PSkills.GetValueOrDefault(TypePrimarySkill.Defense).ToString();
        HeroInfo.Q<Label>("Knowledge").text = activeHero.Data.PSkills.GetValueOrDefault(TypePrimarySkill.Knowledge).ToString();
        HeroInfo.Q<Label>("Power").text = activeHero.Data.PSkills.GetValueOrDefault(TypePrimarySkill.Power).ToString();

        boxinfo.Clear();
        boxinfo.Add(HeroInfo);

        // Init hero info box
        InitHeroBox();
    }

    private void InitHeroBox()
    {
        EntityHero activeHero = player.ActiveHero;
        var _heroForceList = _aside.rootVisualElement.Q<VisualElement>("HeroForceList");
        _heroForceList.Clear();
        for (int i = 0; i < activeHero.Data.Creatures.Count; i++)
        {
            var creature = activeHero.Data.Creatures[i];
            var newForce = _templateTownInfoCreature.Instantiate();
            newForce.AddToClassList("w-25");
            newForce.AddToClassList("h-50");
            if (creature != null)
            {
                newForce.Q<VisualElement>("Img").style.backgroundImage
                    = new StyleBackground(creature.ConfigAttribute.MenuSprite);
            }
            newForce.Q<Label>("Value").text = creature != null
                ? creature.Data.value.ToString()
                : "";

            _heroForceList.Add(newForce);

        }

    }

    private void OnCalculateArrow()
    {
        var ArrowTopBtn = aside.Q<VisualElement>("arrowtop");
        var ArrowBottomBtn = aside.Q<VisualElement>("arrowbottom");
        if (player.DataPlayer.PlayerDataReferences.ListTown.Count < countTown)
        {
            ArrowTopBtn.SetEnabled(false);
            ArrowBottomBtn.SetEnabled(false);
        }
    }

    private void OnResetFocusButton()
    {
        // Queries overlay.
        UQueryBuilder<Button> builder = new UQueryBuilder<Button>(_aside.rootVisualElement);
        List<Button> list = builder.Name(NameAllAsideButton).ToList();
        foreach (var overlay in list)
        {
            overlay.RemoveFromClassList(NameSelectedButton);
            overlay.AddToClassList(NameBorderedButton);
        }
        // boxinfo.style.display = DisplayStyle.None;
    }

    public void OnRedrawResource()
    {

        Player player = LevelManager.Instance.ActivePlayer;

        //_footer.style.unityBackgroundImageTintColor = player.DataPlayer.color;

        var gold = _footer.Q<Label>("GoldValue");
        gold.text = player.DataPlayer.Resource[TypeResource.Gold].ToString();

        var wood = _footer.Q<Label>("WoodValue");
        wood.text = player.DataPlayer.Resource[TypeResource.Wood].ToString();

        var iron = _footer.Q<Label>("IronValue");
        iron.text = player.DataPlayer.Resource[TypeResource.Ore].ToString();

        var mercury = _footer.Q<Label>("MercuryValue");
        mercury.text = player.DataPlayer.Resource[TypeResource.Mercury].ToString();

        var diamond = _footer.Q<Label>("DiamondValue");
        diamond.text = player.DataPlayer.Resource[TypeResource.Crystal].ToString();

        var sulfur = _footer.Q<Label>("SulfurValue");
        sulfur.text = player.DataPlayer.Resource[TypeResource.Sulfur].ToString();

        var gem = _footer.Q<Label>("GemValue");
        gem.text = player.DataPlayer.Resource[TypeResource.Gems].ToString();
    }
}
