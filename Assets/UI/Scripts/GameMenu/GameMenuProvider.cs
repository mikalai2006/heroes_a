using Assets;

using Cysharp.Threading.Tasks;

public struct DataResultGameMenu
{
    public bool isOk;

    public int keyVariant;
}

public class GameMenuProvider : LocalAssetLoader
{
    // private UnitBase _unit;

    // private DataResultGameMenu _dataResultGameMenu;

    // public GameMenuProvider()
    // {
    //     _dataResultGameMenu = new DataResultGameMenu();
    // }

    public async UniTask<DataResultGameMenu> ShowAndHide()
    {
        var loadWindow = await Load();
        loadWindow.Init();
        var result = await loadWindow.ProcessAction();
        Unload();
        return result;
    }

    public UniTask<UIGameMenu> Load()
    {
        return LoadInternal<UIGameMenu>(Constants.UILabels.UI_GAME_MENU);
    }

    public void Unload()
    {
        UnloadInternal();
    }
}