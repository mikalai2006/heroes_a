using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

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
        var allBuildsActiveTown = ResourceSystem.Instance.GetBuildTowns().Where(t => t.TypeFaction == scriptDataTown.TypeFaction).First();

        foreach (var parentBuild in allBuildsActiveTown.Builds)
        {
            var item = _templateBuildItem.Instantiate();

            // check status build and choose actual build.
            var currentBuild = parentBuild.BuildLevels[0];
            for (int i = 0; i < parentBuild.BuildLevels.Count; i++)
            {
                var levelBuild = parentBuild.BuildLevels[i];
                currentBuild = levelBuild;
                if ((_activeTown.Data.ProgressBuilds & levelBuild.TypeBuild) == levelBuild.TypeBuild)
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

            var t = HelperLanguage.GetLocaleText(currentBuild.Locale);
            var label = item.Q<Label>("Title");
            label.text = t.Text.title;

            item.style.width = new StyleLength(new Length(25, LengthUnit.Percent));
            item.style.height = new StyleLength(new Length(20, LengthUnit.Percent));

            item.Q<Button>("TownBuildItem").clickable.clicked += () =>
            {
                OnClickToBuild(currentBuild);
            };
            var allowBuild = _activeTown.Data.ProgressBuilds & currentBuild.RequiredBuilds;
            // Debug.Log($"{build.name} \n" +
            // $"May be build {allowBuild}\n" +
            // $"May be build 2 {town.Data.ProgressBuilds & currentBuild.RequiredBuilds}\n" +
            // $"TypeBuild={System.Convert.ToString((uint)currentBuild.TypeBuild, 2)}\n" +
            // $"Town progress={System.Convert.ToString((uint)town.Data.ProgressBuilds, 2)}\n");


            if (allowBuild == currentBuild.RequiredBuilds)
            {
                item.AddToClassList("town_listbuild_allow");
            }
            else
            {
                item.AddToClassList("town_listbuild_disallow");
            }
            _listBuild.Add(item);
        }

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
        var dialog = new UITownBuildItemDialogOperation(build);
        var result = await dialog.ShowAndHide();
        if (result.isOk)
        {
            _activeTown.Data.ProgressBuilds = _activeTown.Data.ProgressBuilds | build.TypeBuild;
            _dataResultDialog.build = result.build;
            _activeTown.Data.LevelsBuilds.Add(build.TypeBuild, 1);
            // ScriptableTown scriptDataTown = (ScriptableTown)_activeTown.ScriptableData;
            // var _scriptObjectBuildTown = ResourceSystem.Instance.GetBuildTowns().Where(t => t.TypeFaction == scriptDataTown.TypeFaction).First();

            // List<string> buildedBuilds = new List<string>();
            // foreach (var buld in _scriptObjectBuildTown.Builds)
            // {
            //     foreach (var buildLevel in buld.BuildLevels)
            //     {
            //         if ((_activeTown.Data.ProgressBuilds & buildLevel.TypeBuild) != buildLevel.TypeBuild)
            //         {
            //             // _scriptObjectBuildTown.Prefab.
            //         }
            //     }
            // }

            OnClickClose();
        }
    }
}

