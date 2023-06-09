using System;

using Assets;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;

[Serializable]
public struct DialogArenaData
{
    public EntityTown town;
    public EntityHero heroAttacking;
    public EntityHero heroDefending;
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

            ArenaManager arenaManager = GameObject.FindGameObjectWithTag("ArenaManager").GetComponent<ArenaManager>();
            arenaManager.CreateArena(_dialogArenaData);

            UIArena UIArena = GameObject.FindGameObjectWithTag("UIArena").GetComponent<UIArena>();
            if (UIArena != null)
            {
                UIArena.Init(arenaManager);
            }

            var result = await UIArena.ProcessAction(_dialogArenaData);
            await Unload();
            return result;
        }

        public async UniTask Unload()
        {
            await GameManager.Instance.AssetProvider.UnloadAdditiveScene(_scene);
        }
    }
}