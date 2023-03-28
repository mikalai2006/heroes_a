using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine;

[System.Serializable]
public struct DataSkillSchool
{
    public List<ItemSkill> Skills;
    public TypeWork TypeWork;
}
public class BaseSkillSchool : BaseMapObject, IDialogMapObjectOperation
{
    public DataSkillSchool Data;

    public override void InitUnit(ScriptableUnitBase data, Vector3Int pos)
    {
        base.InitUnit(data, pos);
        SetData();
    }

    private void SetData()
    {
        ScriptableMapObject scriptDataObject = ResourceSystem.Instance.GetUnit<ScriptableMapObject>(idObject);

        Data = new DataSkillSchool();
        Data.Skills = new List<ItemSkill>();
        Data.TypeWork = scriptDataObject.TypeWork;

        for (int i = 0; i < scriptDataObject.Skills.Count; i++)
        {
            List<ItemSkill> ListVariant = scriptDataObject.Skills[i].ListVariant;
            for (int j = 0; j < ListVariant.Count; j++)
            {
                Data.Skills.Add(ListVariant[j]);
            }

        }
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
        var listValue = new List<DataDialogItem>(Data.Skills.Count);
        for (int i = 0; i < Data.Skills.Count; i++)
        {
            listValue.Add(new DataDialogItem()
            {
                Sprite = Data.Skills[i].Skill.SpriteMenu,
                Value = Data.Skills[i].Value
            });
        }

        var t = HelperLanguage.GetLocaleText(this.ScriptableData);
        var dialogData = new DataDialog()
        {
            Description = t.Text.visit_ok,
            Header = t.Text.title,
            Sprite = this.ScriptableData.MenuSprite,
            Value = listValue
        };

        var dialogWindow = new DialogMapObjectProvider(dialogData);
        return await dialogWindow.ShowAndHide();
    }
}
