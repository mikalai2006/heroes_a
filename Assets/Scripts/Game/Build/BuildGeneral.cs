using Cysharp.Threading.Tasks;

using System;
using System.Linq;

using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public class BuildGeneral : BaseBuild
{
    public DataBuildGeneral Data = new DataBuildGeneral();

    public BuildGeneral(
        int level,
        ScriptableBuildBase configData,
        SaveDataBuild<DataBuildGeneral> saveData = null
        )
    {
        if (saveData == null)
        {
            ConfigData = configData;
            // ResourceSystem.Instance.GetBuildTowns()
            //     .Where(t => t.TypeFaction == typeFaction && t.typ)
        }
        else
        {
            ConfigData = configData;
            Data = saveData.data;
        }

        base.Init(configData, level);
    }
    // public async UniTask<DataResultBuildDialog> OnClickToBuild()
    // {
    //     var dialogWindow = new UITownListBuildOperation(new DataDialogMapObject(), UITown._activeBuildTown);
    //     return await dialogWindow.ShowAndHide();
    // }

    // public async void OnPointerClick(PointerEventData eventData)
    // {
    //     Debug.Log($"Click council");
    //     var result = await OnClickToBuild();
    //     UITown.DrawBuilds(result);
    // }
}
