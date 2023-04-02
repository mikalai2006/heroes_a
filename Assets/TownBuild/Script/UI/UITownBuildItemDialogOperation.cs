using Assets;

using Cysharp.Threading.Tasks;

public class UITownBuildItemDialogOperation : LocalAssetLoader
{
    private Build _buildTownData;

    public UITownBuildItemDialogOperation(Build data)
    {
        _buildTownData = data;
    }

    public async UniTask<DataResultBuildDialog> ShowAndHide()
    {
        var window = await Load();
        var result = await window.ProcessAction(_buildTownData);
        Unload();
        return result;
    }

    public UniTask<UITownBuildItemDialogWindow> Load()
    {
        return LoadInternal<UITownBuildItemDialogWindow>(Constants.UILabels.UI_TOWN_BUILD_ITEM);
    }

    public void Unload()
    {
        UnloadInternal();
    }
}