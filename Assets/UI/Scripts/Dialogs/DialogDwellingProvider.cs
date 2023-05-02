using System.Collections.Generic;

using Assets;

using Cysharp.Threading.Tasks;

public struct DataResultDialogDwelling
{
    public bool isOk;

    public int keyVariant;
}

public struct DataDialogDwelling
{
    public EntityDwelling dwelling;

    public SerializableDictionary<int, EntityCreature> Creatures;
}

public class DialogDwellingProvider : LocalAssetLoader
{
    private DataDialogDwelling _dataDialog;

    public DialogDwellingProvider(DataDialogDwelling dataDialog)
    {
        _dataDialog = dataDialog;
    }

    public async UniTask<DataResultDialogDwelling> ShowAndHide()
    {
        var window = await Load();
        var result = await window.ProcessAction(_dataDialog);
        Unload();
        return result;
    }

    public UniTask<UIDialogDwellingWindow> Load()
    {
        return LoadInternal<UIDialogDwellingWindow>(Constants.UILabels.UI_DIALOG_DWELLING);
    }

    public void Unload()
    {
        UnloadInternal();
    }
}