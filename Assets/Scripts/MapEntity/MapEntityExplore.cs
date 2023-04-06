using System.Collections.Generic;

using Cysharp.Threading.Tasks;

[System.Serializable]
public struct DataExplore
{

}

public class MapEntityExplore : BaseMapEntity, IDialogMapObjectOperation
{
    public DataExplore Data;
    public override void InitUnit(BaseEntity mapObject)
    {
        base.InitUnit(mapObject);
    }

    public async override void OnGoHero(Player player)
    {
        base.OnGoHero(player);

        DataResultDialog result = await OnTriggeredHero();

        if (result.isOk)
        {
            List<GridTileNode> noskyNodes = GameManager.Instance.MapManager.DrawSky(MapObjectClass.OccupiedNode, 10);

            player.SetNosky(noskyNodes);
        }
        else
        {
            // Click cancel.
        }
    }

    public async UniTask<DataResultDialog> OnTriggeredHero()
    {
        // var t = HelperLanguage.GetLocaleText(this.ScriptableData.Locale);
        var dialogData = new DataDialog()
        {
            Header = MapObjectClass.ScriptableData.Text.title.GetLocalizedString(),
            // Description = t.Text.visit_ok,
            Sprite = MapObjectClass.ScriptableData.MenuSprite,
        };

        var dialogWindow = new DialogMapObjectProvider(dialogData);
        return await dialogWindow.ShowAndHide();
    }

}
