using System.Collections.Generic;

using Loader;

using UnityEngine;
using UnityEngine.EventSystems;

public class Tavern : BaseBuild, IPointerClickHandler
{

    public async void OnPointerClick(PointerEventData eventData)
    {
        // Debug.Log($"Click {name}");
        var dialogWindow = new UITavernOperation();
        await dialogWindow.ShowAndHide();
    }
}
