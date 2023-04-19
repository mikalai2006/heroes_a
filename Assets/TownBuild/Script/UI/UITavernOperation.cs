using Assets;

using Cysharp.Threading.Tasks;

public class UITavernOperation : LocalAssetLoader
{
    BaseBuild build;
    public UITavernOperation(BaseBuild build)
    {
        this.build = build;
    }

    public async UniTask<DataResultBuildDialog> ShowAndHide()
    {
        var window = await Load();
        var result = await window.ProcessAction(build);
        Unload();
        return result;
    }

    public UniTask<UITavernWindow> Load()
    {
        return LoadInternal<UITavernWindow>(Constants.UILabels.UI_TAVERN);
    }

    public void Unload()
    {
        UnloadInternal();
    }
}