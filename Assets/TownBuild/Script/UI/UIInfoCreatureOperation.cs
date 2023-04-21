using Assets;

using Cysharp.Threading.Tasks;

public class UIInfoCreatureOperation : LocalAssetLoader
{
    public UIInfoCreatureOperation()
    {
    }

    public async UniTask<DataResultBuildDialog> ShowAndHide()
    {
        var window = await Load();
        var result = await window.ProcessAction();
        Unload();
        return result;
    }

    public UniTask<UIInfoCreatureWindow> Load()
    {
        return LoadInternal<UIInfoCreatureWindow>(Constants.UILabels.UI_INFO_CREATURE);
    }

    public void Unload()
    {
        UnloadInternal();
    }
}