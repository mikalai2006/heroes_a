using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Localization;
using System.Collections;
using System;

public class UITownListBuildWindow : UILocaleBase
{
    [SerializeField] private UIDocument _uiDoc;
    public UIDocument DialogApp => _uiDoc;
    [SerializeField] private VisualTreeAsset _templateBuildItem;
    private readonly string _nameOverlay = "Overlay";
    private readonly string _nameListBuild = "ListBuild";
    public static event Action OnCloseListBuilds;

    private Button _buttonClose;
    private VisualElement _listBuild;

    private TaskCompletionSource<DataResultBuildDialog> _processCompletionSource;

    public UnityEvent processAction;

    private DataDialogMapObject _dataDialog;
    private DataResultBuildDialog _dataResultDialog;

    private EntityTown _activeTown;
    private Player _activePlayer;
    private ScriptableEntityTown _scriptObjectBuildTown;

    // private Dictionary<TypeBuild, Build> AllBuilds = new Dictionary<TypeBuild, Build>();

    private void Start()
    {
        _buttonClose = DialogApp.rootVisualElement.Q<TemplateContainer>("Cancel").Q<Button>("Btn");
        _buttonClose.clickable.clicked += OnClickClose;

        _listBuild = DialogApp.rootVisualElement.Q<VisualElement>(_nameListBuild);
        base.Localize(DialogApp.rootVisualElement);
    }

    public async Task<DataResultBuildDialog> ProcessAction(DataDialogMapObject dataDialog)
    // , ScriptableBuildTown activeBuildTown
    {
        _dataDialog = dataDialog;
        _dataResultDialog = new DataResultBuildDialog();

        _activePlayer = LevelManager.Instance.ActivePlayer;
        _activeTown = _activePlayer.ActiveTown;

        UQueryBuilder<VisualElement> builder = new UQueryBuilder<VisualElement>(DialogApp.rootVisualElement);
        List<VisualElement> list = builder.Name(_nameOverlay).ToList();
        Color color = _activePlayer.DataPlayer.color;
        color.a = LevelManager.Instance.ConfigGameSettings.alphaOverlay;
        foreach (var overlay in list)
        {
            overlay.style.backgroundColor = color;
        }

        _listBuild.Clear();
        // ScriptableEntityTown scriptDataTown = (ScriptableEntityTown)_activeTown.ScriptableData;
        _scriptObjectBuildTown = _activeTown.ConfigData; // ResourceSystem.Instance.GetBuildTowns().Where(t => t.TypeFaction == scriptDataTown.TypeFaction).First();

        var allowBuilds = _activeTown.GetLisNextLevelBuilds(_scriptObjectBuildTown);

        var rows = new List<VisualElement>();
        int index = 0;
        foreach (var parentBuild in allowBuilds)
        {
            var item = _templateBuildItem.Instantiate();
            // var _bitsProgress = new HelperBitArray(44);
            // _bitsProgress.SetItem<TypeBuild>(_activeTown.Data.ProgressBuilds);
            // check status build and choose actual build.

            var indexNextBuild = 0;

            if (parentBuild.Value == parentBuild.Key.BuildLevels.Count - 1)
            {
                item.AddToClassList("town_listbuild_builded");
                indexNextBuild = parentBuild.Key.BuildLevels.Count - 1;
            }
            else
            {
                indexNextBuild = parentBuild.Value + 1;
            }

            Build currentBuild = parentBuild.Key.BuildLevels[indexNextBuild];

            if (
                (_activeTown.GetListNeedNoBuilds(currentBuild.RequireBuilds).Count == 0
                || currentBuild.RequireBuilds.Count == 0)
                && _activePlayer.IsExistsResource(currentBuild.CostResource)
                && _activeTown.Data.countBuild < LevelManager.Instance.ConfigGameSettings.countBuildPerDay
                )
            {
                item.AddToClassList("town_listbuild_allow");
            }
            else
            {
                item.AddToClassList("town_listbuild_disallow");
            }


            var img = item.Q<VisualElement>("Img");
            img.style.backgroundImage = new StyleBackground(currentBuild.MenuSprite);

            var label = item.Q<Label>("Title");
            label.text = currentBuild.Text.title.GetLocalizedString();

            item.AddToClassList("w-25");
            item.AddToClassList("h-full");

            // item.style.width = new StyleLength(new Length(25, LengthUnit.Percent));
            // item.style.height = new StyleLength(new Length(20, LengthUnit.Percent));

            item.Q<Button>("TownBuildItem").clickable.clicked += () =>
            {
                OnClickToBuild(parentBuild.Key, indexNextBuild);
            };

            // if (_activeTown.Data.ProgressBuilds.Intersect(currentBuild.RequiredBuilds).Count()
            //         == currentBuild.RequiredBuilds.Count()
            //     || currentBuild.RequiredBuilds.Length == 0)

            rows.Add(item);
            index++;
            if (index % 4 == 0 || index == allowBuilds.Count)
            {
                var row = new VisualElement();
                row.AddToClassList("w-full");
                row.style.height = new StyleLength(new Length(150, LengthUnit.Pixel));
                row.style.flexDirection = FlexDirection.Row;
                for (int i = 0; i < rows.Count; i++)
                {
                    row.Add(rows[i]);
                }
                _listBuild.Add(row);
                rows.Clear();
            }
        }

        _processCompletionSource = new TaskCompletionSource<DataResultBuildDialog>();

        return await _processCompletionSource.Task;
    }

    private void OnClickClose()
    {
        _dataResultDialog.isOk = false;
        _processCompletionSource.SetResult(_dataResultDialog);

        OnCloseListBuilds?.Invoke();
        processAction?.Invoke();
    }

    public async void OnClickToBuild(ScriptableBuilding buildConfig, int level)
    {
        List<string> requireBuilds = new List<string>();
        var build = buildConfig.BuildLevels[level];

        var allRequireBuilds = _activeTown.GetListNeedNoBuilds(build.RequireBuilds);
        foreach (var buildItem in allRequireBuilds)
        {
            requireBuilds.Add(buildItem.Text.title.GetLocalizedString());
        }
        // foreach (var buildItem in build.RequiredBuilds)
        // {
        //     var buildNeed = AllBuilds[buildItem];
        //     if (!_activeTown.Data.ProgressBuilds.Contains(buildItem))
        //     {
        //         requireBuilds.Add(buildNeed.Text.title.GetLocalizedString());
        //     }
        // }
        // // foreach (var buld in _scriptObjectBuildTown.Builds)
        // // {
        // //     foreach (var buildLevel in buld.BuildLevels)
        // //     {
        // //         if (
        // //             build.RequiredBuilds.Contains(buildLevel.TypeBuild)
        // //             && !_activeTown.Data.ProgressBuilds.Contains(buildLevel.TypeBuild)
        // //             )
        // //         {
        // //             // var textBuild = HelperLanguage.GetLocaleText(buildLevel.Locale);
        // //             requireBuilds.Add(buildLevel.Text.title.GetLocalizedString());
        // //         }
        // //     }
        // // }
        string textRequireBuilds = "";
        if (requireBuilds.Count > 0)
        {
            LocalizedString require = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "build_require");
            textRequireBuilds += require.GetLocalizedString() + ": " + System.String.Join(", ", requireBuilds);
        }
        else
        {
            LocalizedString message = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "build_enable");
            textRequireBuilds += message.GetLocalizedString();
        }


        // var t = HelperLanguage.GetLocaleText(build.Locale);
        LocalizedString titlePrefix = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "build");

        var dataForDialog = new DataDialogBuild()
        {
            title = titlePrefix.GetLocalizedString() + ": " + build.Text.title.GetLocalizedString(), // t.Text.title,
            description = build.Text.description.GetLocalizedString(), // t.Text.description,
            MenuSprite = build.MenuSprite,
            textRequireBuild = textRequireBuilds,
            CostResource = build.CostResource,
            isNotBuild = _activeTown.GetLevelBuild(buildConfig)
                == buildConfig.BuildLevels.Count - 1
                // || !_activePlayer.IsExistsResource(build.CostResource)
                || _activeTown.GetListNeedNoBuilds(build.RequireBuilds).Count() != 0,
            // _activeTown.Data.ProgressBuilds.Contains(build.TypeBuild)
            //     || !_activePlayer.IsExistsResource(build.CostResource)
            //     || _activeTown.Data.ProgressBuilds.Intersect(build.RequiredBuilds).Count()
            //          < build.RequiredBuilds.Count(),
        };
        var dialog = new UITownBuildItemDialogOperation(dataForDialog);
        var result = await dialog.ShowAndHide();
        if (result.isOk)
        {
            // Create build.
            // _dataResultDialog.PreProgressBuild = _activeTown.Data.ProgressBuilds;
            // _activeTown.Data.ProgressBuilds.Add(build.TypeBuild);
            _dataResultDialog.build = build;
            var newBuild = _activeTown.CreateBuild(buildConfig, level, _activePlayer);
            if (level == 0)
            {
                newBuild.CreateGameObject(true);
            }
            else
            {
                newBuild.UpdateGameObject(true);
            }
            // _activeTown.Data.LevelsBuilds.Add(build.TypeBuild, 1);

            for (int i = 0; i < build.CostResource.Count; i++)
            {
                _activePlayer.ChangeResource(
                    build.CostResource[i].Resource.TypeResource,
                    -build.CostResource[i].Count
                    );
            }
            OnClickClose();
        }
    }
}

