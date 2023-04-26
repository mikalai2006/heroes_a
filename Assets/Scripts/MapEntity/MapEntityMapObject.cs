using System.Collections.Generic;

using Cysharp.Threading.Tasks;

public class MapEntityMapObject : BaseMapEntity, IDialogMapObjectOperation
{
    private DataDialogMapObjectGroup _dialogData;

    public override void InitUnit(BaseEntity mapObject)
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

        EntityMapObject entity = (EntityMapObject)MapObjectClass;
        ScriptableEntityMapObject configData = (ScriptableEntityMapObject)MapObjectClass.ScriptableData;

        foreach (var effect in configData.Effects[entity.DataEffects.index].Item.items)
        {
            effect.CreateDialogData(ref _dialogData, entity);
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
        var description = configData.Effects[entity.DataEffects.index].Item.description.IsEmpty
            ? ""
            : configData.Effects[entity.DataEffects.index].Item.description.GetLocalizedString();

        var dialogData = new DataDialogMapObject()
        {
            Header = configData.title.GetLocalizedString(),
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

        EntityMapObject entity = (EntityMapObject)MapObjectClass;
        if (LevelManager.Instance.ActivePlayer.DataPlayer.playerType != PlayerType.Bot)
        {

            ScriptableEntityMapObject configData = (ScriptableEntityMapObject)MapObjectClass.ScriptableData;
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
        EntityMapObject entity = (EntityMapObject)MapObjectClass;
        ScriptableEntityMapObject configData = (ScriptableEntityMapObject)MapObjectClass.ScriptableData;
        MapObjectClass.SetPlayer(player);
        if (indexVariant == -1)
        {
            configData.RunHero(ref player, entity);
        }
        else
        {
            if (configData.Effects.Count > 0)
            {
                configData.Effects[entity.DataEffects.index].Item.items[indexVariant]?.RunHero(ref player, entity);
            }
        }

        if (configData.TypeWorkObject == TypeWorkObject.One)
        {
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
