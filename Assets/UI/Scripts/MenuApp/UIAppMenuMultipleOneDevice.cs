using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;

public class UIAppMenuMultipleOneDevice : UILocaleBase
{
    [SerializeField] private VisualTreeAsset _templateOneDeviceBlok;
    [SerializeField] private UIDocument _rootDoc;
    public UIDocument RootDoc => _rootDoc;
    private VisualElement _root;
    [SerializeField] private UIMenuApp _parent;
    public UIMenuApp Parent => _parent;
    private Label _headerLabel;
    private VisualElement _generalBlok;

    public void Init()
    {
        _root = _rootDoc.rootVisualElement;

        // Create dialog multiple
        _headerLabel = RootDoc.rootVisualElement.Q<Label>("HeaderDialog");
        _generalBlok = RootDoc.rootVisualElement.Q<VisualElement>("GeneralBlok");
        var panel = RootDoc.rootVisualElement.Q<VisualElement>("Panel");
        panel.AddToClassList("w-50");
        var panelBlok = RootDoc.rootVisualElement.Q<VisualElement>("PanelBlok");
        panelBlok.style.flexGrow = 1;
        VisualElement docDialogBlok = _templateOneDeviceBlok.Instantiate();
        docDialogBlok.style.flexGrow = 1;
        _generalBlok.Clear();
        _generalBlok.Add(docDialogBlok);
        RootDoc.rootVisualElement.style.display = DisplayStyle.None;
        _headerLabel.text = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "Hotseat").GetLocalizedString();
        _root.Q<Button>("back").clickable.clicked += () =>
            {
                Hide();
            };
        _root.Q<Button>("ok").clickable.clicked += () =>
            {
                CreateListTypesPlayer();
                // _parent.AppMenuNewGame.DrawAdvancedOptions();
                _parent.AppMenuNewGame.Show();
            };
        // var btnMultipleGame = _root.Q<Button>("multiplegame");
        // btnMultipleGame.clickable.clicked += () =>
        // {
        //     LevelManager.Instance.Level.Settings.TypeGame = TypeGame.MultipleOneDevice;
        //     _parent.AppMenuNewGame.Show();
        //     Hide();
        // };
        // btnMultipleGame.SetEnabled(false); //TODO

        Hide();
        DrawListNames();
        base.Localize(_root);
    }

    private void DrawListNames()
    {
        var playerFromSettings = LevelManager.Instance.ConfigGameSettings.TypesPlayer
            .Where(t => t.TypePlayer == PlayerType.User).First();
        var listNames = _root.Q<VisualElement>("ListNames");
        for (int i = 0; i < LevelManager.Instance.ConfigGameSettings.maxPlayer; i++)
        {
            var input = new TextField();
            input.name = "text";
            input.label = "";
            input.value = (i == 0) ? playerFromSettings.title.GetLocalizedString() : "";
            listNames.Add(input);
        }
    }

    private void CreateListTypesPlayer()
    {
        LevelManager.Instance.TypePlayers.Clear();

        // Queries text fields.
        UQueryBuilder<TextField> builder = new UQueryBuilder<TextField>(_root);
        List<TextField> list = builder.Name("text").ToList();
        var listNotEmptyFieldText = list.Where(t => t.value != "");
        foreach (var item in listNotEmptyFieldText)
        {
            LevelManager.Instance.TypePlayers.Add(new CurrentPlayerType()
            {
                title = item.value,
                TypePlayer = PlayerType.User
            });
        }

        foreach (var type in LevelManager.Instance.ConfigGameSettings.TypesPlayer)
        {
            if (type.TypePlayer == PlayerType.Bot)
            {
                LevelManager.Instance.TypePlayers.Add(new CurrentPlayerType()
                {
                    title = type.title.GetLocalizedString(),
                    TypePlayer = PlayerType.Bot
                });
            }
        };
        LevelManager.Instance.Level.Settings.countPlayer = listNotEmptyFieldText.Count();
        LevelManager.Instance.Level.Settings.countBot = 0;

        Hide();
    }

    public void Hide()
    {
        _root.style.display = DisplayStyle.None;
    }

    public void Show()
    {
        _root.style.display = DisplayStyle.Flex;
    }
}

