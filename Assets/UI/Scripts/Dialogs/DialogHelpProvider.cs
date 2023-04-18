using System.Collections.Generic;

using Assets;

using Cysharp.Threading.Tasks;

using UnityEngine;

public struct DataDialogHelp
{
    public string Header;
    public string Description;
    public Sprite Sprite;
}

public class DialogHelpProvider : LocalAssetLoader
{
    private DataDialogHelp _dataDialog;

    public DialogHelpProvider(DataDialogHelp dataDialog)
    {
        _dataDialog = dataDialog;
    }

    public async UniTask<DataResultDialog> ShowAndHide()
    {
        var loginWindow = await Load();
        var result = await loginWindow.ProcessAction(_dataDialog);
        Unload();
        return result;
    }

    public UniTask<UIDialogHelpWindow> Load()
    {
        return LoadInternal<UIDialogHelpWindow>(Constants.UILabels.UI_DIALOG_HELP);
    }

    public void Unload()
    {
        UnloadInternal();
    }
}