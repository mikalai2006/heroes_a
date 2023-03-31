// using System.Collections.Generic;

using Assets;

using Cysharp.Threading.Tasks;

// using UnityEngine;

// public class DataDialog
// {
//     public string Header;
//     public string Description;
//     public Sprite Sprite;
//     public List<DataDialogItem> Value;

//     public DataDialog()
//     {
//         Value = new List<DataDialogItem>();
//     }
// }

// public struct DataDialogItem
// {
//     public Sprite Sprite;
//     public int Value;
// }

// public struct DataResultDialog
// {
//     public bool isOk;

//     public int keyVariant;
// }

public class UITownBuildItemDialogOperation : LocalAssetLoader
{
    private DataDialog _dataDialog;

    public UITownBuildItemDialogOperation(DataDialog dataDialog)
    {
        _dataDialog = dataDialog;
    }

    public async UniTask<DataResultDialog> ShowAndHide()
    {
        var window = await Load();
        var result = await window.ProcessAction(_dataDialog);
        Unload();
        return result;
    }

    public UniTask<UITownBuildItemDialogWindow> Load()
    {
        return LoadInternal<UITownBuildItemDialogWindow>(Constants.UILabels.UI_TOWN_BUILD_ITEM);
    }

    public void Unload()
    {
        UnloadInternal();
    }
}