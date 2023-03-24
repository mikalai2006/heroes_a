using System.Collections.Generic;
using Assets;
using Cysharp.Threading.Tasks;
using Loader;

public class LoadingScreenProvider : LocalAssetLoader
{
    public async UniTask LoadAndDestroy(Queue<ILoadingOperation> loadingOperations)
    {
        var loadingScreen = await Load();
        await loadingScreen.Load(loadingOperations);
        Unload();
    }

    public UniTask<UILoaderScreen> Load()
    {
        return LoadInternal<UILoaderScreen>("UILoaderScreen");
    }

    public void Unload()
    {
        UnloadInternal();
    }
}