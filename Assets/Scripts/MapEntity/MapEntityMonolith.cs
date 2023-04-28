using System.Linq;

using Cysharp.Threading.Tasks;


public class MapEntityMonolith : BaseMapEntity, IDialogMapObjectOperation
{
    public override void InitUnit(MapObject mapObject)
    {
        base.InitUnit(mapObject);
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

    public async override UniTask OnGoHero(Player player)
    {
        await base.OnGoHero(player);
        if (LevelManager.Instance.ActivePlayer.DataPlayer.playerType != PlayerType.Bot)
        {
            DataResultDialog result = await OnTriggeredHero();
            if (result.isOk)
            {
                await OnHeroGo(player, result);
            }
            else
            {
                // Click cancel.
            }
        }
        else
        {
            await OnHeroGo(player, default);
        }
    }
    private async UniTask OnHeroGo(Player player, DataResultDialog result)
    {
        MapObject MapObject = (MapObject)_mapObject;
        ScriptableEntityMapObject configData = (ScriptableEntityMapObject)_mapObject.ConfigData;

        if (result.keyVariant == -1)
        {
            await configData.RunHero(player, MapObject.Entity);
        }
        else
        {
            var effect = configData.Effects.ElementAt(MapObject.Entity.Effects.index);
            if (configData.Effects.Count > 0 && effect.Item.items.Count > 0)
            {
                await effect.Item.items[result.keyVariant].RunHero(player, MapObject.Entity);
            }
        }

        _mapObject.DoHero(player);
    }
}
