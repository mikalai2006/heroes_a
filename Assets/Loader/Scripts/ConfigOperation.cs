using System;

using AppInfo;

using Cysharp.Threading.Tasks;

namespace Loader
{
    public class ConfigOperation : ILoadingOperation
    {
        public ConfigOperation(AppInfoContainer appInfoContainer)
        {

        }
        public async UniTask Load(Action<float> onProgress, Action<string> onSetNotify)
        {
            onSetNotify?.Invoke("Load nature configuration ...");
            await ResourceSystem.Instance.LoadCollectionsAsset<TileLandscape>(Constants.Labels.LABEL_LANDSCAPE);
            onProgress?.Invoke(.1f);
            await ResourceSystem.Instance.LoadCollectionsAsset<TileNature>(Constants.Labels.LABEL_NATURE);
            onProgress?.Invoke(.2f);
            onSetNotify?.Invoke("Load hero configuration ...");
            await ResourceSystem.Instance.LoadCollectionsAsset<ScriptableUnitBase>(Constants.Labels.LABEL_HERO);
            onProgress?.Invoke(.3f);
            onSetNotify?.Invoke("Load units configuration ...");
            await ResourceSystem.Instance.LoadCollectionsAsset<ScriptableUnitBase>(Constants.Labels.LABEL_UNIT);
            onProgress?.Invoke(.4f);
            onSetNotify?.Invoke("Load other configuration ...");
            await ResourceSystem.Instance.LoadCollectionsAsset<TileLandscape>(Constants.Labels.LABEL_ROAD);
            onProgress?.Invoke(.5f);
            // await UniTask.Delay(1);
        }
    }
}