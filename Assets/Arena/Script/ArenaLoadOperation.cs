using System;

using Assets;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;

[Serializable]
public struct DialogArenaData
{
    public EntityTown town;
    public EntityHero hero;
    public EntityHero enemy;
    public EntityMapObject creatureBank;
    public EntityCreature creature;
    public SOArenaSetting ArenaSetting;
}
[Serializable]
public struct ResultDialogArenaData
{
    public bool isEnd;
}

namespace Loader
{
    public class ArenaLoadOperation : LocalAssetLoader
    {
        private SceneInstance _scene;
        private DialogArenaData _dialogArenaData;

        public ArenaLoadOperation(DialogArenaData dialogArenaData)
        {
            _dialogArenaData = dialogArenaData;
        }

        public async UniTask<ResultDialogArenaData> ShowHide()
        {
            _scene = await GameManager.Instance.AssetProvider.LoadSceneAdditive(Constants.Scenes.SCENE_ARENA);
            // var rootObjects = _scene.Scene.GetRootGameObjects();

            UIArena UIArena = GameObject.FindGameObjectWithTag("UIArena")?.GetComponent<UIArena>();

            ArenaManager ArenaManager = GameObject.FindGameObjectWithTag("ArenaManager")?.GetComponent<ArenaManager>();
            ArenaManager.CreateArena(_dialogArenaData);
            if (UIArena != null)
            {
                UIArena.Init();
            }

            var result = await UIArena.ProcessAction();
            await Unload();
            return result;
        }

        public async UniTask Unload()
        {
            await GameManager.Instance.AssetProvider.UnloadAdditiveScene(_scene);
        }
    }
}