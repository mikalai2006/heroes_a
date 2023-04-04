using System.Collections.Generic;

using Cysharp.Threading.Tasks;

[System.Serializable]
public struct DataExplore
{

}

public abstract class BaseExplore : BaseMapObject, IDataPlay, IDialogMapObjectOperation
{
    public DataExplore Data;
    public async override void OnGoHero(Player player)
    {
        base.OnGoHero(player);

        DataResultDialog result = await OnTriggeredHero();

        if (result.isOk)
        {
            List<GridTileNode> noskyNodes = GameManager.Instance.MapManager.DrawSky(OccupiedNode, 10);

            player.SetNosky(noskyNodes);
        }
        else
        {
            // Click cancel.
        }
    }

    public async UniTask<DataResultDialog> OnTriggeredHero()
    {
        var t = HelperLanguage.GetLocaleText(this.ScriptableData.Locale);
        var dialogData = new DataDialog()
        {
            Description = t.Text.visit_ok,
            Header = t.Text.title,
            Sprite = this.ScriptableData.MenuSprite,
        };

        var dialogWindow = new DialogMapObjectProvider(dialogData);
        return await dialogWindow.ShowAndHide();
    }

    public void LoadDataPlay(DataPlay data)
    {
    }

    public void SaveDataPlay(ref DataPlay data)
    {
        var sdata = SaveUnit(Data);
        data.Units.explorers.Add(sdata);
    }
}
