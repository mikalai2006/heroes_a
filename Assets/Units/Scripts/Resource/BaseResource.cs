using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.Localization.Settings;

using Random = UnityEngine.Random;

[System.Serializable]
public struct DataResource
{
    public int idPlayer;
    public List<DataResourceValue> Value;
    public TypeWork TypeWork;
}

[System.Serializable]
public struct DataResourceValue
{
    public TypeResource typeResource;
    public int value;

    public ScriptableResource Resource;

}

public abstract class BaseResource : BaseMapObject, IDataPlay, IDialogMapObjectOperation
{
    // private int _value = 0;
    public DataResource Data;

    public async UniTask<DataResultDialog> OnTriggeredHero()
    {

        var listValue = new List<DataDialogItem>(Data.Value.Count);
        for (int i = 0; i < Data.Value.Count; i++)
        {
            listValue.Add(new DataDialogItem()
            {
                Sprite = Data.Value[i].Resource.MenuSprite,
                Value = Data.Value[i].value
            });
        }

        var t = HelperLanguage.GetLocaleText(this.ScriptableData.Locale);
        var dialogData = new DataDialog()
        {
            Header = t.Text.title,
            Description = t.Text.visit_ok,
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
            SetPlayer(player);
        }
        else
        {
            // Click cancel.
        }
    }
    public void SetPlayer(Player player)
    {
        // ScriptableResource dataScriptable = ResourceSystem.Instance.GetUnit<ScriptableResource>(idObject);

        // ItemResource dataResource = dataScriptable.ListResource[Random.Range(0, dataScriptable.ListResource.Count)];
        // int value = dataResource.listValue[Random.Range(0, dataResource.listValue.Length)];
        // player.ChangeResource(dataResource.TypeResource, value);
        for (int i = 0; i < Data.Value.Count; i++)
        {
            player.ChangeResource(Data.Value[i].typeResource, Data.Value[i].value);
        }
        if (Data.TypeWork == TypeWork.One) Destroy(gameObject);
    }

    public override void InitUnit(ScriptableUnitBase data, Vector3Int pos)
    {

        base.InitUnit(data, pos);

        Data = new DataResource();
        var DataResource = (ScriptableResource)data;
        SetData();
    }

    private void SetData()
    {
        ScriptableResource scriptDataObject = ResourceSystem.Instance.GetUnit<ScriptableResource>(idObject);

        Data.Value = new List<DataResourceValue>();
        Data.TypeWork = scriptDataObject.TypeWork;

        int stepValue = scriptDataObject.maxValue / scriptDataObject.step;
        int randomIndexValue = Random.Range(1, stepValue);

        Data.Value.Add(new()
        {
            typeResource = scriptDataObject.TypeResource,
            value = stepValue * randomIndexValue,
            Resource = scriptDataObject,
        });

    }

    public override void OnNextDay()
    {
        base.OnNextDay();
        SetData();
    }

    //public override void OnSaveUnit()
    //{
    //    SaveUnit(Data);
    //}

    public void LoadDataPlay(DataPlay data)
    {
        //throw new System.NotImplementedException();
    }


    public void SaveDataPlay(ref DataPlay data)
    {
        var sdata = SaveUnit(Data);
        data.Units.resources.Add(sdata);
    }
}
