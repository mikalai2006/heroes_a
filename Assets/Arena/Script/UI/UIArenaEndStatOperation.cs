using Assets;

using Cysharp.Threading.Tasks;
public struct ArenaStatResult
{
    public bool isOk;
}
public struct ArenaStat
{
}
public class UIArenaEndStatOperation : LocalAssetLoader
{
    private ArenaStat _stat;
    public UIArenaEndStatOperation(ArenaStat stat)
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

    public UniTask<UIArenaEndStatWindow> Load()
    {
        return LoadInternal<UIArenaEndStatWindow>(Constants.UILabels.UI_INFO_ARENA_ENDSTAT);
    }

    public void Unload()
    {
        UnloadInternal();
    }
}