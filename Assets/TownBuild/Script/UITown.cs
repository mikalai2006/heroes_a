using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.UIElements;


public class UITown : UILocaleBase
{
    [SerializeField] public UnityAction OnHideSetting;
    public static event Action OnExitFromTown;
    [SerializeField] public UnityAction OnSave;
    [SerializeField] private UIDocument _uiDoc;
    [SerializeField] private UITownInfo _uiTownInfo;
    [SerializeField] private UnityEngine.UI.Image _bgImage;
    [SerializeField] private GameObject _townGameObject;

    private VisualElement _box;
    private const string _nameButtonClose = "ButtonClose";
    private SceneInstance _townScene;

    private EntityTown _activeTown;
    private Player _activePlayer;
    private string NameOverlay = "Overlay";

    private Camera _cameraMain;
    public ScriptableBuildTown _activeBuildTown;
    private AsyncOperationHandle<ScriptableBuildTown> _asset;

    public void Init(SceneInstance townScene)
    {
        _cameraMain = Camera.main;
        _cameraMain.gameObject.SetActive(false);

        Player activePlayer = LevelManager.Instance.ActivePlayer;
        _activePlayer = activePlayer;
        _activeTown = _activePlayer.ActiveTown;

        // Init town prefab.
        ScriptableEntityTown scriptDataTown = (ScriptableEntityTown)_activeTown.ScriptableData;
        _activeBuildTown = scriptDataTown.BuildTown;

        _bgImage.sprite = _activeBuildTown.Bg;
        _box = _uiDoc.rootVisualElement;

        _uiTownInfo.Init(_box);
        _box.Q<VisualElement>("TownSide").style.display = DisplayStyle.Flex;
        _box.Q<VisualElement>("TownHeroVisit").style.display = DisplayStyle.Flex;
        _townScene = townScene;

        var btnClose = _box.Q<TemplateContainer>(_nameButtonClose).Q<Button>("Btn");
        btnClose.clickable.clicked += OnClickClose;

        Color color = _activePlayer.DataPlayer.color;
        color.a = LevelManager.Instance.ConfigGameSettings.alphaOverlay;
        UQueryBuilder<VisualElement> builder = new UQueryBuilder<VisualElement>(_box);
        List<VisualElement> list = builder.Name(NameOverlay).ToList();
        foreach (var overlay in list)
        {
            overlay.style.backgroundColor = color;
        }

        base.Localize(_box);
    }


    private async void OnClickClose()
    {
        _cameraMain.gameObject.SetActive(true);

        // Release asset prefab town.
        await GameManager.Instance.AssetProvider.UnloadAdditiveScene(_townScene);
        if (_asset.IsValid())
        {
            Addressables.ReleaseInstance(_asset);
            // Addressables.ReleaseInstance(_townPrefabAsset);
        }
        OnExitFromTown?.Invoke();
    }
}

