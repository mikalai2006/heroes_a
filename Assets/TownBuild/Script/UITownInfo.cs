using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class UITownInfo : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset _templateHeroForce;
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
            itemHeroForce.AddToClassList("w-125");
            itemHeroForce.AddToClassList("h-full");

            if (_activeTown.OccupiedNode.GuestedUnit != null)
            {

                EntityCreature creature;
                EntityHero entityHero = (EntityHero)_activeTown.OccupiedNode.GuestedUnit;
                entityHero.Data.Creatures.TryGetValue(i, out creature);
                if (creature != null)
                {
                    itemHeroForce.Q<VisualElement>("img").style.backgroundImage =
                        new StyleBackground(creature.ScriptableData.MenuSprite);
                    itemHeroForce.Q<Label>("ForceValue").text = creature.Data.value.ToString();
                }
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
            _activeTown.Data.HeroinTown = heroGuest;
            if (_activeTown.Data.HeroinTown != null)
            {
                _activeTown.Data.HeroinTown.MapObjectGameObject.gameObject.SetActive(false);
                _activeTown.Data.HeroinTown.Data.State = StateHero.InTown;
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
        var heroInTown = (EntityHero)_activeTown.Data.HeroinTown;
        if (_chooseHero != null && _chooseHero != _activeTown.Data.HeroinTown)
        {
            // move old hero in town and destroy GameObject.
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
            _activeTown.Data.HeroinTown = _chooseHero;
            _chooseHero.MapObjectGameObject.gameObject.SetActive(false);
            _chooseHero.Data.State = StateHero.InTown;

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
        var heroBlok = _templateHeroButton.Instantiate();
        _btnHeroInTown = heroBlok.Q<Button>("Btn");
        heroBlok.AddToClassList("w-33");
        heroBlok.AddToClassList("h-33");
        if (_activeTown.Data.HeroinTown != null && _activeTown.Data.HeroinTown.ScriptableData != null)
        {
            heroBlok.Q<VisualElement>("img").style.backgroundImage =
                new StyleBackground(_activeTown.Data.HeroinTown.ScriptableData.MenuSprite);
        }
        else
        {
            _btnHeroInTown.SetEnabled(false);
        }
        _townInfoHero.Add(heroBlok);
        _btnHeroInTown.RegisterCallback<ClickEvent>(OnClickHeroInTown);

        for (int i = 0; i < 7; i++)
        {
            var itemHeroForce = _templateHeroForce.Instantiate();
            itemHeroForce.AddToClassList("w-33");
            itemHeroForce.AddToClassList("h-33");

            if (_activeTown.Data.HeroinTown != null)
            {

                EntityCreature creature;
                EntityHero entityHero = (EntityHero)_activeTown.Data.HeroinTown;
                entityHero.Data.Creatures.TryGetValue(i, out creature);
                if (creature != null)
                {
                    itemHeroForce.Q<VisualElement>("img").style.backgroundImage =
                        new StyleBackground(creature.ScriptableData.MenuSprite);
                    itemHeroForce.Q<Label>("ForceValue").text = creature.Data.value.ToString();
                }
            }
            else
            {
                itemHeroForce.SetEnabled(false);
            }
            _townInfoHero.Add(itemHeroForce);
        }
    }
}
