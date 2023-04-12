using Assets;

using Cysharp.Threading.Tasks;

public struct DataResultDialogDwelling
{
    public bool isOk;

    public int keyVariant;
}

public class DialogDwellingProvider : LocalAssetLoader
{
    private EntityDwelling _dwelling;

    public DialogDwellingProvider(EntityDwelling dwelling)
    {
        _dwelling = dwelling;
    }

    public async UniTask<DataResultDialogDwelling> ShowAndHide()
    {
        var window = await Load();
        var result = await window.ProcessAction(_dwelling);
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