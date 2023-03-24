using System;

using Cysharp.Threading.Tasks;

namespace Loader
{
    public class MenuAppOperation : ILoadingOperation
    {
        public string Description => "Menu app loading...";

        public async UniTask Load(Action<float> onProgress)
        {
            onProgress?.Invoke(0.9f);

            await ResourceSystem.Instance.LoadCollectionsAsset<ScriptableGameMode>(Constants.Labels.LABEL_GAMEMODE);

            var environment = await GameManager.Instance.AssetProvider.LoadAsset("UIMenuApp");

            if (environment.TryGetComponent(out UIMenuApp component) == false)
                throw new NullReferenceException("Object of type UIMenuApp is null");

            component.Init(environment);

            onProgress?.Invoke(1f);

        }
    }
}