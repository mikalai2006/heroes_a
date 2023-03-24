using System;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Loader
{
    public class GameInitOperation : ILoadingOperation
    {
        public string Description => "Game init loading ...";

        public async UniTask Load(Action<float> onProgress)
        {
            onProgress?.Invoke(0.1f);

            // var loadOp = SceneManager.LoadSceneAsync(Constants.Scenes.SCENE_GAME, LoadSceneMode.Single);
            // while (loadOp.isDone == false)
            // {
            //     await UniTask.Delay(1);
            // }
            onProgress?.Invoke(0.7f);

            // var scene = SceneManager.GetSceneByName(Constants.Scenes.SCENE_GAME);
            var environment = await GameManager.Instance.AssetProvider.LoadSceneAdditive(Constants.Scenes.SCENE_GAME);
            var rootObjects = environment.Scene.GetRootGameObjects();
            onProgress?.Invoke(0.85f);
            MapManager MapManager = GameObject.FindGameObjectWithTag("MapManager")?.GetComponent<MapManager>();

            if (MapManager != null)
            {
                GameManager.Instance.MapManager = MapManager;
            }
            // editorGame.Init(environment);
            // editorGame.BeginNewGame();
            onProgress?.Invoke(1f);
        }
    }
}