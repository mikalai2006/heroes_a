using System.Collections.Generic;

using Assets;

using Cysharp.Threading.Tasks;

using UnityEngine;

public class DataDialogMapObject
{
    public string Header;
    public string Description;
    public Sprite Sprite;
    public List<DataDialogMapObjectGroup> Groups;
    public TypeCheck TypeCheck;
    public TypeWorkAttribute TypeWorkEffect;
    public DataDialogMapObject()
    {
        Groups = new List<DataDialogMapObjectGroup>();
    }
}

public enum TypeCheck
{
    Default = 0,
    OnlyOk = 1,
    Choose = 2,
    Cost = 3,
}

public struct DataDialogMapObjectGroup
{
    public List<DataDialogMapObjectGroupItem> Values;
    public TypeEntity TypeEntity;
    // public List<DataDialogMapObjectGroupItemArtifact> Artifacts;
}

public struct DataDialogMapObjectGroupItem
{
    public Sprite Sprite;
    public int value;
    public string title;
}
// public struct DataDialogMapObjectGroupItemArtifact
// {
//     public Sprite Sprite;
//     public string idObject;
// }
public struct DataResultDialog
{
    public bool isOk;

    public int keyVariant;
}

public class DialogMapObjectProvider : LocalAssetLoader
{
    // private UnitBase _unit;

    private DataDialogMapObject _dataDialog;

    public DialogMapObjectProvider(DataDialogMapObject dataDialog)
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