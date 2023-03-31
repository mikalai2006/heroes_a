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

        Player player = LevelManager.Instance.ActivePlayer;

        UQueryBuilder<VisualElement> builder = new UQueryBuilder<VisualElement>(DialogApp.rootVisualElement);
        List<VisualElement> list = builder.Name(_nameOverlay).ToList();
        Color color = player.DataPlayer.color;
        color.a = .6f;
        foreach (var overlay in list)
        {
            overlay.style.backgroundColor = color;
        }

        _listBuild.Clear();
        BaseTown town = player.ActiveTown;
        ScriptableTown scriptDataTown = (ScriptableTown)town.ScriptableData;
        var allBuildsActiveTown = ResourceSystem.Instance.GetBuildTowns().Where(t => t.TypeFaction == scriptDataTown.TypeFaction).First();

        foreach (var build in allBuildsActiveTown.Builds)
        {
            var item = _templateBuildItem.Instantiate();

            // check ststus build.
            var currentBuild = build.BuildLevels[0];
            if ((uint)(town.Data.ProgressBuilds & currentBuild.TypeBuild) != 0)
            {
                if (build.BuildLevels.Count > 1)
                {
                    currentBuild = build.BuildLevels[1];
                }
                else
                {
                    item.AddToClassList("town_listbuild_builded");

                }
            }

            var img = item.Q<VisualElement>("Img");
            img.style.backgroundImage = new StyleBackground(build.BuildLevels[0].MenuSprite);

            var t = HelperLanguage.GetLocaleText(build.BuildLevels[0].Locale);
            var label = item.Q<Label>("Title");
            label.text = t.Text.title;

            // float w = (_listBuild.resolvedStyle.width - 30) / 3;
            // float h = w / 2;
            item.style.width = new StyleLength(new Length(25, LengthUnit.Percent));
            item.style.height = new StyleLength(new Length(20, LengthUnit.Percent));

            item.Q<Button>("TownBuildItem").clickable.clicked += () =>
            {
                OnClickToBuild(build);
            };
            var allowBuild = town.Data.ProgressBuilds & build.BuildLevels[0].RequiredBuilds;
            Debug.Log($"{build.name} \n" +
            $"May be build {allowBuild}\n" +
            $"May be build 2 {town.Data.ProgressBuilds & build.BuildLevels[0].RequiredBuilds}\n" +
            $"TypeBuild={System.Convert.ToString((uint)build.BuildLevels[0].TypeBuild, 2)}\n" +
            $"Town progress={System.Convert.ToString((uint)town.Data.ProgressBuilds, 2)}\n");


            if (allowBuild == build.BuildLevels[0].RequiredBuilds)
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

    public async void OnClickToBuild(ScriptableBuildBase build)
    {
        var _build = build.BuildLevels[0];
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
        }
    }
}

