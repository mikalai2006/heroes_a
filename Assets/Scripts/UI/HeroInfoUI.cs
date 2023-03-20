using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HeroInfoUI : MonoBehaviour
{

    [SerializeField] private UIDocument _uiRoot;
    [SerializeField] private VisualTreeAsset _templateHeroForce;

    private VisualElement _hero;

    private void OnDestroy() => GameManager.OnAfterStateChanged -= OnChangeGameState;

    private void Start()
    {
        GameManager.OnAfterStateChanged += OnChangeGameState;

        //_hero = _uiRoot.rootVisualElement.Q<VisualElement>("HeroInfoBox");

        //var btnClose = _hero.Q<Button>("ButtonClose");
        //if (btnClose != null)
        //{
        //    btnClose.clickable.clicked += () =>
        //    {
        //        UIManager.Instance.HideTown();
        //    };
        //}
        //var btnSave = _menu.Q<Button>("ButtonSave");
        //if (btnSave != null)
        //{
        //    btnSave.clickable.clicked += () =>
        //    {
        //        GameManager.Instance.ChangeState(GameState.SaveGame);
        //    };
        //}
    }

    public VisualElement Init(Player player)
    {
        Debug.Log($"Init UI Heroinfo");
        var _root = _uiRoot.rootVisualElement.Q<VisualElement>("HeroForceList");
        for (int i = 0; i < 7; i++)
        {
            var newForce = _templateHeroForce.Instantiate();

            newForce.Q<Label>("ForceValue").text = i.ToString();
            _root.Add(newForce);

        }

        return _root;

    } 


    private void OnChangeGameState(GameState state)
    {
        //if (state == GameState.StepNextPlayer)
        //{

        //}

    }
}
