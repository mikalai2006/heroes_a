using Assets;

using Cysharp.Threading.Tasks;

public class UIInfoCreatureOperation : LocalAssetLoader
{
    private EntityCreature _entityCreature;
    public UIInfoCreatureOperation(EntityCreature entityCreature)
    {
        _entityCreature = entityCreature;
    }

    public async UniTask<DataResultBuildDialog> ShowAndHide()
    {
        var window = await Load();
        var result = await window.ProcessAction(_entityCreature);
        Unload();
        return result;
    }

    public UniTask<UIInfoCreatureWindow> Load()
    {
        return LoadInternal<UIInfoCreatureWindow>(Constants.UILabels.UI_INFO_CREATURE);
    }

    public void Unload()
    {
        UnloadInternal();
    }
}