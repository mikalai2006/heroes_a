using System;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Loader
{
    public class TownLoadOperation : ILoadingOperation
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
            onSetNotify?.Invoke("Load town scene ...");
            var environment = await GameManager.Instance.AssetProvider.LoadSceneAdditive(Constants.Scenes.SCENE_TOWN);
            var rootObjects = environment.Scene.GetRootGameObjects();

            // MapManager MapManager = GameObject.FindGameObjectWithTag("MapManager")?.GetComponent<MapManager>();

            // if (MapManager != null)
            // {
            //     GameManager.Instance.MapManager = MapManager;
            // }

            UITown UITown = GameObject.FindGameObjectWithTag("Town")?.GetComponent<UITown>();

            if (UITown != null)
            {
                UITown.Init(environment);
            }

            // editorGame.Init(environment);
            // editorGame.BeginNewGame();
            onProgress?.Invoke(0.2f);
        }
    }
}