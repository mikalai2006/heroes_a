using Assets;

using Cysharp.Threading.Tasks;
public struct DataResultDialogSplitCreature
{
    public bool isOk;
    public int value1;
    public int value2;
}
public class DialogSplitCreatureOperation : LocalAssetLoader
{
    private EntityCreature _startCreature;
    private EntityCreature _endCreature;

    public DialogSplitCreatureOperation(EntityCreature startCreature, EntityCreature endCreature)
    {
        _startCreature = startCreature;
        _endCreature = endCreature;
    }

    public async UniTask<DataResultDialogSplitCreature> ShowAndHide()
    {
        var window = await Load();
        var result = await window.ProcessAction(_startCreature, _endCreature);
        Unload();
        return result;
    }

    public UniTask<UIDialogSplitCreatureWindow> Load()
    {
        return LoadInternal<UIDialogSplitCreatureWindow>(Constants.UILabels.UI_DIALOG_SPLIT_CREATURE);
    }

    public void Unload()
    {
        UnloadInternal();
    }
}