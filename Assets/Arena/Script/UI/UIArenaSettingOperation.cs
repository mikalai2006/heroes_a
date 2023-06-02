using Assets;

using Cysharp.Threading.Tasks;

public struct ArenaSettingData
{
    public ArenaManager arenaManager;
}
public class UIArenaSettingOperation : LocalAssetLoader
{
    private ArenaSettingData _arenaSettingData;
    public async UniTask<DataResultGameMenu> ShowAndHide(ArenaSettingData arenaSettingData)
    {
        _arenaSettingData = arenaSettingData;
        var loadWindow = await Load();
        var result = await loadWindow.ProcessAction(arenaSettingData);
        Unload();
        return result;
    }

    public UniTask<UIArenaSettingWindow> Load()
    {
        return LoadInternal<UIArenaSettingWindow>(Constants.UILabels.UI_ARENA_SETTING);
    }

    public void Unload()
    {
        UnloadInternal();
    }
}