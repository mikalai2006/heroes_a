using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.EventSystems;

public class Town : BuildBase, IPointerClickHandler, IClickeredBuild
{
    public async UniTask<DataResultDialog> OnClickToBuild()
    {
        var dialogWindow = new UITownListBuildOperation(new DataDialog());
        return await dialogWindow.ShowAndHide();
    }

    public async void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Click council");
        await OnClickToBuild();
    }
}
