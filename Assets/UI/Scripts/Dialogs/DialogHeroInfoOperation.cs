using Assets;

using Cysharp.Threading.Tasks;
public struct DataResultDialogHeroInfo
{
    public bool isOk;
}

public class DialogHeroInfoOperation : LocalAssetLoader
{
    private EntityHero _hero;

    public DialogHeroInfoOperation(EntityHero hero)
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

    public UniTask<UIDialogHeroInfo> Load()
    {
        return LoadInternal<UIDialogHeroInfo>(Constants.UILabels.UI_DIALOG_HEROINFO);
    }

    public void Unload()
    {
        UnloadInternal();
    }
}