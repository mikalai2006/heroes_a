using Assets;

using Cysharp.Threading.Tasks;
public class UIArenaFullStatOperation : LocalAssetLoader
{
    private ArenaStatData _stat;
    public UIArenaFullStatOperation(ArenaStatData stat)
    {
        _stat = stat;
    }

    public async UniTask<ArenaStatResult> ShowAndHide()
    {
        var window = await Load();
        var result = await window.ProcessAction(_stat);
        Unload();
        return result;
    }

    public UniTask<UIArenaFullStatWindow> Load()
    {
        return LoadInternal<UIArenaFullStatWindow>(Constants.UILabels.UI_INFO_ARENA_FULLSTAT);
    }

    public void Unload()
    {
        UnloadInternal();
    }
}