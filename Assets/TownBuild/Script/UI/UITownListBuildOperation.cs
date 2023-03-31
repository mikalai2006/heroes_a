using System.Collections.Generic;

using Assets;

using Cysharp.Threading.Tasks;

using UnityEngine;

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

public class UITownListBuildOperation : LocalAssetLoader
{
    private DataDialog _dataDialog;

    public UITownListBuildOperation(DataDialog dataDialog)
    {
        // _unit = unit;
        _dataDialog = dataDialog;
    }

    public async UniTask<DataResultDialog> ShowAndHide()
    {
        var window = await Load();
        var result = await window.ProcessAction(_dataDialog);
        Unload();
        return result;
    }

    public UniTask<UITownListBuildWindow> Load()
    {
        return LoadInternal<UITownListBuildWindow>(Constants.UILabels.UI_TOWN_LIST_BUILD);
    }

    public void Unload()
    {
        UnloadInternal();
    }
}