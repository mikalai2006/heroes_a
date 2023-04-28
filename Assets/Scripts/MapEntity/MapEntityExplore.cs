using Cysharp.Threading.Tasks;

public class MapEntityExplore : BaseMapEntity, IDialogMapObjectOperation
{
    public override void InitUnit(MapObject mapObject)
    {
        base.InitUnit(mapObject);
    }

    public async override UniTask OnGoHero(Player player)
    {
        await base.OnGoHero(player);

        _mapObject.OccupiedNode.ChangeStatusVisit(true);

        if (LevelManager.Instance.ActivePlayer.DataPlayer.playerType != PlayerType.Bot)
        {
            DataResultDialog result = await OnTriggeredHero();

            if (result.isOk)
            {
                _mapObject.DoHero(player);
            }
            else
            {
                // Click cancel.
            }
        }
        else
        {

        }
    }

    public async UniTask<DataResultDialog> OnTriggeredHero()
    {
        // var t = HelperLanguage.GetLocaleText(this.ScriptableData.Locale);
        var dialogData = new DataDialogMapObject()
        {
            // Header = MapObjectClass.ScriptableData.Text.title.GetLocalizedString(),
            // Description = t.Text.visit_ok,
            Sprite = _mapObject.ConfigData.MenuSprite,
        };

        var dialogWindow = new DialogMapObjectProvider(dialogData);
        return await dialogWindow.ShowAndHide();
    }

}
