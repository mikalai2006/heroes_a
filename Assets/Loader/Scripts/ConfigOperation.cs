using System;

using AppInfo;

using Cysharp.Threading.Tasks;

namespace Loader
{
    public class ConfigOperation : ILoadingOperation
    {
        public string Description => "Configuration loading...";

        public ConfigOperation(AppInfoContainer appInfoContainer)
        {

        }

        public async UniTask Load(Action<float> onProgress)
        {
            await ResourceSystem.Instance.LoadCollectionsAsset<TileLandscape>(Constants.Labels.LABEL_LANDSCAPE);
            onProgress?.Invoke(.1f);
            await ResourceSystem.Instance.LoadCollectionsAsset<TileNature>(Constants.Labels.LABEL_NATURE);
            onProgress?.Invoke(.2f);
            await ResourceSystem.Instance.LoadCollectionsAsset<ScriptableUnitBase>(Constants.Labels.LABEL_HERO);
            onProgress?.Invoke(.3f);
            await ResourceSystem.Instance.LoadCollectionsAsset<ScriptableUnitBase>(Constants.Labels.LABEL_UNIT);
            onProgress?.Invoke(.4f);
            await ResourceSystem.Instance.LoadCollectionsAsset<TileLandscape>(Constants.Labels.LABEL_ROAD);
            onProgress?.Invoke(.5f);
            await UniTask.Delay(1);
        }
    }
}