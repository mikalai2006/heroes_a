using Assets;

using Cysharp.Threading.Tasks;

public struct DataResultBuildDialog
{
    public bool isOk;
    public Build build;
    public BuildBase BaseBuild;
}

public class UITownListBuildOperation : LocalAssetLoader
{
    private DataDialog _dataDialog;

    public UITownListBuildOperation(DataDialog dataDialog)
    {
        _dataDialog = dataDialog;
    }

    public async UniTask<DataResultBuildDialog> ShowAndHide()
    {
        var window = await Load();
        var result = await window.ProcessAction(_dataDialog);
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