using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine;

public class MapEntityDwelling : BaseMapEntity, IDialogMapObjectOperation
{
    public override void InitUnit(BaseEntity mapObject)
    {
        base.InitUnit(mapObject);
        // var mapObjectClass = (EntityDwelling)MapObjectClass;
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

    public async UniTask<DataResultDialog> OnTriggeredHero()
    {
        // var t = HelperLanguage.GetLocaleText(this.ScriptableData.Locale);
        var dialogData = new DataDialogMapObject()
        {
            // Header = MapObjectClass.ScriptableData.Text.title.GetLocalizedString(),
            // Description = t.Text.visit_ok,
            Sprite = MapObjectClass.ScriptableData.MenuSprite,
        };

        var dialogWindow = new DialogMapObjectProvider(dialogData);
        return await dialogWindow.ShowAndHide();
    }

}
