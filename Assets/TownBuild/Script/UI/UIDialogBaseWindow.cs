using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UIElements;

public class UIDialogBaseWindow : UILocaleBase
{
    [SerializeField] private UIDocument _uiDoc;
    public UIDocument DialogApp => _uiDoc;
    [SerializeField] private VisualTreeAsset _templateInsertBlok;
    private readonly string _nameHeaderLabel = "HeaderDialog";
    private readonly string _nameGeneralBlok = "GeneralBlok";
    private readonly string _nameOverlay = "Overlay";
    protected Label Title;
    private VisualElement _generalBlok;
    protected VisualElement Panel;
    protected VisualElement root;
    protected SOGameSetting GameSetting;
    // protected TaskCompletionSource<DataResultBuildDialog> _processCompletionSource;
    // // public UnityEvent processAction;
    // protected DataResultBuildDialog _dataResultDialog;
    protected Player _activePlayer;

    public virtual void Start()
    {
        root = DialogApp.rootVisualElement;

        GameSetting = LevelManager.Instance.ConfigGameSettings;

        Title = root.Q<Label>(_nameHeaderLabel);
        _generalBlok = root.Q<VisualElement>(_nameGeneralBlok);

        Panel = root.Q<VisualElement>("Panel");
        Panel.AddToClassList("w-50");

        var panelBlok = root.Q<VisualElement>("PanelBlok");
        panelBlok.style.flexGrow = 1;

        VisualElement docDialogBlok = _templateInsertBlok.Instantiate();
        docDialogBlok.style.flexGrow = 1;
        _generalBlok.Clear();
        _generalBlok.Add(docDialogBlok);
    }

    protected void Init()
    {
        // _dataResultDialog = new DataResultBuildDialog();
        // _processCompletionSource = new TaskCompletionSource<DataResultBuildDialog>();

        _activePlayer = LevelManager.Instance.ActivePlayer;
        UQueryBuilder<VisualElement> builder = new UQueryBuilder<VisualElement>(root);
        List<VisualElement> list = builder.Name(_nameOverlay).ToList();
        Color color = _activePlayer.DataPlayer.color;
        color.a = LevelManager.Instance.ConfigGameSettings.alphaOverlay;
        foreach (var overlay in list)
        {
            overlay.style.backgroundColor = color;
        }

        base.Localize(root);
    }
}

