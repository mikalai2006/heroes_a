using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;

public class UITownInfo : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset _templateHeroForce;
    [SerializeField] private VisualTreeAsset _templateTownInfoCreature;
    [SerializeField] private VisualTreeAsset _templateHeroButton;
    [SerializeField] private VisualTreeAsset _templateTownInfo;
    private Button _btnHeroInTown;
    private Button _btnHeroGuest;
    private VisualElement _parent;
    private const string _nameTownInfo = "TownInfo";
    private VisualElement _townInfoBox;
    private VisualElement _townInfoHero;
    private VisualElement _townInfoHeroForce;
    private VisualElement _townInfoHeroVisit;
    private VisualElement _townInfoHeroVisitForce;
    private VisualElement _townInfo;
    private EntityTown _activeTown;
    private Player _activePlayer;
    private EntityHero _chooseHero;
    private Button _buttonSplitCreature;
    public static event Action onMoveHero;
    private SerializableDictionary<int, EntityCreature> _startCheckedCreatures
        = new SerializableDictionary<int, EntityCreature>();
    // private EntityCreature _endCheckedCreature;
    private int _startPositionChecked = -1;
    // private int _endChecked;
    private bool _isSplit = false;
    private EntityHero _startHeroMoveCreature;

    private void Start()
    {
        UITavernWindow.onBuyHero += onBuyHero;
        UITownListBuildWindow.OnCloseListBuilds += DrawTownInfo;
    }

    private void OnDestroy()
    {
        UITavernWindow.onBuyHero -= onBuyHero;
        UITownListBuildWindow.OnCloseListBuilds -= DrawTownInfo;
    }

    private void onBuyHero()
    {
        DrawHeroAsGuest();
        onMoveHero?.Invoke();
    }

    public void Init(VisualElement parent)
    {
        _parent = parent;

        Player activePlayer = LevelManager.Instance.ActivePlayer;
        _activePlayer = activePlayer;
        _activeTown = _activePlayer.ActiveTown;

        _townInfoBox = _parent.Q<VisualElement>(_nameTownInfo);
        _townInfo = _templateTownInfo.Instantiate();
        _townInfo.style.flexGrow = 1;
        _townInfoBox.Clear();
        _townInfoBox.Add(_townInfo);

        _townInfoHero = _parent.Q<VisualElement>("TownHero");
        _townInfoHeroForce = _parent.Q<VisualElement>("TownHeroForce");

        _buttonSplitCreature = _parent.Q<TemplateContainer>("SplitCreature").Q<Button>("Btn");
        _buttonSplitCreature.clickable.clicked += () =>
        {
            _isSplit = true;
        };
        _buttonSplitCreature.SetEnabled(false);

        DrawHeroInTown();

        _townInfoHeroVisit = _parent.Q<VisualElement>("TownHeroVisit");
        _townInfoHeroVisitForce = _parent.Q<VisualElement>("TownHeroVisitForce");

        DrawHeroAsGuest();

        DrawTownInfo();
    }

    private void DrawTownInfo()
    {
        // _townInfo.Clear();
        var spriteTownEl = _townInfo.Q<VisualElement>("TownIcon");
        var nameTownEl = _townInfo.Q<VisualElement>("TownName");
        var listTownDwellingEl = _townInfo.Q<VisualElement>("TownDwellingList");
        listTownDwellingEl.Clear();

        foreach (var build in _activeTown.Data.Armys
            .OrderBy(t => ((ScriptableBuildingArmy)((BuildArmy)t.Value).ConfigData).Creatures[t.Value.level].CreatureParams.Level))
        {
            var btnCreature = _templateTownInfoCreature.Instantiate();
            btnCreature.AddToClassList("w-25");
            btnCreature.AddToClassList("h-50");
            btnCreature.Q<VisualElement>("Img").style.backgroundImage
                = new StyleBackground(((ScriptableBuildingArmy)((BuildArmy)build.Value).ConfigData).Creatures[build.Value.level].MenuSprite);
            btnCreature.Q<Label>("Value").text = "+" + (build.Value.Data.quantity.ToString());
            listTownDwellingEl.Add(btnCreature);
        }
        for (int i = _activeTown.Data.Armys.Count; i < 7; i++)
        {
            var btnCreature = _templateTownInfoCreature.Instantiate();
            btnCreature.AddToClassList("w-25");
            btnCreature.AddToClassList("h-50");
            btnCreature.Q<Label>("Value").text = "";
            listTownDwellingEl.Add(btnCreature);
        }
    }

    private void DrawHeroAsGuest()
    {
        _townInfoHeroVisit.Clear();

        // add Hero blok.
        var heroBlok = _templateHeroButton.Instantiate();
        _btnHeroGuest = heroBlok.Q<Button>("Btn");
        heroBlok.AddToClassList("w-full");
        heroBlok.AddToClassList("h-full");
        if (_activeTown.OccupiedNode.GuestedUnit != null)
        {
            heroBlok.Q<VisualElement>("img").style.backgroundImage =
                new StyleBackground(_activeTown.OccupiedNode.GuestedUnit.ScriptableData.MenuSprite);
        }
        else
        {
            _btnHeroGuest.SetEnabled(false);
        }
        _townInfoHeroVisit.Add(heroBlok);
        _btnHeroGuest.RegisterCallback<ClickEvent>(OnClickHeroGuest);

        _townInfoHeroVisitForce.Clear();
        for (int i = 0; i < 7; i++)
        {
            var itemHeroForce = _templateHeroForce.Instantiate();
            itemHeroForce.name = "creature";
            itemHeroForce.AddToClassList("w-14");
            itemHeroForce.AddToClassList("h-full");

            if (_activeTown.OccupiedNode.GuestedUnit != null)
            {
                var index = i;
                EntityCreature creature;
                EntityHero entityHero = (EntityHero)_activeTown.OccupiedNode.GuestedUnit;
                entityHero.Data.Creatures.TryGetValue(i, out creature);
                if (creature != null)
                {
                    itemHeroForce.Q<VisualElement>("img").style.backgroundImage =
                        new StyleBackground(creature.ScriptableData.MenuSprite);
                    itemHeroForce.Q<Label>("ForceValue").text = creature.Data.value.ToString();
                }
                itemHeroForce.RegisterCallback<ClickEvent>((ClickEvent evt) =>
                {
                    if (_startPositionChecked == -1)
                    {
                        _startHeroMoveCreature = entityHero;
                        OnChooseCreature(index, entityHero.Data.Creatures);
                    }
                    else
                    {
                        OnMoveCreature(index, entityHero.Data.Creatures);
                    };
                });
            }
            else
            {
                itemHeroForce.SetEnabled(false);
            }
            _townInfoHeroVisitForce.Add(itemHeroForce);
        }
    }

    private async void OnClickHeroGuest(ClickEvent evt)
    {

        if (_chooseHero != null && _activePlayer.IsMaxCountHero)
        {
            var dialogData = new DataDialogHelp()
            {
                Header = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "Help").GetLocalizedString(),
                Description = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "maxhero_message").GetLocalizedString(),
            };
            var dialogWindow = new DialogHelpProvider(dialogData);
            await dialogWindow.ShowAndHide();
            SetDeactiveButtonsHero();
            _chooseHero = null;
            return;
        }

        var heroGuest = (EntityHero)_activeTown.OccupiedNode.GuestedUnit;
        if (_chooseHero != null && _chooseHero != _activeTown.OccupiedNode.GuestedUnit)
        {
            // move old guest and destroy GameObject.
            _activeTown.Data.HeroinTown = heroGuest?.IdEntity;
            if (heroGuest != null)
            {
                heroGuest.MapObjectGameObject.gameObject.SetActive(false);
                heroGuest.Data.State = StateHero.InTown;
            }

            // create new guest and create GameObject.
            _activeTown.OccupiedNode.SetAsGuested(_chooseHero);
            if (_activeTown.OccupiedNode.GuestedUnit != null)
            {
                _chooseHero.Data.State = StateHero.OnMap;
                if (_activeTown.OccupiedNode.GuestedUnit.MapObjectGameObject != null)
                {
                    _activeTown.OccupiedNode.GuestedUnit.MapObjectGameObject.gameObject.SetActive(true);
                }
                else
                {
                    _activeTown.OccupiedNode.GuestedUnit.CreateMapGameObject(_activeTown.OccupiedNode);
                }
            }

            _activePlayer.SetActiveHero((EntityHero)_activeTown.OccupiedNode.GuestedUnit);

            SetDeactiveButtonsHero();
            _chooseHero = null;
            DrawHeroAsGuest();
            DrawHeroInTown();
        }
        else
        {
            _btnHeroInTown.SetEnabled(true);
            SetActiveButtonsHero();
            _chooseHero = heroGuest;
        }
        onMoveHero?.Invoke();
    }

    private void SetDeactiveButtonsHero()
    {
        _btnHeroInTown.RemoveFromClassList("button_active");
        _btnHeroGuest.RemoveFromClassList("button_active");
    }

    private void SetActiveButtonsHero()
    {
        _btnHeroInTown.AddToClassList("button_active");
        _btnHeroGuest.AddToClassList("button_active");

    }

    private async void OnClickHeroInTown(ClickEvent evt)
    {
        var heroInTown = _activeTown.Data.HeroinTown != null && _activeTown.Data.HeroinTown != ""
            ? (EntityHero)UnitManager.Entities[_activeTown.Data.HeroinTown]
            : null;

        if (_chooseHero != null)
        {
            if (_chooseHero.IdEntity != _activeTown.Data.HeroinTown)
            {
                // merge creatures.
                if (_activeTown.Data.Creatures.Where(t => t.Value != null).Count() > 0)
                {
                    var resultMergeCreatures = Helpers.SummUnitBetweenList(
                        _chooseHero.Data.Creatures,
                        _activeTown.Data.Creatures
                        );
                    if (resultMergeCreatures.Count > 7)
                    {
                        // Show dialog No move.

                    }
                    else
                    {
                        _chooseHero.Data.Creatures = resultMergeCreatures;
                        _activeTown.OccupiedNode.SetAsGuested(heroInTown);
                        // create new hero in town and destroy GameObject.
                        _activeTown.Data.HeroinTown = _chooseHero.IdEntity;
                        _chooseHero.MapObjectGameObject.gameObject.SetActive(false);
                        _chooseHero.Data.State = StateHero.InTown;
                        _activeTown.ResetCreatures();
                    }
                }
                else
                {
                    _activeTown.OccupiedNode.SetAsGuested(heroInTown);
                    if (_activeTown.OccupiedNode.GuestedUnit != null)
                    {
                        ((EntityHero)_activeTown.OccupiedNode.GuestedUnit).Data.State = StateHero.OnMap;
                        if (_activeTown.OccupiedNode.GuestedUnit.MapObjectGameObject != null)
                        {
                            _chooseHero.MapObjectGameObject.gameObject.SetActive(true);
                        }
                        else
                        {
                            _activeTown.OccupiedNode.GuestedUnit.CreateMapGameObject(_activeTown.OccupiedNode);
                        }
                    }
                    _activePlayer.SetActiveHero((EntityHero)_activeTown.OccupiedNode.GuestedUnit);

                    // create new hero in town and destroy GameObject.
                    _activeTown.Data.HeroinTown = _chooseHero.IdEntity;
                    _chooseHero.MapObjectGameObject.gameObject.SetActive(false);
                    _chooseHero.Data.State = StateHero.InTown;
                }

            }
            else
            {
                var dialogHeroInfo = new DialogHeroInfoOperation(_chooseHero);
                var result = await dialogHeroInfo.ShowAndHide();
                if (result.isOk)
                {

                }
            }

            _btnHeroInTown.RemoveFromClassList("button_active");
            _btnHeroGuest.RemoveFromClassList("button_active");
            _chooseHero = null;
            DrawHeroAsGuest();
            DrawHeroInTown();
        }
        else
        {
            // var hero = (EntityHero)_activeTown.Data.HeroinTown;
            _btnHeroGuest.SetEnabled(true);
            _btnHeroGuest.AddToClassList("button_active");
            _chooseHero = heroInTown;
        }
        onMoveHero?.Invoke();
    }

    private void DrawHeroInTown()
    {
        _townInfoHero.Clear();

        // add Hero blok.
        EntityHero hero = _activeTown.Data.HeroinTown != null && _activeTown.Data.HeroinTown != ""
            ? (EntityHero)UnitManager.Entities[_activeTown.Data.HeroinTown]
            : null;
        var heroBlok = _templateHeroButton.Instantiate();
        _btnHeroInTown = heroBlok.Q<Button>("Btn");
        heroBlok.AddToClassList("w-full");
        heroBlok.AddToClassList("h-full");
        if (hero != null)
        {
            heroBlok.Q<VisualElement>("img").style.backgroundImage
                = new StyleBackground(hero.ScriptableData.MenuSprite);
        }
        else
        {
            _btnHeroInTown.SetEnabled(false);
        }
        _townInfoHero.Add(heroBlok);
        _btnHeroInTown.RegisterCallback<ClickEvent>(OnClickHeroInTown);

        _townInfoHeroForce.Clear();
        for (int i = 0; i < 7; i++)
        {
            var index = i;
            var itemHeroForce = _templateHeroForce.Instantiate();
            itemHeroForce.name = "creature";
            itemHeroForce.AddToClassList("w-full");
            itemHeroForce.AddToClassList("h-14");

            SerializableDictionary<int, EntityCreature> creatures;
            if (hero != null)
            {
                creatures = hero.Data.Creatures;
            }
            else
            {
                creatures = _activeTown.Data.Creatures;
            }

            EntityCreature creature;
            creatures.TryGetValue(i, out creature);

            if (creature != null)
            {
                itemHeroForce.Q<VisualElement>("img").style.backgroundImage
                    = new StyleBackground(creature.ScriptableData.MenuSprite);
                itemHeroForce.Q<Label>("ForceValue").text = creature.Data.value.ToString();
            }
            itemHeroForce.RegisterCallback<ClickEvent>((ClickEvent evt) =>
            {
                if (_startPositionChecked == -1)
                {
                    _startHeroMoveCreature = hero;
                    OnChooseCreature(index, creatures);
                }
                else
                {
                    OnMoveCreature(index, creatures);
                };
                // OnClickCreature(index, creature);
            });
            // }
            // else
            // {
            //     itemHeroForce.SetEnabled(false);
            // }
            _townInfoHeroForce.Add(itemHeroForce);
        }
    }

    private void OnChooseCreature(int index, SerializableDictionary<int, EntityCreature> creatures)
    {
        if (creatures[index] == null) return;
        _buttonSplitCreature.SetEnabled(true);

        UQueryBuilder<VisualElement> builder = new UQueryBuilder<VisualElement>(_townInfoHeroForce);
        List<VisualElement> list = builder.Name("creature").ToList();
        for (int i = 0; i < list.Count; i++)
        {
            var item = list[i];
            item.Q<Button>("HeroInfoForce").AddToClassList("button_active");
            item.SetEnabled(true);
        }

        if (_activeTown.OccupiedNode.GuestedUnit != null)
        {
            UQueryBuilder<VisualElement> builderGuest
                = new UQueryBuilder<VisualElement>(_townInfoHeroVisitForce);
            List<VisualElement> listGuestCreature = builderGuest.Name("creature").ToList();
            for (int i = 0; i < listGuestCreature.Count; i++)
            {
                var item = listGuestCreature[i];
                item.Q<Button>("HeroInfoForce").AddToClassList("button_active");
                item.SetEnabled(true);
            }
        }
        _startCheckedCreatures = creatures;
        _startPositionChecked = index;
    }

    private async void OnMoveCreature(int index, SerializableDictionary<int, EntityCreature> creatures)
    {
        if (creatures[index] == _startCheckedCreatures[_startPositionChecked])
        {
            // if (_isSplit)
            // {
            //     // Show dialog split creatures.
            // }
            // else
            // {
            // Show dialog info creature.
            var dialogWindow = new UIInfoCreatureOperation();
            var result = await dialogWindow.ShowAndHide();
            if (result.isOk)
            {

            }
            // }
        }
        else
        {
            if (
                _startCheckedCreatures.Where(t => t.Value != null).Count() == 1
                && _startCheckedCreatures != creatures
                && creatures[index] == null
                && _startHeroMoveCreature != null
                && !_isSplit)
            {
                var dialogData = new DataDialogHelp()
                {
                    Header = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "Help").GetLocalizedString(),
                    Description = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "hero_lastcreature").GetLocalizedString(),
                };
                var dialogWindow = new DialogHelpProvider(dialogData);
                await dialogWindow.ShowAndHide();
            }
            else if (
                _isSplit
                && (
                    creatures[index] == null
                    || creatures[index].IdObject == _startCheckedCreatures[_startPositionChecked].IdObject
                    )
                )
            {
                // Show dialog split creatures.
                var dialogWindow = new DialogSplitCreatureOperation(
                    _startCheckedCreatures[_startPositionChecked],
                    creatures[index]
                    );
                var result = await dialogWindow.ShowAndHide();
                if (result.isOk)
                {
                    Helpers.MoveUnitBetweenList(
                        ref _startCheckedCreatures,
                        _startPositionChecked,
                        ref creatures,
                        index,
                        result.value1,
                        result.value2
                        );
                }
            }
            else
            {
                Helpers.MoveUnitBetweenList(
                    ref _startCheckedCreatures,
                    _startPositionChecked,
                    ref creatures,
                    index
                    );
            }
        }

        UQueryBuilder<VisualElement> builderGuest
                = new UQueryBuilder<VisualElement>(_parent);
        List<VisualElement> listGuestCreature = builderGuest.Name("creature").ToList();
        foreach (var item in listGuestCreature)
        {
            item.Q<Button>("HeroInfoForce").RemoveFromClassList("button_active");
        }
        DrawHeroAsGuest();
        DrawHeroInTown();
        _isSplit = false;
        _buttonSplitCreature.SetEnabled(false);
        _startPositionChecked = -1;
    }
}
