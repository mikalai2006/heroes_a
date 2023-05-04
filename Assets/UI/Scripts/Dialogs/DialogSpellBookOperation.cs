using Assets;

using Cysharp.Threading.Tasks;
public struct DataResultDialogSpellBook
{
    public bool isOk;
    public int value1;
    public int value2;
}
// public struct DataDialogSpellBook
// {
//     public bool isOk;
//     public int value1;
//     public int value2;
// }
public class DialogSpellBookOperation : LocalAssetLoader
{
    private EntityHero _dataDialog;
    public DialogSpellBookOperation(EntityHero dataDialog)
    {
        _dataDialog = dataDialog;
    }

    public async UniTask<DataResultDialogSpellBook> ShowAndHide()
    {
        var window = await Load();
        var result = await window.ProcessAction(_dataDialog);
        Unload();
        return result;
    }

    public UniTask<UIDialogSpellBook> Load()
    {
        return LoadInternal<UIDialogSpellBook>(Constants.UILabels.UI_DIALOG_SPELLBOOK);
    }

    public void Unload()
    {
        UnloadInternal();
    }
}