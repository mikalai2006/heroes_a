using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine;

[System.Serializable]
public struct DataSkillSchool
{
    public List<ItemSkill> Skills;
    public TypeWorkPerk TypeWork;
}
public class BaseSkillSchool : BaseMapObject, IDataPlay, IDialogMapObjectOperation
{
    public DataSkillSchool Data;

    public override void InitUnit(ScriptableEntity data, Vector3Int pos)
    {
        base.InitUnit(data, pos);
        SetData();
    }

    private void SetData()
    {
        // ScriptableMapObject scriptDataObject = ResourceSystem.Instance.GetUnit<ScriptableMapObject>(idObject);
        ScriptableEntityBuilding scriptDataObject = (ScriptableEntityBuilding)ScriptableData;
        Data = new DataSkillSchool();
        Data.Skills = new List<ItemSkill>();
        Data.TypeWork = scriptDataObject.TypeWork;

        for (int i = 0; i < scriptDataObject.PrimarySkills.Count; i++)
        {
            List<ItemSkill> ListVariant = scriptDataObject.PrimarySkills[i].ListVariant;
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
                Sprite = Data.Skills[i].Skill.MenuSprite, //.SpriteMenu,
                Value = Data.Skills[i].Value
            });
        }

        // var t = HelperLanguage.GetLocaleText(this.ScriptableData.Locale);
        var dialogData = new DataDialog()
        {
            Header = this.ScriptableData.Text.title.GetLocalizedString(),
            // Description = t.Text.visit_ok,
            Sprite = this.ScriptableData.MenuSprite,
            Value = listValue
        };

        var dialogWindow = new DialogMapObjectProvider(dialogData);
        return await dialogWindow.ShowAndHide();
    }

    public void LoadDataPlay(DataPlay data)
    {
        //throw new System.NotImplementedException();
    }


    public void SaveDataPlay(ref DataPlay data)
    {
        var sdata = SaveUnit(Data);
        data.Units.skillSchools.Add(sdata);
    }
}
