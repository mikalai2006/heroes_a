using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Cysharp.Threading.Tasks;

using Loader;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.UIElements;


public class UIGameAside : MonoBehaviour
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
    [SerializeField] private VisualTreeAsset _templateButtonHero;
    [SerializeField] private VisualTreeAsset _templateButtonTown;
    [SerializeField] private VisualTreeAsset _templateHeroInfo;
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
    private string NameHit = "hit";
    private string NameMana = "mana";
    private string NameOverlay = "Overlay";

    const int countTown = 3;

    private SceneInstance _scene;

    private void Start()
    {
        GameManager.OnAfterStateChanged += OnAfterStateChanged;
    }
    private void OnDestroy() => GameManager.OnAfterStateChanged -= OnAfterStateChanged;

    private void OnAfterStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.StepNextPlayer:
                NextStep();
                break;
            case GameState.StartMoveHero:
                StartMoveHero();
                break;
            case GameState.StopMoveHero:
                StopMoveHero();
                break;
            case GameState.CreatePathHero:
                OnToogleEnableBtnGoHero();
                break;
            case GameState.ChangeResources:
                OnRedrawResource();
                break;
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
        // aside.style.display = DisplayStyle.None;
        var loadingOperations = new Queue<ILoadingOperation>();

        // GameManager.Instance.AssetProvider.UnloadAdditiveScene(_scene);
        loadingOperations.Enqueue(new TownLoadOperation(player.ActiveTown));
        await GameManager.Instance.LoadingScreenProvider.LoadAndDestroy(loadingOperations);

    }

    public void Init(SceneInstance scene)
    {
        try
        {
            _scene = scene;
            // GameObject GridMap = GameObject.FindGameObjectWithTag("Map")?.GetComponent<GameObject>();
            // GridMap.SetActive(false);

            //GameManager.OnAfterStateChanged += OnChangeGameState;
            aside = _aside.rootVisualElement.Q<VisualElement>(NameWrapper);

            var btnGameMenu = _aside.rootVisualElement.Q<Button>(NameBtnGameMenu);
            btnGameMenu.clickable.clicked += async () =>
            {
                DataResultGameMenu result = await ShowGameMenu();
                if (result.isOk)
                {

                    var loadingOperations = new Queue<ILoadingOperation>();
                    GameManager.Instance.AssetProvider.UnloadAdditiveScene(_scene);
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
            // boxinfo.style.display = DisplayStyle.None;

        }
        catch (Exception e)
        {
            Debug.LogWarning("Aside UI error: \n" + e);
        }

    }

    private void OnMoveHero(ClickEvent evt)
    {
        GameManager.Instance.ChangeState(GameState.StartMoveHero);
    }

    private void NextStep()
    {
        player = LevelManager.Instance.ActivePlayer;

        aside.style.display = DisplayStyle.Flex;
        _footer.style.display = DisplayStyle.Flex;
        //UQueryBuilder<VisualElement> builder = new UQueryBuilder<VisualElement>(_aside.rootVisualElement);
        //List<VisualElement> list = builder.Class("border-color").ToList();
        //foreach (var overlay in list)
        //{
        //    overlay.style.borderTopColor= color; // .RemoveFromClassList("border-color");
        //}

        var herobox = aside.Q<VisualElement>(NameHeroBox);
        herobox.Clear();

        var townbox = aside.Q<VisualElement>(NameTownBox);
        townbox.Clear();


        OnResetFocusButton();
        OnSetActiveHero(null, player.ActiveHero);

        //Debug.Log($"UI Player aside::: id{player.data.id} hero[{player.data.ListHero.Count}] town[{player.data.ListTown.Count}]");

        for (int i = 0; i < 5; i++)
        {

            var newButtonHero = _templateButtonHero.Instantiate();
            //newButtonHero.style.flexGrow = 1;
            newButtonHero.AddToClassList(NameHeroButton);
            //Debug.Log($"hero btn {i}");

            if (i < player.DataPlayer.ListHero.Count)
            {
                Hero hero = player.DataPlayer.ListHero[i];

                var hit = newButtonHero.Q<VisualElement>(NameHit);
                // hit.UnbindAllProperties();
                hit.BindProperty(hero.hit);
                // hit.style.height = new StyleLength(new Length(12, LengthUnit.Percent));

                var mana = newButtonHero.Q<VisualElement>(NameMana); //.style.height = new StyleLength(new Length(37, LengthUnit.Percent));
                // mana.UnbindAllProperties();
                mana.BindProperty(hero.mana);

                newButtonHero.Q<VisualElement>("image").style.backgroundImage = new StyleBackground(hero.ScriptableData.MenuSprite);

                newButtonHero.Q<Button>(NameAllAsideButton).RegisterCallback<ClickEvent>((ClickEvent evt) =>
                {
                    OnResetFocusButton();
                    OnSetActiveHero(evt, hero);

                    newButtonHero.Q<Button>(NameAllAsideButton).AddToClassList(NameSelectedButton);
                });

                if (hero == player.ActiveHero)
                {
                    newButtonHero.Q<Button>(NameAllAsideButton).AddToClassList(NameSelectedButton);
                }
                //.clickable.clicked += () =>
                //{
                //    //Debug.Log($"Click button {hero.HeroData.position}");
                //    OnResetFocusButton();
                //    player.SetActiveHero(hero);

                //    newButtonHero.Q<Button>(NameAllAsideButton).AddToClassList(NameSelectedButton);

                //    if (boxinfo.style.display == DisplayStyle.None)
                //    {
                //        boxinfo.style.display = DisplayStyle.Flex;
                //        ChangeHeroInfo();
                //    } else
                //    {
                //        boxinfo.style.display = DisplayStyle.None;
                //    }
                //};

            }
            else
            {
                newButtonHero.SetEnabled(false);
            }

            herobox.Add(newButtonHero);
        }


        for (int i = 0; i < countTown; i++)
        {
            var newButtonTown = _templateButtonTown.Instantiate();

            newButtonTown.style.flexGrow = 1;
            newButtonTown.AddToClassList(NameTownButton);

            if (i < player.DataPlayer.ListTown.Count)
            {
                BaseTown town = (BaseTown)player.DataPlayer.ListTown[i];

                newButtonTown.Q<VisualElement>("image").style.backgroundImage = new StyleBackground(town.ScriptableData.MenuSprite);

                newButtonTown.Q<Button>(NameAllAsideButton).clickable.clicked += async () =>
                {
                    //Debug.Log($"Click button {town.TownData.position}");
                    if (player.ActiveTown == town)
                    {
                        //Debug.Log($"Go to town");
                        //UIManager.Instance.ShowTown();
                        ShowTown();
                    }
                    else
                    {
                        player.ActiveTown = town;
                    }
                    OnResetFocusButton();
                    newButtonTown.Q<Button>(NameAllAsideButton).AddToClassList(NameSelectedButton);

                };
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
        color.a = .6f;

        // Queries overlay.
        UQueryBuilder<VisualElement> builder = new UQueryBuilder<VisualElement>(_aside.rootVisualElement);
        List<VisualElement> list = builder.Name(NameOverlay).ToList();
        foreach (var overlay in list)
        {
            overlay.style.backgroundColor = color;
        }
    }

    private void StopMoveHero()
    {
        UQueryBuilder<Button> btns2 = new UQueryBuilder<Button>(_aside.rootVisualElement);
        List<Button> listBtn2 = btns2.Class("button").ToList();
        foreach (var btn in listBtn2)
        {
            btn.SetEnabled(true);
        }
        OnToogleEnableBtnGoHero();
    }

    private void StartMoveHero()
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
        Hero activeHero = player.ActiveHero;
        if (activeHero != null)
        {
            btn.SetEnabled(activeHero.CanMove);
        }
        else
        {
            btn.SetEnabled(false);
        }
    }

    private void OnSetActiveHero(ClickEvent evt, Hero hero)
    {

        Hero activeHero = player.ActiveHero;

        // OnResetFocusButton();
        ChangeHeroInfo();

        if (activeHero == hero)
        {
            if (boxinfo.style.display == DisplayStyle.None)
            {
                // ChangeHeroInfo();
                boxinfo.style.display = DisplayStyle.Flex;
            }
            else
            {
                boxinfo.style.display = DisplayStyle.None;
            }
        }
        else
        {
            boxinfo.style.display = DisplayStyle.None;

            //activeHero = hero;
            // player.SetActiveHero(hero);
            hero.SetHeroAsActive();
        }


        // OnToogleEnableBtnGoHero();
    }

    private void ChangeHeroInfo()
    {
        Hero activeHero = player.ActiveHero;
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
        Debug.Log($"Init UI Heroinfo");
        //var newHeroInfo = _templateHeroInfo.Instantiate();
        //newHeroInfo.style.flexGrow = 1;
        //var InfoBox = aside.Q<VisualElement>("AsideBoxInfo");
        //InfoBox.Clear();
        //InfoBox.Add(newHeroInfo);

        var _heroForceList = _aside.rootVisualElement.Q<VisualElement>("HeroForceList");
        _heroForceList.Clear();
        for (int i = 0; i < 7; i++)
        {
            var newForce = _templateHeroForce.Instantiate();
            //newForce.style.flexGrow = 1;
            newForce.AddToClassList("heroinfo_force_el");
            if (i >= 6)
            {
                newForce.Q<VisualElement>("img").style.backgroundImage = null;
            }
            newForce.Q<Label>("ForceValue").text = i < 6 ? i.ToString() : "";
            _heroForceList.Add(newForce);

        }

    }

    private void OnCalculateArrow()
    {
        var ArrowTopBtn = aside.Q<VisualElement>("arrowtop");
        var ArrowBottomBtn = aside.Q<VisualElement>("arrowbottom");
        if (player.DataPlayer.ListTown.Count < countTown)
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
        }
        // boxinfo.style.display = DisplayStyle.None;
    }

    public void OnRedrawResource()
    {

        Player player = LevelManager.Instance.ActivePlayer;

        //_footer.style.unityBackgroundImageTintColor = player.DataPlayer.color;

        var gold = _footer.Q<Label>("GoldValue");
        gold.text = player.DataPlayer.Resource.gold.ToString();

        var wood = _footer.Q<Label>("WoodValue");
        wood.text = player.DataPlayer.Resource.wood.ToString();

        var iron = _footer.Q<Label>("IronValue");
        iron.text = player.DataPlayer.Resource.iron.ToString();

        var mercury = _footer.Q<Label>("MercuryValue");
        mercury.text = player.DataPlayer.Resource.mercury.ToString();

        var diamond = _footer.Q<Label>("DiamondValue");
        diamond.text = player.DataPlayer.Resource.diamond.ToString();

        var sulfur = _footer.Q<Label>("SulfurValue");
        sulfur.text = player.DataPlayer.Resource.sulfur.ToString();

        var gem = _footer.Q<Label>("GemValue");
        gem.text = player.DataPlayer.Resource.gem.ToString();
    }
}
