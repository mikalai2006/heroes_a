using System.Collections.Generic;

using Assets;

using Cysharp.Threading.Tasks;

using UnityEngine;

public struct DataDialogBuild
{
    public Sprite MenuSprite;
    public string title;
    public string description;
    public string textRequireBuild;
    public List<CostEntity> CostResource;
    public bool isNotBuild;
}

public class UITownBuildItemDialogOperation : LocalAssetLoader
{
    private DataDialogBuild _buildTownData;

    public UITownBuildItemDialogOperation(DataDialogBuild data)
    {
        _buildTownData = data;
    }

    public async UniTask<DataResultBuildDialog> ShowAndHide()
    {
        var window = await Load();
        var result = await window.ProcessAction(_buildTownData);
        Unload();
        return result;
    }

    public UniTask<UITownBuildItemDialogWindow> Load()
    {
        return LoadInternal<UITownBuildItemDialogWindow>(Constants.UILabels.UI_TOWN_BUILD_ITEM);
    }

    public void Unload()
    {
        UnloadInternal();
    }
}