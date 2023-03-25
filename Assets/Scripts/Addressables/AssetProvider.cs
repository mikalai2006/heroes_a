using System;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
//using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.SceneManagement;
using Loader;
using UnityEngine;
//using System.Collections.Generic;

public class AssetProvider : ILoadingOperation
{
    private bool _isReady;

    public string Description => "Assets Initialization...";
    public async UniTask Load(Action<float> onProgress, Action<string> onSetNotify)
    {
        Debug.LogWarning("Init addressables");
        var operation = Addressables.InitializeAsync();
        await operation.Task;
        _isReady = true;
    }

    private async UniTask WaitUntilReady()
    {
        while (_isReady == false)
        {
            await UniTask.Yield();
        }
    }

    public async UniTask<SceneInstance> LoadSceneAdditive(string sceneId)
    {
        await WaitUntilReady();
        var op = Addressables.LoadSceneAsync(sceneId,
            LoadSceneMode.Additive);
        return await op.Task;
    }

    public async UniTask UnloadAdditiveScene(SceneInstance scene)
    {
        await WaitUntilReady();
        var op = Addressables.UnloadSceneAsync(scene);
        await op.Task;
    }


    public async UniTask<UnityEngine.GameObject> LoadAsset(string assetId)
    {
        var handle = Addressables.InstantiateAsync(assetId);
        var _cachedObject = await handle.Task;
        // if (_cachedObject.TryGetComponent(out T component) == false)
        //     throw new NullReferenceException($"Object of type {typeof(T)} is null on " +
        //                                      "attempt to load it from addressables");
        return await handle.Task;
    }
    public async UniTask UnloadAsset(UnityEngine.GameObject obj)
    {
        await WaitUntilReady();
        var op = Addressables.ReleaseInstance(obj);
        // await op.Task;
    }
    //public async UniTask<IList<IResourceLocation>> LoadGroupByLabel(string label)
    //{
    //    var handle = Addressables.LoadResourceLocationsAsync(label);
    //    var _cachedObject = await handle.Task;
    //    if (_cachedObject.Count == 0)
    //        throw new NullReferenceException($"Object of type IResourceLocation is null on " +
    //                                         "attempt to load it from addressables");
    //    return _cachedObject;
    //}
}