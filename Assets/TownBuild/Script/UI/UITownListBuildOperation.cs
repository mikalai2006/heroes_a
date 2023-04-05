using System.Collections.Generic;

using Assets;

using Cysharp.Threading.Tasks;

public struct DataResultBuildDialog
{
    public bool isOk;
    public Build build;
    public List<TypeBuild> PreProgressBuild;
}

public class UITownListBuildOperation : LocalAssetLoader
{
    private DataDialog _dataDialog;
    private ScriptableBuildTown _activeBuildTown;

    public UITownListBuildOperation(DataDialog dataDialog, ScriptableBuildTown activeBuildTown)
    {
        _dataDialog = dataDialog;
        _activeBuildTown = activeBuildTown;
    }

    public async UniTask<DataResultBuildDialog> ShowAndHide()
    {
        var window = await Load();
        var result = await window.ProcessAction(_dataDialog, _activeBuildTown);
        Unload();
        return result;
    }

    public UniTask<UITownListBuildWindow> Load()
    {
        return LoadInternal<UITownListBuildWindow>(Constants.UILabels.UI_TOWN_LIST_BUILD);
    }

    public void Unload()
    {
        UnloadInternal();
    }
}