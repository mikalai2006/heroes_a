using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class UITownInfo : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset _templateHeroForce;
    [SerializeField] private VisualTreeAsset _templateTownInfo;
    private VisualElement _parent;
    private const string _nameTownInfo = "TownInfo";
    private VisualElement _townInfoBox;
    private VisualElement _townInfoHero;
    private VisualElement _townInfoHeroVisit;
    private VisualElement _townInfo;
    private BaseTown _activeTown;
    private Player _activePlayer;

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
        _townInfoHero.Clear();
        for (int i = 0; i < 9; i++)
        {
            var itemHeroForce = _templateHeroForce.Instantiate();
            itemHeroForce.AddToClassList("w-33");
            itemHeroForce.AddToClassList("h-33");
            if (i < 4)
            {
                itemHeroForce.Q<VisualElement>("img").style.backgroundImage = new StyleBackground(_activePlayer.ActiveHero.ScriptableData.MenuSprite);
                itemHeroForce.Q<Label>("ForceValue").text = Random.Range(0, 100).ToString();
            }
            _townInfoHero.Add(itemHeroForce);
        }

        _townInfoHeroVisit = _parent.Q<VisualElement>("TownHeroVisitForce");
        _townInfoHeroVisit.Clear();
        for (int i = 0; i < 8; i++)
        {
            var itemHeroForce = _templateHeroForce.Instantiate();
            itemHeroForce.AddToClassList("w-125");
            itemHeroForce.AddToClassList("h-full");
            if (i < 4)
            {
                itemHeroForce.Q<VisualElement>("img").style.backgroundImage = new StyleBackground(_activePlayer.ActiveHero.ScriptableData.MenuSprite);
                itemHeroForce.Q<Label>("ForceValue").text = Random.Range(0, 100).ToString();
            }
            _townInfoHeroVisit.Add(itemHeroForce);
        }
    }
}
