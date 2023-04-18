using System.Collections.Generic;
using UnityEngine;
using AppInfo;
using Login;
using Loader;

public class AppStartup : MonoBehaviour
{
    private LoadingScreenProvider loadingProvider => GameManager.Instance.LoadingScreenProvider;

    private async void Start()
    {
        GameManager.Instance.Init();

        var appInfo = new AppInfoContainer();

        var loadingOperations = new Queue<ILoadingOperation>();
        loadingOperations.Enqueue(GameManager.Instance.AssetProvider);
        // loadingOperations.Enqueue(new LoginOperation(appInfo));
        loadingOperations.Enqueue(new ConfigOperation(appInfo));
        loadingOperations.Enqueue(new MenuAppOperation());
        GameManager.Instance.AppInfo = appInfo;
        //GameManager.Instance.loadingScreenProvider.LoadAndDestroy(loadingOperations);
        await loadingProvider.LoadAndDestroy(loadingOperations);

    }
}
