using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.Localization;

public class UITavernWindow : UIDialogBaseWindow
{
    [SerializeField] private VisualTreeAsset _templateHeroButton;
    private readonly string _nameButtonClose = "ButtonClose";
    private readonly string _nameButtonPrice = "ButtonPrice";
    private readonly string _nameHeroList = "HeroList";
    private readonly string _nameTextHeroInfo = "TextHeroInfo";
    private Button _buttonClose;
    private Button _buttonPrice;
    private VisualElement _heroList;
    private Label _heroInfoLabel;
    private ScriptableEntityHero _activeHeroData;
    public static event Action onBuyHero;

    public override void Start()
    {
        base.Start();

        _buttonClose = root.Q<Button>(_nameButtonClose);
        _buttonClose.clickable.clicked += OnClickClose;

        _buttonPrice = root.Q<VisualElement>(_nameButtonPrice).Q<Button>("Btn");
        _buttonPrice.clickable.clicked += OnClickBuy;

        _heroList = root.Q<VisualElement>(_nameHeroList);
        _heroInfoLabel = root.Q<Label>(_nameTextHeroInfo);
    }

    public async Task<DataResultBuildDialog> ProcessAction(BaseBuild build)
    {
        base.Init();

        Title.text = build.ConfigData.BuildLevels[build.level].Text.title.GetLocalizedString();
        root.Q<Label>("description").text
            = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "tavern_cit").GetLocalizedString();

        _heroList.Clear();

        EntityTown town = (EntityTown)_activePlayer.ActiveTown;
        var listConfigHero = ResourceSystem.Instance.GetEntityByType<ScriptableEntityHero>(TypeEntity.Hero);
        Debug.Log($"Count hero=[{_activePlayer.DataPlayer.HeroesInTavern.Count}]");
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

                // choose first hero.
                if (_activeHeroData == null)
                {
                    ClickHero(currentHeroData);
                }
            }
        }

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

    private async void OnClickBuy()
    {
        if (_activePlayer.ActiveTown.OccupiedNode.GuestedUnit != null)
        {
            var dialogData = new DataDialogHelp()
            {
                Header = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "Help").GetLocalizedString(),
                Description = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "nocardhero").GetLocalizedString(),
            };
            var dialogWindow = new DialogHelpProvider(dialogData);
            await dialogWindow.ShowAndHide();
        }
        else if (!_activePlayer.IsExistsResource(LevelManager.Instance.ConfigGameSettings.CostHero))
        {
            var dialogData = new DataDialogHelp()
            {
                Header = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "Help").GetLocalizedString(),
                Description = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "nocardhero_cash").GetLocalizedString(),
            };
            var dialogWindow = new DialogHelpProvider(dialogData);
            await dialogWindow.ShowAndHide();
        }
        else if (_activePlayer.IsMaxCountHero)
        {
            var dialogData = new DataDialogHelp()
            {
                Header = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "Help").GetLocalizedString(),
                Description = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "maxhero_message").GetLocalizedString(),
            };
            var dialogWindow = new DialogHelpProvider(dialogData);
            await dialogWindow.ShowAndHide();
        }
        else
        {
            _activePlayer.BuyHero(_activeHeroData);
            onBuyHero?.Invoke();
        }
        _dataResultDialog.isOk = true;
        _processCompletionSource.SetResult(_dataResultDialog);
    }

    private void OnClickClose()
    {
        _dataResultDialog.isOk = false;
        _processCompletionSource.SetResult(_dataResultDialog);

    }
}

