using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine.Localization;

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

    private TaskCompletionSource<DataResultDialog> _processCompletionSource;

    public UnityEvent processAction;

    private DataDialog _dataDialog;
    private DataResultDialog _dataResultDialog;

    private BaseTown _activeTown;
    private Player _activePlayer;

    private void Awake()
    {
        _buttonClose = DialogApp.rootVisualElement.Q<Button>(_nameButtonClose);
        _buttonClose.clickable.clicked += OnClickClose;

        _listBuild = DialogApp.rootVisualElement.Q<VisualElement>(_nameListBuild);

    }

    public async Task<DataResultDialog> ProcessAction(DataDialog dataDialog)
    {
        _dataDialog = dataDialog;
        _dataResultDialog = new DataResultDialog();

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

        foreach (var build in allBuildsActiveTown.Builds)
        {
            var item = _templateBuildItem.Instantiate();

            // check status build and choose actual build.
            var currentBuild = build.BuildLevels[0];
            for (int i = 0; i < build.BuildLevels.Count; i++)
            {
                var levelBuild = build.BuildLevels[i];
                currentBuild = levelBuild;
                if ((_activeTown.Data.ProgressBuilds & levelBuild.TypeBuild) == levelBuild.TypeBuild)
                {
                    if (i == build.BuildLevels.Count - 1)
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

        _processCompletionSource = new TaskCompletionSource<DataResultDialog>();

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
        var _build = build; //.BuildLevels[0];
        var t = HelperLanguage.GetLocaleText(_build.Locale);

        LocalizedString titlePrefix = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "build");

        var requireResource = new List<DataDialogItem>();
        foreach (var res in _build.CostResource)
        {
            requireResource.Add(new DataDialogItem()
            {
                Sprite = res.Resource.MenuSprite,
                Value = res.Count
            });
        }

        var dialog = new UITownBuildItemDialogOperation(new DataDialog()
        {
            Header = titlePrefix.GetLocalizedString() + t.Text.title,
            Description = t.Text.description,
            Sprite = _build.MenuSprite,
            Value = requireResource
        });
        var result = await dialog.ShowAndHide();
        if (result.isOk)
        {
            Debug.Log("Go build!");
            _activeTown.Data.ProgressBuilds = _activeTown.Data.ProgressBuilds | build.TypeBuild;
            OnClickClose();
        }
    }
}

