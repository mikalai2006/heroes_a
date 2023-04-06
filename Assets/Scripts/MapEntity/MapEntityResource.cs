using System.Collections.Generic;

using Cysharp.Threading.Tasks;

public class MapEntityResource : BaseMapEntity, IDialogMapObjectOperation
{

    public override void InitUnit(BaseEntity mapObject)
    {

        base.InitUnit(mapObject);

        var mapObjectClass = (EntityResource)MapObjectClass;
        mapObjectClass.SetData();
    }

    public async UniTask<DataResultDialog> OnTriggeredHero()
    {
        EntityResource entity = (EntityResource)MapObjectClass;
        var listValue = new List<DataDialogItem>(entity.Data.Value.Count);
        for (int i = 0; i < entity.Data.Value.Count; i++)
        {
            listValue.Add(new DataDialogItem()
            {
                Sprite = entity.Data.Value[i].Resource.MenuSprite,
                Value = entity.Data.Value[i].value
            });
        }

        var dialogData = new DataDialog()
        {
            // Header = MapObjectClass.ScriptableData.Text.title.GetLocalizedString(),
            // Description = MapObjectClass.ScriptableData.DialogText.VisitOk.GetLocalizedString(),
            // Sprite = this.ScriptableData.MenuSprite,
            Value = listValue
        };

        var dialogWindow = new DialogMapObjectProvider(dialogData);
        return await dialogWindow.ShowAndHide();
    }

    public async override void OnGoHero(Player player)
    {
        base.OnGoHero(player);
        DataResultDialog result = await OnTriggeredHero();
        if (result.isOk)
        {
            MapObjectClass.SetPlayer(player);

        }
        else
        {
            // Click cancel.
        }
    }

    public override void OnNextDay()
    {
        base.OnNextDay();
        var mapObjectClass = (EntityResource)MapObjectClass;
        mapObjectClass.SetData();
    }

}
