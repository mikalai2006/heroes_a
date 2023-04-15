using System.Collections.Generic;

using Assets;

using Cysharp.Threading.Tasks;

public class UITavernOperation : LocalAssetLoader
{
    public UITavernOperation()
    {
    }

    public async UniTask<DataResultBuildDialog> ShowAndHide()
    {
        var window = await Load();
        var result = await window.ProcessAction();
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