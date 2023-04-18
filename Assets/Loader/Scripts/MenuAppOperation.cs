using System;

using Cysharp.Threading.Tasks;

using UnityEngine.Localization;

namespace Loader
{
    public class MenuAppOperation : ILoadingOperation
    {
        public async UniTask Load(Action<float> onProgress, Action<string> onSetNotify)
        {
            var t = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "loadgamemenu").GetLocalizedString();
            onSetNotify?.Invoke(t);

            onProgress?.Invoke(0.1f);

            var environment = await GameManager.Instance.AssetProvider.LoadAsset("UIMenuApp");

            if (environment.TryGetComponent(out UIMenuApp component) == false)
                throw new NullReferenceException("Object of type UIMenuApp is null");

            component.Init(environment);

            onProgress?.Invoke(.3f);

        }
    }
}