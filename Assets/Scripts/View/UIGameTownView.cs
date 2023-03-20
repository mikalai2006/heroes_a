using System;
using UnityEngine;
using UnityEngine.UIElements;

public class UIGameTownView : MonoBehaviour
{
    [SerializeField] private UIDocument _uiRoot;

    private VisualElement _town;

    public void Init()
    {
        try
        {
            _town = _uiRoot.rootVisualElement.Q<VisualElement>("Town");

            var btnClose = _town.Q<Button>("ButtonClose");
            btnClose.clickable.clicked += Hide;
            
            Hide();
        }
        catch (Exception e)
        {
            Debug.LogWarning("Town UI error: \n" + e);
        }
    }

    public void Show()
    {
        _town.style.display = DisplayStyle.Flex;
    }

    private void Hide()
    {

        _town.style.display = DisplayStyle.None;
    }

}
