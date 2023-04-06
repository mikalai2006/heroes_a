using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine;

public class MapEntitySkills : BaseMapEntity, IDialogMapObjectOperation
{
    public override void InitUnit(BaseEntity mapObject)
    {
        base.InitUnit(mapObject);
    }

    public override async void OnGoHero(Player player)
    {
        base.OnGoHero(player);

        DataResultDialog result = await OnTriggeredHero();

        if (result.isOk)
        {
            Debug.Log($"Check {result.keyVariant}");

        }
        else
        {
            // Click cancel.
        }
    }

    public async UniTask<DataResultDialog> OnTriggeredHero()
    {
        EntitySkillSchool entity = (EntitySkillSchool)MapObjectClass;
        var listValue = new List<DataDialogItem>(entity.Data.Skills.Count);
        for (int i = 0; i < entity.Data.Skills.Count; i++)
        {
            listValue.Add(new DataDialogItem()
            {
                Sprite = entity.Data.Skills[i].Skill.MenuSprite, //.SpriteMenu,
                Value = entity.Data.Skills[i].Value
            });
        }

        // var t = HelperLanguage.GetLocaleText(this.ScriptableData.Locale);
        var dialogData = new DataDialog()
        {
            // Header = MapObjectClass.ScriptableData.Text.title.GetLocalizedString(),
            // Description = t.Text.visit_ok,
            Sprite = MapObjectClass.ScriptableData.MenuSprite,
            Value = listValue
        };

        var dialogWindow = new DialogMapObjectProvider(dialogData);
        return await dialogWindow.ShowAndHide();
    }

}
