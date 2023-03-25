using System;

using Cysharp.Threading.Tasks;

namespace Loader
{
    public interface ILoadingOperation
    {
        // string Description { get; }

        UniTask Load(Action<float> onProgress, Action<string> onSetNotify);
    }
}