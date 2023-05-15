using System;
using System.Collections.Generic;

using Loader;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.UIElements;


public class UIArena : UILocaleBase
{
    public ArenaManager arenaManager;
    [SerializeField] private UIDocument _uiDoc;
    [SerializeField] private VisualTreeAsset _templateButton;
    [SerializeField] private VisualTreeAsset _templateShortCreatureInfo;
    [SerializeField] private UnityEngine.UI.Image _bgImage;

    public static event Action OnNextCreature;
    public static event Action OnOpenSpellBook;

    private VisualElement _box;
    private VisualElement _helpHero;
    private VisualElement _helpEnemy;
    private const string _arenaButtons = "ArenaButtons";
    private SceneInstance _arenaScene;
    private Camera _cameraMain;
    private AsyncOperationHandle<ScriptableEntityTown> _asset;

    private void Awake()
    {
        ArenaManager.OnSetNextCreature += DrawHelpCreature;
    }
    private void OnDestroy()
    {
        ArenaManager.OnSetNextCreature -= DrawHelpCreature;
    }

    public void Init(SceneInstance arenaScene)
    {
        _cameraMain = Camera.main;
        _cameraMain.gameObject.SetActive(false);

        _arenaScene = arenaScene;

        // _bgImage.sprite = _activeBuildTown.Bg;
        _box = _uiDoc.rootVisualElement;

        _helpEnemy = _box.Q<VisualElement>("SideRight");
        _helpHero = _box.Q<VisualElement>("SideLeft");

        // var btnClose = _templateButton.Instantiate();
        // btnClose.Q<Button>("Btn").clickable.clicked += OnClickClose;
        // _box.Q<VisualElement>(_arenaButtons).Add(btnClose);

        var btnWaitCreature = _box.Q<Button>("WaitButton");
        btnWaitCreature.clickable.clicked += OnClickNextCreature;

        var btnDamageCreature = _box.Q<Button>("DamageButton");
        btnDamageCreature.clickable.clicked += OnClickNextCreature;

        var btnRunCreature = _box.Q<Button>("RunButton");
        btnRunCreature.clickable.clicked += OnClickClose;

        var btnSpellBook = _box.Q<Button>("SpellBookButton");
        btnSpellBook.clickable.clicked += () =>
        {
            OnOpenSpellBook?.Invoke();
        };

        DrawHelpCreature();

        base.Localize(_box);
    }

    private void OnClickNextCreature()
    {
        OnNextCreature?.Invoke();
        DrawHelpCreature();
    }

    private void DrawHelpCreature()
    {
        _helpHero.style.display = DisplayStyle.None;
        _helpEnemy.style.display = DisplayStyle.None;

        VisualElement blokInfoCreature;
        var activeEntity = arenaManager.ArenaQueue.activeEntity;
        if (activeEntity.TypeArenaPlayer == TypeArenaPlayer.Left)
        {
            blokInfoCreature = _helpHero;
            _helpHero.style.display = DisplayStyle.Flex;
        }
        else
        {
            blokInfoCreature = _helpEnemy;
            _helpEnemy.style.display = DisplayStyle.Flex;
        }
        VisualElement infoBlok = blokInfoCreature.Q<VisualElement>("Info");
        infoBlok.Clear();

        VisualElement blokParamsCreature = _templateShortCreatureInfo.Instantiate();
        infoBlok.Add(blokParamsCreature);
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
        infoBlok.Q<Label>("Damage").text = string.Format("{0}", creatureData.CreatureParams.DamageMin, creatureData.CreatureParams.HP);

        // Label label = new Label();
        // label.text = activeEntity.Entity.ScriptableDataAttribute.name;
        // blokInfoCreature.Add(label);
        base.Localize(_box);
    }

    private async void OnClickClose()
    {
        _cameraMain.gameObject.SetActive(true);

        // Release asset prefab town.
        await GameManager.Instance.AssetProvider.UnloadAdditiveScene(_arenaScene);
        if (_asset.IsValid())
        {
            Addressables.ReleaseInstance(_asset);
        }

        var loadingOperations = new Queue<ILoadingOperation>();
        loadingOperations.Enqueue(new MenuAppOperation());
        await GameManager.Instance.LoadingScreenProvider.LoadAndDestroy(loadingOperations);
    }
}

