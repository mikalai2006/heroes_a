using UnityEngine;
using UnityEngine.EventSystems;

public class Tavern : BuildBase, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Click {name}");
    }
}
