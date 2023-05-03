using System.Collections.Generic;

using Assets;

using Cysharp.Threading.Tasks;

using UnityEngine;

public struct DataResultDialogLevelHero
{
    public bool isOk;
    public TypeSecondarySkill typeSecondarySkill;
}
public struct DataDialogLevelHero
{
    public Sprite sprite;
    public string name;
    public string level;
    public string gender;
    public SerializableDictionary<TypeSecondarySkill, int> SecondarySkills;
    public ScriptableAttributePrimarySkill PrimarySkill;

}
public class DialogHeroLevelOperation : LocalAssetLoader
{
    private DataDialogLevelHero _data;

    public DialogHeroLevelOperation(DataDialogLevelHero data)
    {
        _data = data;
    }

    public async UniTask<DataResultDialogLevelHero> ShowAndHide()
    {
        var window = await Load();
        var result = await window.ProcessAction(_data);
        Unload();
        return result;
    }

    public UniTask<UIDialogHeroLevel> Load()
    {
        return LoadInternal<UIDialogHeroLevel>(Constants.UILabels.UI_DIALOG_LEVELHERO);
    }

    public void Unload()
    {
        UnloadInternal();
    }
}