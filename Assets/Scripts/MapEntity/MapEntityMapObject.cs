using System.Collections.Generic;

using Cysharp.Threading.Tasks;

public class MapEntityMapObject : BaseMapEntity, IDialogMapObjectOperation
{

    public override void InitUnit(BaseEntity mapObject)
    {

        base.InitUnit(mapObject);

        var mapObjectClass = (EntityMapObject)MapObjectClass;
        mapObjectClass.SetData();
    }

    public async UniTask<DataResultDialog> OnTriggeredHero()
    {
        EntityMapObject entity = (EntityMapObject)MapObjectClass;
        ScriptableEntityMapObject configData = (ScriptableEntityMapObject)MapObjectClass.ScriptableData;
        var groups = new List<DataDialogMapObjectGroup>();
        var group = new DataDialogMapObjectGroup();
        // var group = groups[0].Items;
        group.Values = new List<DataDialogMapObjectGroupItem>();
        for (int i = 0; i < entity.Data.AttributeValues.Count; i++)
        {
            group.Values.Add(new DataDialogMapObjectGroupItem()
            {
                Sprite = entity.Data.AttributeValues[i].Attribute.MenuSprite,
                Value = entity.Data.AttributeValues[i].value
            });
        }
        groups.Add(group);

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
        var description = configData.Attributes[entity.Data.index].Item.description.IsEmpty ?
            "" : configData.Attributes[entity.Data.index].Item.description.GetLocalizedString();
        var dialogData = new DataDialogMapObject()
        {
            Header = configData.title.GetLocalizedString(),
            Description = description,
            // Sprite = this.ScriptableData.MenuSprite,
            TypeCheck = TypeCheck.OnlyOk,
            TypeWorkAttribute = configData.TypeWorkAttribute,
            Groups = groups
        };

        var dialogWindow = new DialogMapObjectProvider(dialogData);
        return await dialogWindow.ShowAndHide();
    }

    public async override void OnGoHero(Player player)
    {
        base.OnGoHero(player);

        EntityMapObject entity = (EntityMapObject)MapObjectClass;
        ScriptableEntityMapObject configData = (ScriptableEntityMapObject)MapObjectClass.ScriptableData;
        DataResultDialog result = await OnTriggeredHero();
        if (result.isOk)
        {
            MapObjectClass.SetPlayer(player);
            player.ActiveHero.SetPathHero(null);

        }
        else
        {
            // Click cancel.
        }
    }

    public override void OnNextDay()
    {
        base.OnNextDay();
        // var mapObjectClass = (EntityResource)MapObjectClass;
        // mapObjectClass.SetData();
    }

}
