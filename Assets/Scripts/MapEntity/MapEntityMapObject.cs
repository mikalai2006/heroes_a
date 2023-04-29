using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

public class MapEntityMapObject : BaseMapEntity, IDialogMapObjectOperation
{
    private DataDialogMapObjectGroup _dialogData;

    // public override void InitUnit(MapObject mapObject)
    // {

    //     base.InitUnit(mapObject);
    // }

    public async UniTask<DataResultDialog> OnTriggeredHero()
    {
        _dialogData = new DataDialogMapObjectGroup()
        {
            Values = new List<DataDialogMapObjectGroupItem>()
        };

        ScriptableEntityMapObject configData = (ScriptableEntityMapObject)_mapObject.ConfigData;

        foreach (var effect in configData.Effects[_mapObject.Entity.Effects.index].Item.items)
        {
            effect.CreateDialogData(ref _dialogData, _mapObject.Entity);
        }
        var groups = new List<DataDialogMapObjectGroup>();

        groups.Add(_dialogData);

        var description = configData.Effects[_mapObject.Entity.Effects.index].Item.description.IsEmpty
            ? ""
            : configData.Effects[_mapObject.Entity.Effects.index].Item.description.GetLocalizedString();

        var dialogData = new DataDialogMapObject()
        {
            Header = configData.Text.title.GetLocalizedString(),
            Description = description,
            // Sprite = this.ScriptableData.MenuSprite,
            TypeCheck = TypeCheck.Default,
            TypeWorkEffect = configData.TypeWorkEffect,
            Groups = groups
        };

        var dialogWindow = new DialogMapObjectProvider(dialogData);
        return await dialogWindow.ShowAndHide();
    }

    public async override UniTask OnGoHero(Player player)
    {
        await base.OnGoHero(player);

        MapObject entity = (MapObject)_mapObject;
        if (LevelManager.Instance.ActivePlayer.DataPlayer.playerType != PlayerType.Bot)
        {

            ScriptableEntityMapObject configData = (ScriptableEntityMapObject)_mapObject.ConfigData;
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
            await UniTask.Delay(LevelManager.Instance.ConfigGameSettings.timeDelayDoBot);
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
        if (configData.TypeWorkObject == TypeWorkObject.One)
        {
            // base.MapObject.Entity.DestroyEntity();
            Destroy(gameObject);
        }
        // else {
        //     foreach (var res in entity.Data.AttributeValues)
        //     {
        //         Debug.Log($"Status choose={res.TypeAttribute}-{res.value}");
        //     }
        // }
    }

}
