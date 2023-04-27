using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

public class MapEntityMapObject : BaseMapEntity, IDialogMapObjectOperation
{
    private DataDialogMapObjectGroup _dialogData;

    public override void InitUnit(MapObject mapObject)
    {

        base.InitUnit(mapObject);

        // var mapObjectClass = (EntityMapObject)MapObjectClass;
        //mapObjectClass.SetData();
    }

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
        // var group = new DataDialogMapObjectGroup();
        // // var group = groups[0].Items;
        // group.Values = new List<DataDialogMapObjectGroupItem>();
        // for (int i = 0; i < entity.Data.AttributeValues.Count; i++)
        // {
        //     group.Values.Add(new DataDialogMapObjectGroupItem()
        //     {
        //         Sprite = entity.Data.AttributeValues[i].Attribute.MenuSprite,
        //         Value = entity.Data.AttributeValues[i].value
        //     });
        // }
        groups.Add(_dialogData);

        // var groupArtifact = new DataDialogMapObjectGroup();
        // groupArtifact.Values = new List<DataDialogMapObjectGroupItem>();
        // for (int i = 0; i < entity.Data.Artifacts.Count; i++)
        // {
        //     groupArtifact.Values.Add(new DataDialogMapObjectGroupItem()
        //     {
        //         Sprite = entity.Data.Artifacts[i].Artifact.spriteMap,
        //     });
        // }
        // groups.Add(groupArtifact);
        var description = configData.Effects[_mapObject.Entity.Effects.index].Item.description.IsEmpty
            ? ""
            : configData.Effects[_mapObject.Entity.Effects.index].Item.description.GetLocalizedString();

        var dialogData = new DataDialogMapObject()
        {
            Header = configData.Text.title.GetLocalizedString(),
            Description = description,
            // Sprite = this.ScriptableData.MenuSprite,
            TypeCheck = TypeCheck.OnlyOk,
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
                OnHeroGo(player, result.keyVariant);
            }
            else
            {
                // Click cancel.
            }
        }
        else
        {
            await UniTask.Delay(LevelManager.Instance.ConfigGameSettings.timeDelayDoBot);
            OnHeroGo(player, 0);
        }
    }

    private void OnHeroGo(Player player, int indexVariant)
    {
        MapObject entity = (MapObject)_mapObject;
        ScriptableEntityMapObject configData = (ScriptableEntityMapObject)_mapObject.ConfigData;
        _mapObject.SetPlayer(player);

        if (indexVariant == -1)
        {
            configData.RunHero(player, entity.Entity);
        }
        else
        {
            var effect = configData.Effects.ElementAt(entity.Entity.Effects.index);
            if (configData.Effects.Count > 0 && effect.Item.items.Count > 0)
            {
                effect.Item.items[indexVariant].RunHero(player, entity.Entity);
            }
        }

        if (configData.TypeWorkObject == TypeWorkObject.One)
        {
            MapObject.Entity.DestroyEntity();
            Destroy(gameObject);
        }
        // else {
        //     foreach (var res in entity.Data.AttributeValues)
        //     {
        //         Debug.Log($"Status choose={res.TypeAttribute}-{res.value}");
        //     }
        // }
    }

    public override void OnNextDay()
    {
        base.OnNextDay();
        // var mapObjectClass = (EntityResource)MapObjectClass;
        // mapObjectClass.SetData();
    }

}
