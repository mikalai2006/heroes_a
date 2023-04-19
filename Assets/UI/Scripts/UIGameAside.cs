using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Cysharp.Threading.Tasks;

using Loader;

using UnityEngine;
using UnityEngine.Events;
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

    private VisualElement _herobox;

    const int countTown = 3;

    private SceneInstance _scene;

    private void Start()
    {
        GameManager.OnAfterStateChanged += OnAfterStateChanged;
        EntityHero.onChangeParamsActiveHero += ChangeParamsActiveHero;
        UITownInfo.onMoveHero += DrawHeroBox;
        UITown.OnExitFromTown += ShowMapButtons;
        UITown.OnInputToTown += HideMapButtons;
    }

    private void OnDestroy()
    {
        GameManager.OnAfterStateChanged -= OnAfterStateChanged;
        EntityHero.onChangeParamsActiveHero -= ChangeParamsActiveHero;
        UITownInfo.onMoveHero -= DrawHeroBox;
        UITown.OnExitFromTown -= ShowMapButtons;
        UITown.OnInputToTown -= HideMapButtons;
    }

    private void OnAfterStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.StepNextPlayer:
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
                DrawAside();
                break;
        }
    }

    private void ChangeParamsActiveHero(EntityHero hero)
    {
        VisualElement heroHit = _herobox.Q<VisualElement>(hero.IdEntity);
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
                GameManager.Instance.ChangeState(GameState.StepNextPlayer);
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

    private void DrawTownInfo()
    {
        EntityTown activeTown = player.ActiveTown;
        if (activeTown == null) return;

        Debug.Log("Change info hero");
        VisualElement townInfo = _templateTownInfo.Instantiate();
        townInfo.style.flexGrow = 1;

        var spriteTownEl = townInfo.Q<VisualElement>("TownIcon");
        spriteTownEl.style.backgroundImage = new StyleBackground(activeTown.ScriptableData.MenuSprite);
        var nameTownEl = townInfo.Q<Label>("TownName");
        nameTownEl.text = activeTown.Data.name;
        var listTownDwellingEl = townInfo.Q<VisualElement>("DwellingList");
        listTownDwellingEl.Clear();

        foreach (var build in activeTown.Data.Armys
            .OrderBy(t => ((ScriptableBuildingArmy)((BuildArmy)t.Value).ConfigData).Creatures[t.Value.level].CreatureParams.Level))
        {
            var btnCreature = _templateTownInfoCreature.Instantiate();
            btnCreature.AddToClassList("w-25");
            btnCreature.AddToClassList("h-50");
            btnCreature.Q<VisualElement>("Img").style.backgroundImage
                = new StyleBackground(((ScriptableBuildingArmy)((BuildArmy)build.Value).ConfigData).Creatures[build.Value.level].MenuSprite);
            btnCreature.Q<Label>("Value").text = "+" + (build.Value.Data.quantity.ToString());
            listTownDwellingEl.Add(btnCreature);
        }
        for (int i = activeTown.Data.Armys.Count; i < 7; i++)
        {
            var btnCreature = _templateTownInfoCreature.Instantiate();
            btnCreature.AddToClassList("w-25");
            btnCreature.AddToClassList("h-50");
            btnCreature.Q<Label>("Value").text = "";
            listTownDwellingEl.Add(btnCreature);
        }

        boxinfo.Clear();
        boxinfo.Add(townInfo);
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
                MapEntityHero heroGameObject = (MapEntityHero)hero.MapObjectGameObject;
                var hit = newButtonHero.Q<VisualElement>(NameHit);
                hit.name = hero.IdEntity;

                hit.style.height = new StyleLength(new Length(hero.Data.hit, LengthUnit.Percent));

                var mana = newButtonHero.Q<VisualElement>(NameMana);
                mana.style.height = new StyleLength(new Length(hero.Data.mana, LengthUnit.Percent));

                newButtonHero.Q<VisualElement>("image").style.backgroundImage =
                    new StyleBackground(hero.ScriptableData.MenuSprite);

                newButtonHero.Q<Button>(NameAllAsideButton).clickable.clicked += () =>
                {
                    OnResetFocusButton();
                    OnSetActiveHero(hero);

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
        player = LevelManager.Instance.ActivePlayer;

        ShowMapButtons();
        aside.style.display = DisplayStyle.Flex;
        _footer.style.display = DisplayStyle.Flex;

        _herobox = aside.Q<VisualElement>(NameHeroBox);
        _herobox.Clear();

        var townbox = aside.Q<VisualElement>(NameTownBox);
        townbox.Clear();

        OnResetFocusButton();

        DrawHeroBox();

        for (int i = 0; i < countTown; i++)
        {
            var newButtonTown = _templateButtonTown.Instantiate();

            newButtonTown.style.flexGrow = 1;
            newButtonTown.AddToClassList(NameTownButton);

            if (i < player.DataPlayer.PlayerDataReferences.ListTown.Count)
            {
                EntityTown town = (EntityTown)player.DataPlayer.PlayerDataReferences.ListTown[i];

                newButtonTown.Q<VisualElement>("image").style.backgroundImage =
                    new StyleBackground(town.ScriptableData.MenuSprite);

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
            }
            else
            {
                newButtonTown.SetEnabled(false);
            }


            townbox.Add(newButtonTown);
        }

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

        Debug.Log("Change info hero");
        VisualElement HeroInfo = _templateHeroInfo.Instantiate();
        HeroInfo.style.flexGrow = 1;

        VisualElement heroImg = HeroInfo.Q<VisualElement>("HeroIcon");
        heroImg.style.backgroundImage = new StyleBackground(activeHero.ScriptableData.MenuSprite);
        Label name = HeroInfo.Q<Label>("HeroName");
        name.text = activeHero.Data.name;

        boxinfo.Clear();
        boxinfo.Add(HeroInfo);

        // Init hero info box
        InitHeroBox();
    }

    private void InitHeroBox()
    {
        EntityHero activeHero = player.ActiveHero;
        //var newHeroInfo = _templateHeroInfo.Instantiate();
        //newHeroInfo.style.flexGrow = 1;
        //var InfoBox = aside.Q<VisualElement>("AsideBoxInfo");
        //InfoBox.Clear();
        //InfoBox.Add(newHeroInfo);

        var _heroForceList = _aside.rootVisualElement.Q<VisualElement>("HeroForceList");
        _heroForceList.Clear();
        for (int i = 0; i < activeHero.Data.Creatures.Count; i++)
        {
            var creature = activeHero.Data.Creatures[i];
            var newForce = _templateHeroForce.Instantiate();
            newForce.AddToClassList("heroinfo_force_el");
            //newForce.style.flexGrow = 1;

            if (creature != null)
            {
                newForce.Q<VisualElement>("img").style.backgroundImage
                    = new StyleBackground(creature.ScriptableData.MenuSprite);
                newForce.Q<Label>("ForceValue").text = creature.Data.value.ToString();
            }

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
