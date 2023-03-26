using System.Collections.Generic;

using Assets;

using Cysharp.Threading.Tasks;

using UnityEngine;

public class DataDialog
{
    public string Header;
    public string Description;
    public Sprite Sprite;
    public List<DataDialogItem> Value;

    public DataDialog()
    {
        Value = new List<DataDialogItem>();
    }
}

public struct DataDialogItem
{
    public Sprite Sprite;
    public int Value;
}

public struct DataResultDialog
{
    public bool isOk;

    public int keyVariant;
}

public class DialogMapObjectProvider : LocalAssetLoader
{
    // private UnitBase _unit;

    private DataDialog _dataDialog;

    public DialogMapObjectProvider(DataDialog dataDialog)
    {
        // _unit = unit;
        _dataDialog = dataDialog;
    }

    public async UniTask<DataResultDialog> ShowAndHide()
    {
        var loginWindow = await Load();
        var result = await loginWindow.ProcessAction(_dataDialog);
        Unload();
        return result;
    }

    public UniTask<UIDialogMapObjectWindow> Load()
    {
        return LoadInternal<UIDialogMapObjectWindow>(Constants.UILabels.UI_DIALOG_MAP_OBJECTS);
    }

    public void Unload()
    {
        UnloadInternal();
    }
}