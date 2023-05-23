using Assets;

using Cysharp.Threading.Tasks;

public class UIInfoArenaHeroOperation : LocalAssetLoader
{
    private EntityHero _hero;

    public UIInfoArenaHeroOperation(EntityHero hero)
    {
        _hero = hero;
    }

    public async UniTask<DataResultDialogHeroInfo> ShowAndHide()
    {
        var window = await Load();
        var result = await window.ProcessAction(_hero);
        Unload();
        return result;
    }

    public UniTask<UIInfoArenaHero> Load()
    {
        return LoadInternal<UIInfoArenaHero>(Constants.UILabels.UI_DIALOG_ARENA_HEROINFO);
    }

    public void Unload()
    {
        UnloadInternal();
    }
}