using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Localization;
using System.Collections;
using System;

public class UITavernWindow : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDoc;
    public UIDocument DialogApp => _uiDoc;
    [SerializeField] private VisualTreeAsset _templateTavernBlok;
    [SerializeField] private VisualTreeAsset _templateHeroButton;
    private readonly string _nameButtonClose = "ButtonClose";
    private readonly string _nameButtonPrice = "ButtonPrice";
    private readonly string _nameHeaderLabel = "HeaderDialog";
    private readonly string _nameGeneralBlok = "GeneralBlok";
    private readonly string _nameOverlay = "Overlay";
    private readonly string _nameHeroList = "HeroList";
    private readonly string _nameTextHeroInfo = "TextHeroInfo";

    private Button _buttonClose;
    private Button _buttonPrice;
    private VisualElement _heroList;
    private Label _headerLabel;
    private Label _heroInfoLabel;
    private VisualElement _generalBlok;

    private TaskCompletionSource<DataResultBuildDialog> _processCompletionSource;

    public UnityEvent processAction;

    private DataResultBuildDialog _dataResultDialog;

    private Player _activePlayer;
    private ScriptableBuildTown _scriptObjectBuildTown;
    private ScriptableEntityHero _activeHeroData;
    public static event Action onBuyHero;

    private void Start()
    {
        _headerLabel = DialogApp.rootVisualElement.Q<Label>(_nameHeaderLabel);
        _generalBlok = DialogApp.rootVisualElement.Q<VisualElement>(_nameGeneralBlok);

        var panel = DialogApp.rootVisualElement.Q<VisualElement>("Panel");
        panel.AddToClassList("w-50");
        // panel.AddToClassList("h-full");

        var panelBlok = DialogApp.rootVisualElement.Q<VisualElement>("PanelBlok");
        panelBlok.style.flexGrow = 1;

        VisualElement docDialogBlok = _templateTavernBlok.Instantiate();
        docDialogBlok.style.flexGrow = 1;
        _generalBlok.Clear();
        _generalBlok.Add(docDialogBlok);

        _buttonClose = DialogApp.rootVisualElement.Q<Button>(_nameButtonClose);
        _buttonClose.clickable.clicked += OnClickClose;

        _buttonPrice = DialogApp.rootVisualElement.Q<VisualElement>(_nameButtonPrice).Q<Button>("Btn");
        _buttonPrice.clickable.clicked += OnClickBuy;

        _heroList = DialogApp.rootVisualElement.Q<VisualElement>(_nameHeroList);
        _heroInfoLabel = DialogApp.rootVisualElement.Q<Label>(_nameTextHeroInfo);
    }

    public async Task<DataResultBuildDialog> ProcessAction()
    {
        _dataResultDialog = new DataResultBuildDialog();

        _activePlayer = LevelManager.Instance.ActivePlayer;

        // _headerLabel.text = ;

        UQueryBuilder<VisualElement> builder = new UQueryBuilder<VisualElement>(DialogApp.rootVisualElement);
        List<VisualElement> list = builder.Name(_nameOverlay).ToList();
        Color color = _activePlayer.DataPlayer.color;
        color.a = .6f;
        foreach (var overlay in list)
        {
            overlay.style.backgroundColor = color;
        }

        _heroList.Clear();
        EntityTown town = (EntityTown)_activePlayer.ActiveTown;
        var listConfigHero = ResourceSystem.Instance.GetEntityByType<ScriptableEntityHero>(TypeEntity.Hero);
        if (_activePlayer.DataPlayer.HeroesInTavern.Count > 0)
        {
            foreach (var heroId in _activePlayer.DataPlayer.HeroesInTavern)
            {
                var newBoxBtnHero = _templateHeroButton.Instantiate();
                newBoxBtnHero.AddToClassList("mr-2");
                var newBtnHero = newBoxBtnHero.Q<Button>("Btn");
                var currentHeroData = listConfigHero.Where(t => t.idObject == heroId).First();
                newBtnHero.Q<VisualElement>("img").style.backgroundImage
                    = new StyleBackground(currentHeroData.MenuSprite);
                newBtnHero.name = currentHeroData.idObject;
                newBtnHero.RegisterCallback<ClickEvent>((ClickEvent evt) =>
                {
                    ClickHero(currentHeroData);
                });
                _heroList.Add(newBoxBtnHero);
            }
        }

        _processCompletionSource = new TaskCompletionSource<DataResultBuildDialog>();

        return await _processCompletionSource.Task;
    }

    private void ClickHero(ScriptableEntityHero heroData)
    {
        UQueryBuilder<Button> builder = new UQueryBuilder<Button>(_heroList);
        List<Button> list = builder.OfType<Button>().ToList();
        foreach (var box in list)
        {
            box.RemoveFromClassList("button_bordered");
            box.RemoveFromClassList("button_active");
            if (box.name == heroData.idObject)
            {
                box.AddToClassList("button_active");
            }
            else
            {
                box.AddToClassList("button_bordered");
            }
        }

        _activeHeroData = heroData;
        // show info hero.
        _heroInfoLabel.text = heroData.title.GetLocalizedString();
    }

    private void OnClickBuy()
    {
        _dataResultDialog.isOk = false;
        var newHero = UnitManager.CreateHero(
            TypeFaction.Neutral,
            _activePlayer.ActiveTown.OccupiedNode,
            _activeHeroData);
        newHero.SetPlayer(_activePlayer);
        // _activePlayer.ActiveTown.Data.HeroinTown = newHero.IdEntity;
        _processCompletionSource.SetResult(_dataResultDialog);
        onBuyHero?.Invoke();
        processAction?.Invoke();
    }

    private void OnClickClose()
    {
        _dataResultDialog.isOk = false;
        _processCompletionSource.SetResult(_dataResultDialog);

        processAction?.Invoke();
    }
}

