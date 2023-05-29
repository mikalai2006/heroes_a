using Assets;

using Cysharp.Threading.Tasks;

public class UIInfoCreatureArenaOperation : LocalAssetLoader
{
    private ArenaEntityBase _entityCreature;
    public UIInfoCreatureArenaOperation(ArenaEntityBase entity)
    {
        _entityCreature = entity;
    }

    public async UniTask<DataResultBuildDialog> ShowAndHide()
    {
        var window = await Load();
        var result = await window.ProcessAction(_entityCreature);
        Unload();
        return result;
    }

    public UniTask<UIInfoCreatureArenaWindow> Load()
    {
        return LoadInternal<UIInfoCreatureArenaWindow>(Constants.UILabels.UI_INFO_ARENA_CREATURE);
    }

    public void Unload()
    {
        UnloadInternal();
    }
}