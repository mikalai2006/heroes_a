// using Assets;

// using Cysharp.Threading.Tasks;

// public struct DataResultTown
// {
//     public bool close;

// }

// public class TownProvider : LocalAssetLoader
// {
//     public async UniTask<DataResultTown> ShowAndHide()
//     {
//         var townWindow = await Load();
//         townWindow.Init();
//         var result = await townWindow.ProcessAction();
//         Unload();
//         return result;
//     }

//     public UniTask<UITown> Load()
//     {
//         return LoadInternal<UITown>(Constants.UILabels.UI_TOWN);
//     }

//     public void Unload()
//     {
//         UnloadInternal();
//     }
// }