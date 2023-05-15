using System;
using System.Linq;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public struct DialogArenaData
{
    public EntityTown town;
    public EntityHero hero;
    public EntityHero enemy;
    public EntityMapObject creatureBank;
}

namespace Loader
{
    public class ArenaLoadOperation : ILoadingOperation
    {
        private DialogArenaData _dialogArenaData;

        public ArenaLoadOperation(DialogArenaData dialogArenaData)
        {
            _dialogArenaData = dialogArenaData;
        }

        public async UniTask Load(Action<float> onProgress, Action<string> onSetNotify)
        {
            onProgress?.Invoke(0.1f);

            onSetNotify?.Invoke("Load arena scene ...");
            var environment = await GameManager.Instance.AssetProvider.LoadSceneAdditive(Constants.Scenes.SCENE_ARENA);
            var rootObjects = environment.Scene.GetRootGameObjects();

            UIArena UIArena = GameObject.FindGameObjectWithTag("UIArena")?.GetComponent<UIArena>();

            if (UIArena != null)
            {
                UIArena.Init(environment);
            }
            // GameObject Town = GameObject.FindGameObjectWithTag("Town");

            // if (Town != null)
            // {
            //     foreach (BuildBase build in Town.transform.GetComponentsInChildren<BuildBase>())
            //     {
            //         build.UITown = UITown;
            //     }
            // }

            // editorGame.Init(environment);
            // editorGame.BeginNewGame();
            onProgress?.Invoke(0.2f);
        }
    }
}