using System;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Loader
{
    public class GameInitOperation : ILoadingOperation
    {
        public async UniTask Load(Action<float> onProgress, Action<string> onSetNotify)
        {
            onProgress?.Invoke(0.1f);

            // var loadOp = SceneManager.LoadSceneAsync(Constants.Scenes.SCENE_GAME, LoadSceneMode.Single);
            // while (loadOp.isDone == false)
            // {
            //     await UniTask.Delay(1);
            // }

            // var scene = SceneManager.GetSceneByName(Constants.Scenes.SCENE_GAME);
            onSetNotify?.Invoke("Load game scene ...");
            var environment = await GameManager.Instance.AssetProvider.LoadSceneAdditive(Constants.Scenes.SCENE_GAME);
            var rootObjects = environment.Scene.GetRootGameObjects();


            onProgress?.Invoke(0.7f);

            MapManager MapManager = GameObject.FindGameObjectWithTag("MapManager")?.GetComponent<MapManager>();

            if (MapManager != null)
            {
                GameManager.Instance.MapManager = MapManager;
            }

            onProgress?.Invoke(0.85f);

            UIGameAside UIGameAside = GameObject.FindGameObjectWithTag("GameAside")?.GetComponent<UIGameAside>();

            if (UIGameAside != null)
            {
                UIGameAside.Init();
            }

            // editorGame.Init(environment);
            // editorGame.BeginNewGame();
            onProgress?.Invoke(1f);
        }
    }
}