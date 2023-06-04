using System;
using System.Collections.Generic;

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
    public EntityMapObject creaturesBank;
    public EntityCreature creature;
    public SOArenaSetting ArenaSetting;
}
[Serializable]
public struct ResultDialogArenaData
{
    public bool isEnd;
    public int experienceLeft;
    public Dictionary<BaseEntity, int> deathLeft;
    public Dictionary<BaseEntity, int> deathRight;
    public int experienceRight;
    public bool isWinLeftHero;
    public bool isWinRightHero;
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