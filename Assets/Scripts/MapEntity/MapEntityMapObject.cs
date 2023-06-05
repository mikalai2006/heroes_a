using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using UnityEngine;

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

        // EntityMapObject entity = (EntityMapObject)_mapObject.Entity;
        ScriptableEntityMapObject configData = (ScriptableEntityMapObject)_mapObject.ConfigData;
        // if (entity.ConfigData.TypeMapObject == TypeMapObject.Resources) {
        //     return;
        // }

        if (LevelManager.Instance.ActivePlayer.DataPlayer.playerType != PlayerType.Bot)
        {
            if (configData.TypeWorkEffect == TypeWorkAttribute.OneNoDialog)
            {
                await OnHeroGo(player, default);
                return;
            }

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
