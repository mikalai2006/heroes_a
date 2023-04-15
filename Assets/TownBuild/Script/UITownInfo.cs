using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
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
    private VisualElement _townInfoHeroVisit;
    private VisualElement _townInfo;
    private EntityTown _activeTown;
    private Player _activePlayer;
    private EntityHero _chooseHero;
    public static event Action onMoveHero;
    private SerializableDictionary<int, EntityCreature> _startCheckedCreatures
        = new SerializableDictionary<int, EntityCreature>();
    private EntityCreature _endCheckedCreature;
    private int _startPositionChecked = -1;
    private int _endChecked;

    private void Start()
    {
        UITavernWindow.onBuyHero += onBuyHero;
    }

    private void OnDestroy()
    {
        UITavernWindow.onBuyHero -= onBuyHero;
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

        _townInfoHero = _parent.Q<VisualElement>("TownHeroForce");
        DrawHeroInTown();
        // for (int i = 0; i < 9; i++)
        // {
        //     var itemHeroForce = _templateHeroForce.Instantiate();
        //     itemHeroForce.AddToClassList("w-33");
        //     itemHeroForce.AddToClassList("h-33");
        //     if (i < 4)
        //     {
        //         itemHeroForce.Q<VisualElement>("img").style.backgroundImage =
        //             new StyleBackground(_activePlayer.ActiveHero.ScriptableData.MenuSprite);
        //         itemHeroForce.Q<Label>("ForceValue").text = Random.Range(0, 100).ToString();
        //     }
        //     _townInfoHero.Add(itemHeroForce);
        // }
        _townInfoHeroVisit = _parent.Q<VisualElement>("TownHeroVisitForce");
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

        foreach (var build in _activeTown.Data.Armys)
        {
            var btnCreature = _templateTownInfoCreature.Instantiate();
            btnCreature.AddToClassList("w-25");
            btnCreature.AddToClassList("h-50");
            btnCreature.Q<VisualElement>("Img").style.backgroundImage
                = new StyleBackground(((ScriptableBuildingArmy)((BuildArmy)build.Value).ConfigData).Creatures[build.Value.level].MenuSprite);
            btnCreature.Q<Label>("Value").text = "+" + (build.Value.Data.quantity.ToString());
            listTownDwellingEl.Add(btnCreature);
        }


    }

    private void DrawHeroAsGuest()
    {
        _townInfoHeroVisit.Clear();

        // add Hero blok.
        var heroBlok = _templateHeroButton.Instantiate();
        _btnHeroGuest = heroBlok.Q<Button>("Btn");
        heroBlok.AddToClassList("w-125");
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

        for (int i = 0; i < 7; i++)
        {
            var itemHeroForce = _templateHeroForce.Instantiate();
            itemHeroForce.name = "creature";
            itemHeroForce.AddToClassList("w-125");
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
            _townInfoHeroVisit.Add(itemHeroForce);
        }
    }

    private void OnClickHeroGuest(ClickEvent evt)
    {
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

            _btnHeroInTown.RemoveFromClassList("button_active");
            _btnHeroGuest.RemoveFromClassList("button_active");
            _chooseHero = null;
            DrawHeroAsGuest();
            DrawHeroInTown();
        }
        else
        {
            _btnHeroInTown.SetEnabled(true);
            _btnHeroInTown.AddToClassList("button_active");
            _chooseHero = heroGuest;
        }
        onMoveHero?.Invoke();
    }

    private void OnClickHeroInTown(ClickEvent evt)
    {
        var heroInTown = _activeTown.Data.HeroinTown != null && _activeTown.Data.HeroinTown != ""
            ? (EntityHero)UnitManager.Entities[_activeTown.Data.HeroinTown]
            : null;

        if (_chooseHero != null && _chooseHero.IdEntity != _activeTown.Data.HeroinTown)
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
        heroBlok.AddToClassList("h-125");
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

        for (int i = 0; i < 7; i++)
        {
            var index = i;
            var itemHeroForce = _templateHeroForce.Instantiate();
            itemHeroForce.name = "creature";
            itemHeroForce.AddToClassList("w-full");
            itemHeroForce.AddToClassList("h-125");

            SerializableDictionary<int, EntityCreature> creatures;
            if (hero != null)
            {
                creatures = hero.Data.Creatures; //.TryGetValue(i, out creature);
            }
            else
            {
                creatures = _activeTown.Data.Creatures;
            }

            EntityCreature creature;
            creatures.TryGetValue(i, out creature);

            if (creature != null)
            {
                // Debug.Log($"Create creature::: [{creature.ScriptableData.idObject}]");
                itemHeroForce.Q<VisualElement>("img").style.backgroundImage
                    = new StyleBackground(creature.ScriptableData.MenuSprite);
                itemHeroForce.Q<Label>("ForceValue").text = creature.Data.value.ToString();
            }
            itemHeroForce.RegisterCallback<ClickEvent>((ClickEvent evt) =>
            {
                if (_startPositionChecked == -1)
                {
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
            _townInfoHero.Add(itemHeroForce);
        }
    }

    // private void OnEndMoveCreature()
    // {
    //     Debug.Log($"OnEndMoveCreature ::: start-{_startPositionChecked}:end-{_endChecked}");
    //     UQueryBuilder<VisualElement> builderGuest
    //             = new UQueryBuilder<VisualElement>(_parent);
    //     List<VisualElement> listGuestCreature = builderGuest.Name("creature").ToList();
    //     foreach (var item in listGuestCreature)
    //     {
    //         item.Q<Button>("HeroInfoForce").RemoveFromClassList("button_active");
    //     }
    //     _startCheckedCreature = null;
    //     _endCheckedCreature = null;
    //     _startChecked = -1;
    //     _endChecked = -1;

    // }

    private void OnChooseCreature(int index, SerializableDictionary<int, EntityCreature> creatures)
    {
        if (creatures[index] == null) return;

        UQueryBuilder<VisualElement> builder = new UQueryBuilder<VisualElement>(_townInfoHero);
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
                = new UQueryBuilder<VisualElement>(_townInfoHeroVisit);
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

    private void OnMoveCreature(int index, SerializableDictionary<int, EntityCreature> creatures)
    {
        Debug.Log($"OnMoveCreature ::: start-{_startPositionChecked}:end-{_endChecked}");
        UQueryBuilder<VisualElement> builderGuest
                = new UQueryBuilder<VisualElement>(_parent);
        List<VisualElement> listGuestCreature = builderGuest.Name("creature").ToList();
        foreach (var item in listGuestCreature)
        {
            item.Q<Button>("HeroInfoForce").RemoveFromClassList("button_active");
        }

        Helpers.MoveUnitBetweenList(
            _startCheckedCreatures,
            _startPositionChecked,
            creatures,
            index
            );
        // creatures[index] = _startCheckedCreatures[_startPositionChecked];
        DrawHeroAsGuest();
        DrawHeroInTown();
        // _startCheckedCreatures.Clear();
        _startPositionChecked = -1;
        // _endCheckedCreature = null;
        // _endChecked = -1;

    }

    // private void OnClickCreature(int index, EntityCreature creature)
    // {

    //     if (_startChecked == -1)
    //     {
    //         _startChecked = index;
    //         _startCheckedCreature = creature;
    //     }
    //     else
    //     {
    //         _endChecked = index;
    //         _endCheckedCreature = creature;
    //     }

    //     UQueryBuilder<VisualElement> builder = new UQueryBuilder<VisualElement>(_townInfoHero);
    //     List<VisualElement> list = builder.Name("creature").ToList();
    //     for (int i = 0; i < list.Count; i++)
    //     {
    //         var item = list[i];
    //         item.Q<Button>("HeroInfoForce").AddToClassList("button_active");
    //         item.SetEnabled(true);
    //         // item.RegisterCallback<ClickEvent>((ClickEvent evt) =>
    //         // {
    //         //     OnMoveCreature();
    //         //     _endChecked = i;
    //         // });
    //     }

    //     if (_activeTown.OccupiedNode.GuestedUnit != null)
    //     {
    //         UQueryBuilder<VisualElement> builderGuest
    //             = new UQueryBuilder<VisualElement>(_townInfoHeroVisit);
    //         List<VisualElement> listGuestCreature = builderGuest.Name("creature").ToList();
    //         for (int i = 0; i < listGuestCreature.Count; i++)
    //         {
    //             var item = listGuestCreature[i];
    //             item.Q<Button>("HeroInfoForce").AddToClassList("button_active");
    //             item.SetEnabled(true);
    //             // item.RegisterCallback<ClickEvent>((ClickEvent evt) =>
    //             // {
    //             //     OnMoveCreature();
    //             //     _endChecked = i;
    //             // });
    //         }
    //     }

    //     if (_endChecked != -1)
    //     {
    //         OnEndMoveCreature();
    //     }
    // }
}
