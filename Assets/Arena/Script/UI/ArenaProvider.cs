// using System.Collections.Generic;

// using Assets;

// using Cysharp.Threading.Tasks;

// using UnityEngine;

// // [Serializable]
// // public struct DialogArenaData
// // {
// //     public EntityTown town;
// //     public EntityHero hero;
// //     public EntityHero enemy;
// //     public EntityMapObject creatureBank;
// //     public EntityCreature creature;
// // }

// public class ArenaProvider : LocalAssetLoader
// {
//     // private UnitBase _unit;

//     private DialogArenaData _dataDialog;

//     public ArenaProvider(DialogArenaData dataDialog)
//     {
//         // _unit = unit;
//         _dataDialog = dataDialog;
//     }

//     public async UniTask<DialogArenaData> ShowAndHide()
//     {
//         var window = await Load();
//         await GameManager.Instance.AssetProvider.LoadSceneAdditive(Constants.Scenes.SCENE_ARENA);
//         var result = await window.ProcessAction(_dataDialog);
//         Unload();
//         return result;
//     }

//     public UniTask<UIArena> Load()
//     {
//         return LoadInternal<UIArena>(Constants.UILabels.UI_DIALOG_MAP_OBJECTS);
//     }

//     public void Unload()
//     {
//         UnloadInternal();
//     }
// }