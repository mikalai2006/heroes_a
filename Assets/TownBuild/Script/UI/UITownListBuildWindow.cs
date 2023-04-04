using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Localization;
using System.Collections;

public class UITownListBuildWindow : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDoc;
    public UIDocument DialogApp => _uiDoc;
    [SerializeField] private VisualTreeAsset _templateBuildItem;
    private readonly string _nameButtonClose = "ButtonClose";
    private readonly string _nameOverlay = "Overlay";
    private readonly string _nameListBuild = "ListBuild";

    private Button _buttonClose;
    private VisualElement _listBuild;

    private TaskCompletionSource<DataResultBuildDialog> _processCompletionSource;

    public UnityEvent processAction;

    private DataDialog _dataDialog;
    private DataResultBuildDialog _dataResultDialog;

    private BaseTown _activeTown;
    private Player _activePlayer;
    private ScriptableBuildTown _scriptObjectBuildTown;

    private void Awake()
    {
        _buttonClose = DialogApp.rootVisualElement.Q<Button>(_nameButtonClose);
        _buttonClose.clickable.clicked += OnClickClose;

        _listBuild = DialogApp.rootVisualElement.Q<VisualElement>(_nameListBuild);

    }

    public async Task<DataResultBuildDialog> ProcessAction(DataDialog dataDialog)
    {
        _dataDialog = dataDialog;
        _dataResultDialog = new DataResultBuildDialog();

        _activePlayer = LevelManager.Instance.ActivePlayer;
        _activeTown = _activePlayer.ActiveTown;

        UQueryBuilder<VisualElement> builder = new UQueryBuilder<VisualElement>(DialogApp.rootVisualElement);
        List<VisualElement> list = builder.Name(_nameOverlay).ToList();
        Color color = _activePlayer.DataPlayer.color;
        color.a = .6f;
        foreach (var overlay in list)
        {
            overlay.style.backgroundColor = color;
        }

        _listBuild.Clear();
        ScriptableTown scriptDataTown = (ScriptableTown)_activeTown.ScriptableData;
        _scriptObjectBuildTown = ResourceSystem.Instance.GetBuildTowns().Where(t => t.TypeFaction == scriptDataTown.TypeFaction).First();

        foreach (var parentBuild in _scriptObjectBuildTown.Builds)
        {
            var item = _templateBuildItem.Instantiate();

            // var _bitsProgress = new HelperBitArray(44);
            // _bitsProgress.SetItem<TypeBuild>(_activeTown.Data.ProgressBuilds);
            // check status build and choose actual build.
            var currentBuild = parentBuild.BuildLevels[0];
            for (int i = 0; i < parentBuild.BuildLevels.Count; i++)
            {
                var levelBuild = parentBuild.BuildLevels[i];
                currentBuild = levelBuild;
                if (_activeTown.Data.ProgressBuilds.Contains(levelBuild.TypeBuild))
                {
                    if (i == parentBuild.BuildLevels.Count - 1)
                    {
                        item.AddToClassList("town_listbuild_builded");
                    }
                }
                else
                {
                    // currentBuild = levelBuild;
                    break;
                }
            }

            var img = item.Q<VisualElement>("Img");
            img.style.backgroundImage = new StyleBackground(currentBuild.MenuSprite);

            // var t = HelperLanguage.GetLocaleText(currentBuild.Locale);
            var label = item.Q<Label>("Title");
            label.text = currentBuild.Text.title.GetLocalizedString(); //t.Text.title;

            item.style.width = new StyleLength(new Length(25, LengthUnit.Percent));
            item.style.height = new StyleLength(new Length(20, LengthUnit.Percent));

            item.Q<Button>("TownBuildItem").clickable.clicked += () =>
            {
                OnClickToBuild(currentBuild);
            };

            // var _bitsRequired = new HelperBitArray(44);
            // _bitsRequired.SetItem<TypeBuild>(currentBuild.RequiredBuilds);
            // var allowBuild = _bitsProgress.Bits.And(_bitsRequired.Bits);
            //var allowBuildArmy = _activeTown.Data.ProgressBuildsArmy & currentBuild.RequiredBuildsArmy;
            // Debug.Log($"{build.name} \n" +
            // $"May be build {allowBuild}\n" +
            // $"May be build 2 {town.Data.ProgressBuilds & currentBuild.RequiredBuilds}\n" +
            // $"TypeBuild={System.Convert.ToString((uint)currentBuild.TypeBuild, 2)}\n" +
            // $"Town progress={System.Convert.ToString((uint)town.Data.ProgressBuilds, 2)}\n");


            if (_activeTown.Data.ProgressBuilds.Intersect(currentBuild.RequiredBuilds).Any()
                || currentBuild.RequiredBuilds.Length == 0)
            {
                item.AddToClassList("town_listbuild_allow");
            }
            else
            {
                item.AddToClassList("town_listbuild_disallow");
            }
            _listBuild.Add(item);
        }

        // foreach (var parentBuild in _scriptObjectBuildTown.BuildsArmy)
        // {
        //     var item = _templateBuildItem.Instantiate();

        //     // check status build and choose actual build.
        //     var currentBuild = parentBuild.BuildLevels[0];
        //     for (int i = 0; i < parentBuild.BuildLevels.Count; i++)
        //     {
        //         var levelBuild = parentBuild.BuildLevels[i];
        //         currentBuild = levelBuild;
        //         if ((_activeTown.Data.ProgressBuildsArmy & levelBuild.TypeBuild) == levelBuild.TypeBuild)
        //         {
        //             if (i == parentBuild.BuildLevels.Count - 1)
        //             {
        //                 item.AddToClassList("town_listbuild_builded");
        //             }
        //         }
        //         else
        //         {
        //             // currentBuild = levelBuild;
        //             break;
        //         }
        //     }

        //     var img = item.Q<VisualElement>("Img");
        //     img.style.backgroundImage = new StyleBackground(currentBuild.MenuSprite);

        //     var t = HelperLanguage.GetLocaleText(currentBuild.Locale);
        //     var label = item.Q<Label>("Title");
        //     label.text = t.Text.title;

        //     item.style.width = new StyleLength(new Length(25, LengthUnit.Percent));
        //     item.style.height = new StyleLength(new Length(20, LengthUnit.Percent));

        //     item.Q<Button>("TownBuildItem").clickable.clicked += () =>
        //     {
        //         OnShowDialogBuildArmy(currentBuild);
        //     };
        //     var allowBuild = _activeTown.Data.ProgressBuilds & currentBuild.RequiredBuilds;
        //     var allowBuildArmy = _activeTown.Data.ProgressBuildsArmy & currentBuild.RequiredBuildsArmy;
        //     // Debug.Log($"{build.name} \n" +
        //     // $"May be build {allowBuild}\n" +
        //     // $"May be build 2 {town.Data.ProgressBuilds & currentBuild.RequiredBuilds}\n" +
        //     // $"TypeBuild={System.Convert.ToString((uint)currentBuild.TypeBuild, 2)}\n" +
        //     // $"Town progress={System.Convert.ToString((uint)town.Data.ProgressBuilds, 2)}\n");


        //     if (allowBuild == currentBuild.RequiredBuilds && allowBuildArmy == currentBuild.RequiredBuildsArmy)
        //     {
        //         item.AddToClassList("town_listbuild_allow");
        //     }
        //     else
        //     {
        //         item.AddToClassList("town_listbuild_disallow");
        //     }
        //     _listBuild.Add(item);
        // }

        _processCompletionSource = new TaskCompletionSource<DataResultBuildDialog>();

        return await _processCompletionSource.Task;
    }

    private void OnClickClose()
    {
        _dataResultDialog.isOk = false;
        _processCompletionSource.SetResult(_dataResultDialog);

        processAction?.Invoke();
    }

    public async void OnClickToBuild(Build build)
    {
        List<string> requireBuilds = new List<string>();
        foreach (var buld in _scriptObjectBuildTown.Builds)
        {
            foreach (var buildLevel in buld.BuildLevels)
            {
                if (
                    build.RequiredBuilds.Contains(buildLevel.TypeBuild)
                    && !_activeTown.Data.ProgressBuilds.Contains(buildLevel.TypeBuild)
                    )
                {
                    // var textBuild = HelperLanguage.GetLocaleText(buildLevel.Locale);
                    requireBuilds.Add(buildLevel.Text.title.GetLocalizedString());
                }
            }
        }
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
            isBuilded = _activeTown.Data.ProgressBuilds.Contains(build.TypeBuild),
        };
        var dialog = new UITownBuildItemDialogOperation(dataForDialog);
        var result = await dialog.ShowAndHide();
        if (result.isOk)
        {
            _dataResultDialog.PreProgressBuild = _activeTown.Data.ProgressBuilds;
            _activeTown.Data.ProgressBuilds.Add(build.TypeBuild);
            _dataResultDialog.build = build;
            _activeTown.Data.LevelsBuilds.Add(build.TypeBuild, 1);

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

