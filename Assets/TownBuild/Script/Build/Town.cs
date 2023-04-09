using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.EventSystems;

public class Town : BuildBase, IPointerClickHandler, IClickeredBuild
{
    public async UniTask<DataResultBuildDialog> OnClickToBuild()
    {
        var dialogWindow = new UITownListBuildOperation(new DataDialogMapObject(), UITown._activeBuildTown);
        return await dialogWindow.ShowAndHide();
    }

    public async void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Click council");
        var result = await OnClickToBuild();
        UITown.DrawBuilds(result);
    }
}
